using System.Globalization;
using AutoMapper;
using BitBucket.REST.API.Models.Standard;

namespace BitBucket.REST.API.Mappings.Converters
{
    public class RepositoryV1TypeConverter : ITypeConverter<RepositoryV1, Repository>
    {
        public Repository Convert(RepositoryV1 source, Repository destination, ResolutionContext context)
        {
            return new Repository()
            {
                Name = source.Name,
                Description = source.Description,
                IsPrivate = source.IsPrivate,
                Scm = source.Scm,
                CreatedOn = source.UtcCreatedOn,
                UpdatedOn = source.UtcLastUpdated,
                HasIssues = source.HasIssues,
                HasWiki = source.HasWiki,
                Parent = source.IsFork ? new Parent() : null,//todo this is only because mapping between repository and gitremoterepository uses that
                Owner = new User() { Username = source.Owner, Uuid = source.Owner },
            };
        }
    }
}
