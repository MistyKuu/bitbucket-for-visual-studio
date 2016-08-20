using GitClientVS.Contracts.Interfaces.Views;

namespace GitClientVS.Contracts.Interfaces.ViewModels
{
    public interface IPullRequestsWindowContainer : IView
    {
        IPullRequestsWindow Window { get; set; }
    }
}