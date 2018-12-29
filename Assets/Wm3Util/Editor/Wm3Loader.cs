using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEditor;
using UnityEngine;

using Wm3Util;

public class Wm3Loader
{
    private const int c_WM3HEADER = 0x20334D57; /* 'WM3 ' */
    private const int c_WM3BITMAP = 0x50414d42; /* 'BMAP' */
    private const int c_WM3MODEL = 0x204c444d; /* 'MDL ' */
    private const int c_WM3TEXTURE = 0x20584554; /* 'TEX ' */
    private const int c_WM3MESH = 0x4853454d; /* 'MESH' */
    private const int c_WM3OBJECT = 0x204a424f; /* 'OBJ ' */

    private Wm3Data m_data;
    private BinaryReader m_br;

    public Wm3Data Data
    {
        get
        {
            return m_data;
        }
    }

    public bool LoadWM3(TextAsset level)
    {
        Debug.Log("Wm3Loader: Loading <" + level.name + "> ...");

        bool ret = false;
        if (level)
        {
            m_data = new Wm3Data();
            Stream stream = new MemoryStream(level.bytes);
            m_br = new BinaryReader(stream);

            ret = ReadHeader();
            if (ret)
                ret = ReadBitmaps();
            if (ret)
                ret = ReadModels();
            if (ret)
                ret = ReadTextures();
            if (ret)
                ret = ReadMeshes();
            if (ret)
                ret = ReadObjects();

            m_br.Close();

            if (ret)
                ret = m_data.Link();
        }
        Debug.Log("Wm3Loader: Finished.");

        return ret;
    }

    private bool ReadHeader()
    {
        uint header = m_br.ReadUInt32();
        if (header == c_WM3HEADER)
        {
            uint version = m_br.ReadUInt32();
            Debug.Log("Found section: WM3 " + string.Format("V{0:X}", version));
            if (version == 0x100)
                return true;
        }

        return false;
    }

    private bool ReadBitmaps()
    {
        uint header = m_br.ReadUInt32();
        if (header == c_WM3BITMAP)
        {
            uint count = m_br.ReadUInt32();
            Debug.Log("Found section: BMP - Loading " + count + " items...");
            m_data.InitBitmaps(count);
            for (int i = 0; i < count; i++)
            {
                Wm3Bitmap bitmap = new Wm3Bitmap(i);
                if (bitmap.Read(m_br))
                {
                    m_data.Bitmaps[i] = bitmap;
                }
            }
            return true;
        }
        return false;
    }

    private bool ReadModels()
    {
        uint header = m_br.ReadUInt32();
        if (header == c_WM3MODEL)
        {
            uint count = m_br.ReadUInt32();
            Debug.Log("Found section: MDL - Loading " + count + " items...");
            m_data.InitModels(count);
            for (int i = 0; i < count; i++)
            {
                m_data.Models[i] = m_br.ReadNullTerminatedString();
            }
            return true;
        }
        return false;
    }

    private bool ReadTextures()
    {
        uint header = m_br.ReadUInt32();
        if (header == c_WM3TEXTURE)
        {
            uint count = m_br.ReadUInt32();
            Debug.Log("Found section: TEX - Loading " + count + " items...");
            m_data.InitTextures(count);
            for (int i = 0; i < count; i++)
            {
                Wm3Texture texture = new Wm3Texture(i);
                if (texture.Read(m_br))
                {
                    m_data.Textures[i] = texture;
                }
            }
            return true;
        }
        return false;
    }

    private bool ReadMeshes()
    {
        uint header = m_br.ReadUInt32();
        if (header == c_WM3MESH)
        {
            uint count = m_br.ReadUInt32();
            Debug.Log("Found section: MESH - Loading " + count + " items...");
            m_data.InitMeshes(count);
            for (int i = 0; i < count; i++)
            {
                Wm3Mesh mesh = new Wm3Mesh(i);
                if (mesh.Read(m_br))
                {
                    m_data.Meshes[i] = mesh;
                }
            }
            return true;
        }
        return false;
    }

    private bool ReadObjects()
    {
        uint header = m_br.ReadUInt32();
        if (header == c_WM3OBJECT)
        {
            uint count = m_br.ReadUInt32();
            Debug.Log("Found section: OBJ - Loading " + count + " items...");
            m_data.InitObjects(count);
            for (int i = 0; i < count; i++)
            {
                Wm3Object obj = new Wm3Object(i);
                if (obj.Read(m_br))
                {
                    m_data.Objects[i] = obj;
                }
            }
            return true;
        }
        return false;
    }

}
