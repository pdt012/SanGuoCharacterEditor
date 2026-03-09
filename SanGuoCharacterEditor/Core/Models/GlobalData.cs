using SanGuoCharacterEditor.Core.CodeConverters;
using SanGuoCharacterEditor.Core.Structs;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace SanGuoCharacterEditor.Core.Models
{
    public partial class GlobalData
    {
        public static GlobalData Instance { set; get; }

        static GlobalData()
        {
            Instance = new();
        }

        public static string[] ArmyLevelNames { get; } = new[] { "C", "B", "A", "S", "S1", "S2", "S3", "S4", "S5", "S6", "S7", "S8", "S9", "S10" };

        public Dictionary<int, string> ProvinceId2NameDict { get; private set; } = new();
        public Dictionary<string, int> ProvinceName2IdDict { get; private set; } = new();
        public Dictionary<int, string> SkillId2NameDict { get; private set; } = new();
        public Dictionary<string, int> SkillName2IdDict { get; private set; } = new();

        public unsafe void LoadGlobalScenario(string scenarioPath)
        {
            ProvinceId2NameDict.Clear();
            ProvinceName2IdDict.Clear();

            byte[] data = new byte[PK22GlobalScenario.Size];
            using (FileStream fs = File.OpenRead(scenarioPath))
            {
                fs.ReadExactly(data);
            }

            PK22GlobalScenario pk22GlobalScenario = new();
            pk22GlobalScenario.FromStream(new(data));

            var codeConverter = new PK22CodeConverter();

            for (int id = 0; id < pk22GlobalScenario.provinceArray.Length; id++)
            {
                Province pk22Province = pk22GlobalScenario.provinceArray[id];
                var name = codeConverter.Decode(new(pk22Province.name, 5));
                ProvinceId2NameDict.TryAdd(id, name);
                ProvinceName2IdDict.TryAdd(name, id);
            }
            ProvinceId2NameDict[-1] = "";
            ProvinceName2IdDict[""] = -1;

            // todo: load skills
        }

        public void LoadPK22Skills(string skillXmlPath)
        {
            SkillId2NameDict.Clear();
            SkillName2IdDict.Clear();

            Regex regColorStart = new(@"(\x1b\[\d{1,2}x)");

            XmlDocument xmlDoc = new();
            xmlDoc.Load(skillXmlPath);
            XmlElement? rootEle = xmlDoc.DocumentElement;
            XmlNodeList? skillNodeList = rootEle?.SelectNodes("skill");
            if (skillNodeList == null) return;
            foreach (XmlNode skillNode in skillNodeList)
            {
                if (skillNode is not XmlElement) continue;

                string? str_id = skillNode.Attributes?["id"]?.Value;
                if (str_id == null) continue;
                int id = int.Parse(str_id);
                string? name = skillNode.SelectSingleNode("name")?.Attributes?["value"]?.Value;
                if (name != null)
                {
                    name = regColorStart.Replace(name, "");  // 去除颜色代码
                    SkillId2NameDict.TryAdd(id, name);
                    SkillName2IdDict.TryAdd(name, id);
                }
            }
        }
    }
}
