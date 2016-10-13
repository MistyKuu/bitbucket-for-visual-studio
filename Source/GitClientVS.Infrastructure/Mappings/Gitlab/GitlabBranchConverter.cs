using AutoMapper;
using GitClientVS.Contracts.Models.GitClientModels;
using GitClientVS.Infrastructure.Extensions;
using GitLab.NET.ResponseModels;

namespace GitClientVS.Infrastructure.Mappings.Gitlab
{
    public class GitlabBranchConverter : ITypeConverter<Branch, GitBranch>
    {
        public GitBranch Convert(Branch source, GitBranch destination, ResolutionContext context)
        {
            return new GitBranch()
            {
                Name = source.Name,
                Target = source.Commit.MapTo<GitCommit>()
            };
        }
    }
}