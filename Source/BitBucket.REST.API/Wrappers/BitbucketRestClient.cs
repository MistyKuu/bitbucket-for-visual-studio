using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using BitBucket.REST.API.Authentication;
using BitBucket.REST.API.Exceptions;
using BitBucket.REST.API.Models;
using BitBucket.REST.API.QueryBuilders;
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

        public async Task<IteratorBasedPage<T>> GetAllPages<T>(string url, int limit = 10, IQueryConnector query = null)
        {
            var result = new IteratorBasedPage<T>()
            {
                Values = new List<T>()
            };
            IRestResponse<IteratorBasedPage<T>> response;
            var pageNumber = 1;
            do
            {
                var request = new BitbucketRestRequest(url, Method.GET);
                request.AddQueryParameter("pagelen", limit.ToString()).AddQueryParameter("page", pageNumber.ToString());
                if (query != null)
                {
                    request.AddQueryParameter("q", query.Build());
                }
                response = await this.ExecuteTaskAsync<IteratorBasedPage<T>>(request);
                if (response.Data.Values != null)
                {
                    result.Values.AddRange(response.Data.Values);
                }
                pageNumber++;
            } while (response.Data != null && response.Data.Next != null);

            return result;
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

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new ForbiddenException(response.ErrorMessage);
            }

            if (response.StatusCode == HttpStatusCode.BadRequest
                || response.StatusCode == HttpStatusCode.NotFound
                || response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new RequestFailedException(response.ErrorMessage);
            }
           
        }



    }
}