using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BitBucket.REST.API.Models.Standard;

namespace BitBucket.REST.API.QueryBuilders
{
    public class QueryBuilder : IQueryBuilder
    {
        private readonly Dictionary<string, string> _params = new Dictionary<string, string>();
        public IQueryBuilder UpdatedOn(DateTime date, Operators queryOperator)
        {
            throw new NotSupportedException("Only equals works");
        }

        public IQueryBuilder CreatedOn(DateTime date, Operators queryOperator)
        {
            throw new NotSupportedException("Only equals works");
        }
        public IQueryBuilder State(PullRequestOptions option)
        {
            _params.Add("state", $@"""{option}""");
            return this;
        }

        public IQueryBuilder Add(string prop, string value)
        {
            _params.Add(prop, $@"""{value}""");
            return this;
        }

        public string Build()
        {
            return string.Join(" AND ", _params
                .Where(x => x.Value != null)
                .Select(x => x.Key + " = " + x.Value));
        }
    }
}