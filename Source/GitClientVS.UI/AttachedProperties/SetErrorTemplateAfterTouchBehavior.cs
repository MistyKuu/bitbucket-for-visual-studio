using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;
using MahApps.Metro.Controls;
using Microsoft.VisualStudio.PlatformUI;

namespace GitClientVS.UI.AttachedProperties
{
    public class SetErrorTemplateAfterTouchBehavior : AttachableForStyleBehavior<TextBox, SetErrorTemplateAfterTouchBehavior>
    {
        private TextBox _textBox;

        private ControlTemplate _actualTemplate;

        protected override void OnAttached()
        {
            base.OnAttached();
            _textBox = AssociatedObject;
            _textBox.TextChanged += _textBox_TextChanged;
            _textBox.Loaded += _textBox_Loaded;
        }

        private void _textBox_Loaded(object sender, RoutedEventArgs e)
        {
            _actualTemplate = Validation.GetErrorTemplate(_textBox);
            Validation.SetErrorTemplate(_textBox, new ControlTemplate());
        }

        private void _textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_actualTemplate != null)
                Validation.SetErrorTemplate(_textBox, _actualTemplate);
        }
    }
}
