using System;
using System.Globalization;
using System.Windows;

namespace GitClientVS.UI.Converters
{
    public class ShortCommitHashConverter : BaseMarkupExtensionConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "";

            var commitHash = (string) value;


            return commitHash.Substring(0, 6);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}