using SanGuoCharacterEditor.Utils;
using System.Runtime.CompilerServices;

namespace SanGuoCharacterEditor.Core.Structs
{
    internal class PK22ExtraPersonData : IBinarySerializable
    {
        public S11FileHeader fileHeader = new();    // 文件头
        public int count;                           // 数据数量
        public PK22CustomPerson[] personArray = []; // 武将列表
        public CharacterInfo[] infoArray = [];      // 武将id-uuid表

        public PK22ExtraPersonData(int count)
        {
            fileHeader.fileType = 0x08;

            this.count = count;
            personArray = new PK22CustomPerson[count];
            infoArray = new CharacterInfo[count];
        }

        public int Size => Unsafe.SizeOf<S11FileHeader>() + sizeof(int) +
            (Unsafe.SizeOf<PK22CustomPerson>() + Unsafe.SizeOf<CharacterInfo>()) * count;

        public void FromStream(BinaryStructStream stream)
        {
            stream.Read(ref fileHeader);
            if (fileHeader.fileType != 0x08)
                throw new("不是创作档文件！");
            stream.Read(ref count);

            personArray = new PK22CustomPerson[count];
            infoArray = new CharacterInfo[count];

            stream.Read(personArray);
            stream.Read(infoArray);
        }

        public void ToStream(BinaryStructStream stream)
        {
            stream.Write(ref fileHeader);
            stream.Write(ref count);
            stream.Write(personArray);
            stream.Write(infoArray);
        }
    }

    internal unsafe struct CharacterInfo
    {
        public fixed byte uuid[64];     // 扩展人物的唯一标识码
        public fixed byte fatherId[64];
        public fixed byte motherId[64];
        public fixed byte spouseId[64];
        public fixed byte brotherId[64];
        public fixed byte likedPersonIds[64 * 5];
        public fixed byte dislikedPersonIds[64 * 5];
    }
}
