using System;
using System.IO;
using System.Linq;
using GitClientVS.Contracts.Models.GitClientModels;
using LibGit2Sharp;
using Microsoft.VisualStudio.TeamFoundation.Git.Extensibility;

namespace GitClientVS.TeamFoundation.Extensions
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

            var repoUrl = repo?.Network.Remotes["origin"]?.Url ?? repo?.Network.Remotes.FirstOrDefault()?.Url;
            string repoName = null, ownerName = null;
            if (repoUrl != null)
            {
                try
                {
                    var repoUri = new Uri(repoUrl);
                    repoName = repoUri.Segments.Last().TrimEnd('/').TrimEnd(".git");
                    ownerName = (repoUri.Segments[repoUri.Segments.Length - 2] ?? "").TrimEnd('/');
                }
                catch (Exception)
                {
                    // probably ssh remote
                    return null;
                }

            }

            var branches = repo.Branches
                .Select(x => new GitLocalBranch()
                {
                    Name = x.FriendlyName,
                    IsRemote = x.IsRemote,
                    IsHead = x.IsCurrentRepositoryHead,
                    TrackedBranchName = x.TrackedBranch?.FriendlyName.Replace(x.TrackedBranch?.Remote.Name + "/", string.Empty),
                    Target = new GitCommit() { Hash = x.Tip?.Sha },
                })
                .ToList();


            var repository = new GitRemoteRepository(repoName, ownerName, repoUrl, branches);
            return repository;
        }
    }
}