namespace SanGuoCharacterEditor.Core.Enums
{
    public enum Gender
    {
        无 = -1,
        男 = 0,
        女 = 1,
    }

    public enum CauseOfDeath
    {
        自然死,
        不自然死
    }

    public enum Identity
    {
        无 = -1,
        君主,
        都督,
        太守,
        一般,
        在野,
        俘虏,
        未登场,
        未发现,
        死亡
    }

    public enum StatAging
    {
        超持续,
        持续,
        早熟,
        早熟持续,
        普通,
        普通持续,
        晚成,
        超晚成,
        开眼  // 张飞智力
    }

    public enum ArgueTopic
    {
        故事,
        道理,
        时节
    }

    public enum ArgueSkillType
    {
        愤怒,
        镇静,
        无视,
        诡辩,
        大喝
    }

    public enum LoyalMind
    {
        低,
        较低,
        普通,
        较高,
        高
    }

    public enum Ambition
    {
        低,
        较低,
        普通,
        较高,
        高
    }

    public enum PersonnelPolicy
    {
        能力,
        实绩,
        名声,
        义理,
        随意
    }

    public enum Character
    {
        胆小,
        冷静,
        刚胆,
        莽撞
    }

    public enum Voice
    {
        胆小,
        冷静,
        刚胆,
        莽撞,
        冷静_女,
        刚胆_女,
        吕布,
        诸葛亮
    }

    public enum Tone
    {
        蛮族_女,
        男装,
        郑重_女,
        小女,
        普通_女,
        张飞,
        蛮族_男,
        关羽,
        豪放,
        威严,
        尊大,
        粗暴,
        谦逊,
        殷勤,
        郑重_男,
        普通_男
    }

    public enum AttitudeToHan
    {
        无视,
        普通,
        重视
    }

    public enum StrategicTendency
    {
        中华统一,
        地方统一,
        州统一,
        现状维持
    }

    public enum LocalAffiliation
    {
        执着,
        应变,
        无视
    }

    public enum PersonModelPart
    {
        骨架,
        头部,
        面部,
        身体,
        披风,
        手腕,
        腿部,
        箭袋,
        其他,
        左手武器,
        右手武器,
        马匹
    }
}
