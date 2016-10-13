using AutoMapper;
using GitClientVS.Contracts.Models.GitClientModels;
using GitLab.NET.ResponseModels;

namespace GitClientVS.Infrastructure.Mappings.Gitlab
{
    public class GitlabRepositoryConverter : ITypeConverter<Project, GitRepository>
    {
        public GitRepository Convert(Project source, GitRepository destination, ResolutionContext context)
        {
            return new GitRepository()
            {
                Name = source.Name,
                Description = source.Description,
                IsPrivate = !source.Public,
                HasWiki = source.WikiEnabled,
                HasIssues = source.OpenIssuesCount != 0,
                IsForked = (source.ForksCount != 0),
                CloneUrl = source.HttpUrlToRepo,
                Owner = source.Owner.Username,
            };
        }
    }
}