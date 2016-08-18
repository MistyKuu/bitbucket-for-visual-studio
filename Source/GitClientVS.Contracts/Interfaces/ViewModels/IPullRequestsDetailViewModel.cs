using GitClientVS.Contracts.Models.GitClientModels;

namespace GitClientVS.Contracts.Interfaces.ViewModels
{
    public interface IPullRequestsDetailViewModel : IInitializable, IViewModelWithErrorMessage, ILoadableViewModel
    {
        GitPullRequest PullRequest { get; set; }
    }
}