using AutoMapper;
using GitClientVS.Contracts.Models.GitClientModels;
using GitClientVS.Infrastructure.Extensions;
using GitLab.NET.ResponseModels;

namespace GitClientVS.Infrastructure.Mappings.Gitlab
{
    public class GitlabNamespaceConverter : ITypeConverter<Namespace, GitTeam>
    {
        public GitTeam Convert(Namespace source, GitTeam destination, ResolutionContext context)
        {
            return new GitTeam()
            {
                Name = source.Path
            };
        }
    }
}