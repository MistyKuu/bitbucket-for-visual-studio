using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BitBucket.REST.API.Models;
using BitBucket.REST.API.Models.Standard;
using GitClientVS.Contracts.Models.GitClientModels;

namespace GitClientVS.Infrastructure.Mappings
{
    public class RepositoryTypeConverter : ITypeConverter<Repository, GitRemoteRepository>
    {
        public GitRemoteRepository Convert(Repository source, GitRemoteRepository destination, ResolutionContext context)
        {
            return new GitRemoteRepository()
            {
                Name = source.Name,
                Description = source.Description,
                IsPrivate = source.IsPrivate,
                HasWiki = source.HasWiki,
                HasIssues = source.HasIssues,
                IsForked = (source.Parent != null),
                CloneUrl = source.Links.Clone.First().Href,
                Owner = source.Owner.UserName,
            };
        }
    }

    public class ReverseRepositoryTypeConverter : ITypeConverter<GitRemoteRepository, Repository>
    {
        public Repository Convert(GitRemoteRepository source, Repository destination, ResolutionContext context)
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
                    UserName = source.Owner,
                    Uuid = source.Owner
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