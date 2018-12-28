using System;

namespace Wm3Util
{
    public class Wm3Flags
    {
        /* general flags */
        public const UInt32 Flag1 = 0x00000001;
        public const UInt32 Flag2 = 0x00000002;
        public const UInt32 Flag3 = 0x00000004;
        public const UInt32 Flag4 = 0x00000008;
        public const UInt32 Flag5 = 0x00000010;
        public const UInt32 Flag6 = 0x00000020;
        public const UInt32 Flag7 = 0x00000040;
        public const UInt32 Flag8 = 0x00000080;

        /* texture flags */
        public const UInt32 Ghost = 0x00000100;
        public const UInt32 Diaphanous = 0x00000200;
        public const UInt32 Transparent = Ghost | Diaphanous;
        public const UInt32 Sky = 0x00002000;

        /* wall (mesh) flags */
        public const UInt32 Invisible = 0x00010000;
        public const UInt32 Passable = 0x00020000;
        public const UInt32 Overlay = 0x00100000;

        /* thing flags */
        //public const UInt32 Invisible = 0x00010000;
        //public const UInt32 Passable = 0x00020000;
        public const UInt32 Actor = 0x00080000;

        /* custom flags */
        public const UInt32 Scripting = 0x80000000;

        public static bool IsSet(UInt32 status, UInt32 flag)
        {
            return ((status & flag) != 0);
        }
    }
}
