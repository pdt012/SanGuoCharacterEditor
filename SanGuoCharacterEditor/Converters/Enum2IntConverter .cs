using System.Globalization;
using System.Windows.Data;

namespace SanGuoCharacterEditor.Converters
{
    internal class Enum2NumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            // numeric -> enum
            Type enumType = parameter as Type;
            if (enumType == null)
                return value;

            return Enum.ToObject(enumType, value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            // enum -> numeric
            Type numericType = Nullable.GetUnderlyingType(targetType) ?? targetType;
            return System.Convert.ChangeType(value, numericType);
        }
    }
}
