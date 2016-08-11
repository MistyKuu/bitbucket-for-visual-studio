using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using GitClientVS.UI.Helpers;
using Microsoft.VisualStudio.PlatformUI;

namespace GitClientVS.UI.Styles
{
    public class SharedResourceDictionary : ResourceDictionary
    {
        /// <summary>
        /// Internal cache of loaded dictionaries 
        /// </summary>
        public static Dictionary<Uri, ResourceDictionary> SharedDictionaries =
            new Dictionary<Uri, ResourceDictionary>();

        /// <summary>
        /// Local member of the source uri
        /// </summary>
        private Uri _sourceUri;

        private bool isListeningOnThemeChanged = false;
        private bool isDark;
        /// <summary>
        /// Gets or sets the uniform resource identifier (URI) to load resources from.
        /// </summary>
        public new Uri Source
        {
            get { return _sourceUri; }
            set
            {
                if (value.ToString() == "pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseLight.xaml" ||
                    value.ToString() == "pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseDark.xaml")
                {
                    if (!isListeningOnThemeChanged)
                    {
                        SetIsDark();
                        VSColorTheme.ThemeChanged += OnThemeChange;
                        isListeningOnThemeChanged = true;
                    }

                    value = GetThemeUri();
                }
              
                _sourceUri = value;

        
                ResourceDictionary ret;
                if (SharedDictionaries.TryGetValue(value, out ret))
                {
                    if (ret != this)
                    {
                        MergedDictionaries.Add(ret);
                        return;
                    }
                }
                base.Source = value;
                if (ret == null)
                    SharedDictionaries.Add(value, this);
               
            }
        }

        private void SetIsDark()
        {
            var theme = VSHelpers.DetectTheme();
            isDark = theme == "Dark";
        }

        private Uri GetThemeUri()
        {
            return isDark
                ? new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseDark.xaml")
                : new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseLight.xaml");
        }

        private void OnThemeChange(ThemeChangedEventArgs e)
        {
            var oldStyle = GetThemeUri();
            ResourceDictionary ret;
            if (SharedDictionaries.TryGetValue(oldStyle, out ret))
            {
                MergedDictionaries.Remove(ret);
            }

            SetIsDark();
            Source = oldStyle;
        }
    }
}
