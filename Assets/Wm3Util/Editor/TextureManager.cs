using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Wm3Util
{
    public class TextureManager
    {
        private List<LevelTexture> m_levelTextures;
        private string m_searchFolder;
        private string m_outputFolder;

        public TextureManager(string searchFolder, string outputFolder)
        {
            m_searchFolder = searchFolder;
            m_outputFolder = outputFolder;
            m_levelTextures = new List<LevelTexture>();
        }

        public TextureManager() : this("", "")
        {
        }

        private bool Construct(Wm3Texture texture, float ambient, LevelTextureType textureType, out LevelTexture levelTexture)
        {
            bool ret = false;
            levelTexture = new LevelTexture(m_searchFolder);
            if (levelTexture.Construct(texture, ambient, textureType))
            {
                m_levelTextures.Add(levelTexture);
                AssetDatabase.CreateAsset(levelTexture.Material, m_outputFolder + "/" + levelTexture.Material.name + ".mat");
                ret = true;
            }

            return ret;
        }

        private bool BuildMaterial(Wm3Texture texture, float ambient, LevelTextureType textureType, out Material material, out Sprite sprite)
        {
            bool ret = true;
            LevelTexture levelTexture = m_levelTextures.Find(x => x.Has(texture, ambient, textureType));
            //no material was found - create new one
            if (levelTexture == null)
            {
                ret = Construct(texture, ambient, textureType, out levelTexture);
            }

            if(ret)
            {
                material = levelTexture.Material;
                sprite = levelTexture.Sprite;
            }
            else
            {
                material = null;
                sprite = null;
                Debug.LogWarning("MaterialManager.BuildMaterial: unable to build Material");
            }

            return ret;
        }

        public bool BuildObjectMaterial(Wm3Texture texture, float ambient, out Material material, out Sprite sprite)
        {
            return BuildMaterial(texture, ambient, LevelTextureType.SPRITE, out material, out sprite);
        }

        public bool BuildMeshMaterials(Wm3Texture[] textures, float ambient, bool isOverlay, out Material[] materials)
        {
            bool ret = true;
            LevelTextureType textureType = LevelTextureType.STANDARD;
            if (isOverlay)
                textureType = LevelTextureType.OVERLAY;
            int count = textures.Length;
            materials = new Material[count];
            for (int j = 0; j < count; j++)
            {
                Material material;
                Sprite sprite;
                if (BuildMaterial(textures[j], ambient, textureType, out material, out sprite))
                {
                    materials[j] = material;
                }
                else
                {
                    Debug.LogWarning("MaterialManager.BuildMaterials: unable to build all Materials");
                    ret = false;
                }
            }

            return ret;
        }

        public void Report()
        {
            Debug.Log("TextureManager: " + m_levelTextures.Count + " materials generated.");
        }

    }
}
