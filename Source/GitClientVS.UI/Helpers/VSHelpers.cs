using System;
using System.Windows.Media;
using Microsoft.VisualStudio.PlatformUI;

namespace GitClientVS.UI.Helpers
{
    public static class VSHelpers
    {

        public static Color ToColor(this System.Drawing.Color color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        static Color AccentMediumDarkTheme = Color.FromRgb(45, 45, 48);
        static Color AccentMediumLightTheme = Color.FromRgb(238, 238, 242);
        static Color AccentMediumBlueTheme = Color.FromRgb(255, 236, 181);

        public static string DetectTheme()
        {
            try
            {
                var color = VSColorTheme.GetThemedColor(EnvironmentColors.AccentMediumColorKey);
                var cc = color.ToColor();
                if (cc == AccentMediumBlueTheme)
                    return "Blue";
                if (cc == AccentMediumLightTheme)
                    return "Light";
                if (cc == AccentMediumDarkTheme)
                    return "Dark";
                var brightness = color.GetBrightness();
                var dark = brightness > 0.5f;
                return dark ? "Dark" : "Light";
            }
            // this throws in design time and when running outside of VS
            catch (ArgumentNullException)
            {
                return "Dark";
            }
        }
    }
  
}