namespace SanGuoCharacterEditor.Core.Models
{
    public record AppConfig(
        string MediaExPath = "data/mediaEX",
        string CodeTablePath = "data/enc_3.xml",
        string GlobalScenarioPath = "data/scenario.s11",
        string SkillNamesPath = "data/19 skill.xml",
        string LastSenarioPath = "data/SCEN007.s11",
        string LastMakeDataPath = "",
        string LastJsonDataPath = "data/characters.json",
        string LastExcelDataPath = "data/characters.json"
        );
}
