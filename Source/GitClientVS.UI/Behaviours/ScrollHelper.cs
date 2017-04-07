using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using WpfControls;

namespace GitClientVS.UI.Behaviours
{
    public static class ScrollHelper
    {
        public static readonly DependencyProperty ScrollToBottomProperty = DependencyProperty.RegisterAttached("ScrollToBottom", typeof(ICommand), typeof(ScrollHelper), new FrameworkPropertyMetadata(null, OnScrollToBottomPropertyChanged));

        public static ICommand GetScrollToBottom(DependencyObject ob)
        {
            return (ICommand)ob.GetValue(ScrollToBottomProperty);
        }

        public static void SetScrollToBottom(DependencyObject ob, ICommand value)
        {
            ob.SetValue(ScrollToBottomProperty, value);
        }

        private static void OnScrollToBottomPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var scrollViewer = obj as ScrollViewer;

            scrollViewer.Loaded += OnScrollViewerLoaded;

        }

        private static void OnScrollViewerLoaded(object sender, RoutedEventArgs e)
        {
            (sender as ScrollViewer).Loaded -= OnScrollViewerLoaded;

            (sender as ScrollViewer).Unloaded += OnScrollViewerUnloaded;
            (sender as ScrollViewer).ScrollChanged += OnScrollViewerScrollChanged;
        }

        private static void OnScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalChange > 0)
            {
                if (e.VerticalOffset + e.ViewportHeight == e.ExtentHeight)
                {
                    var command = GetScrollToBottom((ScrollViewer)sender);
                    if (command == null || !command.CanExecute(null))
                        return;

                    command.Execute(null);
                }
            }
        }

        private static void OnScrollViewerUnloaded(object sender, RoutedEventArgs e)
        {
            (sender as ScrollViewer).Unloaded -= OnScrollViewerUnloaded;
            (sender as ScrollViewer).ScrollChanged -= OnScrollViewerScrollChanged;
        }

    }
}
