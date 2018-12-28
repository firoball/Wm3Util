using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Wm3Util
{
    public class Wm3Texture
    {
        private byte m_textureType;
        private UInt32 m_flags;
        private float m_ambient;
        private float m_albedo;
        private UInt16 m_bitmapCount;
        private UInt32[] m_bitmapRefId;

        private Wm3Bitmap[] m_bitmaps;
        private bool m_loaded = false;


        private Sprite m_sprite;
        private Material m_material;

        static private Material[] c_baseMaterials;

        private const byte c_sprite = 1;
        private const byte c_model = 2;

        public Texture2D Texture
        {
            get
            {
                return m_sprite.texture;
            }
        }

        public Sprite Sprite
        {
            get
            {
                return m_sprite;
            }
        }

        public static Material[] BaseMaterials
        {
            get
            {
                return c_baseMaterials;
            }

            set
            {
                c_baseMaterials = value;
            }
        }

        public Material Material
        {
            get
            {
                return m_material;
            }
        }

        public bool IsSprite()
        {
            return m_textureType == c_sprite;
        }

        public bool Read(BinaryReader reader)
        {
            try
            {
                m_textureType = reader.ReadByte();
                m_flags = reader.ReadUInt32();
                m_ambient = reader.ReadSingle();
                m_albedo = reader.ReadSingle();
                m_bitmapCount = reader.ReadUInt16();

                m_bitmapRefId = new UInt32[m_bitmapCount];
                for (int i = 0; i < m_bitmapCount; i++)
                {
                    m_bitmapRefId[i] = reader.ReadUInt32();
                }
                m_loaded = true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                m_loaded = false;
            }
            return m_loaded;
        }

        public bool Link(Wm3Bitmap[] bitmaps)
        {
            bool success = false;
            if (m_loaded)
            {
                success = true;
                m_bitmaps = new Wm3Bitmap[m_bitmapCount];
                for (int i = 0; i < m_bitmapCount; i++)
                {
                    if (bitmaps.Length > m_bitmapRefId[i])
                    {
                        m_bitmaps[i] = bitmaps[m_bitmapRefId[i]];
                    }
                    else
                    {
                        success = false;
                    }
                }
                success = true;
            }
            return success;
        }

        public bool SetupTexture()
        {
            //A3 models were referenced as texture, but did not have bitmaps assigned. Skip those here
            if (m_textureType == c_sprite)
            {
                //this is a hack. a texture could have multiple bitmaps, so multiple textures should be created not just the first one
                Wm3Bitmap bitmap = m_bitmaps[0];
                bool sky = false;
                if ((m_flags & Wm3Flags.Sky) != 0)
                    sky = true;
                bitmap.LocateTexture(sky);
                m_sprite = bitmap.Sprite;
            }
            return true;
        }

        public bool BuildMaterial (string name)
        {
            if (c_baseMaterials != null && c_baseMaterials.Length > 0)
            {
                float alpha = 1.0f;
                if ((m_flags & Wm3Flags.Transparent) != 0)
                {
                    m_material = new Material(c_baseMaterials[Wm3Builder.c_overlay]);
                    alpha = 0.7f;
                }

                if ((m_flags & Wm3Flags.Sky) != 0)
                {
                    m_material = new Material(c_baseMaterials[Wm3Builder.c_sky]);
                }
                
                if (!m_material)
                {
                    m_material = new Material(c_baseMaterials[Wm3Builder.c_solid]);
                }
                m_material.mainTexture = m_sprite.texture;
                m_material.name = name;
                float ambient = (m_ambient * 0.01f) + 0.5f;
                Color col = new Color(ambient, ambient, ambient, alpha);
                m_material.color = col;
                return true;
            }
            else
            {
                Debug.LogError("WM3Texture.BuildMaterial: BaseMaterial not configured");
                m_material = null;
                return false;
            }
        }

    }
}
