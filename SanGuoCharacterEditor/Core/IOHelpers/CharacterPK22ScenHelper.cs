using SanGuoCharacterEditor.Core.CodeConverters;
using SanGuoCharacterEditor.Core.Enums;
using SanGuoCharacterEditor.Core.Models;
using SanGuoCharacterEditor.Core.Structs;
using System.Collections.Immutable;
using System.IO;

namespace SanGuoCharacterEditor.Core.IOHelpers
{
    public static class CharacterPK22ScenHelper
    {
        public static void ToPK22Scenario(string scenarioPath, List<SanGuoCharacter> characters)
        {
            return;
        }

        public unsafe static List<SanGuoCharacter> FromPK22Scenario(string scenarioPath)
        {
            byte[] data = new byte[PK22Scenario.Size];
            using (FileStream fs = File.OpenRead(scenarioPath))
            {
                fs.ReadExactly(data);
            }
            PK22Scenario scenario = new();
            scenario.FromStream(new(data));

            List<SanGuoCharacter> list = new();
            for (int id = 0; id < scenario.personArray.Length; id++)
            {
                ref PK22Person person = ref scenario.personArray[id];
                SanGuoCharacter character = new()
                {
                    PackageName = "PK22",
                    StringId = id.ToString("d3"),
                    San11Id = id,
                };

                if (id < 700)
                    character.Type = CharacterType.三国史实;
                else if (id < 800)
                    character.Type = CharacterType.事件武将;
                else if (id < 900)
                    character.Type = CharacterType.古代武将;

                CharacterFromPK22Person(character, in person);

                list.Add(character);
            }
            return list;
        }

        public unsafe static List<SanGuoCharacter> FromPK22MakeData(string mkaeDataPath)
        {
            byte[] data = new byte[PK22MakeData.Size];
            using (FileStream fs = File.OpenRead(mkaeDataPath))
            {
                fs.ReadExactly(data);
            }
            PK22MakeData makeData = new();
            makeData.FromStream(new(data));

            var codeConverter = new PK22CodeConverter();

            List<SanGuoCharacter> list = new();
            for (int i = 0; i < makeData.personArray.Length; i++)
            {
                ref PK22CustomPerson person = ref makeData.personArray[i];
                SanGuoCharacter character = new()
                {
                    PackageName = "PK22",
                    StringId = (i + 850).ToString("d3"),
                    San11Id = i + 850,
                };

                character.Type = CharacterType.新武将;

                CharacterFromPK22CustomPerson(character, in person, codeConverter);

                list.Add(character);
            }
            return list;
        }

        private static string San11Id2ID(short san11Id)
        {
            return $"PK22@{san11Id:d3}";
        }

        internal unsafe static void CharacterFromPK22Person(SanGuoCharacter character, in PK22Person person, PK22CodeConverter? codeConverter = null)
        {
            codeConverter ??= new PK22CodeConverter();

            // 对参数的字段必须加fixed
            fixed (byte* family_name = person.family_name)
            fixed (byte* given_name = person.given_name)
            fixed (byte* courtecy_name = person.courtecy_name)
            fixed (byte* name_read = person.name_read)
            {
                character.FamilyName = codeConverter.Decode(new(family_name, 5));
                character.GivenName = codeConverter.Decode(new(given_name, 5));
                character.CourtecyName = codeConverter.Decode(new(courtecy_name, 5));
                character.HonoraryTitle = codeConverter.Decode(new(name_read, 25));
            }
            character.Gender = (Gender)person.gender;
            character.AppearYear = person.appearance;
            character.BirthYear = person.birth;
            character.DeathYear = person.death;
            character.CauseOfDeath = (CauseOfDeath)person.cause_of_death;
            character.BirthPlace = GlobalData.Instance.ProvinceId2NameDict[person.birthplace];
            character.LoyalMind = (LoyalMind)person.loyal_mind;
            character.Ambition = (Ambition)person.ambition;
            character.PersonnelPolicy = (PersonnelPolicy)person.personnel_policy;
            character.Character = (Character)person.character;
            character.Voice = (Voice)person.voice;
            character.Tone = (Tone)person.tone;
            character.AttitudeToHan = (AttitudeToHan)person.attitude_to_Han;
            character.StrategicTendency = (StrategicTendency)person.strategic_tendency;
            character.LocalAffiliation = (LocalAffiliation)person.local_affiliation;

            character.Proficiency.Spearmen = person.army_level[0];
            character.Proficiency.Halberdiers = person.army_level[1];
            character.Proficiency.Crossbowmen = person.army_level[2];
            character.Proficiency.Cavalry = person.army_level[3];
            character.Proficiency.Navy = person.army_level[4];
            character.Proficiency.SiegeUnits = person.army_level[5];

            character.Stats.Leadership = person.base_stat[0];
            character.Stats.Strength = person.base_stat[1];
            character.Stats.Intelligence = person.base_stat[2];
            character.Stats.Politics = person.base_stat[3];
            character.Stats.Charm = person.base_stat[4];

            character.StatAgings.Leadership = (StatAging)person.stat_aging[0];
            character.StatAgings.Strength = (StatAging)person.stat_aging[1];
            character.StatAgings.Intelligence = (StatAging)person.stat_aging[2];
            character.StatAgings.Politics = (StatAging)person.stat_aging[3];
            character.StatAgings.Charm = (StatAging)person.stat_aging[4];

            var SkillId = person.skill | person.argue_topic >> 2 << 8;
            if (SkillId == 0xffff >> 2) SkillId = -1;
            character.ArgueTopic = (ArgueTopic)(person.argue_topic & 0b11);
            var Skill2Id = person.skill2;
            var Skill3Id = person.skill3;

            character.ArgueSkills.Fury = (person.flag_argue_skill & 0b10000000) != 0;
            character.ArgueSkills.Calm = (person.flag_argue_skill & 0b01000000) != 0;
            character.ArgueSkills.Ignore = (person.flag_argue_skill & 0b00100000) != 0;
            character.ArgueSkills.Trickery = (person.flag_argue_skill & 0b00010000) != 0;
            character.ArgueSkills.Shout = (person.flag_argue_skill & 0b00001000) != 0;

            character.Skills = [];
            GlobalData.Instance.SkillId2NameDict.TryGetValue(SkillId, out var name);
            character.Skills = character.Skills.Add(name ?? "");
            GlobalData.Instance.SkillId2NameDict.TryGetValue(Skill2Id, out name);
            character.Skills = character.Skills.Add(name ?? "");
            GlobalData.Instance.SkillId2NameDict.TryGetValue(Skill3Id, out name);
            character.Skills = character.Skills.Add(name ?? "");

            character.AncestryId = person.ancestry >= 0 ? San11Id2ID(person.ancestry) : "";
            character.Generation = person.generation;
            character.FatherId = person.father >= 0 ? San11Id2ID(person.father) : "";
            character.MotherId = person.mother >= 0 ? San11Id2ID(person.mother) : "";
            character.SpouseId = person.spouse >= 0 ? San11Id2ID(person.spouse) : "";
            character.BrotherId = person.brother >= 0 ? San11Id2ID(person.brother) : "";
            character.AffinityId = person.xiangXing;

            fixed (short* liked = person.liked)
            fixed (short* disliked = person.disliked)
            {
                character.LikedPersonIdArray = new ReadOnlySpan<short>(liked, 5)
                    .ToArray()
                    .Where(id => id >= 0)
                    .Select(San11Id2ID)
                    .ToImmutableArray();
                character.DislikedPersonIdArray = new ReadOnlySpan<short>(disliked, 5)
                    .ToArray()
                    .Where(id => id >= 0)
                    .Select(San11Id2ID)
                    .ToImmutableArray();
            }
            character.ReservedKingId = person.reserved_king >= 0 ? San11Id2ID(person.reserved_king) : "";

            character.FaceId = person.face;
            // model
            character.Model.Skeleton = person.model[0];
            character.Model.Head = person.model[1];
            character.Model.Face = person.model[2];
            character.Model.Body = person.model[3];
            character.Model.Cloak = person.model[4];
            character.Model.Arms = person.model[5];
            character.Model.Legs = person.model[6];
            character.Model.Quiver = person.model[7];
            character.Model.Other = person.model[8];
            character.Model.LeftHand = person.model[9];
            character.Model.RightHand = person.model[10];
            character.Model.Mount = person.model[11];

            character.OldAge = person.old_age;
        }

        internal unsafe static void CharacterToPK22Person(SanGuoCharacter character, ref PK22Person person,
            Dictionary<string, int> Uuid2IdMap, PK22CodeConverter? codeConverter = null)
        {
            short ID2Index(string uuid)
            {
                return Uuid2IdMap.TryGetValue(uuid, out var id) ? (short)id : (short)-1;
            }

            codeConverter ??= new PK22CodeConverter();

            var familyName = codeConverter.Encode(character.FamilyName ?? "");
            var givenName = codeConverter.Encode(character.GivenName ?? "");
            var courtecyName = codeConverter.Encode(character.CourtecyName ?? "");
            var nameRead = codeConverter.Encode(character.HonoraryTitle ?? "");

            fixed (byte* family_name = person.family_name)
            fixed (byte* given_name = person.given_name)
            fixed (byte* courtecy_name = person.courtecy_name)
            fixed (byte* name_read = person.name_read)
            {
                codeConverter.Encode(character.FamilyName ?? "", new Span<byte>(family_name, 5));
                codeConverter.Encode(character.GivenName ?? "", new Span<byte>(given_name, 5));
                codeConverter.Encode(character.CourtecyName ?? "", new Span<byte>(courtecy_name, 5));
                codeConverter.Encode(character.HonoraryTitle ?? "", new Span<byte>(name_read, 5));
            }

            person.gender = (sbyte)character.Gender;
            person.appearance = character.AppearYear;
            person.birth = character.BirthYear;
            person.death = character.DeathYear;
            person.cause_of_death = (sbyte)character.CauseOfDeath;
            person.birthplace = (sbyte)(GlobalData.Instance.ProvinceName2IdDict.TryGetValue(character.BirthPlace, out var birthplace) ? birthplace : -1);
            person.loyal_mind = (sbyte)character.LoyalMind;
            person.ambition = (sbyte)character.Ambition;
            person.personnel_policy = (sbyte)character.PersonnelPolicy;
            person.character = (sbyte)character.Character;
            person.voice = (sbyte)character.Voice;
            person.tone = (sbyte)character.Tone;
            person.attitude_to_Han = (sbyte)character.AttitudeToHan;
            person.strategic_tendency = (sbyte)character.StrategicTendency;
            person.local_affiliation = (sbyte)character.LocalAffiliation;

            person.army_level[0] = character.Proficiency.Spearmen;
            person.army_level[1] = character.Proficiency.Halberdiers;
            person.army_level[2] = character.Proficiency.Crossbowmen;
            person.army_level[3] = character.Proficiency.Cavalry;
            person.army_level[4] = character.Proficiency.Navy;
            person.army_level[5] = character.Proficiency.SiegeUnits;

            person.base_stat[0] = character.Stats.Leadership;
            person.base_stat[1] = character.Stats.Strength;
            person.base_stat[2] = character.Stats.Intelligence;
            person.base_stat[3] = character.Stats.Politics;
            person.base_stat[4] = character.Stats.Charm;

            person.stat_aging[0] = (byte)character.StatAgings.Leadership;
            person.stat_aging[1] = (byte)character.StatAgings.Strength;
            person.stat_aging[2] = (byte)character.StatAgings.Intelligence;
            person.stat_aging[3] = (byte)character.StatAgings.Politics;
            person.stat_aging[4] = (byte)character.StatAgings.Charm;

            short skillId = -1, skill2Id = -1, skill3Id = -1;
            if (character.Skills.Length > 0)
                skillId = (byte)(GlobalData.Instance.SkillName2IdDict.TryGetValue(character.Skills[0], out var id) ? id : -1);
            if (character.Skills.Length > 1)
                skill2Id = (byte)(GlobalData.Instance.SkillName2IdDict.TryGetValue(character.Skills[1], out var id) ? id : -1);
            if (character.Skills.Length > 2)
                skill3Id = (byte)(GlobalData.Instance.SkillName2IdDict.TryGetValue(character.Skills[2], out var id) ? id : -1);

            person.skill = (byte)(skillId & 0xff);
            person.argue_topic = (byte)((int)character.ArgueTopic | ((skillId >> 8) << 2));
            person.skill2 = skill2Id;
            person.skill3 = skill3Id;

            byte argueSkill = 0;
            if (character.ArgueSkills.Fury) argueSkill |= 0b10000000;
            if (character.ArgueSkills.Calm) argueSkill |= 0b01000000;
            if (character.ArgueSkills.Ignore) argueSkill |= 0b00100000;
            if (character.ArgueSkills.Trickery) argueSkill |= 0b00010000;
            if (character.ArgueSkills.Shout) argueSkill |= 0b00001000;
            person.flag_argue_skill = argueSkill;

            person.ancestry = string.IsNullOrEmpty(character.AncestryId) ? (short)-1 : ID2Index(character.AncestryId);
            person.generation = character.Generation;
            person.father = string.IsNullOrEmpty(character.FatherId) ? (short)-1 : ID2Index(character.FatherId);
            person.mother = string.IsNullOrEmpty(character.MotherId) ? (short)-1 : ID2Index(character.MotherId);
            person.spouse = string.IsNullOrEmpty(character.SpouseId) ? (short)-1 : ID2Index(character.SpouseId);
            person.brother = string.IsNullOrEmpty(character.BrotherId) ? (short)-1 : ID2Index(character.BrotherId);
            person.xiangXing = character.AffinityId;

            fixed (short* liked = person.liked)
            fixed (short* disliked = person.disliked)
            {
                var likedSpan = new Span<short>(liked, 5);
                var dislikedSpan = new Span<short>(disliked, 5);
                likedSpan.Fill(-1);
                dislikedSpan.Fill(-1);

                for (int i = 0; i < Math.Min(character.LikedPersonIdArray.Length, 5); i++)
                    likedSpan[i] = ID2Index(character.LikedPersonIdArray[i]);
                for (int i = 0; i < Math.Min(character.DislikedPersonIdArray.Length, 5); i++)
                    dislikedSpan[i] = ID2Index(character.DislikedPersonIdArray[i]);
            }

            person.reserved_king = string.IsNullOrEmpty(character.ReservedKingId) ? (short)-1 : ID2Index(character.ReservedKingId);

            person.face = character.FaceId;

            person.model[0] = character.Model.Skeleton;
            person.model[1] = character.Model.Head;
            person.model[2] = character.Model.Face;
            person.model[3] = character.Model.Body;
            person.model[4] = character.Model.Cloak;
            person.model[5] = character.Model.Arms;
            person.model[6] = character.Model.Legs;
            person.model[7] = character.Model.Quiver;
            person.model[8] = character.Model.Other;
            person.model[9] = character.Model.LeftHand;
            person.model[10] = character.Model.RightHand;
            person.model[11] = character.Model.Mount;

            person.old_age = character.OldAge;
        }

        internal unsafe static void CharacterFromPK22CustomPerson(SanGuoCharacter character, in PK22CustomPerson person, PK22CodeConverter? codeConverter = null)
        {
            codeConverter ??= new PK22CodeConverter();

            CharacterFromPK22Person(character, in person.personStruct, codeConverter);

            fixed (byte* biography = person.biography)
            {
                character.Biography = codeConverter.Decode(new(biography, 583));
            }
            character.BrotherId = San11Id2ID(person.brother1);
            character.Brother2Id = San11Id2ID(person.brother2);
            //character.AffinityId = person.xiangXing_person;
        }

        internal unsafe static void CharacterToPK22CustomPerson(SanGuoCharacter character, ref PK22CustomPerson person,
            Dictionary<string, int> Uuid2IdMap, PK22CodeConverter? codeConverter = null)
        {
            short ID2Index(string uuid)
            {
                return Uuid2IdMap.TryGetValue(uuid, out var id) ? (short)id : (short)-1;
            }

            codeConverter ??= new PK22CodeConverter();

            CharacterToPK22Person(character, ref person.personStruct, Uuid2IdMap, codeConverter);
            fixed (byte* biography = person.biography)
            {
                codeConverter.Encode(character.Biography ?? "", new Span<byte>(biography, 583));
            }
            person.brother1 = string.IsNullOrEmpty(character.BrotherId) ? (short)-1 : ID2Index(character.BrotherId);
            person.brother2 = string.IsNullOrEmpty(character.Brother2Id) ? (short)-1 : ID2Index(character.Brother2Id);
            //person.xiangXing_person = character.AffinityId;
        }
    }
}