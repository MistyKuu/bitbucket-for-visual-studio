using GitClientVS.Contracts.Models.GitClientModels;

namespace GitClientVS.Contracts.Interfaces.ViewModels
{
    public interface IPullRequestsDetailViewModel : IInitializable, IViewModelWithErrorMessage, ILoadableViewModel,IWithPageTitle
    {
        GitPullRequest PullRequest { get; set; }
    }
}