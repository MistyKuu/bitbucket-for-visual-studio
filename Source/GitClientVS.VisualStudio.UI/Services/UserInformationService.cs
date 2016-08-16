using System;
using System.ComponentModel.Composition;
using System.Reactive.Linq;
using System.Windows.Input;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Models;
using GitClientVS.Infrastructure.Events;
using GitClientVS.UI.Helpers;
using Microsoft.VisualStudio.PlatformUI;

namespace GitClientVS.VisualStudio.UI.Services
{
    [Export(typeof(IUserInformationService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class UserInformationService : IUserInformationService
    {
        private readonly IEventAggregatorService _eventAggregator;
        private readonly IStorageService _storageService;
        private IDisposable _observable;
        private IDisposable _themeObs;

        public ConnectionData ConnectionData { get; private set; } = ConnectionData.NotLogged;
        public Theme CurrentTheme { get; private set; } = ConvertToTheme(VSHelpers.DetectTheme());

        [ImportingConstructor]
        public UserInformationService(
            IEventAggregatorService eventAggregator,
            IStorageService storageService)
        {
            _eventAggregator = eventAggregator;
            _storageService = storageService;
        }

        public void StartListening()
        {
            _observable = _eventAggregator.GetEvent<ConnectionChangedEvent>().Subscribe(ConnectionChanged);
            _themeObs = Observable
                .FromEvent<ThemeChangedEventHandler, ThemeChangedEventArgs>(handler => VSColorTheme.ThemeChanged += handler, handler => VSColorTheme.ThemeChanged -= handler)
                .Subscribe(args =>
                {
                    CurrentTheme = ConvertToTheme(VSHelpers.DetectTheme());
                    _eventAggregator.Publish(new ThemeChangedEvent(CurrentTheme));
                });
        }

        private static Theme ConvertToTheme(string theme)
        {
            return theme == "Dark" ? Theme.Dark : Theme.Light;
        }

        public void Dispose()
        {
            _observable?.Dispose();
            _themeObs?.Dispose();
        }

        private void ConnectionChanged(ConnectionChangedEvent connectionChangedEvent)
        {
            ConnectionData = connectionChangedEvent.Data;
            _storageService.SaveUserData(ConnectionData);
        }
    }
}
