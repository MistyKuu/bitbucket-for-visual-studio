using System.Linq;
using AutoMapper;
using BitBucket.REST.API.Models;
using GitClientVS.Contracts.Models.GitClientModels;

namespace GitClientVS.Infrastructure.Mappings
{
    public class PullRequestTypeConverter
    : ITypeConverter<PullRequest, GitPullRequest>
    {
        public GitPullRequest Convert(PullRequest source, GitPullRequest destination, ResolutionContext context)
        {
            var gitPullRequest = new GitPullRequest(source.Title, source.Description, source.Source.Branch.Name,
                source.Destination.Branch.Name);
            gitPullRequest.Id = source.Id.ToString();

            //todo how
            if (source.State == PullRequestOptions.DECLINED)
            {
                gitPullRequest.Status = GitPullRequestStatus.Declined;
            }
            else if (source.State == PullRequestOptions.MERGED)
            {
                gitPullRequest.Status = GitPullRequestStatus.Merged;
            }
            else
            {
                gitPullRequest.Status = GitPullRequestStatus.Open;
            }


            return gitPullRequest;
        }

    }
}