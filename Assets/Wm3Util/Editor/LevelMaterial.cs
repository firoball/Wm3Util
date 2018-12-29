using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace Wm3Util
{
    public class LevelMaterial
    {
        private string m_searchFolder;
        private Sprite m_sprite;
        private Material m_material;
        private Wm3Texture m_textureReference;

        public Material Material
        {
            get
            {
                return m_material;
            }
        }

        public Wm3Texture TextureReference
        {
            get
            {
                return m_textureReference;
            }
        }

        public Sprite Sprite
        {
            get
            {
                return m_sprite;
            }
        }

        public LevelMaterial()
        {
            m_searchFolder = "";
        }

        public LevelMaterial(string searchFolder)
        {
            m_searchFolder = searchFolder;
        }

        public bool Construct(Wm3Texture texture, Material[] baseMaterials)
        {
            bool success = true;
            m_textureReference = texture;
            success = SetupTexture(texture);
            if (success)
                success = BuildMaterial(texture, baseMaterials);

            return success;
        }

        private bool SetupTexture(Wm3Texture texture)
        {
            //A3 models were referenced as texture, but did not have bitmaps assigned. Skip those here
            if (texture.IsSprite())
            {
                //this is a hack. a texture could have multiple bitmaps, so multiple textures should be created not just the first one
                Wm3Bitmap bitmap = texture.Bitmaps[0];
                bool sky = false;
                if (texture.Flags.IsSet(Wm3Flags.Sky))
                    sky = true;
                SpriteLocator locator = new SpriteLocator(m_searchFolder);
                m_sprite = locator.LocateAndConfigure(bitmap.FileName, sky);
                return true;
            }
            else
            {
                //A3 MDL format currently not handled
                return false;
            }
        }

        private bool BuildMaterial(Wm3Texture texture, Material[] baseMaterials)
        {
            if (baseMaterials != null && baseMaterials.Length >= LevelBuilder.c_max)
            {
                float alpha = 1.0f;
                if (texture.Flags.IsSet(Wm3Flags.Ghost) || texture.Flags.IsSet(Wm3Flags.Diaphanous))
                {
                    m_material = new Material(baseMaterials[LevelBuilder.c_overlay]);
                    alpha = 0.7f;
                }

                if (texture.Flags.IsSet(Wm3Flags.Sky))
                {
                    m_material = new Material(baseMaterials[LevelBuilder.c_sky]);
                }

                if (!m_material)
                {
                    m_material = new Material(baseMaterials[LevelBuilder.c_solid]);
                }
                m_material.mainTexture = m_sprite.texture;
                m_material.name = texture.Name;
                float ambient = (texture.Ambient * 0.01f) + 0.5f;
                Color col = new Color(ambient, ambient, ambient, alpha);
                m_material.color = col;
                return true;
            }
            else
            {
                Debug.LogError("LevelMaterial.BuildMaterial: BaseMaterial not configured");
                m_material = null;
                return false;
            }
        }

    }
}
