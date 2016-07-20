using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GitClientVS.UI.AttachedProperties
{
    public class TouchTracker : DependencyObject
    {
        public static bool GetIsTouchedEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsTouchedEnabledProperty);
        }
        public static void SetIsTouchedEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsTouchedEnabledProperty, value);
        }

        public static readonly DependencyProperty IsTouchedEnabledProperty = DependencyProperty.RegisterAttached("IsTouchedEnabled", typeof(bool), typeof(TouchTracker), new UIPropertyMetadata(false, OnAttached));


        public static bool GetIsTouched(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsTouchedProperty);
        }
        public static void SetIsTouched(DependencyObject obj, bool value)
        {
            obj.SetValue(IsTouchedProperty, value);
        }

        public static readonly DependencyProperty IsTouchedProperty = DependencyProperty.RegisterAttached("IsTouched", typeof(bool), typeof(TouchTracker), new UIPropertyMetadata(false));


        private static void OnAttached(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = d as TextBox;
            if (obj == null)
                throw new Exception("TouchTracker can be only assigned to TextBox");

            obj.TextChanged += (s, ev) =>
            {
                obj.SetValue(IsTouchedProperty, true);
            };
        }

    }
}
