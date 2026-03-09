using ReactiveUI.SourceGenerators;

namespace SanGuoCharacterEditor.Core.Models
{
    public partial class ArgueSkills : BaseModel
    {
        /// <summary>
        /// 愤怒
        /// </summary>
        [Reactive] private bool _fury;

        /// <summary>
        /// 镇静
        /// </summary>
        [Reactive] private bool _calm;

        /// <summary>
        /// 无视
        /// </summary>
        [Reactive] private bool _ignore;

        /// <summary>
        /// 诡辩
        /// </summary>
        [Reactive] private bool _trickery;

        /// <summary>
        /// 大喝
        /// </summary>
        [Reactive] private bool _shout;
    }
}