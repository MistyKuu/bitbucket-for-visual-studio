using System;
using System.Globalization;

namespace GitClientVS.UI.Converters
{
    public class TreeViewFolderIconConverter : BaseMarkupExtensionConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isExpanded = (bool) value;
            return isExpanded ? "../Images/FolderOpen.png" : "../Images/Folder.png";
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}