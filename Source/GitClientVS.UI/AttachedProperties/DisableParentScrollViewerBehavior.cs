using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Interactivity;
using GitClientVS.UI.Helpers;
using Microsoft.Internal.VisualStudio.PlatformUI;
using UIElement = System.Windows.UIElement;

namespace GitClientVS.UI.AttachedProperties
{
    public class DisableParentScrollViewerBehavior : Behavior<UserControl>
    {
        private UserControl _element;

        public bool DisableHorizontal { get; set; }
        public bool DisableVertical { get; set; }

        protected override void OnAttached()
        {
            base.OnAttached();
            _element = AssociatedObject;
            _element.Loaded += _element_Loaded;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            _element.Loaded -= _element_Loaded;
        }

        private void _element_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var sv = VisualTreeHelpers.FindParent<ScrollViewer>(_element);
            if (sv != null)
            {
                if (DisableHorizontal)
                    sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                if (DisableVertical)
                    sv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            }
        }
    }
}
