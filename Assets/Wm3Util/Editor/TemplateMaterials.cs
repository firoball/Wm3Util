using UnityEngine;

namespace Wm3Util
{
    public static class TemplateMaterials
    {
        private static Material s_matSolid = null;
        private static Material s_matOverlay = null;
        private static Material s_matSky = null;
        private static Material s_matSprite = null;

        public static bool IsInitialized
        {
            get
            {
                //at least solid material must be configured
                return (GetSolid() != null);
            }
        }

        public static Material GetSolid()
        {
            if (s_matSolid == null)
            {
                s_matSolid = FindMaterial("Solid");
            }

            return s_matSolid;
        }

        public static Material GetOverlay()
        {
            if (s_matOverlay == null)
            {
                s_matOverlay = FindMaterial("Overlay");
            }

            return s_matOverlay;
        }

        public static Material GetSky()
        {
            if (s_matSky == null)
            {
                s_matSky = FindMaterial("Sky");
            }

            return s_matSky;
        }

        public static Material GetSprite()
        {
            if (s_matSprite == null)
            {
                s_matSprite = FindMaterial("Sprite");
            }

            return s_matSprite;
        }

        public static void Reset()
        {
            s_matSolid = null;
            s_matOverlay = null;
            s_matSky = null;
            s_matSprite = null;
        }

        private static Material FindMaterial(string identifier)
        {
            Material material = null;
            for (int i = 0; i < Wm3Import.MaterialLabels.Length; i++)
            {
                if (Wm3Import.MaterialLabels[i] == identifier)
                {
                    material = Wm3Import.DefaultMaterials[i];
                    break;
                }
            }

            if (material == null)
            {
                if (Wm3Import.MaterialLabels.Length > 0)
                {
                    material = Wm3Import.DefaultMaterials[0];
                    Debug.LogWarning("TemplateMaterial.FindMaterial: Template <" + identifier + "> not found. Defaulting to <" + Wm3Import.MaterialLabels[0] + ">.");
                }
                else
                {
                    Debug.LogWarning("TemplateMaterial.FindMaterial: No template materials configured!");
                }
            }

            return material;
        }

    }
}
