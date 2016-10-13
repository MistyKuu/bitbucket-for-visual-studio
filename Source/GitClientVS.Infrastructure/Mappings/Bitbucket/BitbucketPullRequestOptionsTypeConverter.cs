using AutoMapper;
using BitBucket.REST.API.Models;
using GitClientVS.Contracts.Models.GitClientModels;

namespace GitClientVS.Infrastructure.Mappings.Bitbucket
{
    public class BitbucketPullRequestOptionsTypeConverter
          : ITypeConverter<PullRequestOptions, GitPullRequestStatus>
    {
        public GitPullRequestStatus Convert(PullRequestOptions source, GitPullRequestStatus destination, ResolutionContext context)
        {
            if (source == PullRequestOptions.OPEN)
            {
                return GitPullRequestStatus.Open;
            }
            if (source == PullRequestOptions.MERGED)
            {
                return GitPullRequestStatus.Merged;
            }
            if (source == PullRequestOptions.DECLINED)
            {
                return GitPullRequestStatus.Declined;
            }
        
            return GitPullRequestStatus.Superseded;
        }
    }

    public class BitbucketReversePullRequestOptionsTypeConverter : ITypeConverter<GitPullRequestStatus, PullRequestOptions>
    {
        public PullRequestOptions Convert(GitPullRequestStatus source, PullRequestOptions destination, ResolutionContext context)
        {
            if (source == GitPullRequestStatus.Open)
            {
                return PullRequestOptions.OPEN;
            }
            if (source == GitPullRequestStatus.Merged)
            {
                return PullRequestOptions.MERGED;
            }
            if (source == GitPullRequestStatus.Declined)
            {
                return PullRequestOptions.DECLINED;
            }
          
            return PullRequestOptions.SUPERSEDED;
        }
    }
}