using System;
using System.Collections.Generic;
using System.Linq;
using BitBucket.REST.API.Interfaces;

namespace BitBucket.REST.API.QueryBuilders
{
    public class EnterprisePullRequestQueryBuilder : IPullRequestQueryBuilder
    {
        private string _status = "Open";
        private string _fromBranch;
        private string _toBranch;
        private Order _order = Order.Newest;
        private readonly List<ParticipantFilter> _participantFilters = new List<ParticipantFilter>();
        private bool _includeAttributes = true;
        private bool _includeProperties = true;


        public IPullRequestQueryBuilder WithState(string status)
        {
            _status = status;
            return this;
        }

        public IPullRequestQueryBuilder WithSourceBranch(string fullQualifiedBranchName)
        {
            if (!string.IsNullOrEmpty(_toBranch))
                throw new Exception("You cannot specify both: source and destination branch");

            _fromBranch = fullQualifiedBranchName;
            return this;
        }

        public IPullRequestQueryBuilder WithDestinationBranch(string fullQualifiedBranchName)
        {
            if (!string.IsNullOrEmpty(_fromBranch))
                throw new Exception("You cannot specify both: source and destination branch");

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
            _includeAttributes = include;
            return this;
        }

        public IPullRequestQueryBuilder WithProperties(bool include)
        {
            _includeProperties = include;
            return this;
        }

        public IPullRequestQueryBuilder WithAuthor(string username, bool? approved)
        {
            _participantFilters.Add(new ParticipantFilter(username, "AUTHOR", approved));
            return this;
        }

        public Dictionary<string, string> GetQueryParameters()
        {
            var result = new Dictionary<string, string>
            {
                ["state"] = _status,
                ["order"] = _order.ToString(),
                ["withAttributes"] = _includeAttributes.ToString(),
                ["withProperties"] = _includeProperties.ToString(),
            };

            if (!string.IsNullOrEmpty(_toBranch))
            {
                result["direction"] = "INCOMING";
                result["at"] = _toBranch;
            }
            else if (!string.IsNullOrEmpty(_fromBranch))
            {
                result["direction"] = "OUTGOING";
                result["at"] = _fromBranch;
            }


            int i = 1;
            foreach (var participantFilter in _participantFilters)
            {
                if (!string.IsNullOrEmpty(participantFilter.UserName))
                {
                    result.Add($"username.{i}", participantFilter.UserName);

                    if (!string.IsNullOrEmpty(participantFilter.Role))
                        result.Add($"role.{i}", participantFilter.Role);
                    if (!string.IsNullOrEmpty(participantFilter.Approved))
                        result.Add($"approved.{i}", participantFilter.Approved);
                }

                i++;
            }

            return result.Where(x => x.Value != null).ToDictionary(x => x.Key, x => x.Value);
        }
    }
}