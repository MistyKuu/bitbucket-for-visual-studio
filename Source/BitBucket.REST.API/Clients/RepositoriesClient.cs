using System;
using System.Net;
using System.Threading.Tasks;
using BitBucket.REST.API.Exceptions;
using BitBucket.REST.API.Helpers;
using BitBucket.REST.API.Models;
using BitBucket.REST.API.Wrappers;
using RestSharp;

namespace BitBucket.REST.API.Clients
{
    public class RepositoriesClient : ApiClient
    {
        public RepositoriesClient(BitbucketRestClient restClient, Connection connection) : base(restClient, connection)
        {
        }

        public IteratorBasedPage<Repository> GetRepositories()
        {
          
            var wtf = ApiUrls.Repositories("nibaa");
            var request = new BitbucketRestRequest(wtf, Method.GET);
            //   var response = RestClient.Execute<IteratorBasedPage<Repository>>(request);       
            var response = RestClient.Execute<IteratorBasedPage<Repository>>(request);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new AuthorizationException();
            }
            //Console.WriteLine(response.Content);  
            //Console.WriteLine(response.ErrorMessage);
            return response.Data;
           
      
        }
    }
}