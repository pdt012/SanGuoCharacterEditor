using Riok.Mapperly.Abstractions;
using SanGuoCharacterEditor.Core.Models;

namespace SanGuoCharacterEditor.Core.Mapper
{
    [Mapper(UseDeepCloning = true, IncludedMembers = MemberVisibility.Private)]
    internal static partial class ModelMapper
    {
        public static partial SanGuoCharacter Clone(SanGuoCharacter source);
    }
}
