using System;
using UnityEngine;

namespace Wm3Util
{
    public class LevelTexture
    {
        private string m_searchFolder;
        private Sprite m_sprite;
        private Material m_material;
        private Wm3Texture m_textureReference;
        private float m_ambient;
        private LevelTextureType m_textureType;

        public Sprite Sprite
        {
            get
            {
                return m_sprite;
            }
        }

        public Material Material
        {
            get
            {
                return m_material;
            }
        }

        public LevelTexture() : this("")
        {
        }

        public LevelTexture(string searchFolder)
        {
            m_searchFolder = searchFolder;
        }

        public bool Construct(Wm3Texture texture, float ambient, LevelTextureType textureType)
        {
            bool success = true;
            m_textureReference = texture;
            m_textureType = textureType;

            success = (texture != null);
            if (success)
                success = SetupTexture(texture);
            if (success)
                success = BuildMaterial(texture, ambient);

            return success;
        }

        public bool Has(Wm3Texture texture, float ambient, LevelTextureType textureType)
        {
            bool ret = false;
            ambient = CalculateAmbient(texture, ambient, textureType);
            if (
                (m_textureReference != null) && (m_textureReference == texture)
                && (Convert.ToInt32((ambient * 100) - (m_ambient * 100)) == 0)
                && (m_textureType == textureType)
                )
            {
                ret = true;
            }
            return ret;
        }

        private bool SetupTexture(Wm3Texture texture)
        {
            //A3 models were referenced as texture, but did not have bitmaps assigned. Skip those here
            //all textures are of Wm3 sprite type. Not to be confused with Unity sprite 3D objects!
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

        private bool BuildMaterial(Wm3Texture texture, float ambient)
        {
            if (TemplateMaterials.IsInitialized)
            {
                float alpha = 1.0f;
                float albedo = texture.Albedo;

                m_ambient = CalculateAmbient(texture, ambient, m_textureType);
                if (m_textureType == LevelTextureType.SPRITE)
                {
                    if (texture.Flags.IsSet(Wm3Flags.Ghost) || texture.Flags.IsSet(Wm3Flags.Diaphanous))
                    {
                        alpha = 0.7f;
                    }
                    m_material = new Material(TemplateMaterials.GetSprite());
                }
                else
                {
                    if (texture.Flags.IsSet(Wm3Flags.Ghost) || texture.Flags.IsSet(Wm3Flags.Diaphanous))
                    {
                        m_material = new Material(TemplateMaterials.GetOverlay());
                        alpha = 0.7f;
                    }

                    else if (texture.Flags.IsSet(Wm3Flags.Sky))
                    {
                        m_material = new Material(TemplateMaterials.GetSky());
                        albedo = 0;
                    }

                    else if (m_textureType == LevelTextureType.OVERLAY)
                    {
                        m_material = new Material(TemplateMaterials.GetOverlay());
                    }

                    else
                    {
                        m_material = new Material(TemplateMaterials.GetSolid());
                    }
                }

                m_material.renderQueue = -1; //force shader render queue in case template material was setup the wrong way
                m_material.mainTexture = m_sprite.texture;
                m_material.name = texture.Name + " A" + m_ambient.ToString("0.00");
                if (m_textureType == LevelTextureType.SPRITE)
                    m_material.name += "S";
                Color col = new Color(m_ambient, m_ambient, m_ambient, alpha);
                m_material.color = col;
                m_material.SetFloat("_Diffuse", albedo * 0.01f);
                return true;
            }
            else
            {
                Debug.LogError("LevelTexture.BuildMaterial: BaseMaterial not configured");
                m_material = null;
                return false;
            }
        }

        private float CalculateAmbient(Wm3Texture texture, float ambient, LevelTextureType textureType)
        {
            float ambient_sum;
            if (textureType == LevelTextureType.SPRITE)
            {
                //WM3 format already merges texture and object ambient (special treatment for A8 engine)
                //--> only use object ambient and drop any texture ambient
                ambient_sum = (ambient * 0.01f) + 0.5f;
            }
            else
            {
                //add mesh specific ambient to texture ambient
                ambient_sum = ((texture.Ambient + ambient) * 0.01f) + 0.5f;
            }

            return ambient_sum;
        }
    }
}
