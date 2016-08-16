using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace GitClientVS.UI.AttachedProperties
{
    public static class HandleMouseWheelInParentBehaviour
    {
        public static bool GetIsRedirectEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsRedirectEnabledProperty);
        }
        public static void SetIsRedirectEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsRedirectEnabledProperty, value);
        }
        // Using a DependencyProperty as the backing store for AllowOnlyString. This enables animation, styling, binding, etc...  
        public static readonly DependencyProperty IsRedirectEnabledProperty =
        DependencyProperty.RegisterAttached("IsRedirectEnabled", typeof(bool), typeof(SelectTreeViewAfterItemClickBehavior), new PropertyMetadata(false, IsRedirectEnabledChanged));

        private static void IsRedirectEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var itemsControl = d as UIElement;
            if (itemsControl != null)
            {
                if ((bool)e.NewValue)
                    itemsControl.PreviewMouseWheel += PreviewMouseWheel;
                else
                    itemsControl.PreviewMouseWheel -= PreviewMouseWheel;
            }

        }


        private static void PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true;
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
                {
                    RoutedEvent = UIElement.MouseWheelEvent,
                    Source = sender
                };
                var parent = ((Control)sender).Parent as UIElement;
                parent.RaiseEvent(eventArg);
            }
        }
    }
}
