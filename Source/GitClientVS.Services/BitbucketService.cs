using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitClientVS.Contracts.Interfaces.ViewModels;

namespace GitClientVS.Services
{
    [Export(typeof(IBitbucketService))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class BitbucketService : IBitbucketService
    {
        public BitbucketService()
        {

        }

        public async Task ConnectAsync(string login, string password)
        {
            await Task.Delay(100);

            if (new Random().Next(2) % 2 == 0)
                throw new Exception();
        }
    }
}
