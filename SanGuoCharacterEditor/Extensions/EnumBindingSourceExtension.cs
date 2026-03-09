using System.Windows.Markup;

namespace SanGuoCharacterEditor.Extensions
{
    public class EnumBindingSourceExtension : MarkupExtension
    {
        public Type EnumType { get; set; }

        public EnumBindingSourceExtension()
        {
        }

        public EnumBindingSourceExtension(Type enumType)
        {
            EnumType = enumType;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (EnumType == null)
                throw new InvalidOperationException("EnumType must be specified.");

            Type actualEnumType = Nullable.GetUnderlyingType(EnumType) ?? EnumType;

            if (!actualEnumType.IsEnum)
                throw new ArgumentException("Type must be an Enum.");

            return Enum.GetValues(actualEnumType);
        }
    }
}