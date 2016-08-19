using System;
using GitClientVS.Contracts.Events;
using GitClientVS.Contracts.Interfaces.Views;

namespace GitClientVS.Contracts.Interfaces.Services
{
    public interface IPageNavigationService<TWindow> : IObservable<NavigationEvent> where TWindow : IWorkflowWindow
    {
        void NavigateBack();
        void Navigate<TView>(object parameter = null) where TView : class, IView;
        IObservable<bool> CanNavigateBackObservable { get; }
        IObservable<bool> CanNavigateForwardObservable { get; }
        void NavigateForward();
    }
}