using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using GitClientVS.Contracts.Models.Tree;
using GitClientVS.UI.Converters;
using HTMLConverter;
using ReactiveUI;

namespace GitClientVS.UI.Controls
{
    public class HtmlRichTextBoxBehavior : DependencyObject
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.RegisterAttached("Text", typeof(string),
                typeof(HtmlRichTextBoxBehavior), new UIPropertyMetadata(null, OnValueChanged));

        public static string GetText(RichTextBox o) { return (string)o.GetValue(TextProperty); }

        public static void SetText(RichTextBox o, string value) { o.SetValue(TextProperty, value); }

        private static void OnValueChanged(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            var richTextBox = (RichTextBox)dependencyObject;
            var text = (e.NewValue ?? string.Empty).ToString();
            var xaml = HtmlToXamlConverter.ConvertHtmlToXaml(text, true);
            var flowDocument = XamlReader.Parse(xaml) as FlowDocument;
            HyperlinksSubscriptions(flowDocument);
            richTextBox.Document = flowDocument;
        }

        private static void HyperlinksSubscriptions(FlowDocument flowDocument)
        {
            if (flowDocument == null) return;
            GetVisualChildren(flowDocument).OfType<Hyperlink>().ToList()
                .ForEach(i => i.RequestNavigate += HyperlinkNavigate);
        }

        private static IEnumerable<DependencyObject> GetVisualChildren(DependencyObject root)
        {
            foreach (var child in LogicalTreeHelper.GetChildren(root).OfType<DependencyObject>())
            {
                yield return child;
                foreach (var descendants in GetVisualChildren(child)) yield return descendants;
            }
        }

        private static void HyperlinkNavigate(object sender,
            System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
    public class WebBrowserBehavior
    {
        public static readonly DependencyProperty BodyProperty =
            DependencyProperty.RegisterAttached("Body", typeof(string), typeof(WebBrowserBehavior),
                new PropertyMetadata(OnChanged));

        public static string GetBody(DependencyObject dependencyObject)
        {
            return (string)dependencyObject.GetValue(BodyProperty);
        }

        public static void SetBody(DependencyObject dependencyObject, string body)
        {
            dependencyObject.SetValue(BodyProperty, body);
        }

        private static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var browser = ((WebBrowser) d);
            browser.LoadCompleted += Browser_LoadCompleted;
            browser.Navigating += Browser_Navigating;

            browser.NavigateToString((string) e.NewValue);
        }

        private static void Browser_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            var browser = ((WebBrowser)sender);
            browser.Width = 0;
            browser.Height = 0;
        }

        private static void Browser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            var browser = ((WebBrowser)sender);
            browser.Width = ((dynamic)browser.Document).Body.parentElement.scrollWidth;
            browser.Height = ((dynamic)browser.Document).Body.parentElement.scrollHeight;
        }
    }
    public class HtmlToFlowDocConverter : BaseMarkupExtensionConverter
    {
        public override object Convert(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            var xaml = HtmlToXamlConverter.ConvertHtmlToXaml((string)value, true);
            var flowDocument = XamlReader.Parse(xaml);
            if (flowDocument is FlowDocument)
                SubscribeToAllHyperlinks((FlowDocument)flowDocument);
            return flowDocument;
        }

        private void SubscribeToAllHyperlinks(FlowDocument flowDocument)
        {
            var hyperlinks = GetVisuals(flowDocument).OfType<Hyperlink>();
            foreach (var link in hyperlinks)
                link.RequestNavigate += LinkRequestNavigate;
        }

        private static IEnumerable<DependencyObject> GetVisuals(DependencyObject root)
        {
            foreach (var child in
                LogicalTreeHelper.GetChildren(root).OfType<DependencyObject>())
            {
                yield return child;
                foreach (var descendants in GetVisuals(child))
                    yield return descendants;
            }
        }

        private void LinkRequestNavigate(object sender,
            System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        public override object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
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

        public CommentsTreeView()
        {
            InitializeComponent();
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
