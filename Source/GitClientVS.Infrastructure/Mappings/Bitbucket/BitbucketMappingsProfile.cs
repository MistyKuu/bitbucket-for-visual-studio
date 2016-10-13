using AutoMapper;
using BitBucket.REST.API.Models;
using GitClientVS.Contracts.Models.GitClientModels;
using GitClientVS.Infrastructure.Utils;

namespace GitClientVS.Infrastructure.Mappings.Bitbucket
{
    public class BitbucketMappingsProfile : Profile
    {
        public BitbucketMappingsProfile()
        {
            CreateMap<Repository, GitRepository>().ConvertUsing<BitbucketRepositoryTypeConverter>();
            CreateMap<GitRepository, Repository>().ConvertUsing<BitbucketReverseRepositoryTypeConverter>();

            CreateMap<GitPullRequest, PullRequest>().ConvertUsing<BitbucketReversePullRequestTypeConverter>();
            CreateMap<PullRequest, GitPullRequest>().ConvertUsing<BitbucketPullRequestTypeConverter>();

            CreateMap<PullRequestOptions, GitPullRequestStatus>().ConvertUsing<BitbucketPullRequestOptionsTypeConverter>();
            CreateMap<GitPullRequestStatus, PullRequestOptions>().ConvertUsing<BitbucketReversePullRequestOptionsTypeConverter>();

            CreateMap<Team, GitTeam>().ConvertUsing<BitbucketTeamTypeConverter>();
            CreateMap<GitTeam, Team>().ConvertUsing<BitbucketReverseTeamTypeConverter>();

            CreateMap<Commit, GitCommit>()
                .ForMember(dto => dto.Date, e => e.MapFrom(o => TimeConverter.GetDate(o.Date)))
                .ForMember(dto => dto.Author, e => e.MapFrom(o => o.Author.User));

            CreateMap(typeof(IteratorBasedPage<>), typeof(PageIterator<>));

            // todo: make a general parent
            CreateMap<Parent, GitCommentParent>();

            CreateMap<Comment, GitComment>()
                .ForMember(dto => dto.CreatedOn, e => e.MapFrom(o => TimeConverter.GetDate(o.CreatedOn)))
                .ForMember(dto => dto.UpdatedOn, e => e.MapFrom(o => TimeConverter.GetDate(o.UpdatedOn)))
                .ForMember(dto => dto.IsFile, e => e.MapFrom(o => o.Inline != null));

            CreateMap<Content, GitCommentContent>();
            CreateMap<Branch, GitBranch>();
            CreateMap<User, GitUser>();
            CreateMap<UserShort, GitUser>();
            CreateMap<Links, GitLinks>();
            CreateMap<Link, GitLink>();
        }
    }

}