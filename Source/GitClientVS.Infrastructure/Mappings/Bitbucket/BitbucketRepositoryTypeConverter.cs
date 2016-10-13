using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BitBucket.REST.API.Models;
using GitClientVS.Contracts.Models.GitClientModels;

namespace GitClientVS.Infrastructure.Mappings.Bitbucket
{
    public class BitbucketRepositoryTypeConverter: ITypeConverter<Repository, GitRepository>
    {
        public GitRepository Convert(Repository source, GitRepository destination, ResolutionContext context)
        {
            return new GitRepository()
            {
                Name = source.Name,
                Description = source.Description,
                IsPrivate = source.IsPrivate,
                HasWiki = source.HasWiki,
                HasIssues = source.HasIssues,
                IsForked = (source.Parent != null),
                CloneUrl = source.Links.Clone.First().Href,
                Owner = source.Owner.Username,
            };
        }
    }

    public class BitbucketReverseRepositoryTypeConverter : ITypeConverter<GitRepository, Repository>
    {
        public Repository Convert(GitRepository source, Repository destination, ResolutionContext context)
        {
            return new Repository()
            {
                Name = source.Name,
                Description = source.Description,
                IsPrivate = source.IsPrivate,
                HasIssues = source.HasIssues,
                HasWiki = source.HasWiki,
                Owner = new User()
                {
                  Username  = source.Owner
                },
                Links = new Links()
                {
                    Clone = new List<Link>(new Link[]
                    {
                        new Link() {Href = source.CloneUrl}
                    })
                }
            };
        }
    }
}