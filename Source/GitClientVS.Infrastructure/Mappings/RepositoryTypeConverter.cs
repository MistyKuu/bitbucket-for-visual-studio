using System;
using System.Linq;
using AutoMapper;
using BitBucket.REST.API.Models;
using GitClientVS.Contracts.Models.GitClientModels;

namespace GitClientVS.Infrastructure.Mappings
{
    public class RepositoryTypeConverter: ITypeConverter<Repository, GitRemoteRepository>
    {
        public GitRemoteRepository Convert(Repository source, GitRemoteRepository destination, ResolutionContext context)
        {
            var remoteRepository = new GitRemoteRepository();
            remoteRepository.Name = source.Name;
            remoteRepository.Description = source.Description;
            remoteRepository.IsPrivate = source.IsPrivate;
            remoteRepository.HasIssues = source.HasIssues;
            remoteRepository.HasWiki = source.HasWiki;
            remoteRepository.CloneUrl = source.Links.Clone.First().Href;

            return remoteRepository;
        }

    }
}