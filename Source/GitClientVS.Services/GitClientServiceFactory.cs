using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitClientVS.Contracts;
using GitClientVS.Contracts.Events;
using GitClientVS.Contracts.Interfaces;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Models;

namespace GitClientVS.Services
{
    public enum ProductType
    {
        Top =0,
        Medium,
        Bottom
    }

    public class Product
    {
        public ProductType Type { get; set; }
        //the rest of properties here
    }

    public class ProductComparer : IComparer<Product>
    {
        public int Compare(Product x, Product y)
        {
            return ((int) x.Type).CompareTo((int) y.Type);
        }

        public static void Main()
        {
            var list = new List<Product>();
            var orderedList = list.OrderBy(x => (int)x.Type).ToList();
        }
    }

   

    [Export(typeof(IGitClientServiceFactory))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class GitClientServiceFactory : IGitClientServiceFactory
    {
        private readonly IEventAggregatorService _eventAggregator;
        private ConnectionData _userInformation;

        [ImportMany()]
        private IEnumerable<Lazy<IGitClientService, IGitClientMetadata>> _gitServices;

        [ImportingConstructor]
        public GitClientServiceFactory(IEventAggregatorService eventAggregator,IUserInformationService userInformationService)
        {
            _eventAggregator = eventAggregator;
            _userInformation = userInformationService.ConnectionData;
            _eventAggregator.GetEvent<ConnectionChangedEvent>().Subscribe(ev => { _userInformation = ev.Data; });
        }


        public IGitClientService GetService()
        {
            var service = _gitServices.First(x => x.Metadata.GitProvider == _userInformation.GitProvider);
            return service.Value;
        }

        public IGitClientService GetService(GitProviderType gitProvider)
        {
            var service = _gitServices.First(x => x.Metadata.GitProvider == gitProvider);
            return service.Value;
        }

        public IEnumerable<GitProviderType> AvailableServices => _gitServices.Select(x => x.Metadata.GitProvider).ToList();

    }
}
