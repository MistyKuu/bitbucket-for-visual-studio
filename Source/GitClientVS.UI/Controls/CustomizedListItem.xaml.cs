using System;
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
            DependencyProperty.Register("Title", typeof (string), typeof (CustomizedListItem),
                new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for Date.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DateProperty =
            DependencyProperty.Register("Date", typeof (string), typeof (CustomizedListItem),
                new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for Body.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BodyProperty =
            DependencyProperty.Register("Body", typeof (string), typeof (CustomizedListItem), new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for Sender.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SenderProperty =
            DependencyProperty.Register("Sender", typeof (string), typeof (CustomizedListItem),
                new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for UnderImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnderImageProperty =
            DependencyProperty.Register("UnderImage", typeof (string), typeof (CustomizedListItem),
                new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for Image.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof (ImageSource), typeof (CustomizedListItem),
                new PropertyMetadata(null));

        public static readonly DependencyProperty DetailsCommandProperty =
        DependencyProperty.Register("DetailsCommand", typeof(ICommand), typeof(CustomizedListItem), new PropertyMetadata(null));

        public static readonly DependencyProperty DetailsCommandParameterProperty =
          DependencyProperty.Register("DetailsCommandParameter", typeof(object), typeof(CustomizedListItem), new PropertyMetadata(null));

        public CustomizedListItem()
        {
            InitializeComponent();
        }

        public string Title
        {
            get { return (string) GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public string Date
        {
            get { return (string) GetValue(DateProperty); }
            set { SetValue(DateProperty, value); }
        }

        public string Body
        {
            get { return (string) GetValue(BodyProperty); }
            set { SetValue(BodyProperty, value); }
        }

        public string Sender
        {
            get { return (string) GetValue(SenderProperty); }
            set { SetValue(SenderProperty, value); }
        }

        public string UnderImage
        {
            get { return (string) GetValue(UnderImageProperty); }
            set { SetValue(UnderImageProperty, value); }
        }

        public ImageSource Image
        {
            get { return (ImageSource) GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

    
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

        public event EventHandler Removed;


        private void OnRemoved()
        {
            var evt = Removed;
            evt?.Invoke(this, new EventArgs());
        }
    }
}