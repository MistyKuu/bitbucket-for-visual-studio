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
            return new GitPullRequest(source.Title, source.Description, source.Source.Branch.Name, source.Destination.Branch.Name)
            {
                Id = source.Id.ToString(),
                Author = source.Author.MapTo<GitUser>(),
                Status = source.State.MapTo<GitPullRequestStatus>(),
                Created = DateTime.Parse(source.CreatedOn),
                Updated = DateTime.Parse(source.UpdatedOn),  //TODO PARSE WON"T WORK WITH ALL CULTURES FIX
                CloseSourceBranch = source.CloseSourceBranch

            };
        }
    }

    public class ReversePullRequestTypeConverter
  : ITypeConverter<GitPullRequest, PullRequest>
    {
        public PullRequest Convert(GitPullRequest source, PullRequest destination, ResolutionContext context)
        {
            return new PullRequest()
            {
                Title = source.Title,
                Description = source.Description,
                Source = new Source
                {
                    Branch = new Branch()
                    {
                        Name = source.SourceBranch
                    },
                },
                Destination = new Source()
                {
                    Branch = new Branch()
                    {
                        Name = source.DestinationBranch
                    }
                },
                State = source.Status.MapTo<PullRequestOptions>(),
                CloseSourceBranch = source.CloseSourceBranch
            };
        }
    }
}