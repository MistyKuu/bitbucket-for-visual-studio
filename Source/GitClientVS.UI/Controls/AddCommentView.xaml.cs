using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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



        public object AddCommandParameter
        {
            get { return (object)GetValue(AddCommandParameterProperty); }
            set { SetValue(AddCommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AddCommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddCommandParameterProperty =
            DependencyProperty.Register("AddCommandParameter", typeof(object), typeof(AddCommentView), new PropertyMetadata(null));




        public string ButtonLabelContent
        {
            get { return (string)GetValue(ButtonLabelContentProperty); }
            set { SetValue(ButtonLabelContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ButtonLabelContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonLabelContentProperty =
            DependencyProperty.Register("ButtonLabelContent", typeof(string), typeof(AddCommentView), new PropertyMetadata(null));

        private ICommand _cancelCommand;


        public ICommand CancelCommand => _cancelCommand ?? (_cancelCommand = ReactiveCommand.Create(() =>
                                             {
                                                 Visibility = Visibility.Collapsed;
                                             }));

        public AddCommentView()
        {
            InitializeComponent();
            (this.Content as FrameworkElement).DataContext = this;
        }
    }
}
