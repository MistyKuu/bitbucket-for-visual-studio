using System;
using System.Collections.Generic;
using System.Linq;
using BitBucket.REST.API.Interfaces;
using BitBucket.REST.API.Models.Standard;

namespace BitBucket.REST.API.QueryBuilders
{
    public class PullRequestQueryBuilder : IPullRequestQueryBuilder
    {
        private string _status;
        private string _fromBranch;
        private string _toBranch;
        private Order _order = Order.Newest;
        private readonly List<ParticipantFilter> _participantFilters = new List<ParticipantFilter>();

        public IPullRequestQueryBuilder WithState(string status)
        {
            _status = status;
            return this;
        }

        public IPullRequestQueryBuilder WithSourceBranch(string fullQualifiedBranchName)
        {
            _fromBranch = fullQualifiedBranchName;
            return this;
        }

        public IPullRequestQueryBuilder WithDestinationBranch(string fullQualifiedBranchName)
        {
            _toBranch = fullQualifiedBranchName;
            return this;
        }

        public IPullRequestQueryBuilder WithOrder(Order order)
        {
            _order = order;
            return this;
        }

        public IPullRequestQueryBuilder WithAttributes(bool include)
        {
            throw new NotSupportedException();
        }

        public IPullRequestQueryBuilder WithProperties(bool include)
        {
            throw new NotSupportedException();
        }

        public IPullRequestQueryBuilder WithAuthor(string username, bool? approved)
        {
            _participantFilters.Add(new ParticipantFilter(username, null, approved));
            return this;
        }

        public Dictionary<string, string> GetQueryParameters()
        {
            var queryBuilder = new QueryBuilder();

            if (!string.IsNullOrEmpty(_fromBranch))
                queryBuilder.Add("source.branch.name", _fromBranch);
            if (!string.IsNullOrEmpty(_toBranch))
                queryBuilder.Add("destination.branch.name", _toBranch);
            if (_participantFilters.Any() && !string.IsNullOrEmpty(_participantFilters[0].UserName))
                queryBuilder.Add("author.username", _participantFilters[0].UserName);

            var res= new Dictionary<string, string>()
            {
                ["sort"] = _order == Order.Newest ? "-updated_on" : "updated_on"
            };

            var queryBuilderString = queryBuilder.Build();
            if (!string.IsNullOrEmpty(queryBuilderString))
                res["q"] = queryBuilderString;

            if (!string.IsNullOrEmpty(_status))
                res["state"] = $"{_status.ToUpper()}";

            return res;
        }
    }
}