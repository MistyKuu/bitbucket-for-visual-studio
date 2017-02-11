using AutoMapper;
using BitBucket.REST.API.Mappings.Converters;
using BitBucket.REST.API.Models.Enterprise;
using BitBucket.REST.API.Models.Standard;

namespace BitBucket.REST.API.Mappings
{
    public class EnterpriseToStandardMappingsProfile : Profile
    {
        public EnterpriseToStandardMappingsProfile()
        {
            CreateMap<EnterpriseLink, Link>();
            CreateMap<Link, EnterpriseLink>();

            CreateMap<EnterpriseUser, User>();
            CreateMap<User, EnterpriseUser>();

            CreateMap<EnterpriseLinks, Links>().ConvertUsing<EnterpriseLinksTypeConverter>();
            CreateMap<Links, EnterpriseLinks>().ConvertUsing<EnterpriseLinksTypeConverter>();
            CreateMap<EnterpriseBranch, Branch>().ConvertUsing<EnterpriseBranchTypeConverter>();

            CreateMap<EnterpriseRepository, Repository>().ConvertUsing<EnterpriseRepositoryTypeConverter>();
            CreateMap<Repository, EnterpriseRepository>().ConvertUsing<EnterpriseRepositoryTypeConverter>();
            CreateMap(typeof(IteratorBasedPage<>), typeof(IteratorBasedPage<>));
        }
    }
}