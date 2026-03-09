using SanGuoCharacterEditor.Utils;
using System.Runtime.CompilerServices;

namespace SanGuoCharacterEditor.Core.Structs
{
    internal class PK22MakeData : IBinarySerializable
    {
        public S11FileHeader fileHeader = new();     // 文件头       
        public PK22CustomPerson[] personArray = new PK22CustomPerson[150];  // 5a 武将    

        public PK22MakeData()
        {
            for (int i = 0; i < personArray.Length; i++)
                personArray[i] = new();
        }

        public static int Size => Unsafe.SizeOf<S11FileHeader>() +
            Unsafe.SizeOf<PK22CustomPerson>() * 150;

        int IBinarySerializable.Size => Size;

        public void FromStream(BinaryStructStream stream)
        {
            stream.Read(ref fileHeader);
            if (fileHeader.fileType != 0x08)
                throw new("不是创作档文件！");
            stream.Read(personArray);
        }

        public void ToStream(BinaryStructStream stream)
        {
            stream.Write(ref fileHeader);
            stream.Write(personArray);
        }
    }
}
