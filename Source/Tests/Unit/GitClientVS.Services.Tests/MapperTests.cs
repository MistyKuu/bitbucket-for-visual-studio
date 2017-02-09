using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BitBucket.REST.API.Models;
using GitClientVS.Contracts.Models.GitClientModels;
using GitClientVS.Infrastructure.Extensions;
using GitClientVS.Infrastructure.Mappings;
using NUnit.Framework;

namespace GitClientVS.Services.Tests
{
    public class MapperTests
    {
        [Test]
        public void RepositoryMapping()
        {
            Mapper.Initialize(config =>
            {
                config.CreateMap<Repository, GitRemoteRepository>().ConvertUsing<RepositoryTypeConverter>();
            });

            Mapper.AssertConfigurationIsValid();

            var repository = new Repository
            {
                Name = "test",
                HasIssues = true,
                IsPrivate = true,
                Description = "test",
                HasWiki = false,
                Owner = new User(),
                Links = new Links()
                {
                    Clone = new List<Link>(new Link[]
                    {
                        new Link() {Name = "test", Href = "urlexample"}
                    })
                }
            };

            var remoteRepository = repository.MapTo<GitRemoteRepository>();

            Assert.AreEqual(repository.Name, remoteRepository.Name);
            Assert.AreEqual(repository.Links.Clone.First().Href, remoteRepository.CloneUrl);
        }
    }
}