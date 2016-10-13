using System;
using System.Collections.Generic;
using AutoMapper;
using BitBucket.REST.API.Helpers;
using BitBucket.REST.API.Models;
using GitClientVS.Contracts.Models.GitClientModels;
using GitClientVS.Infrastructure.Extensions;
using GitClientVS.Infrastructure.Utils;
using GitLab.NET.ResponseModels;

namespace GitClientVS.Infrastructure.Mappings.Gitlab
{
    public class GitlabMergeRequestTypeConverter : ITypeConverter<MergeRequest, GitPullRequest>
    {
        public GitPullRequest Convert(MergeRequest source, GitPullRequest destination, ResolutionContext context)
        {
            return new GitPullRequest(source.Title, source.Description, source.SourceBranch, source.TargetBranch)
            {
                Id = source.Id ?? int.MaxValue,
                Author = source.Author.MapTo<GitUser>(),
                Status = source.State.MapTo<GitPullRequestStatus>(),
                Created = source.CreatedAt ?? DateTime.MinValue,
                Updated = source.UpdatedAt ?? DateTime.MinValue,
                Reviewers = new Dictionary<string, bool>(),
                // Link = source.,
                //CloseSourceBranch = source.CloseSourceBranch,
                // Url = source.Links.Html.Href,
                // Reviewers = reviewers,
                // CommentsCount = source.CommentsCount
            };
        }
    }

}