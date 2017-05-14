using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using BitBucket.REST.API.Helpers;
using BitBucket.REST.API.Interfaces;
using BitBucket.REST.API.Mappings;
using BitBucket.REST.API.Models.Enterprise;
using BitBucket.REST.API.Models.Standard;
using BitBucket.REST.API.QueryBuilders;
using BitBucket.REST.API.Wrappers;
using RestSharp;

namespace BitBucket.REST.API.Clients.Enterprise
{
    public class EnterpriseUserClient : ApiClient, IUserClient
    {

        public EnterpriseUserClient(IEnterpriseBitbucketRestClient restClient, Connection connection) : base(restClient, connection)
        {

        }

        public Task<User> GetUser()
        {
            throw new NotImplementedException(); //todo its returning all users (not used anywhere)
            //var url = EnterpriseApiUrls.User();
            //var response = await RestClient.GetAllPages<EnterpriseUser>(url);
            //if (!response.Values.Any())
            //    throw new Exception("User not found");

            //return response.Values.First().MapTo<User>();
        }
    }
}