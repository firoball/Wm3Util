using System;
using System.IO;
using UnityEngine;

namespace Wm3Util
{
    public class Wm3Bitmap
    {
        private string m_fileName;
        private string m_name;

        private bool m_loaded;

        public string FileName
        {
            get
            {
                return m_fileName;
            }
        }

        public string Name
        {
            get
            {
                return m_name;
            }
        }

        public bool Loaded
        {
            get
            {
                return m_loaded;
            }
        }

        public Wm3Bitmap()
        {
            m_name = "Bitmap";
            m_loaded = false;
        }

        public Wm3Bitmap(int index) : this()
        {
            m_name += " " + index;
        }

        public bool Read(BinaryReader reader)
        {
            try
            {
                m_fileName = reader.ReadNullTerminatedString();
                m_loaded = true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                m_loaded = false;
            }
            return m_loaded;
        }

    }
}
