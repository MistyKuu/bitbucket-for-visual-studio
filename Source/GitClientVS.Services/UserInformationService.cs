using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Models;
using GitClientVS.Infrastructure.Events;

namespace GitClientVS.Services
{
    [Export(typeof(IUserInformationService))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class UserInformationService : IUserInformationService
    {
        private readonly IEventAggregatorService _eventAggregator;
        private readonly IStorageService _storageService;
        private readonly IDisposable _observable;

        public ConnectionData ConnectionData { get; private set; } = ConnectionData.NotLogged;

        [ImportingConstructor]
        public UserInformationService(IEventAggregatorService eventAggregator, IStorageService storageService)
        {
            _eventAggregator = eventAggregator;
            _storageService = storageService;
            _observable = _eventAggregator.GetEvent<ConnectionChangedEvent>().Subscribe(ConnectionChanged);
        }


        public void LoadStoreInformation()
        {
            var result = _storageService.LoadUserData();
            ConnectionData = result.IsSuccess ? result.Data : ConnectionData.NotLogged;
            _eventAggregator.Publish(new ConnectionChangedEvent(ConnectionData));
        }

        public void Dispose()
        {
            _observable.Dispose();
        }

        private void ConnectionChanged(ConnectionChangedEvent connectionChangedEvent)
        {
            ConnectionData = connectionChangedEvent.Data;
            _storageService.SaveUserData(ConnectionData);
        }
    }
}
