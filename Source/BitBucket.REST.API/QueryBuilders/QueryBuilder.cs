using System;
using System.Text;
using BitBucket.REST.API.Extensions;
using BitBucket.REST.API.Models;

namespace BitBucket.REST.API.QueryBuilders
{
    public class QueryBuilder : IQueryConnector, IQueryParam
    {
        private StringBuilder query;

        public QueryBuilder()
        {
            query = new StringBuilder();
        }

        public IQueryParam And()
        {
            query.Append(" and ");
            return this;
        }

        public IQueryParam Or()
        {
            query.Append(" or ");
            return this;
        }

        public IQueryConnector UpdatedOn(DateTime date, Operators queryOperator)
        {
            var dateInProperFormat = date.ToString("yyyy-MM-dd");
            query.Append($" updated_on {EnumExtensions.GetEnumDescription(queryOperator)} {dateInProperFormat}");
            return this;
        }

        public IQueryConnector CreatedOn(DateTime date, Operators queryOperator)
        {
            var dateInProperFormat = date.ToString("yyyy-MM-dd");
            query.Append($" created_on {EnumExtensions.GetEnumDescription(queryOperator)} {dateInProperFormat}");
            return this;
        }

        public IQueryConnector State(PullRequestOptions option)
        {
            query.Append($" state='{option}'");
            return this;
        }

        public string Build()
        {
            return query.ToString();
        }
    }
}