using ReactiveUI.SourceGenerators;

namespace SanGuoCharacterEditor.Core.Models
{
    public partial class San11Model : BaseModel
    {
        /// <summary>
        /// 骨架
        /// </summary>
        [Reactive] private sbyte _skeleton;

        /// <summary>
        /// 头部
        /// </summary>
        [Reactive] private sbyte _head = -1;

        /// <summary>
        /// 面部
        /// </summary>
        [Reactive] private sbyte _face = -1;

        /// <summary>
        /// 身体
        /// </summary>
        [Reactive] private sbyte _body = -1;

        /// <summary>
        /// 披风
        /// </summary>
        [Reactive] private sbyte _cloak = -1;

        /// <summary>
        /// 手腕
        /// </summary>
        [Reactive] private sbyte _arms = -1;

        /// <summary>
        /// 腿部
        /// </summary>
        [Reactive] private sbyte _legs = -1;

        /// <summary>
        /// 箭袋
        /// </summary>
        [Reactive] private sbyte _quiver = -1;

        /// <summary>
        /// 其他
        /// </summary>
        [Reactive] private sbyte _other = -1;

        /// <summary>
        /// 左手武器
        /// </summary>
        [Reactive] private sbyte _leftHand = -1;

        /// <summary>
        /// 右手武器
        /// </summary>
        [Reactive] private sbyte _rightHand = -1;

        /// <summary>
        /// 坐骑
        /// </summary>
        [Reactive] private sbyte _mount = -1;
    }
}