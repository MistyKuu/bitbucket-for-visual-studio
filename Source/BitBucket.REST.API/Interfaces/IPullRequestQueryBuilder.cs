using System.Collections.Generic;
using BitBucket.REST.API.QueryBuilders;

namespace BitBucket.REST.API.Interfaces
{
    public interface IPullRequestQueryBuilder
    {
        IPullRequestQueryBuilder WithState(string status);
        IPullRequestQueryBuilder WithSourceBranch(string fullQualifiedBranchName);
        IPullRequestQueryBuilder WithDestinationBranch(string fullQualifiedBranchName);
        IPullRequestQueryBuilder WithOrder(Order order);
        IPullRequestQueryBuilder WithAttributes(bool include);
        IPullRequestQueryBuilder WithProperties(bool include);
        IPullRequestQueryBuilder WithAuthor(string username, bool? approved);
        Dictionary<string, string> GetQueryParameters();
    }



    public enum Order
    {
        Oldest,
        Newest
    }

    public class ParticipantFilter
    {
        public ParticipantFilter(string username, string role, bool? approved)
        {
            UserName = username;
            Role = role;
            Approved = approved?.ToString();
        }

        public string UserName { get; }
        public string Role { get; }
        public string Approved { get; }
    }
}