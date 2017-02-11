using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BitBucket.REST.API.Authentication;
using BitBucket.REST.API.Exceptions;
using BitBucket.REST.API.Models.Standard;
using BitBucket.REST.API.QueryBuilders;
using BitBucket.REST.API.Serializers;
using RestSharp;

namespace BitBucket.REST.API.Wrappers
{
    public abstract class BitbucketRestClientBase : RestClient
    {
        protected Connection Connection;

        protected BitbucketRestClientBase(Connection connection) : base(connection.ApiUrl)
        {
            Connection = connection;
            var serializer = new NewtonsoftJsonSerializer();
            this.AddHandler("application/json", serializer);
            this.AddHandler("text/json", serializer);
            this.AddHandler("text/plain", serializer);
            this.AddHandler("text/x-json", serializer);
            this.AddHandler("text/javascript", serializer);
            this.AddHandler("*+json", serializer);

            var auth = new Authenticator(connection.Credentials);
            this.Authenticator = auth.CreatedAuthenticator;
            this.FollowRedirects = false;
        }


        public abstract Task<IteratorBasedPage<T>> GetAllPages<T>(string url, int limit = 100, IQueryConnector query = null);
        protected abstract string DeserializeError(IRestResponse response);

        public override async Task<IRestResponse<T>> ExecuteTaskAsync<T>(IRestRequest request)
        {
            var response = await base.ExecuteTaskAsync<T>(request);
            response = await RedirectIfNeededAndGetResponse(response, request);
            this.CheckResponseStatusCode(response);
            return response;
        }

        public override async Task<IRestResponse> ExecuteTaskAsync(IRestRequest request)
        {
            var response = await base.ExecuteTaskAsync(request);
            response = await RedirectIfNeededAndGetResponse(response, request);
            this.CheckResponseStatusCode(response);
            return response;
        }

        private void CheckResponseStatusCode(IRestResponse response)
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
                var errorMessage = response.ErrorMessage;
                var friendly = false;
                if (response.Content != null)
                {
                    try
                    {
                        errorMessage = DeserializeError(response);
                        friendly = true;
                    }
                    catch (Exception)
                    {

                    }
                }

                throw new RequestFailedException(errorMessage, friendly);
            }

        }


        private async Task<IRestResponse<T>> RedirectIfNeededAndGetResponse<T>(IRestResponse<T> response, IRestRequest request)
        {
            while (response.StatusCode == HttpStatusCode.Redirect)
            {
                var newLocation = GetNewLocationFromHeader(response);

                if (newLocation == null)
                    return response;

                request.Resource = RemoveBaseUrl(newLocation);
                response = await base.ExecuteTaskAsync<T>(request);
            }

            return response;
        }

        private async Task<IRestResponse> RedirectIfNeededAndGetResponse(IRestResponse response, IRestRequest request)
        {
            while (response.StatusCode == HttpStatusCode.Redirect)
            {
                var newLocation = GetNewLocationFromHeader(response);

                if (newLocation == null)
                    return response;

                request.Resource = RemoveBaseUrl(newLocation);
                response = await base.ExecuteTaskAsync(request);
            }

            return response;
        }


        private static Parameter GetNewLocationFromHeader(IRestResponse response)
        {
            return response.Headers
                .FirstOrDefault(x => x.Type == ParameterType.HttpHeader &&
                                     x.Name.Equals(HttpResponseHeader.Location.ToString(), StringComparison.InvariantCultureIgnoreCase));
        }

        private string RemoveBaseUrl(Parameter newLocation)
        {
            return newLocation.Value.ToString().Replace(Connection.ApiUrl.ToString(), string.Empty);
        }


    }
}