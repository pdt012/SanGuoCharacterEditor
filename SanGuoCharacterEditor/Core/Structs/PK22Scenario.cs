using SanGuoCharacterEditor.Utils;

namespace SanGuoCharacterEditor.Core.Structs
{
    internal class PK22Scenario : IBinarySerializable
    {
        public S11FileHeader fileHeader = new();                // 文件头

        public PK22Person[] personArray = new PK22Person[850];  // 3bcd 武将

        public const int Size = 167559;

        int IBinarySerializable.Size => Size;

        public PK22Scenario()
        {
            for (int i = 0; i < personArray.Length; i++)
                personArray[i] = new();
        }

        public void FromStream(BinaryStructStream stream)
        {
            stream.Read(ref fileHeader);
            if (fileHeader.fileType != 0x16)
                throw new("不是剧本文件！");
            stream.Seek(0x3bcd, System.IO.SeekOrigin.Begin);
            stream.Read(personArray);
        }

        public void ToStream(BinaryStructStream stream)
        {
            stream.Write(ref fileHeader);
            stream.Seek(0x3bc, System.IO.SeekOrigin.Begin);
            stream.Write(personArray);
        }
    }
}
