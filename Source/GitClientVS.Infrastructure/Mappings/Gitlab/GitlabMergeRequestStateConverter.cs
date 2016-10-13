using System;
using AutoMapper;
using GitClientVS.Contracts.Models.GitClientModels;
using GitLab.NET;

namespace GitClientVS.Infrastructure.Mappings.Gitlab
{
    public class GitlabMergeRequestStateConverter : ITypeConverter<MergeRequestState, GitPullRequestStatus>
    {
        public GitPullRequestStatus Convert(MergeRequestState source, GitPullRequestStatus destination, ResolutionContext context)
        {
            if (source == MergeRequestState.Opened)
            {
                return GitPullRequestStatus.Open;
            }
            if (source == MergeRequestState.Merged)
            {
                return GitPullRequestStatus.Merged;
            }
            if (source == MergeRequestState.Closed)
            {
                return GitPullRequestStatus.Declined;
            }

            throw new Exception($"Not expected enum type {source}");
        }
    }
}