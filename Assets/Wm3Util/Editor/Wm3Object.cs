using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private UInt32 m_flags;

        private Wm3Texture m_texture;
        private bool m_loaded = false;

        private bool m_isSprite = false;
        private Sprite m_sprite = null;
        private Material m_material;

        public bool IsSprite
        {
            get
            {
                return m_isSprite;
            }
        }

        public Sprite Sprite
        {
            get
            {
                return m_sprite;
            }
        }

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

        public uint Flags
        {
            get
            {
                return m_flags;
            }
        }

        public Material Material
        {
            get
            {
                return m_material;
            }
        }

        public float Ambient
        {
            get
            {
                return m_ambient;
            }
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
                m_flags = reader.ReadUInt32();
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

        public bool Construct()
        {
            if (m_loaded)
            {
                m_isSprite = m_texture.IsSprite();
                if (m_isSprite)
                {
                    m_sprite = m_texture.Sprite;
                    m_material = m_texture.Material;
                }
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
