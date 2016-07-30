namespace GitClientVS.Contracts.Interfaces.Services
{
    public interface IPageNavigationService
    {
        void Navigate(string pageId);
        void NavigateBack();
    }
}