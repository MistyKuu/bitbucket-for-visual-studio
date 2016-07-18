using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Models;
using GitClientVS.Infrastructure.Events;

namespace GitClientVS.Services
{
    [Export(typeof(IUserInformationService))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class UserInformationService : IUserInformationService
    {
        private readonly IStorageService _storageService;
        private readonly IDisposable _observable;

        public ConnectionData ConnectionData { get; private set; } = ConnectionData.NotLogged;

        [ImportingConstructor]
        public UserInformationService(
            IEventAggregatorService eventAggregator,
            IStorageService storageService)
        {
            _storageService = storageService;
            _observable = eventAggregator.GetEvent<ConnectionChangedEvent>().Subscribe(ConnectionChanged);
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
