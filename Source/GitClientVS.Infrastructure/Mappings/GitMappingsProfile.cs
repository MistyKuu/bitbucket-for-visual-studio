using AutoMapper;
using BitBucket.REST.API.Models;
using GitClientVS.Contracts.Models.GitClientModels;

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
            CreateMap<Branch, GitBranch>();
            CreateMap<User, GitUser>();
            CreateMap<Links, GitLinks>();
            CreateMap<Link, GitLink>();
        }
    }

}