using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace GitClientVS.UI.Behaviours
{
    public class ScrollViewerMonitor
    {
        public static DependencyProperty AtEndCommandProperty
            = DependencyProperty.RegisterAttached(
                "AtEndCommand", typeof(ICommand),
                typeof(ScrollViewerMonitor),
                new PropertyMetadata(OnAtEndCommandChanged));

        public static ICommand GetAtEndCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(AtEndCommandProperty);
        }

        public static void SetAtEndCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(AtEndCommandProperty, value);
        }


        public static void OnAtEndCommandChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)d;
            if (element != null)
            {
                element.Loaded -= element_Loaded;
                element.Loaded += element_Loaded;
            }
        }

        static void element_Loaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            element.Loaded -= element_Loaded;
            ScrollViewer scrollViewer = GetDescendantByType<ScrollViewer>(element);
            if (scrollViewer == null)
            {
                throw new InvalidOperationException("ScrollViewer not found.");
            }

            var dpd = DependencyPropertyDescriptor.FromProperty(ScrollViewer.VerticalOffsetProperty, typeof(ScrollViewer));
            dpd.AddValueChanged(scrollViewer, delegate
            {
                bool atBottom = scrollViewer.VerticalOffset
                                    >= scrollViewer.ScrollableHeight;

                if (atBottom)
                {
                    var atEnd = GetAtEndCommand(element);
                    atEnd?.Execute(null);
                }
            });
        }


        public static TType GetDescendantByType<TType>(Visual element) where TType : Visual
        {
            if (element == null)
            {
                return null;
            }
            if (element.GetType() == typeof(TType))
            {
                return (TType)element;
            }
            TType foundElement = null;
            (element as FrameworkElement)?.ApplyTemplate();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                Visual visual = VisualTreeHelper.GetChild(element, i) as Visual;
                foundElement = GetDescendantByType<TType>(visual);
                if (foundElement != null)
                {
                    break;
                }
            }
            return foundElement;
        }
    }
}
