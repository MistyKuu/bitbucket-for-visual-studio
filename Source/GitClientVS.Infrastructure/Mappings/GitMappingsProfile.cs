using System;
using AutoMapper;
using BitBucket.REST.API.Mappings.Converters;
using BitBucket.REST.API.Models;
using BitBucket.REST.API.Models.Standard;
using GitClientVS.Contracts.Models.GitClientModels;
using GitClientVS.Infrastructure.Extensions;
using GitClientVS.Infrastructure.Utils;

namespace GitClientVS.Infrastructure.Mappings
{
    public class GitMappingsProfile : Profile
    {
        public GitMappingsProfile()
        {
            CreateMap<Repository, GitRemoteRepository>().ConvertUsing<RepositoryTypeConverter>();
            CreateMap<GitRemoteRepository, Repository>().ConvertUsing<ReverseRepositoryTypeConverter>();

            CreateMap<GitPullRequest, PullRequest>().ConvertUsing<ReversePullRequestTypeConverter>();
            CreateMap<PullRequest, GitPullRequest>().ConvertUsing<PullRequestTypeConverter>();

            CreateMap<PullRequestOptions, GitPullRequestStatus>().ConvertUsing<PullRequestOptionsTypeConverter>();
            CreateMap<GitPullRequestStatus, PullRequestOptions>().ConvertUsing<ReversePullRequestOptionsTypeConverter>();

            CreateMap<Team, GitTeam>().ConvertUsing<TeamTypeConverter>();
            CreateMap<GitTeam, Team>().ConvertUsing<ReverseTeamTypeConverter>();

            CreateMap<Commit, GitCommit>()
                .ForMember(dto => dto.Date, e => e.MapFrom(o => TimeConverter.GetDate(o.Date)))
                .ForMember(dto => dto.Author, e => e.MapFrom(o => o.Author.User));

            CreateMap<GitCommit, Commit>();

            CreateMap(typeof(IteratorBasedPage<>), typeof(PageIterator<>));

            CreateMap<Parent, GitCommentParent>();

            CreateMap<Comment, GitComment>().ConvertUsing<CommentConverter>();
          
            CreateMap<Content, GitCommentContent>();
            CreateMap<Branch, GitBranch>();

            CreateMap<GitBranch, Branch>();


            CreateMap<GitUser, GitUser>();
            CreateMap<User, GitUser>();
            CreateMap<Links, GitLinks>();
            CreateMap<Link, GitLink>();
            CreateMap<GitMergeRequest, MergeRequest>();
        }
    }
}