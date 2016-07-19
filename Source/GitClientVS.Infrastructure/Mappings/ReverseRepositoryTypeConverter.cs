using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BitBucket.REST.API.Models;
using GitClientVS.Contracts.Models.GitClientModels;

namespace GitClientVS.Infrastructure.Mappings
{
    public class ReverseRepositoryTypeConverter: ITypeConverter<GitRemoteRepository, Repository>
    {
        public Repository Convert(GitRemoteRepository source, Repository destination, ResolutionContext context)
        {
            var remoteRepository = new Repository();
            remoteRepository.Name = source.Name;
            remoteRepository.Description = source.Description;
            remoteRepository.IsPrivate = source.IsPrivate;
            remoteRepository.HasIssues = source.HasIssues;
            remoteRepository.HasWiki = source.HasWiki;
            remoteRepository.Links = new Links()
            {
                Clone = new List<Link>(new Link[]
                {
                    new Link() {Href = source.CloneUrl}
                })
            };

            return remoteRepository;
        }

    }
}