using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Linq;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Models.GitClientModels;
using GitClientVS.TeamFoundation.Extensions;
using log4net;
using LibGit2Sharp;
using Microsoft.TeamFoundation.Git.Controls.Extensibility;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TeamFoundation.Git.Extensibility;
using CloneOptions = Microsoft.TeamFoundation.Git.Controls.Extensibility.CloneOptions;
using System.Collections.Generic;
using GitClientVS.Contracts.Models;

namespace GitClientVS.TeamFoundation
{
    [Export(typeof(IGitService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class GitService : IGitService
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string remoteName;
        private readonly string mainBranch;

        private readonly IAppServiceProvider _appServiceProvider;

        /// <summary>
        /// This MEF export requires specific versions of TeamFoundation. 
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        IGitExt _gitService;


        [ImportingConstructor]
        public GitService(IAppServiceProvider appServiceProvider)
        {
            _appServiceProvider = appServiceProvider;
            remoteName = "origin";
            mainBranch = "master";
        }

        private Credentials CreateCredentials(string url, string user, SupportedCredentialTypes supportedCredentialTypes)
        {
            var userInformationService = _appServiceProvider.GetService<IUserInformationService>();
            return new SecureUsernamePasswordCredentials
            {
                Username = userInformationService.ConnectionData.UserName,
                Password = userInformationService.ConnectionData.Password.ToSecureString(),
            };
        }

        public GitRemoteRepository GetActiveRepository()
        {
            _gitService = _appServiceProvider.GetService<IGitExt>();
            var activeRepository = _gitService.ActiveRepositories.FirstOrDefault();
            return activeRepository.ToGitRepo();
        }

        public IEnumerable<LocalRepo> GetLocalRepositories()
        {
            var localRepositories = RegistryHelper
                .GetLocalRepositories()
                .Select(path => new LocalRepo(path.Split('\\').LastOrDefault(), path))
                .Where(x => x.Name != null)
                .ToList();

            return localRepositories;
        }

        public void CloneRepository(string cloneUrl, string repositoryName, string repositoryPath)
        {
            var gitExt = _appServiceProvider.GetService<IGitRepositoriesExt>();

            string path = Path.Combine(repositoryPath, repositoryName);

            Directory.CreateDirectory(path);

            try
            {
                gitExt.Clone(cloneUrl, path, CloneOptions.RecurseSubmodule);
            }
            catch (Exception ex)
            {
                Logger.Error($"Could not clone {cloneUrl} to {path}. {ex}");
                throw;
            }
        }

        private Repository GetRepository()
        {
            var gitExt = _appServiceProvider.GetService<IGitExt>();
            // _gitService = gitExt;
            var vsRepo = gitExt.ActiveRepositories.FirstOrDefault();

            Repository activeRepository;
            if (vsRepo == null)
            {
                var vsSolution = _appServiceProvider.GetService<IVsSolution>();
                string solutionDir, solutionFile, userFile;
                if (!ErrorHandler.Succeeded(vsSolution.GetSolutionInfo(out solutionDir, out solutionFile, out userFile)))
                {
                    Logger.Error($"Could not find active repository");
                    throw new Exception();
                }
                if (solutionDir == null)
                {
                    Logger.Error($"Could not find active repository");
                    throw new Exception();
                }
                activeRepository = new Repository(Repository.Discover(solutionDir));
            }
            else
            {
                activeRepository = new Repository(Repository.Discover(vsRepo.RepositoryPath));
            }

            return activeRepository;
        }

        public string GetDefaultRepoPath()
        {
            try
            {
                return RegistryHelper.GetLocalClonePath();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private void SetRemote(Repository activeRepository, string cloneUrl)
        {
            activeRepository.Config.Set($"remote.{remoteName}.url", cloneUrl);
            activeRepository.Config.Set($"remote.{remoteName}.fetch", $"+refs/heads/*:refs/remotes/{remoteName}/*");
        }

        private void Push(Repository activeRepository)
        {
            var pushOptions = new PushOptions()
            {
                CredentialsProvider = this.CreateCredentials
            };

            var remote = activeRepository.Network.Remotes[remoteName];
            if (activeRepository.Head?.Commits != null && activeRepository.Head.Commits.Any())
            {
                activeRepository.Network.Push(remote, "HEAD", @"refs/heads/" + mainBranch, pushOptions);
            }
        }

        private void Fetch(Repository activeRepository)
        {
            var fetchOptions = new FetchOptions()
            {
                CredentialsProvider = this.CreateCredentials
            };
            Commands.Fetch(activeRepository, remoteName, new string[0], fetchOptions, null);
        }

        private void SetTrackingRemote(Repository activeRepository)
        {
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

        public void PublishRepository(GitRemoteRepository repository)
        {
            Repository activeRepository = GetRepository();
            SetRemote(activeRepository, repository.CloneUrl);
            Push(activeRepository);
            Fetch(activeRepository);
            SetTrackingRemote(activeRepository);
        }




    }
}
