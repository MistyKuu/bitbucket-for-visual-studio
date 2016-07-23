using AutoMapper;
using BitBucket.REST.API.Models;
using GitClientVS.Contracts.Models.GitClientModels;

namespace GitClientVS.Infrastructure.Mappings
{
    public class ReversePullRequestTypeConverter
    : ITypeConverter<GitPullRequest, PullRequest>
    {
        public PullRequest Convert(GitPullRequest source, PullRequest destination, ResolutionContext context)
        {
            var pullRequest = new PullRequest();
            pullRequest.Title = source.Title;
            pullRequest.Description = source.Description;
            pullRequest.Source.Branch.Name = source.SourceBranch;
            pullRequest.Destination.Branch.Name = source.DestinationBranch;
            pullRequest.Source.Repository.Name = source.RepoName;
          
            return pullRequest;
        }

    }
}