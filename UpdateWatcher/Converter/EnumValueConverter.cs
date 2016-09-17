using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Alisha.UpdateWatcher.Converter
{
    public class EnumValueConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                if (targetType.IsValueType)
                    return Activator.CreateInstance(targetType);
                return "NOT DEFINDED";
            }
            return ((Enum) value).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
