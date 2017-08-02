using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;
using GitClientVS.Contracts.Models.GitClientModels;
using GitClientVS.Contracts.Models.Tree;

namespace GitClientVS.UI.Behaviours
{
    public class MoveToTouchedTreeViewItem : Behavior<TreeView> //todo one day make it generic for all treeviews :)
    {
        private TreeView _tv;

        protected override void OnAttached()
        {
            base.OnAttached();
            _tv = AssociatedObject;
        }

        public GitComment TrackedItem
        {
            get { return (GitComment)GetValue(TrackedItemProperty); }
            set { SetValue(TrackedItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TrackedItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TrackedItemProperty =
            DependencyProperty.Register("TrackedItem", typeof(GitComment), typeof(MoveToTouchedTreeViewItem), new PropertyMetadata(null, OnTrackedItemChanged));

        private static void OnTrackedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var beh = d as MoveToTouchedTreeViewItem;
            var newObject = e.NewValue;
            ShowSelectedThing(beh?.AssociatedObject, (GitComment)newObject);
        }

        private static bool ShowSelectedThing(ItemsControl parentContainer, GitComment objectToFind)
        {
            if (objectToFind == null)
                return false;

            // check current level of tree
            foreach (ICommentTree item in parentContainer.Items)
            {
                TreeViewItem currentContainer = (TreeViewItem)parentContainer.ItemContainerGenerator.ContainerFromItem(item);
                if ((currentContainer != null) && (item.Comment.Id == objectToFind.Id))
                {
                    currentContainer.BringIntoView();
                    return true;
                }
            }
            // item is not found at current level, check the kids
            foreach (ICommentTree item in parentContainer.Items)
            {
                TreeViewItem currentContainer = (TreeViewItem)parentContainer.ItemContainerGenerator.ContainerFromItem(item);
                if ((currentContainer != null) && (currentContainer.Items.Count > 0))
                {
                    // Have to expand the currentContainer or you can't look at the children
                    currentContainer.IsExpanded = true;
                    currentContainer.UpdateLayout();
                    if (!ShowSelectedThing(currentContainer, objectToFind))
                    {
                        // Haven't found the thing, so collapse it back
                        currentContainer.IsExpanded = false;
                    }
                    else
                    {
                        // We found the thing
                        return true;
                    }
                }
            }
            // default
            return false;
        }
    }
}