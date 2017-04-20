using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Interfaces.Views;
using GitClientVS.Infrastructure.Extensions;

namespace GitClientVS.UI.Views
{
    /// <summary>
    /// Interaction logic for PublishSectionView.xaml
    /// </summary>
    [Export(typeof(IPublishSectionView))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class PublishSectionView : UserControl, IPublishSectionView
    {
        [ImportingConstructor]
        public PublishSectionView(IPublishSectionViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
            vm.Initialize();
        }
    }
}
