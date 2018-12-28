namespace System.IO
{
    public static class BinaryReaderExtension
    {
        public static string ReadNullTerminatedString(this BinaryReader reader)
        {
            string str = "";
            char ch;
            while ((int)(ch = reader.ReadChar()) != 0)
                str = str + ch;
            return str;
        }

    }
}
