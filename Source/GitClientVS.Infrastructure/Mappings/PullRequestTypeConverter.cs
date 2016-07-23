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
                source.Destination.Branch.Name, source.Source.Repository.Name);
            gitPullRequest.Id = source.Id.ToString();
            return gitPullRequest;
        }

    }
}