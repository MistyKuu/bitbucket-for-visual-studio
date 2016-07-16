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
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class UserInformationService : IUserInformationService
    {
        private readonly IDisposable _observable;

        public ConnectionData ConnectionData { get; private set; } = ConnectionData.NotLogged;

        public UserInformationService(IEventAggregatorService eventAggregator)
        {
            _observable = eventAggregator.GetEvent<ConnectionChangedEvent>().Subscribe(ConnectionChanged);
        }

       
        public void LoadStoreInformation()
        {

        }

        public void Dispose()
        {
            _observable.Dispose();
        }

        private void ConnectionChanged(ConnectionChangedEvent connectionChangedEvent)
        {
            ConnectionData = connectionChangedEvent.Data;
        }
    }
}
