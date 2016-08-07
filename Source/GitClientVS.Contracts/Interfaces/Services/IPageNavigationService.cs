namespace GitClientVS.Contracts.Interfaces.Services
{
    public interface IPageNavigationService
    {
        void NavigateBack();
        void Navigate(string pageId, object parameter = null);
    }
}