using System;
using AutoMapper;
using BitBucket.REST.API.Models;
using GitClientVS.Contracts.Models.GitClientModels;
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

            CreateMap<Team, GitTeam>().ConvertUsing<TeamTypeConverter>();
            CreateMap<GitTeam, Team>().ConvertUsing<ReverseTeamTypeConverter>();

            CreateMap<Commit, GitCommit>()
                .ForMember(dto => dto.Date, e => e.MapFrom(o => TimeConverter.GetDate(o.Date)))
                .ForMember(dto => dto.Author, e => e.MapFrom(o => o.Author.User));

            CreateMap<Comment, GitComment>()
                .ForMember(dto => dto.CreatedOn, e => e.MapFrom(o => TimeConverter.GetDate(o.CreatedOn)))
                .ForMember(dto => dto.UpdatedOn, e => e.MapFrom(o => TimeConverter.GetDate(o.UpdatedOn)));

            CreateMap<Content, GitCommentContent>();
            CreateMap<Branch, GitBranch>();
            CreateMap<User, GitUser>();
            CreateMap<UserShort, GitUser>();
            CreateMap<Links, GitLinks>();
            CreateMap<Link, GitLink>();
        }
    }

}