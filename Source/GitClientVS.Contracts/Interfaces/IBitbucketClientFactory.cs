using BitBucket.REST.API.Interfaces;
using BitBucket.REST.API.Models;
using System;
using System.Threading.Tasks;

namespace GitClientVS.Contracts.Interfaces
{
    public interface IBitbucketClientFactory
    {
        Task<IBitbucketClient> CreateEnterpriseBitBucketClient(Uri host, Credentials cred);
        Task<IBitbucketClient> CreateStandardBitBucketClient(Credentials cred);
    }
}