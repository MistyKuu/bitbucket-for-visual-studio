using GitClientVS.Contracts.Interfaces.GitClientServices;
using GitClientVS.Contracts.Models.GitClientModels;
using SharpBucket.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitClientVS.Api
{
    public class BitbucketClient: IGitClient
    {
        private SharpBucket.V2.SharpBucketV2 bitbucketClient;

        public BitbucketClient(GitClientHostAddress hostAddress, string name, string password)
        {
            bitbucketClient = new SharpBucketV2();
            bitbucketClient.BasicAuthentication(name, password);  
        }

        public BitbucketClient(string name, string password)
        {
            bitbucketClient = new SharpBucketV2();
            bitbucketClient.BasicAuthentication(name, password);
            var wtf = bitbucketClient.RepositoriesEndPoint().ListRepositories(name);
            if (wtf.Count == 0)
            {
                throw new Exception("Bla bla");
            }
        }

        public IConnectionService ConnectionService
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IRepositoriesService RepositoriesService
        {
            get
            {
                throw new NotImplementedException();
            }
        }

         

    }
}
