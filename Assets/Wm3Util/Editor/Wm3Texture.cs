using System;
using System.IO;
using UnityEngine;

namespace Wm3Util
{
    public class Wm3Texture
    {
        private byte m_textureType;
        private Wm3Flags m_flags;
        private float m_ambient;
        private float m_albedo;
        private string m_name;
        private UInt16 m_bitmapCount;
        private UInt32[] m_bitmapRefId;

        private Wm3Bitmap[] m_bitmaps;
        private bool m_loaded = false;

        private const byte c_sprite = 1;
        private const byte c_model = 2;

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

        public float Albedo
        {
            get
            {
                return m_albedo;
            }
        }

        public string Name
        {
            get
            {
                return m_name;
            }
        }

        public Wm3Bitmap[] Bitmaps
        {
            get
            {
                return m_bitmaps;
            }
        }

        public bool Loaded
        {
            get
            {
                return m_loaded;
            }
        }

        public Wm3Texture()
        {
            m_name = "Texture";
            m_loaded = false;
        }

        public Wm3Texture(int index) : this()
        {
            m_name += " " + index;
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
                m_flags = new Wm3Flags(reader.ReadUInt32());
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

    }
}
