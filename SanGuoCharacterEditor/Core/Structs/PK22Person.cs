#define PERSON_MERIT_SKILL_EXPAND  // 功绩特技扩展

using System.Runtime.InteropServices;

namespace SanGuoCharacterEditor.Core.Structs
{
    using int16 = Int16;
    using int8 = SByte;
    using uint16 = UInt16;
    using uint8 = Byte;

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal unsafe struct PK22Person
    {
        public fixed byte family_name[5];   // 0 姓
        public fixed byte given_name[5];    // 5 名
        public fixed byte courtecy_name[5]; // 9 字
        public fixed byte name_read[25];    // e 姓名读音
#if PERSON_MERIT_SKILL_EXPAND
        private fixed byte __zi_read[9];    // 28 字读音
        public int16 skill2;                // 特技2（功阶特技）
        public int16 skill3;                // 特技3（功阶特技）
#else
        private fixed byte __zi_read[13];   // 28 字读音
#endif
        public int16 face = 100;            // 35 头像
        public int8 gender;                 // 37 性别
        public uint16 appearance;           // 38 登场年
        public uint16 birth;                // 3a 出生年
        public uint16 death;                // 3c 死亡年
        public int8 cause_of_death;         // 3e 死因
        public int16 ancestry;              // 3f 血缘
        public int16 father;                // 41 父亲
        public int16 mother;                // 43 母亲
        public int8 generation;             // 45 世代
        public int16 spouse;                // 46 配偶
        public int16 brother;               // 48 义兄
        public uint8 xiangXing;             // 4a 相性
        public fixed int16 liked[5];        // 4b 亲爱武将
        public fixed int16 disliked[5];     // 55 厌恶武将
        public int8 district;               // 5f 军团
        public int16 service;               // 60 所属
        public int16 location;              // 62 所在
        public int8 identity;               // 64 身份
        public int8 rank;                   // 65 官职
        public int16 reserved_king;         // 66 预定君主
        public uint8 loyalty;               // 68 忠诚
        public uint16 merits;               // 69 功绩
        public fixed uint8 army_level[6];   // 6b 适性
        public fixed uint8 base_stat[5];    // 71 基础能力
        public fixed uint8 stat_aging[5];   // 76 能力成长
        public int8 birthplace;             // 7b 出生地
        public uint8 skill;                 // 7c 特技 (无符号，防止位操作时异常)
        public uint8 argue_topic;           // 7d 舌战话题 (无符号，防止位操作时异常)
        public int8 loyal_mind;             // 7e 义理
        public int8 ambition;               // 7f 野心
        public int8 personnel_policy;       // 80 起用
        public int8 character;              // 81 性格
        public int8 voice;                  // 82 声音
        public int8 tone;                   // 83 语气
        public int8 attitude_to_Han;        // 84 汉室态度
        public int8 strategic_tendency;     // 85 战略倾向
        public int8 local_affiliation;      // 86 地域执着
        public fixed int8 model[12];        // 87 模型
        public int8 old_age;                // 93 头像变更年龄
        public int flag_argue_skill;        // 94 舌战特技（愤怒|镇静|无视|诡辩|大喝）

        public PK22Person()
        {
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal unsafe struct PK22CustomPerson
    {
        // 前0x98字节同 PK22Person
        public PK22Person personStruct;
        // 创作武将附加信息
        public fixed byte biography[583];   // 98 列传
        public int16 brother1;              // 2df 义兄弟
        public int16 brother2;              // 2e1 义兄弟
        public int16 xiangXing_person;      // 2e3 相性合武将
        private fixed byte __2e5[16];       // 2e5 空白
    }
}
