using System;
using System.Net;
using System.Threading.Tasks;
using BitBucket.REST.API.Exceptions;
using BitBucket.REST.API.Helpers;
using BitBucket.REST.API.Models;
using RestSharp;

namespace BitBucket.REST.API.Clients
{
    public class RepositoriesClient : ApiClient
    {
        public RepositoriesClient(RestClient restClient, Connection connection) : base(restClient, connection)
        {
        }

        public IteratorBasedPage<Repository> GetRepositories()
        {
          
            var wtf = ApiUrls.Repositories("nibaa");
            var request = new RestRequest(wtf, Method.GET);
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