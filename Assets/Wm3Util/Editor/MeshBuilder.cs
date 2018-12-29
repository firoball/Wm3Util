using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wm3Util
{
    public static class MeshBuilder
    {
        public static bool Construct(Mesh mesh, Wm3Mesh wm3Mesh)
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


        public static GameObject SpawnMeshObject(Vector3 position, Mesh mesh)
        {
            GameObject obj = new GameObject();
            obj.transform.position = position;

            MeshFilter filter = obj.AddComponent<MeshFilter>();
            filter.sharedMesh = mesh;

            MeshRenderer renderer = obj.AddComponent<MeshRenderer>();
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;

            MeshCollider collider = obj.AddComponent<MeshCollider>();
            collider.sharedMesh = filter.sharedMesh;

            return obj;
        }
    }
}
