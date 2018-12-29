using System;
using System.IO;
using UnityEngine;

namespace Wm3Util
{
    public class Wm3Object
    {
        private UInt32 m_textureRef;
        private Vector3 m_position;
        private Quaternion m_rotation;
        private Vector3 m_scale;
        private float m_ambient;
        private Wm3Flags m_flags;
        private string m_name;

        private Wm3Texture m_texture;
        private bool m_loaded;

        public Vector3 Position
        {
            get
            {
                return m_position;
            }
        }

        public Vector3 Scale
        {
            get
            {
                return m_scale;
            }
        }

        public Quaternion Rotation
        {
            get
            {
                return m_rotation;
            }
        }

        public Wm3Flags Flags
        {
            get
            {
                return m_flags;
            }
        }

        public float Ambient
        {
            get
            {
                return m_ambient;
            }
        }

        public string Name
        {
            get
            {
                return m_name;
            }
        }

        public Wm3Texture Texture
        {
            get
            {
                return m_texture;
            }
        }

        public bool Loaded
        {
            get
            {
                return m_loaded;
            }
        }

        public Wm3Object()
        {
            m_name = "Object";
            m_loaded = false;
        }

        public Wm3Object(int index) : this()
        {
            m_name += " " + index;
        }

        public bool Read(BinaryReader reader)
        {
            try
            {
                m_textureRef = reader.ReadUInt32();
                m_position.x = reader.ReadSingle();
                m_position.z = reader.ReadSingle();
                m_position.y = reader.ReadSingle();

                float angle = reader.ReadSingle();
                m_rotation = Quaternion.Euler(0, angle, 0);

                m_scale.x = reader.ReadSingle();
                m_scale.y = reader.ReadSingle();
                m_scale.z = reader.ReadSingle();
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
                if (textures.Length > m_textureRef)
                {
                    m_texture = textures[m_textureRef];
                    success = true;
                }
            }

            return success;
        }
    }
}
