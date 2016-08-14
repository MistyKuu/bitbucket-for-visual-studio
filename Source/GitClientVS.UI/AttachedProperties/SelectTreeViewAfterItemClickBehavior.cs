using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace GitClientVS.UI.AttachedProperties
{
    public static class SelectTreeViewAfterItemClickBehavior
    {
        public static TreeViewItem GetTreeViewItem(DependencyObject obj)
        {
            return (TreeViewItem)obj.GetValue(TreeViewItemProperty);
        }
        public static void SetTreeViewItem(DependencyObject obj, TreeViewItem value)
        {
            obj.SetValue(TreeViewItemProperty, value);
        }
        // Using a DependencyProperty as the backing store for AllowOnlyString. This enables animation, styling, binding, etc...  
        public static readonly DependencyProperty TreeViewItemProperty =
        DependencyProperty.RegisterAttached("TreeViewItem", typeof(TreeViewItem), typeof(SelectTreeViewAfterItemClickBehavior), new PropertyMetadata(null, TreeViewItemChanged));

        private static Hyperlink _hyperLink;

        private static void TreeViewItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            _hyperLink = d as Hyperlink;
            var treeViewItem = (TreeViewItem)e.NewValue;
            if (_hyperLink != null)
                _hyperLink.Click += delegate
                {
                    if (treeViewItem != null)
                    {
                        treeViewItem.IsSelected = true;
                    }
                };
        }
    }
}
