using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitBucketVs.Contracts.Interfaces.ViewModels;

namespace BitBucketVs.Services
{
    [Export(typeof(IBitbucketService))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class BitbucketService : IBitbucketService
    {
        public BitbucketService()
        {
            
        }
    }
}
