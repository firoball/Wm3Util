using System.IO;
using UnityEditor;
using UnityEngine;

namespace Wm3Util
{
    public class SpriteLocator
    {
        private string m_searchFolder;

        public SpriteLocator()
        {
            m_searchFolder = "";
        }

        public SpriteLocator(string searchFolder)
        {
            m_searchFolder = searchFolder;
       }

        public Sprite LocateAndConfigure(string fileName, bool isSky)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                Sprite sprite;
                string assetName = Path.GetFileNameWithoutExtension(fileName);
                string[] matches = AssetDatabase.FindAssets(assetName + " t:Texture", new string[] { m_searchFolder });
                if ((matches.Length > 0) && !string.IsNullOrEmpty(matches[0]))
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(matches[0]);
                    ConfigureImportSettings(assetPath, isSky);
                    sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                    sprite.name = assetName;
                    return sprite;
                }
                else
                {
                    Debug.LogWarning("SpriteLocator.LocateAndConfigure: Sprite <" + assetName + "> not found.");
                    return null;
                }
            }
            else
            {
                Debug.LogError("SpriteLocator.LocateAndConfigure: invalid filename.");
                return null;
            }

        }

        private static void ConfigureImportSettings(string assetPath, bool isSky)
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
