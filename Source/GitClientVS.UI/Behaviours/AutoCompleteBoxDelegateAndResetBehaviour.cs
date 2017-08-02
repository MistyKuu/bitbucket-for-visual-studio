using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interactivity;
using GitClientVS.Contracts.Models.GitClientModels;
using WpfControls;

namespace GitClientVS.UI.Behaviours
{
    public class AutoCompleteBoxDelegateAndResetBehaviour : Behavior<AutoCompleteTextBox>
    {
        private AutoCompleteTextBox _tb;

        public IList SelectedItems
        {
            get { return (IList)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof(IList), typeof(AutoCompleteBoxDelegateAndResetBehaviour), new PropertyMetadata(null));



        protected override void OnAttached()
        {
            base.OnAttached();
            _tb = AssociatedObject;
            DependencyPropertyDescriptor
                             .FromProperty(AutoCompleteTextBox.SelectedItemProperty, typeof(AutoCompleteTextBox))
                             .AddValueChanged(_tb, SelectedItemChanged);
        }

        private void SelectedItemChanged(object sender, EventArgs e)
        {
            if (_tb.SelectedItem != null && _tb.Text != null)
            {
                SelectedItems.Add(_tb.SelectedItem);
            }

            _tb.SelectedItem = null;
            _tb.Text = null;
            _tb.Editor.Text = null;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            DependencyPropertyDescriptor
                            .FromProperty(AutoCompleteTextBox.SelectedItemProperty, typeof(AutoCompleteTextBox))
                            .RemoveValueChanged(_tb, SelectedItemChanged);
        }
    }
}
