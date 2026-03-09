using System.Globalization;
using System.Windows.Data;

namespace SanGuoCharacterEditor.Converters
{
    internal class EnumLocalNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Enum e)
                return Enum.GetName(e.GetType(), e) ?? e.ToString();

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
