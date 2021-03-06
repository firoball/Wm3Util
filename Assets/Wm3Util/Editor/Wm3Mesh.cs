﻿using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace Wm3Util
{
    public class Wm3Mesh
    {
        private uint m_numVertex;
        private uint m_numTriangles;
        private int m_numSubmeshes;
        private Vector3 m_position;
        private Vector3[] m_vertices;
        private Vector2[] m_uv;
        private Vector3[] m_normals;
        private List<int>[] m_triangles;
        private uint[] m_textureRef;
        private byte[] m_textureSub;
        private float m_ambient;
        private Wm3Flags m_flags;
        private string m_name;

        private Wm3Texture[] m_textures;

        private bool m_loaded;

        public int NumSubmeshes
        {
            get
            {
                return m_numSubmeshes;
            }
        }

        public Vector3 Position
        {
            get
            {
                return m_position;
            }
        }

        public Vector3[] Vertices
        {
            get
            {
                return m_vertices;
            }
        }

        public Vector2[] Uv
        {
            get
            {
                return m_uv;
            }
        }

        public Vector3[] Normals
        {
            get
            {
                return m_normals;
            }
        }

        public List<int>[] Triangles
        {
            get
            {
                return m_triangles;
            }
        }

        public float Ambient
        {
            get
            {
                return m_ambient;
            }
        }

        public Wm3Flags Flags
        {
            get
            {
                return m_flags;
            }
        }

        public string Name
        {
            get
            {
                return m_name;
            }
        }

        public Wm3Texture[] Textures
        {
            get
            {
                return m_textures;
            }
        }

        public bool Loaded
        {
            get
            {
                return m_loaded;
            }
        }

        public Wm3Mesh()
        {
            m_name = "Mesh";
            m_loaded = false;
        }

        public Wm3Mesh(int index) : this()
        {
            m_name += " " + index;
        }

        public bool Read(BinaryReader reader)
        {
            try
            {
                ReadPosition(reader);
                ReadCounters(reader);
                ReadVertices(reader);
                ReadTriangles(reader);
                ReadReferences(reader);

                //currently not used
                m_ambient = reader.ReadSingle();
                m_flags = new Wm3Flags(reader.ReadUInt32());

                m_loaded = true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                m_loaded = false;
            }
            return m_loaded;
        }

        public bool Link(Wm3Texture[] textures)
        {
            bool success = false;
            if (m_loaded)
            {
                success = true;
                m_textures = new Wm3Texture[m_numSubmeshes];
                for (int i = 0; i < m_numSubmeshes; i++)
                {
                    if (textures.Length > m_textureRef[i])
                    {
                        m_textures[i] = textures[m_textureRef[i]];
                    }
                    else
                    {
                        success = false;
                    }
                }
            }

            return success;
        }

        private void ReadTriangles(BinaryReader reader)
        {
            m_triangles = new List<int>[m_numSubmeshes];
            for (int c = 0; c < m_numSubmeshes; c++)
            {
                m_triangles[c] = new List<int>();
            }

            int[] triangle = new int[3];
            int subMeshIndex;
            for (int i = 0; i < m_numTriangles; i++)
            {
                triangle[0] = reader.ReadUInt16();
                triangle[1] = reader.ReadUInt16();
                triangle[2] = reader.ReadUInt16();
                subMeshIndex = reader.ReadByte();
                if (subMeshIndex < m_numSubmeshes)
                {
                    m_triangles[subMeshIndex].AddRange(triangle);
                }
            }
        }

        private void ReadVertices(BinaryReader reader)
        {
            m_vertices = new Vector3[m_numVertex];
            m_uv = new Vector2[m_numVertex];
            m_normals = new Vector3[m_numVertex];

            for (int i = 0; i < m_numVertex; i++)
            {
                m_vertices[i].x = reader.ReadSingle();
                m_vertices[i].y = reader.ReadSingle();
                m_vertices[i].z = reader.ReadSingle();

                m_uv[i].x = -reader.ReadSingle();
                m_uv[i].y = -reader.ReadSingle();

                m_normals[i].x = reader.ReadSingle();
                m_normals[i].y = reader.ReadSingle();
                m_normals[i].z = reader.ReadSingle();
            }
        }

        private void ReadPosition(BinaryReader reader)
        {
            m_position.x = reader.ReadSingle();
            m_position.z = reader.ReadSingle();
            m_position.y = reader.ReadSingle();
        }

        private void ReadCounters(BinaryReader reader)
        {
            m_numVertex = reader.ReadUInt32();
            m_numTriangles = reader.ReadUInt32();
            m_numSubmeshes = reader.ReadInt16();
        }

        private void ReadReferences(BinaryReader reader)
        {
            m_textureRef = new uint[m_numSubmeshes];
            m_textureSub = new byte[m_numSubmeshes];

            for (int i = 0; i < m_numSubmeshes; i++)
            {
                m_textureRef[i] = reader.ReadUInt32();
                m_textureSub[i] = reader.ReadByte();
            }
        }
    }
}
