using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wm3Util
{
    public static class MeshBuilder
    {
        public static bool Build(Wm3Mesh mesh, TextureManager textureManager, out GameObject gameObj)
        {
            bool success = false;
            Mesh unityMesh = new Mesh();
            gameObj = null;
            if (Construct(unityMesh, mesh))
            {

                Material[] materials;
                bool isOverlay = mesh.Flags.IsSet(Wm3Flags.Overlay);
                if (textureManager.BuildMeshMaterials(mesh.Textures, mesh.Ambient, isOverlay, out materials))
                {
                    gameObj = SpawnMeshObject(unityMesh, materials);
                    gameObj.name = unityMesh.name;
                    gameObj.transform.position = mesh.Position;
                    gameObj.isStatic = true;
                    success = true;

                    //TODO: move to static helper class
                    Renderer renderer = gameObj.GetComponent<Renderer>();
                    Collider collider = gameObj.GetComponent<Collider>();

                    if (mesh.Flags.IsSet(Wm3Flags.Invisible))
                    {
                        renderer.enabled = false;
                    }

                    if (mesh.Flags.IsSet(Wm3Flags.Passable))
                    {
                        collider.enabled = false;
                    }
                }
            }
            return success;
        }

        private static bool Construct(Mesh mesh, Wm3Mesh wm3Mesh)
        {
            if (wm3Mesh.Loaded)
            {
                mesh.subMeshCount = wm3Mesh.NumSubmeshes;
                mesh.vertices = wm3Mesh.Vertices;
                mesh.normals = wm3Mesh.Normals;
                mesh.uv = wm3Mesh.Uv;
                mesh.name = wm3Mesh.Name;
                for (int j = 0; j < wm3Mesh.NumSubmeshes; j++)
                {
                    mesh.SetTriangles(wm3Mesh.Triangles[j], j);
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        private static GameObject SpawnMeshObject(Mesh mesh, Material[] materials)
        {
            GameObject obj = new GameObject();

            MeshFilter filter = obj.AddComponent<MeshFilter>();
            filter.sharedMesh = mesh;

            MeshRenderer renderer = obj.AddComponent<MeshRenderer>();
            renderer.sharedMaterials = materials;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;

            MeshCollider collider = obj.AddComponent<MeshCollider>();
            collider.sharedMesh = filter.sharedMesh;

            return obj;
        }
    }
}
