using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GitClientVS.Contracts.Models.GitClientModels;
using GitClientVS.Contracts.Models.Tree;
using GitClientVS.UI.Controls.DiffControlUtils;
using ParseDiff;
using ReactiveUI;

namespace GitClientVS.UI.Controls
{
    /// <summary>
    /// Interaction logic for AddCommentView.xaml
    /// </summary>
    public partial class MarginControl : UserControl
    {
        public ChunkDiff Chunk
        {
            get { return (ChunkDiff)GetValue(ChunkProperty); }
            set { SetValue(ChunkProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Chunk.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChunkProperty =
            DependencyProperty.Register("Chunk", typeof(ChunkDiff), typeof(MarginControl), new PropertyMetadata(null));




        public ICommand EnterAddModeCommand
        {
            get { return (ICommand)GetValue(EnterAddModeCommandProperty); }
            set { SetValue(EnterAddModeCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnterAddModeCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnterAddModeCommandProperty =
            DependencyProperty.Register("EnterAddModeCommand", typeof(ICommand), typeof(MarginControl), new PropertyMetadata(null));



        public MarginControl()
        {
            InitializeComponent();
            (this.Content as FrameworkElement).DataContext = this;
        }
    }
}
