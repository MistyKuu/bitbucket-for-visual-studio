using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitBucket.REST.API;
using BitBucket.REST.API.Models;
using GitClientVS.Contracts.Interfaces.ViewModels;

namespace GitClientVS.Services
{
    [Export(typeof(IBitbucketService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class BitbucketService : IBitbucketService
    {
        public BitbucketService()
        {
            
        }

        public async Task ConnectAsync(string login, string password)
        {
            var credentials = new Credentials(login, password);
            var connection = new Connection(credentials);
            var bitbucketInitializer = new BitbucketClientInitializer(connection);
            var bitbucketClient = await bitbucketInitializer.Initialize();
        }
    }
}
