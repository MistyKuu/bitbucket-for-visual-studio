using GitClientVS.Contracts.Models.GitClientModels;

namespace GitClientVS.Contracts.Interfaces.ViewModels
{
    public interface IPullRequestsDetailViewModel : IInitializable, IViewModelWithErrorMessage, ILoadableViewModel,IWithTitle
    {
        GitPullRequest PullRequest { get; set; }
    }
}