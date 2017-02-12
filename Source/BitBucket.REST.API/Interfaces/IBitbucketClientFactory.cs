using System;
using System.Threading.Tasks;
using BitBucket.REST.API.Models;

namespace BitBucket.REST.API.Interfaces
{
    public interface IBitbucketClientFactory
    {
        Task<IBitbucketClient> CreateEnterpriseBitBucketClient(Uri host, Credentials cred);
        Task<IBitbucketClient> CreateStandardBitBucketClient(Uri host, Credentials cred);
    }
}