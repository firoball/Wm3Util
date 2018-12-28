using System.Collections.Generic;
using System.Linq;
namespace Wm3Util
{
    public class Wm3Data
    {
        private Wm3Bitmap[] m_bitmaps;
        private string[] m_models;
        private Wm3Texture[] m_textures;
        private Wm3Mesh[] m_meshes;
        private Wm3Object[] m_objects;

        public Wm3Bitmap[] Bitmaps
        {
            get
            {
                return m_bitmaps;
            }

            set
            {
                m_bitmaps = value;
            }
        }

        public string[] Models
        {
            get
            {
                return m_models;
            }

            set
            {
                m_models = value;
            }
        }

        public Wm3Texture[] Textures
        {
            get
            {
                return m_textures;
            }

            set
            {
                m_textures = value;
            }
        }

        public Wm3Mesh[] Meshes
        {
            get
            {
                return m_meshes;
            }

            set
            {
                m_meshes = value;
            }
        }

        public Wm3Object[] Objects
        {
            get
            {
                return m_objects;
            }

            set
            {
                m_objects = value;
            }
        }

        public void InitBitmaps(uint count)
        {
            m_bitmaps = new Wm3Bitmap[count];
        }

        public void InitModels(uint count)
        {
            m_models = new string[count];
        }

        public void InitTextures(uint count)
        {
            m_textures = new Wm3Texture[count];
        }

        public void InitMeshes(uint count)
        {
            m_meshes = new Wm3Mesh[count];
        }

        public void InitObjects(uint count)
        {
            m_objects = new Wm3Object[count];
        }

        public bool Link()
        {
            //resolve all id references to object references
            bool result = true;
            foreach (Wm3Texture texture in m_textures)
            {
                if (result)
                    result = texture.Link(m_bitmaps);
            }

            foreach (Wm3Mesh mesh in m_meshes)
            {
                if (result)
                    result = mesh.Link(m_textures);
            }

            foreach (Wm3Object obj in m_objects)
            {
                if (result)
                    result = obj.Link(m_textures);
            }

            return result;
        }

    }
}
