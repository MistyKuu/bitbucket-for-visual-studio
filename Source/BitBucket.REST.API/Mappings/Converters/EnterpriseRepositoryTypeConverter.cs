using AutoMapper;
using BitBucket.REST.API.Models.Enterprise;
using BitBucket.REST.API.Models.Standard;

namespace BitBucket.REST.API.Mappings.Converters
{
    public class EnterpriseRepositoryTypeConverter : ITypeConverter<EnterpriseRepository, Repository>
    {
        public Repository Convert(EnterpriseRepository source, Repository destination, ResolutionContext context)
        {
            return new Repository()
            {
                Scm = source.Scm,
                Links = source.Links.MapTo<Links>(),
                IsPrivate = !source.IsPublic,
                ForkPolicy = source.Forkable != null && source.Forkable.Value ? "YES" : "NO", //todo check valid stirng
                Owner = source.Project.Owner.MapTo<User>(),
                Name = source.Name
            };
        }
    }
}