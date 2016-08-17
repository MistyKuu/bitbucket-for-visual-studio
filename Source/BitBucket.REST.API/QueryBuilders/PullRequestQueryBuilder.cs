using System;
using System.Text;
using BitBucket.REST.API.Extensions;
using BitBucket.REST.API.Models;

namespace BitBucket.REST.API.QueryBuilders
{
    public class PullRequestQueryBuilder 
    {
        private QueryBuilder query;

        public PullRequestQueryBuilder()
        {
            query = new QueryBuilder();   
        }

        public IQueryParam StartBuilding()
        {
            return query;
        }

    }
}