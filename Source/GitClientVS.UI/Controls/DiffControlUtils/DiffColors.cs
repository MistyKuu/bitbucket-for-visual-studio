using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GitClientVS.UI.Controls.DiffControlUtils
{
    public static class DiffColors
    {
        public static readonly SolidColorBrush LightWordAddedBackground = new SolidColorBrush(Color.FromRgb(215, 227, 188));
        public static readonly SolidColorBrush LightWordRemovedBackground = new SolidColorBrush(Color.FromRgb(255, 153, 153));

        public static readonly SolidColorBrush DarkWordAddedBackground = new SolidColorBrush(Color.FromRgb(47, 128, 103));
        public static readonly SolidColorBrush DarkWordRemovedBackground = new SolidColorBrush(Color.FromRgb(108, 7, 7));

        public static readonly SolidColorBrush DarkLineAddedBackground = new SolidColorBrush(Color.FromRgb(38, 94, 77));
        public static readonly SolidColorBrush DarkLineRemovedBackground = new SolidColorBrush(Color.FromRgb(60, 0, 0));

        public static readonly SolidColorBrush LightLineAddedBackground = new SolidColorBrush(Color.FromRgb(235, 241, 221));
        public static readonly SolidColorBrush LightLineRemovedBackground = new SolidColorBrush(Color.FromRgb(255, 204, 204));

        public static readonly SolidColorBrush DarkLinkForeground = new SolidColorBrush(Color.FromRgb(86, 156, 214));
        public static readonly SolidColorBrush LightLinkForeground = new SolidColorBrush(Color.FromRgb(0, 89, 214));
    }
}
