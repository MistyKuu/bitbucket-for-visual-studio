using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Input;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.VisualStudio.UI.Pages;
using Microsoft.TeamFoundation.Controls;
using Microsoft.VisualStudio.Shell;

namespace GitClientVS.VisualStudio.UI.Services
{
    [Export(typeof(IPageNavigationService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class PageNavigationService : IPageNavigationService
    {
        private readonly IAppServiceProvider _appServiceProvider;

        [ImportingConstructor]
        public PageNavigationService(IAppServiceProvider appServiceProvider)
        {
            _appServiceProvider = appServiceProvider;
        }

        public void Navigate(string pageId, object parameter = null)
        {
            ITeamExplorer service = _appServiceProvider.GetService<ITeamExplorer>();
            service?.NavigateToPage(new Guid(pageId), parameter);
        }

        public void NavigateBack()
        {
            ITeamExplorer service = _appServiceProvider.GetService<ITeamExplorer>();
            service?.ClosePage(service.CurrentPage);
        }
    }
}
