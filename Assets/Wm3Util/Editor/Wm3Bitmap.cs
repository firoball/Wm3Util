using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Wm3Util
{
    public class Wm3Bitmap
    {
        private string m_fileName;
        private string m_assetName;
        private Sprite m_sprite;

        private static string s_searchFolder = "";

        public Sprite Sprite
        {
            get
            {
                return m_sprite;
            }
        }

        public static string SearchFolder
        {
            get
            {
                return s_searchFolder;
            }

            set
            {
                s_searchFolder = value;
            }
        }

        public bool Read(BinaryReader reader)
        {
            try
            {
                m_fileName = reader.ReadNullTerminatedString();
                return true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool LocateTexture(bool isSky)
        {
            if (!string.IsNullOrEmpty(m_fileName))
            {
                m_assetName = Path.GetFileNameWithoutExtension(m_fileName);
                string[] matches = AssetDatabase.FindAssets(m_assetName + " t:Texture", new string[] { s_searchFolder });
                if ((matches.Length > 0) && !string.IsNullOrEmpty(matches[0]))
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(matches[0]);
                    ConfigureImportSettings(assetPath, isSky);
                    m_sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                    m_sprite.name = m_assetName;
                    return true;
                }
                else
                {
                    Debug.LogWarning("Wm3Bitmap.LocateTexture: Texture <" + m_assetName + "> not found.");
                    m_sprite = null;
                    return false;
                }
            }
            else
            {
                Debug.LogError("Wm3Bitmap.LocateTexture: Bitmap not initialized.");
                m_sprite = null;
                return false;
            }

        }

        private void ConfigureImportSettings(string assetPath, bool isSky)
        {
            TextureImporter texImp = AssetImporter.GetAtPath(assetPath) as TextureImporter;

            texImp.textureType = TextureImporterType.Sprite;
            texImp.spriteImportMode = SpriteImportMode.Single;
            texImp.npotScale = TextureImporterNPOTScale.None;
            texImp.spritePixelsPerUnit = 1;
            texImp.filterMode = FilterMode.Point;
            texImp.wrapModeU = TextureWrapMode.Repeat;
            if (isSky)
                texImp.wrapModeV = TextureWrapMode.Clamp;
            else
                texImp.wrapModeV = TextureWrapMode.Repeat;

            texImp.SaveAndReimport();
        }
    }
}
