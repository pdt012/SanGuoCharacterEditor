using SanGuoCharacterEditor.Core.CodeConverters;
using SanGuoCharacterEditor.Core.Models;
using SanGuoCharacterEditor.Core.Structs;
using System.IO;

namespace SanGuoCharacterEditor.Core.IOHelpers
{
    public static class CharacterExPersonDataHelper
    {
        public static unsafe void ToExPersonData(string exPersonDataPath, List<SanGuoCharacter> characters)
        {
            PK22ExtraPersonData exPersonData = new(characters.Count);

            PK22CodeConverter codeConverter = new();
            Dictionary<string, int> Uuid2IdMap = new();

            for (int i = 0; i < characters.Count; i++)
            {
                SanGuoCharacter character = characters[i];
                Uuid2IdMap.Add(character.Id, i);
            }

            for (int i = 0; i < characters.Count; i++)
            {
                SanGuoCharacter character = characters[i];
                exPersonData.personArray[i] = new PK22CustomPerson();
                ref PK22CustomPerson person = ref exPersonData.personArray[i];
                CharacterPK22ScenHelper.CharacterToPK22CustomPerson(character, ref person, Uuid2IdMap, codeConverter);

                exPersonData.infoArray[i] = new CharacterInfo();
                ref CharacterInfo info = ref exPersonData.infoArray[i];

                fixed (byte* pUuid = info.uuid)
                fixed (byte* pFather = info.fatherId)
                fixed (byte* pMother = info.motherId)
                fixed (byte* pSpouse = info.spouseId)
                fixed (byte* pBrother = info.brotherId)
                fixed (byte* pLike = info.likedPersonIds)
                fixed (byte* pDislike = info.dislikedPersonIds)
                {
                    codeConverter.Encode(character.Id, new Span<byte>(pUuid, 64));
                    codeConverter.Encode(character.FatherId, new Span<byte>(pFather, 64));
                    codeConverter.Encode(character.MotherId, new Span<byte>(pMother, 64));
                    codeConverter.Encode(character.SpouseId, new Span<byte>(pSpouse, 64));
                    codeConverter.Encode(character.BrotherId, new Span<byte>(pBrother, 64));

                    for (int k = 0; k < 5; k++)
                    {
                        string id = k < character.LikedPersonIdArray.Length ? character.LikedPersonIdArray[k] : "";
                        codeConverter.Encode(id, new Span<byte>(pLike + k * 64, 64));
                    }

                    for (int k = 0; k < 5; k++)
                    {
                        string id = k < character.DislikedPersonIdArray.Length ? character.DislikedPersonIdArray[k] : "";
                        codeConverter.Encode(id, new Span<byte>(pDislike + k * 64, 64));
                    }
                }
            }

            byte[] data = new byte[exPersonData.Size];
            exPersonData.ToStream(new(data));

            using FileStream fs = File.OpenWrite(exPersonDataPath);
            fs.Write(data);
        }

        public static List<SanGuoCharacter> FromExPersonData(string exPersonDataPath)
        {
            return [];
        }
    }
}
