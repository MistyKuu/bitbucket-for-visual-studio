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
            var pullRequest = new PullRequest
            {
                Title = source.Title,
                Description = source.Description,
                CloseSourceBranch = source.CloseSourceBranch
            };

            //todo how
            if (source.Status == GitPullRequestStatus.Declined)
            {
                pullRequest.State = PullRequestOptions.DECLINED;
            } else if (source.Status == GitPullRequestStatus.Merged)
            {
                pullRequest.State = PullRequestOptions.MERGED;
            } else 
            {
                pullRequest.State = PullRequestOptions.OPEN;
            }
          

            pullRequest.Source = new Source
            {
                Branch = new Branch()
                {
                    Name = source.SourceBranch
                },
            };
            pullRequest.Destination = new Source()
            {
                Branch = new Branch()
                {
                    Name = source.DestinationBranch
                }
            };

            return pullRequest;
        }

    }
}