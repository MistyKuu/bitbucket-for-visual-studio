using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Interfaces.Views;

namespace GitClientVS.UI.Views
{
    /// <summary>
    /// Interaction logic for PublishSectionView.xaml
    /// </summary>
    [Export(typeof(IPublishSectionView))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class PublishSectionView : UserControl, IPublishSectionView
    {
        private readonly IPublishSectionViewModel _vm;

        [ImportingConstructor]
        public PublishSectionView(IPublishSectionViewModel vm)
        {
            _vm = vm;
            DataContext = vm;
            InitializeComponent();
            Loaded += ViewLoaded;
        }

        private void ViewLoaded(object sender, EventArgs e)
        {
            _vm.InitializeCommand.Execute(null);
        }
    }
}
