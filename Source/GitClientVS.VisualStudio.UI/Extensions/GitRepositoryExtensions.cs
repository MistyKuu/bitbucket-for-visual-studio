using System.IO;
using GitClientVS.Contracts.Models.GitClientModels;
using LibGit2Sharp;
using Microsoft.VisualStudio.TeamFoundation.Git.Extensibility;

namespace GitClientVS.VisualStudio.UI.Extensions
{
    public static class GitRepositoryExtensions
    {
        public static GitRemoteRepository ToModel(this IGitRepositoryInfo source)
        {
            if (source == null) return null;

            var dir = new DirectoryInfo(source.RepositoryPath);
            var repoPath = Repository.Discover(source.RepositoryPath);
            var repo = repoPath == null ? null : new Repository(repoPath);
            if (repo == null) return null;

            return new GitRemoteRepository(dir.Name, repo?.Network.Remotes["origin"]?.Url);
        }
    }
}