using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GitClientVS.Contracts.Models.Tree;

namespace GitClientVS.UI.Controls
{
    /// <summary>
    /// Interaction logic for CommentsTreeView.xaml
    /// </summary>
    public partial class CommentsTreeView : UserControl
    {
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


        public CommentsTreeView()
        {
            InitializeComponent();
        }
    }
}
