using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Models.GitClientModels;
using log4net;
using LibGit2Sharp;
using Microsoft.TeamFoundation.Git.Controls.Extensibility;
using Microsoft.VisualStudio.TeamFoundation.Git.Extensibility;
using CloneOptions = Microsoft.TeamFoundation.Git.Controls.Extensibility.CloneOptions;

namespace GitClientVS.VisualStudio.UI.Services
{
    [Export(typeof(IGitService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class GitService: IGitService
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IAppServiceProvider _appServiceProvider; 

        [ImportingConstructor]
        public GitService(IAppServiceProvider appServiceProvider)
        {
            _appServiceProvider = appServiceProvider;
        }

        public void CloneRepository(string cloneUrl, string repositoryName, string repositoryPath)
        {
            var gitExt = _appServiceProvider.GetService<IGitRepositoriesExt>();
            string path = Path.Combine(repositoryPath, repositoryName);

            Directory.CreateDirectory(path);

            try
            {
                gitExt.Clone(cloneUrl, path, CloneOptions.None);
            }
            catch (Exception ex)
            {
                Logger.Error($"Could not clone {cloneUrl} to {path}. {ex}");
                throw;
            }
        }

        public void PublishRepository(GitRemoteRepository repository, string username, string password)
        {
            var gitExt = _appServiceProvider.GetService<IGitExt>();
            var vsRepo = gitExt.ActiveRepositories.FirstOrDefault();
            // if null take git repository from solution
            if (vsRepo == null)
            {
                Logger.Error($"Could not find active repository");
                throw new Exception();
            }
            var remoteName = "origin";
            var mainBranch = "master";
            var activeRepository = new Repository(Repository.Discover(vsRepo.RepositoryPath));

            // set remote
            activeRepository.Config.Set($"remote.{remoteName}.url", repository.CloneUrl);
            activeRepository.Config.Set($"remote.{remoteName}.fetch", $"+refs/heads/*:refs/remotes/{remoteName}/*");

            // push

            var pushOptions = new PushOptions()
            {
                CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials
                {
                    Username = username,
                    Password = password
                }
            };

            var fetchOptions = new FetchOptions()
            {
                CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials
                {
                    Username = username,
                    Password = password
                }
            };

            var remote = activeRepository.Network.Remotes[remoteName];
            if (activeRepository.Head?.Commits != null && activeRepository.Head.Commits.Any())
            {
                activeRepository.Network.Push(remote, "HEAD", @"refs/heads/" + mainBranch, pushOptions);
            }

            // fetch


            activeRepository.Network.Fetch(remote, fetchOptions);

            // set tracking branch

            var remoteBranchName = "refs/remotes/" + remoteName + "/" + mainBranch;
            var remoteBranch = activeRepository.Branches[remoteBranchName];
            // if it's null, it's because nothing was pushed
            if (remoteBranch != null)
            {
                var localBranchName = "refs/heads/" + mainBranch;
                var localBranch = activeRepository.Branches[localBranchName];
                activeRepository.Branches.Update(localBranch, b => b.TrackedBranch = remoteBranch.CanonicalName);
            }
        }


    }
}
