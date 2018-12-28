using UnityEngine;

namespace Wm3Util
{
    public static class Wm3Extensions
    {
        public static bool Wm3Construct(this Mesh mesh, Wm3Mesh wm3Mesh, Wm3Texture[] textures)
        {
            /*if (m_loaded)
            {
                mesh = new Mesh();
                mesh.subMeshCount = m_numSubmeshes;
                mesh.vertices = m_vertices;
                mesh.normals = m_normals;
                mesh.uv = m_uv;
                for (int j = 0; j < m_numSubmeshes; j++)
                {
                    mesh.SetTriangles(m_triangles[j], j);
                }

                return true;
            }
            else*/
            {
                return false;
            }
        }

        public static bool Wm3Construct(this Material material)
        {
            return false;
        }

        public static bool Wm3Construct(this Texture texture)
        {
            return false;
        }

        public static void Wm3Reimport(this Texture texture)
        {

        }
    }
}
