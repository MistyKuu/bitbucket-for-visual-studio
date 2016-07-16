using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using BitBucket.REST.API.Authentication;
using BitBucket.REST.API.Exceptions;
using BitBucket.REST.API.Models;
using BitBucket.REST.API.Serializers;
using RestSharp;

namespace BitBucket.REST.API.Wrappers
{
    public class BitbucketRestClient : RestClient
    {
        public BitbucketRestClient(Connection connection) : base(connection.BitbucketUrl)
        {
            var serializer = new NewtonsoftJsonSerializer();
            this.AddHandler("application/json", serializer);
            this.AddHandler("text/json", serializer);
            this.AddHandler("text/x-json", serializer);
            this.AddHandler("text/javascript", serializer);
            this.AddHandler("*+json", serializer);

            var auth = new Authenticator(connection.Credentials);
            this.Authenticator = auth.CreatedAuthenticator;
        }

        public override async Task<IRestResponse<T>> ExecuteTaskAsync<T>(IRestRequest request)
        {
            var response = await base.ExecuteTaskAsync<T>(request);
            this.CheckResponseStatusCode<T>(response);
            return response;
        }


        public override async Task<IRestResponse<T>> ExecuteTaskAsync<T>(IRestRequest request, CancellationToken token)
        {
            var response = await base.ExecuteTaskAsync<T>(request, token);

            this.CheckResponseStatusCode<T>(response);
            return response;
        }

        private void CheckResponseStatusCode<T>(IRestResponse<T> response)
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new AuthorizationException();
            }
           
        }



    }
}