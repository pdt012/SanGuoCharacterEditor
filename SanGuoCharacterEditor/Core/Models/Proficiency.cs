using ReactiveUI.SourceGenerators;

namespace SanGuoCharacterEditor.Core.Models
{
    public partial class Proficiency : BaseModel
    {
        [Reactive] private byte _spearmen;
        [Reactive] private byte _halberdiers;
        [Reactive] private byte _crossbowmen;
        [Reactive] private byte _cavalry;
        [Reactive] private byte _navy;
        [Reactive] private byte _siegeUnits;
    }
}
