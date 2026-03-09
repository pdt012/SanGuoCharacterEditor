using System.Runtime.InteropServices;

namespace SanGuoCharacterEditor.Core.Structs
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal unsafe struct S11FileHeader
    {
        private int __0;                // (hex) 00 00 FE FF
        public int fileType;            // 4 文件格式类型
        private fixed byte title[16];   // 8 KOEI%SAN11
        private int __18;               // 18 未知
        private int __1c;               // 1c 未知
        public bool isCompressed;       // 20 是否压缩
        private fixed byte __21[55];    // 21 未知（固定0x00）
        private Int16 __58;             // 58 未知

        public const int Size = 0x5a;
    }
}
