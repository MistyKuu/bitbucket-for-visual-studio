using System;
using System.Linq;
using AutoMapper;
using BitBucket.REST.API.Models;
using GitClientVS.Contracts.Models.GitClientModels;
using GitClientVS.Infrastructure.Extensions;

namespace GitClientVS.Infrastructure.Mappings
{
    public class PullRequestTypeConverter
    : ITypeConverter<PullRequest, GitPullRequest>
    {
        public GitPullRequest Convert(PullRequest source, GitPullRequest destination, ResolutionContext context)
        {
            var gitPullRequest = new GitPullRequest(source.Title, source.Description, source.Source.Branch.Name, source.Destination.Branch.Name)
            {
                Id = source.Id.ToString(),
                Author = source.Author.MapTo<GitUser>(),
                CreationDate = DateTime.Parse(source.CreatedOn).ToString("F") // TODO PARSE WON"T WORK WITH ALL CULTURES FIX
            };

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