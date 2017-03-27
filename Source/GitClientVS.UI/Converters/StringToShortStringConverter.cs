using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitClientVS.UI.Converters
{
    public class StringToShortStringConverter : BaseMarkupExtensionConverter
    {
        private readonly int _maxLength;

        public StringToShortStringConverter(int maxLength)
        {
            _maxLength = maxLength;
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            var str = (string) value;
            if (str.Length <= _maxLength)
                return str;

            return str.Substring(0, _maxLength);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
