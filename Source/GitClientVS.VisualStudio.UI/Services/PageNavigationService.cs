using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;
using GitClientVS.Contracts.Events;
using GitClientVS.Contracts.Interfaces;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Interfaces.Views;
using Microsoft.TeamFoundation.Controls;
using Microsoft.VisualStudio.Shell;
using ReactiveUI;

namespace GitClientVS.VisualStudio.UI.Services
{
    [Export(typeof(IPageNavigationService<>))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class PageNavigationService<TWindow> : ReactiveObject, IPageNavigationService<TWindow> where TWindow : IWorkflowWindow
    {
        private readonly IAppServiceProvider _appServiceProvider;
        private readonly Subject<NavigationEvent> _navigationSubject = new Subject<NavigationEvent>();
        private readonly ReactiveList<NavigationEvent> _navigationHistory = new ReactiveList<NavigationEvent>();
        private int _currentPageIndex = -1;

        private int CurrentPageIndex
        {
            get
            {
                return _currentPageIndex;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _currentPageIndex, value);
            }
        }

        [ImportingConstructor]
        public PageNavigationService(IAppServiceProvider appServiceProvider)
        {
            _appServiceProvider = appServiceProvider;
        }



        public IObservable<bool> CanNavigateBackObservable
        {
            get { return this.WhenAnyValue(x => x.CurrentPageIndex).Select(_ => CanNavigateBack()); }
        }
        public IObservable<bool> CanNavigateForwardObservable
        {
            get { return this.WhenAnyValue(x => x.CurrentPageIndex).Select(_ => CanNavigateForward()); }
        }

        public void NavigateBack(bool removeFromHistory = false)
        {
            if (CanNavigateBack())
            {
                if (removeFromHistory)
                    _navigationHistory.RemoveAt(CurrentPageIndex);

                CurrentPageIndex--;
                var ev = _navigationHistory[CurrentPageIndex];
                var vm = ev.View.DataContext as IInitializable;
                vm?.InitializeCommand.Execute(ev.Parameter);
                _navigationSubject.OnNext(ev);
            }
        }


        public void ClearNavigationHistory()
        {
            _navigationHistory.Clear();
            CurrentPageIndex = -1;
        }

        public void NavigateForward()
        {
            if (CanNavigateForward())
            {
                CurrentPageIndex++;
                var ev = _navigationHistory[CurrentPageIndex];
                _navigationSubject.OnNext(ev);
            }
        }

        public void Navigate<TView>(object parameter = null) where TView : class, IView
        {
            ClearForwardNavigationHistory();
            var view = _appServiceProvider.GetService<TView>();
            var vm = view.DataContext as IInitializable;
            vm?.InitializeCommand.Execute(parameter);

            var ev = new NavigationEvent(typeof(TWindow), view) { Parameter = parameter };
            _navigationSubject.OnNext(ev);

            CurrentPageIndex++;
            _navigationHistory.Add(ev);
        }


        private void ClearForwardNavigationHistory()
        {
            for (int i = CurrentPageIndex + 1; i < _navigationHistory.Count; i++)
                _navigationHistory.RemoveAt(i);
        }

        public IDisposable Subscribe(IObserver<NavigationEvent> observer)
        {
            return _navigationSubject.Subscribe(observer);
        }

        private bool CanNavigateForward()
        {
            return CurrentPageIndex + 1 < _navigationHistory.Count;
        }

        private bool CanNavigateBack()
        {
            return CurrentPageIndex - 1 >= 0;
        }
    }
}
