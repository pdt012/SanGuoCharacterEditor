using DocumentFormat.OpenXml.Packaging;
using MiniExcelLibs;
using SanGuoCharacterEditor.Core.Enums;
using SanGuoCharacterEditor.Core.Models;
using SanGuoCharacterEditor.Utils;
using System.Collections.Immutable;

namespace SanGuoCharacterEditor.Core.FormatConverters
{
    internal static class CharacterExcelConverter
    {
        public static void ToExcel(string excelPath, IEnumerable<SanGuoCharacter> characters, string sheetName = "人物")
        {
            MiniExcel.SaveAs(excelPath, YieldExcelData(characters), sheetName: "人物", overwriteFile: true);
            CreateValidation(excelPath);
        }

        private static IEnumerable<Dictionary<string, object>> YieldExcelData(IEnumerable<SanGuoCharacter> characters)
        {
            foreach (var character in characters)
            {
                var row = new Dictionary<string, object>
                {
                    ["包名"] = character.PackageName,
                    ["字符ID"] = character.StringId,
                    ["姓"] = character.FamilyName,
                    ["名"] = character.GivenName,

                    ["字"] = character.CourtecyName,
                    ["称号"] = character.HonoraryTitle,
                    ["性别"] = character.Gender,
                    ["类型"] = character.Type,

                    ["登场年"] = character.AppearYear,
                    ["出生年"] = character.BirthYear,
                    ["死亡年"] = character.DeathYear,
                    ["死因"] = character.CauseOfDeath,

                    ["出生地"] = character.BirthPlace,
                    ["列传"] = character.Biography,

                    ["祖籍ID"] = character.AncestryId,
                    ["世代"] = character.Generation,
                    ["父亲ID"] = character.FatherId,
                    ["母亲ID"] = character.MotherId,
                    ["配偶ID"] = character.SpouseId,
                    ["兄弟ID"] = character.BrotherId,
                    ["相性"] = character.AffinityId,

                    ["喜爱列表"] = Array2String(character.LikedPersonIdArray),
                    ["厌恶列表"] = Array2String(character.DislikedPersonIdArray),

                    ["统率"] = character.Stats.Leadership,
                    ["武力"] = character.Stats.Strength,
                    ["智力"] = character.Stats.Intelligence,
                    ["政治"] = character.Stats.Politics,
                    ["魅力"] = character.Stats.Charm,

                    ["枪兵适性"] = character.Proficiency.Spearmen,
                    ["戟兵适性"] = character.Proficiency.Halberdiers,
                    ["弩兵适性"] = character.Proficiency.Crossbowmen,
                    ["骑兵适性"] = character.Proficiency.Cavalry,
                    ["水军适性"] = character.Proficiency.Navy,
                    ["兵器适性"] = character.Proficiency.SiegeUnits,

                    ["统率成长"] = character.StatAgings.Leadership,
                    ["武力成长"] = character.StatAgings.Strength,
                    ["智力成长"] = character.StatAgings.Intelligence,
                    ["政治成长"] = character.StatAgings.Politics,
                    ["魅力成长"] = character.StatAgings.Charm,

                    ["特技列表"] = Array2String(character.Skills),

                    ["舌战话题"] = character.ArgueTopic,
                    ["舌战_愤怒"] = character.ArgueSkills.Fury,
                    ["舌战_镇静"] = character.ArgueSkills.Calm,
                    ["舌战_无视"] = character.ArgueSkills.Ignore,
                    ["舌战_诡辩"] = character.ArgueSkills.Trickery,
                    ["舌战_大喝"] = character.ArgueSkills.Shout,

                    ["义理"] = character.LoyalMind,
                    ["野心"] = character.Ambition,
                    ["用人"] = character.PersonnelPolicy,
                    ["性格"] = character.Character,
                    ["声音"] = character.Voice,
                    ["语气"] = character.Tone,
                    ["汉室态度"] = character.AttitudeToHan,
                    ["战略倾向"] = character.StrategicTendency,
                    ["地域执着"] = character.LocalAffiliation,

                    ["头像ID"] = character.FaceId,
                    ["老年年龄"] = character.OldAge,

                    ["模型_骨架"] = character.Model.Skeleton,
                    ["模型_头部"] = character.Model.Head,
                    ["模型_面部"] = character.Model.Face,
                    ["模型_身体"] = character.Model.Body,
                    ["模型_披风"] = character.Model.Cloak,
                    ["模型_手腕"] = character.Model.Arms,
                    ["模型_腿部"] = character.Model.Legs,
                    ["模型_箭袋"] = character.Model.Quiver,
                    ["模型_其他"] = character.Model.Other,
                    ["模型_左手"] = character.Model.LeftHand,
                    ["模型_右手"] = character.Model.RightHand,
                    ["模型_坐骑"] = character.Model.Mount,

                    ["中头像X"] = character.MidFacePos.X,
                    ["中头像Y"] = character.MidFacePos.Y,

                    ["预设君主ID"] = character.ReservedKingId,
                };

                yield return row;
            }
        }

        private static void CreateValidation(string filename)
        {
            using var doc = SpreadsheetDocument.Open(filename, true);

            ExcelDataValidationHelper.CreateEnumSheet(doc,
                new Dictionary<string, string[]>
            {
                { "性别", Enum.GetNames<Gender>() },
                { "人物类型", Enum.GetNames<CharacterType>() },
                { "死因", Enum.GetNames<CauseOfDeath>() },
                { "成长", Enum.GetNames<StatAging>() },
                { "舌战话题", Enum.GetNames<ArgueTopic>() },
                { "义理", Enum.GetNames<LoyalMind>() },
                { "野心", Enum.GetNames<Ambition>() },
                { "用人", Enum.GetNames<PersonnelPolicy>() },
                { "性格", Enum.GetNames<Character>() },
                { "声音", Enum.GetNames<Voice>() },
                { "语气", Enum.GetNames<Tone>() },
                { "汉室态度", Enum.GetNames<AttitudeToHan>() },
                { "战略倾向", Enum.GetNames<StrategicTendency>() },
                { "地域执着", Enum.GetNames<LocalAffiliation>() }
            });

            ExcelDataValidationHelper.ApplyEnumValidations(doc, "人物",
                ("性别", "性别"),
                ("类型", "人物类型"),
                ("死因", "死因"),
                ("统率成长", "成长"),
                ("武力成长", "成长"),
                ("智力成长", "成长"),
                ("政治成长", "成长"),
                ("魅力成长", "成长"),
                ("舌战话题", "舌战话题"),
                ("义理", "义理"),
                ("野心", "野心"),
                ("用人", "用人"),
                ("性格", "性格"),
                ("声音", "声音"),
                ("语气", "语气"),
                ("汉室态度", "汉室态度"),
                ("战略倾向", "战略倾向"),
                ("地域执着", "地域执着")
            );

            doc.WorkbookPart.Workbook.Save();
        }

        public static IEnumerable<SanGuoCharacter> FromExcel(string excelPath)
        {
            var rows = MiniExcel.Query(excelPath, sheetName: "人物", useHeaderRow: true);

            foreach (var rowObj in rows)
            {
                var row = (IDictionary<string, object>)rowObj;

                var character = new SanGuoCharacter();

                if (TryGetString(row, "包名", out string s)) character.PackageName = s;
                if (TryGetString(row, "字符ID", out s)) character.StringId = s;
                if (TryGetString(row, "姓", out s)) character.FamilyName = s;
                if (TryGetString(row, "名", out s)) character.GivenName = s;

                if (TryGetString(row, "字", out s)) character.CourtecyName = s;
                if (TryGetString(row, "称号", out s)) character.HonoraryTitle = s;
                if (TryGetEnum(row, "性别", out Gender gender)) character.Gender = gender;
                if (TryGetEnum(row, "类型", out CharacterType characterType)) character.Type = characterType;

                if (TryGetInt(row, "登场年", out int i)) character.AppearYear = (ushort)i;
                if (TryGetInt(row, "出生年", out i)) character.BirthYear = (ushort)i;
                if (TryGetInt(row, "死亡年", out i)) character.DeathYear = (ushort)i;
                if (TryGetEnum(row, "死因", out CauseOfDeath causeOfDeath)) character.CauseOfDeath = causeOfDeath;

                if (TryGetString(row, "出生地", out s)) character.BirthPlace = s;
                if (TryGetString(row, "列传", out s)) character.Biography = s;

                if (TryGetString(row, "祖籍ID", out s)) character.AncestryId = s;
                if (TryGetInt(row, "世代", out i)) character.Generation = (sbyte)i;
                if (TryGetString(row, "父亲ID", out s)) character.FatherId = s;
                if (TryGetString(row, "母亲ID", out s)) character.MotherId = s;
                if (TryGetString(row, "配偶ID", out s)) character.SpouseId = s;
                if (TryGetString(row, "兄弟ID", out s)) character.BrotherId = s;
                if (TryGetInt(row, "相性", out i)) character.AffinityId = (byte)i;

                if (TryGetString(row, "喜爱列表", out s)) character.LikedPersonIdArray = String2Array(s);
                if (TryGetString(row, "厌恶列表", out s)) character.DislikedPersonIdArray = String2Array(s);

                if (TryGetInt(row, "统率", out i)) character.Stats.Leadership = (byte)i;
                if (TryGetInt(row, "武力", out i)) character.Stats.Strength = (byte)i;
                if (TryGetInt(row, "智力", out i)) character.Stats.Intelligence = (byte)i;
                if (TryGetInt(row, "政治", out i)) character.Stats.Politics = (byte)i;
                if (TryGetInt(row, "魅力", out i)) character.Stats.Charm = (byte)i;

                if (TryGetInt(row, "枪兵适性", out i)) character.Proficiency.Spearmen = (byte)i;
                if (TryGetInt(row, "戟兵适性", out i)) character.Proficiency.Halberdiers = (byte)i;
                if (TryGetInt(row, "弩兵适性", out i)) character.Proficiency.Crossbowmen = (byte)i;
                if (TryGetInt(row, "骑兵适性", out i)) character.Proficiency.Cavalry = (byte)i;
                if (TryGetInt(row, "水军适性", out i)) character.Proficiency.Navy = (byte)i;
                if (TryGetInt(row, "兵器适性", out i)) character.Proficiency.SiegeUnits = (byte)i;

                if (TryGetEnum(row, "统率成长", out StatAging statAging)) character.StatAgings.Leadership = statAging;
                if (TryGetEnum(row, "武力成长", out statAging)) character.StatAgings.Strength = statAging;
                if (TryGetEnum(row, "智力成长", out statAging)) character.StatAgings.Intelligence = statAging;
                if (TryGetEnum(row, "政治成长", out statAging)) character.StatAgings.Politics = statAging;
                if (TryGetEnum(row, "魅力成长", out statAging)) character.StatAgings.Charm = statAging;

                if (TryGetString(row, "特技列表", out s)) character.Skills = String2Array(s);

                if (TryGetEnum(row, "舌战话题", out ArgueTopic argueTopic)) character.ArgueTopic = argueTopic;
                if (TryGetBool(row, "舌战_愤怒", out bool b)) character.ArgueSkills.Fury = b;
                if (TryGetBool(row, "舌战_镇静", out b)) character.ArgueSkills.Calm = b;
                if (TryGetBool(row, "舌战_无视", out b)) character.ArgueSkills.Ignore = b;
                if (TryGetBool(row, "舌战_诡辩", out b)) character.ArgueSkills.Trickery = b;
                if (TryGetBool(row, "舌战_大喝", out b)) character.ArgueSkills.Shout = b;

                if (TryGetEnum(row, "义理", out LoyalMind loyalMind)) character.LoyalMind = loyalMind;
                if (TryGetEnum(row, "野心", out Ambition ambition)) character.Ambition = ambition;
                if (TryGetEnum(row, "用人", out PersonnelPolicy personnelPolicy)) character.PersonnelPolicy = personnelPolicy;
                if (TryGetEnum(row, "性格", out Character character_)) character.Character = character_;
                if (TryGetEnum(row, "声音", out Voice voice)) character.Voice = voice;
                if (TryGetEnum(row, "语气", out Tone tone)) character.Tone = tone;
                if (TryGetEnum(row, "汉室态度", out AttitudeToHan attitudeToHan)) character.AttitudeToHan = attitudeToHan;
                if (TryGetEnum(row, "战略倾向", out StrategicTendency strategicTendency)) character.StrategicTendency = strategicTendency;
                if (TryGetEnum(row, "地域执着", out LocalAffiliation localAffiliation)) character.LocalAffiliation = localAffiliation;

                if (TryGetInt(row, "头像ID", out i)) character.FaceId = (short)i;
                if (TryGetInt(row, "老年年龄", out i)) character.OldAge = (sbyte)i;

                if (TryGetInt(row, "模型_骨架", out i)) character.Model.Skeleton = (sbyte)i;
                if (TryGetInt(row, "模型_头部", out i)) character.Model.Head = (sbyte)i;
                if (TryGetInt(row, "模型_面部", out i)) character.Model.Face = (sbyte)i;
                if (TryGetInt(row, "模型_身体", out i)) character.Model.Body = (sbyte)i;
                if (TryGetInt(row, "模型_披风", out i)) character.Model.Cloak = (sbyte)i;
                if (TryGetInt(row, "模型_手腕", out i)) character.Model.Arms = (sbyte)i;
                if (TryGetInt(row, "模型_腿部", out i)) character.Model.Legs = (sbyte)i;
                if (TryGetInt(row, "模型_箭袋", out i)) character.Model.Quiver = (sbyte)i;
                if (TryGetInt(row, "模型_其他", out i)) character.Model.Other = (sbyte)i;
                if (TryGetInt(row, "模型_左手", out i)) character.Model.LeftHand = (sbyte)i;
                if (TryGetInt(row, "模型_右手", out i)) character.Model.RightHand = (sbyte)i;
                if (TryGetInt(row, "模型_坐骑", out i)) character.Model.Mount = (sbyte)i;

                if (TryGetInt(row, "中头像X", out int x) && TryGetInt(row, "中头像Y", out int y))
                    character.MidFacePos = new(x, y);

                if (TryGetString(row, "预设君主ID", out s)) character.ReservedKingId = s;

                yield return character;
            }
        }

        private static string Array2String(ImmutableArray<string> array)
        {
            return string.Join(", ", array);
        }

        private static ImmutableArray<string> String2Array(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return [];
            return text
                .Split(',')  // 不能StringSplitOptions.RemoveEmptyEntries，因为特技可能存在空位，需要保留
                .Select(x => x.Trim())
                .ToImmutableArray();
        }

        private static bool TryGetString(IDictionary<string, object> row, string key, out string value)
        {
            value = null;

            if (!row.TryGetValue(key, out var v) || v == null)
                return false;

            var str = v.ToString();

            if (string.IsNullOrWhiteSpace(str))
                return false;

            value = str;
            return true;
        }

        private static bool TryGetInt(IDictionary<string, object> row, string key, out int value)
        {
            value = 0;

            if (!row.TryGetValue(key, out var v) || v == null)
                return false;

            return int.TryParse(v.ToString(), out value);
        }

        private static bool TryGetBool(IDictionary<string, object> row, string key, out bool value)
        {
            value = false;

            if (!row.TryGetValue(key, out var v) || v == null)
                return false;

            return bool.TryParse(v.ToString(), out value);
        }

        private static bool TryGetEnum<EnumT>(IDictionary<string, object> row, string key, out EnumT value) where EnumT : struct, Enum
        {
            value = default;

            if (!row.TryGetValue(key, out var v) || v == null)
                return false;

            return Enum.TryParse(v.ToString(), true, out value);
        }
    }
}
