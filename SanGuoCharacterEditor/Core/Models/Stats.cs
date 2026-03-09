using ReactiveUI.SourceGenerators;
using SanGuoCharacterEditor.Core.Enums;

namespace SanGuoCharacterEditor.Core.Models
{
    public partial class Stats : BaseModel
    {
        [Reactive] private byte _leadership;
        [Reactive] private byte _strength;
        [Reactive] private byte _intelligence;
        [Reactive] private byte _politics;
        [Reactive] private byte _charm;
    }

    public partial class StatAgings : BaseModel
    {
        [Reactive] private StatAging _leadership;
        [Reactive] private StatAging _strength;
        [Reactive] private StatAging _intelligence;
        [Reactive] private StatAging _politics;
        [Reactive] private StatAging _charm;
    }
}