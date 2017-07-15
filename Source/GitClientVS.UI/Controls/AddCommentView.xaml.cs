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
using GitClientVS.Contracts.Models.GitClientModels;
using GitClientVS.Contracts.Models.Tree;
using ReactiveUI;

namespace GitClientVS.UI.Controls
{
    /// <summary>
    /// Interaction logic for AddCommentView.xaml
    /// </summary>
    public partial class AddCommentView : UserControl
    {
        public string CurrentText
        {
            get { return (string)GetValue(CurrentTextProperty); }
            set { SetValue(CurrentTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentTextProperty =
            DependencyProperty.Register("CurrentText", typeof(string), typeof(AddCommentView), new PropertyMetadata(null));

        public ICommand AddCommand
        {
            get { return (ICommand)GetValue(AddCommandProperty); }
            set { SetValue(AddCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AddCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddCommandProperty =
            DependencyProperty.Register("AddCommand", typeof(ICommand), typeof(AddCommentView), new PropertyMetadata(null));


        public GitCommentInline Inline
        {
            get { return (GitCommentInline)GetValue(InlineProperty); }
            set { SetValue(InlineProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Inline.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InlineProperty =
            DependencyProperty.Register("Inline", typeof(GitCommentInline), typeof(AddCommentView), new PropertyMetadata(null));



        public AddCommentView()
        {
            InitializeComponent();
            (this.Content as FrameworkElement).DataContext = this;
        }
    }
}
