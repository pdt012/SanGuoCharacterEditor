using ReactiveUI.SourceGenerators;
using SanGuoCharacterEditor.Core.Enums;
using System.Collections.Immutable;
using System.Windows;

namespace SanGuoCharacterEditor.Core.Models
{
    public partial class SanGuoCharacter : BaseModel, IComparable
        {
        #region 标识码

        [Reactive] private int _san11Id = -1;
        [Reactive] private string _packageName = "";
        [Reactive] private string _stringId = "";

        public string Id => $"{PackageName}@{StringId}";

        #endregion

        #region 人物基础信息

        [Reactive] private CharacterType _type = CharacterType.三国史实;
        [Reactive] private string _familyName = "";
        [Reactive] private string _givenName = "";
        [Reactive] private string _courtecyName = "";
        [Reactive] private string _honoraryTitle = "";
        [Reactive] private Gender _gender;
        [Reactive] private ushort _appearYear;
        [Reactive] private ushort _birthYear;
        [Reactive] private ushort _deathYear;
        [Reactive] private CauseOfDeath _causeOfDeath;
        [Reactive] private string _birthPlace = "";
        [Reactive] private string _biography = "";

        #endregion

        #region 人际关系

        [Reactive] private string _ancestryId = "";
        [Reactive] private sbyte _generation = 1;
        [Reactive] private string _fatherId = "";
        [Reactive] private string _motherId = "";
        [Reactive] private string _spouseId = "";
        [Reactive] private string _brotherId = "";
        [Reactive] private byte _affinityId;

        [Reactive] private ImmutableArray<string> _likedPersonIdArray = [];
        [Reactive] private ImmutableArray<string> _dislikedPersonIdArray = [];

        #endregion

        #region 能力信息

        [Reactive] private Stats _stats = new();
        [Reactive] private Proficiency _proficiency = new();
        [Reactive] private StatAgings _statAgings = new();

        [Reactive] private ImmutableArray<string> _skills = [];

        [Reactive] private ArgueTopic _argueTopic;
        [Reactive] private ArgueSkills _argueSkills = new();

        #endregion

        #region 其他信息

        [Reactive] private LoyalMind _loyalMind;
        [Reactive] private Ambition _ambition;
        [Reactive] private PersonnelPolicy _personnelPolicy;
        [Reactive] private Character _character;
        [Reactive] private Voice _voice;
        [Reactive] private Tone _tone;
        [Reactive] private AttitudeToHan _attitudeToHan;
        [Reactive] private StrategicTendency _strategicTendency;
        [Reactive] private LocalAffiliation _localAffiliation;

        #endregion

        #region 头像/模型

        [Reactive] private short _faceId = -1;
        [Reactive] private San11Model _model = new();
        [Reactive] private sbyte _oldAge;
        [Reactive] private Point _midFacePos = new(1, 2);

        #endregion

        #region 剧本所属信息

        [Reactive] private string _reservedKingId = "";

        #endregion

        public string GetName()
        {
            return FamilyName + GivenName;
        }

        public string GetIdName()
        {
            return $"{Id} - {GetName()}";
        }

        public override string ToString()
        {
            return GetIdName();
        }

        public int CompareTo(object? other)
        {
            if (other is null)
                return 1;
            else if (other is SanGuoCharacter c)
                return Id.CompareTo(c.Id);
            else
                return 0;
        }
    }
}
