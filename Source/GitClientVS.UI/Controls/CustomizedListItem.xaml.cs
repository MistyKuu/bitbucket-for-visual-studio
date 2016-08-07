using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GitClientVS.UI.Controls
{
    public sealed partial class CustomizedListItem : UserControl
    {
        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(CustomizedListItem),
                new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for Date.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DateProperty =
            DependencyProperty.Register("Date", typeof(string), typeof(CustomizedListItem),
                new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for Body.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BodyProperty =
            DependencyProperty.Register("Body", typeof(string), typeof(CustomizedListItem), new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for Sender.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SenderProperty =
            DependencyProperty.Register("Sender", typeof(string), typeof(CustomizedListItem),
                new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for UnderImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnderImageProperty =
            DependencyProperty.Register("UnderImage", typeof(string), typeof(CustomizedListItem),
                new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for Image.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(ImageSource), typeof(CustomizedListItem),
                new PropertyMetadata(null));

        public static readonly DependencyProperty DetailsCommandProperty =
        DependencyProperty.Register("DetailsCommand", typeof(ICommand), typeof(CustomizedListItem), new PropertyMetadata(null));

        public static readonly DependencyProperty DetailsCommandParameterProperty =
          DependencyProperty.Register("DetailsCommandParameter", typeof(object), typeof(CustomizedListItem), new PropertyMetadata(null));




        public string GoToLink
        {
            get { return (string)GetValue(GoToLinkProperty); }
            set { SetValue(GoToLinkProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GoToLink.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GoToLinkProperty =
            DependencyProperty.Register("GoToLink", typeof(string), typeof(CustomizedListItem), new PropertyMetadata(null));



        public CustomizedListItem()
        {
            InitializeComponent();
            this.MouseDown += CustomizedListItem_MouseDown;
            (this.Content as FrameworkElement).DataContext = this;
        }



        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public string Date
        {
            get { return (string)GetValue(DateProperty); }
            set { SetValue(DateProperty, value); }
        }

        public string Body
        {
            get { return (string)GetValue(BodyProperty); }
            set { SetValue(BodyProperty, value); }
        }

        public string Sender
        {
            get { return (string)GetValue(SenderProperty); }
            set { SetValue(SenderProperty, value); }
        }

        public string UnderImage
        {
            get { return (string)GetValue(UnderImageProperty); }
            set { SetValue(UnderImageProperty, value); }
        }

        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }


        public string DateSeparator
        {
            get { return (string)GetValue(DateSeparatorProperty); }
            set { SetValue(DateSeparatorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DateSeparator.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DateSeparatorProperty =
            DependencyProperty.Register("DateSeparator", typeof(string), typeof(CustomizedListItem), new PropertyMetadata(null));



        public TextWrapping TextWrapping
        {
            get { return (TextWrapping)GetValue(TextWrappingProperty); }
            set { SetValue(TextWrappingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextWrapping.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextWrappingProperty =
            DependencyProperty.Register("TextWrapping", typeof(TextWrapping), typeof(CustomizedListItem), new PropertyMetadata(TextWrapping.NoWrap));




        public TextTrimming TextTrimming
        {
            get { return (TextTrimming)GetValue(TextTrimmingProperty); }
            set { SetValue(TextTrimmingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextTrimming.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextTrimmingProperty =
            DependencyProperty.Register("TextTrimming", typeof(TextTrimming), typeof(CustomizedListItem), new PropertyMetadata(TextTrimming.None));





        public ICommand DetailsCommand
        {
            get { return (ICommand)GetValue(DetailsCommandProperty); }
            set { SetValue(DetailsCommandProperty, value); }
        }

        public object DetailsCommandParameter
        {
            get { return (object)GetValue(DetailsCommandParameterProperty); }
            set { SetValue(DetailsCommandParameterProperty, value); }
        }

        private void CustomizedListItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (DetailsCommand != null && DetailsCommand.CanExecute(DetailsCommandParameter))
                DetailsCommand.Execute(DetailsCommandParameter);
        }

        private void NavigateButton_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start(GoToLink);
        }
    }
}