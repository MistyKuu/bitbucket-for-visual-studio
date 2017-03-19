using System;
using System.Text;
using BitBucket.REST.API.Extensions;
using BitBucket.REST.API.Models;
using BitBucket.REST.API.Models.Standard;

namespace BitBucket.REST.API.QueryBuilders
{
    public class QueryBuilder : IQueryConnector
    {
        private readonly StringBuilder _query;

        public QueryBuilder()
        {
            _query = new StringBuilder();
        }

        public IQueryParam And()
        {
            _query.Append(" and ");
            return this;
        }

        public IQueryParam Or()
        {
            _query.Append(" or ");
            return this;
        }

        public IQueryConnector UpdatedOn(DateTime date, Operators queryOperator)
        {
            var dateInProperFormat = date.ToString("yyyy-MM-dd");
            _query.Append($" updated_on {OperatorsMappings.MappingsDictionary[queryOperator]} {dateInProperFormat}");
            return this;
        }

        public IQueryConnector CreatedOn(DateTime date, Operators queryOperator)
        {
            var dateInProperFormat = date.ToString("yyyy-MM-dd");
            _query.Append($" created_on {OperatorsMappings.MappingsDictionary[queryOperator]} {dateInProperFormat}");
            return this;
        }

        public IQueryConnector SortAsc(string fieldName)
        {
            _query.Append($" sort=-{fieldName}");
            return this;
        }

        public IQueryConnector SortDesc(string fieldName)
        {
            _query.Append($" sort={fieldName}");
            return this;
        }

        public IQueryConnector State(PullRequestOptions option)
        {
            _query.Append($@" state=""{option}""");
            return this;
        }

        public IQueryConnector Add(string prop,string value)
        {
            _query.Append($@" {prop}=""{value}""");
            return this;
        }

        public string Build()
        {
            return _query.ToString();
        }
    }
}