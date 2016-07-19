using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitBucket.REST.API;
using BitBucket.REST.API.Models;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Models;
using GitClientVS.Infrastructure.Events;

namespace GitClientVS.Services
{
    [Export(typeof(IGitClientService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class BitbucketService : IGitClientService
    {
        private readonly IEventAggregatorService _eventAggregator;
        private BitbucketClient _bitbucketClient;

        public bool IsConnected => _bitbucketClient != null;

        [ImportingConstructor]
        public BitbucketService(IEventAggregatorService eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public string Title => "Bitbucket Extension";

        public async Task LoginAsync(string login, string password)
        {
            if (IsConnected)
                return;

            var credentials = new Credentials(login, password);
            var connection = new Connection(credentials);
            var bitbucketInitializer = new BitbucketClientInitializer(connection);
            _bitbucketClient = await bitbucketInitializer.Initialize();
            OnConnectionChanged(ConnectionData.Create(login, password));
        }

        public void Logout()
        {
            _bitbucketClient = null;
            OnConnectionChanged(ConnectionData.NotLogged);
        }

        private void OnConnectionChanged(ConnectionData connectionData)
        {
            _eventAggregator.Publish(new ConnectionChangedEvent(connectionData));
        }
    }
}
