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
            CreateMap<User, GitUser>();
            CreateMap<Links, GitLinks>();
            CreateMap<Link, GitLink>();
        }
    }

}