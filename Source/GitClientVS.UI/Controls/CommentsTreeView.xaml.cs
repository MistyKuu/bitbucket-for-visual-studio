using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using GitClientVS.Contracts.Models.GitClientModels;
using GitClientVS.Contracts.Models.Tree;
using GitClientVS.UI.Converters;
using GitClientVS.UI.Helpers;
using MahApps.Metro.Controls;
using ReactiveUI;

namespace GitClientVS.UI.Controls
{
    /// <summary>
    /// Interaction logic for CommentsTreeView.xaml
    /// </summary>
    public partial class CommentsTreeView : UserControl
    {
        public GitComment TrackedItem
        {
            get { return (GitComment)GetValue(TrackedItemProperty); }
            set { SetValue(TrackedItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TrackedItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TrackedItemProperty =
            DependencyProperty.Register("TrackedItem", typeof(GitComment), typeof(CommentsTreeView), new PropertyMetadata(null));



        public List<ICommentTree> CommentTree
        {
            get { return (List<ICommentTree>)GetValue(CommentTreeProperty); }
            set { SetValue(CommentTreeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommentTree.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommentTreeProperty =
            DependencyProperty.Register("CommentTree", typeof(List<ICommentTree>), typeof(CommentsTreeView), new PropertyMetadata(null));



        public ICommentTree SingleCommentTree
        {
            get { return (ICommentTree)GetValue(SingleCommentTreeProperty); }
            set { SetValue(SingleCommentTreeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SingleCommentTree.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SingleCommentTreeProperty =
            DependencyProperty.Register("SingleCommentTree", typeof(ICommentTree), typeof(CommentsTreeView), new PropertyMetadata(OnSingleCommentTreeChanged));

        private static void OnSingleCommentTreeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var treeView = d as CommentsTreeView;
            if (treeView != null)
                treeView.CommentTree = new List<ICommentTree>() { (ICommentTree)e.NewValue };
        }


        public ICommand ReplyCommand
        {
            get { return (ICommand)GetValue(ReplyCommandProperty); }
            set { SetValue(ReplyCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ReplyCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ReplyCommandProperty =
            DependencyProperty.Register("ReplyCommand", typeof(ICommand), typeof(CommentsTreeView), new PropertyMetadata(null));




        public ICommand EditCommand
        {
            get { return (ICommand)GetValue(EditCommandProperty); }
            set { SetValue(EditCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EditCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EditCommandProperty =
            DependencyProperty.Register("EditCommand", typeof(ICommand), typeof(CommentsTreeView), new PropertyMetadata(null));

        public ICommand DeleteCommand
        {
            get { return (ICommand)GetValue(DeleteCommandProperty); }
            set { SetValue(DeleteCommandProperty, value); }
        }

        public ICommand EnterEditModeCommand => _enterEditModeCommand ?? (_enterEditModeCommand = ReactiveCommand.Create<ICommentTree>(EnterEditMode));
        public ICommand EnterReplyModeCommand => _enterReplyModeCommand ?? (_enterReplyModeCommand = ReactiveCommand.Create<ICommentTree>(EnterReplyMode));

        // Using a DependencyProperty as the backing store for DeleteCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DeleteCommandProperty =
            DependencyProperty.Register("DeleteCommand", typeof(ICommand), typeof(CommentsTreeView), new PropertyMetadata(null));



        public string UserName
        {
            get { return (string)GetValue(UserNameProperty); }
            set { SetValue(UserNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UserName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UserNameProperty =
            DependencyProperty.Register("UserName", typeof(string), typeof(CommentsTreeView), new PropertyMetadata(null));




        private ICommand _enterEditModeCommand;
        private ICommand _enterReplyModeCommand;
        private ScrollViewer _sv;

        public CommentsTreeView()
        {
            InitializeComponent();
            TView.Loaded += TView_Loaded;
        }

        private void TView_Loaded(object sender, RoutedEventArgs e)
        {
            if (VisualTreeHelper.GetChildrenCount(TView) > 0)
            {
                var border = VisualTreeHelper.GetChild(TView, 0);
                _sv = VisualTreeHelper.GetChild(border, 0) as ScrollViewer;
                if (_sv != null)
                    _sv.PreviewMouseWheel += Sv_PreviewMouseWheel;
            }

        }

        private void Sv_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            _sv.ScrollToVerticalOffset(_sv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void EnterReplyMode(ICommentTree commentTree)
        {
            if (commentTree != null)
            {
                commentTree.IsReplyExpanded = !commentTree.IsReplyExpanded;

                if (commentTree.IsReplyExpanded)
                    commentTree.IsEditExpanded = false;
            }
        }

        private void EnterEditMode(ICommentTree commentTree)
        {
            if (commentTree != null)
            {
                commentTree.IsEditExpanded = !commentTree.IsEditExpanded;

                if (commentTree.IsEditExpanded)
                    commentTree.IsReplyExpanded = false;
            }
        }
    }
}
