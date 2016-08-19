using System.ComponentModel.Composition;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Interfaces.Views;
using GitClientVS.Contracts.Models.GitClientModels;
using GitClientVS.Infrastructure;
using GitClientVS.UI;
using GitClientVS.VisualStudio.UI.TeamFoundation;
using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.Controls.WPF.TeamExplorer.Framework;

namespace GitClientVS.VisualStudio.UI.Pages
{
    [TeamExplorerPage(PageIds.PullRequestsDetailPageId)]
    public class PullRequestDetailsPage : TeamExplorerBasePage
    {
        private readonly IPullRequestDetailView _view;
        private static GitPullRequest _lastPullReq = null;

        [ImportingConstructor]
        public PullRequestDetailsPage(IPullRequestDetailView view)
        {
            Title = Resources.PullRequestDetailsPageTitle;
            _view = view;
            PageContent = view;
        }

        public override void Initialize(object sender, PageInitializeEventArgs e)
        {
            var gitPullRequest = (GitPullRequest)e.Context;
            if (gitPullRequest != null)
            {
                Title += $" #{gitPullRequest.Id}";
                _view.InitializeCommand.Execute(gitPullRequest);
                _lastPullReq = gitPullRequest;
            }
            else if (_lastPullReq != null)
            {
                Title += $" #{_lastPullReq.Id}";
                _view.InitializeCommand.Execute(_lastPullReq);
            }
        }

    }
}
