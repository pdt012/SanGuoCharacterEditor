using SanGuoCharacterEditor.Utils;
using System.Runtime.InteropServices;

namespace SanGuoCharacterEditor.Core.Structs
{
    using int8 = sbyte;

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal unsafe struct Province
    {
        public fixed byte name[5];
        public fixed byte read[13];
        private fixed byte __12[12];    // 18
        public fixed byte desc[8];      // 30
        public int8 region;             // 38
        public fixed int8 adjacent[5];  // 39
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal unsafe struct Region
    {
        public fixed byte name[5];
        public fixed byte read[13];
    }

    internal class PK22GlobalScenario : IBinarySerializable
    {
        public S11FileHeader fileHeader = new();                        // 文件头

        public readonly Province[] provinceArray = new Province[12];    // 906 州
        public readonly Region[] regionArray = new Region[6];           // b16 地方

        public const int Size = 0xBAB8;

        int IBinarySerializable.Size => Size;

        public PK22GlobalScenario()
        {
            for (int i = 0; i < provinceArray.Length; i++)
                provinceArray[i] = new();
            for (int i = 0; i < regionArray.Length; i++)
                regionArray[i] = new();
        }

        public void FromStream(BinaryStructStream stream)
        {
            stream.Read(ref fileHeader);
            if (fileHeader.fileType != 0x18)
                throw new("不是全局文件！");
            stream.Seek(0x906, System.IO.SeekOrigin.Begin);
            stream.Read(provinceArray);
            stream.Read(regionArray);
        }

        public void ToStream(BinaryStructStream stream)
        {
            stream.Write(ref fileHeader);
            stream.Seek(0x3bc, System.IO.SeekOrigin.Begin);
            stream.Write(provinceArray);
            stream.Write(regionArray);
        }
    }
}
