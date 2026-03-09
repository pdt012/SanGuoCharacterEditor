namespace SanGuoCharacterEditor.Core.Enums
{
    public enum StatType
    {
        统率,
        武力,
        智力,
        政治,
        魅力
    }

    public enum ArmyType
    {
        枪兵,
        戟兵,
        弩兵,
        骑兵,
        兵器,
        水军
    }

    public enum EquipmentType
    {
        剑,
        枪,
        戟,
        弩,
        马,
        直接兵器,
        间接兵器,
        舰船
    }

    public enum ProductionType
    {
        枪,
        戟,
        弩,
        马,
        兵器,
        舰船
    }

    public enum FacilityType
    {
        固定设施,
        军事设施,
        障碍物,
        陷阱,
        内政设施
    }

    public enum SkillType
    {
        行军,
        攻击,
        防御,
        计略,
        补助,
        内政,
        收入,
        灾害,
        关系
    }

    public enum RouteType
    {
        无 = -1,
        一般,
        栈道,
        间道,
        海洋,
        浅滩,
        毒泉
    }

    public enum TreasureType
    {
        名马,
        宝剑,
        兵刃,
        暗器,
        弓弩,
        书籍,
        玉玺,
        铜雀,
        兵符 = 8,
        珍品 = 9
    }

    public enum TreasureState
    {
        未登场,
        登场,
        破坏
    }

    public enum ForcePolicy
    {
        无 = -1,
        中华统一,
        地方统一,
        州统一,
        现状维持,
        吴越割据,
        巴蜀割据
    }

    public enum DistrictPolicy
    {
        无 = -1,
        势力攻略,
        地方攻略,
        州攻略,
        城市攻略,
        重视防卫,
        保护皇帝
    }

    public enum DistrictPlan
    {
        无 = -1,
        充实军备,
        充实内政,
        确保人材,
        攻击城市,
        注重外交,
        紧急防备
    }

    public enum TechnologyType
    {
        枪兵,
        戟兵,
        弩兵,
        骑兵,
        通用,
        兵器,
        建设,
        火攻,
        内政
    }

    public enum TacticTarget
    {
        部队,
        兵器,
        船只,
        建筑
    }

    public enum TerrainDamageType
    {
        无,
        毒泉,
        栈道
    }

    public enum GameDifficulty
    {
        初级,
        上级,
        超级
    }

    public enum IsHistorical
    {
        假想,
        史实
    }
}
