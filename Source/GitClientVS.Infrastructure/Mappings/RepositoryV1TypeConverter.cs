using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BitBucket.REST.API.Models.Standard;

namespace GitClientVS.Infrastructure.Mappings
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
                HasIssues = source.HasIssues,
                HasWiki = source.HasWiki,
                Parent = source.IsFork ? new Parent() : null,//todo this is only because mapping between repository and gitremoterepository uses that
                Owner = new User() { Username = source.Owner },
            };
        }
    }
}
