﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GitClientVS.Contracts.Events;
using GitClientVS.Contracts.Interfaces;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Models;
using GitClientVS.Contracts.Models.GitClientModels;
using GitClientVS.Contracts.Models.Tree;
using ReactiveUI;

namespace GitClientVS.Infrastructure.ViewModels
{
    [Export(typeof(IPullRequestsDetailViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class PullRequestsDetailViewModel : ViewModelBase, IPullRequestsDetailViewModel
    {
        private readonly IGitClientService _gitClientService;
        private readonly IUserInformationService _userInformationService;
        private readonly IMessageBoxService _messageBoxService;
        private string _errorMessage;
        private bool _isLoading;
        private ReactiveCommand _initializeCommand;
        private ReactiveCommand<Unit, Unit> _approveCommand;
        private ReactiveCommand<Unit, Unit> _disapproveCommand;
        private ReactiveCommand<Unit, Unit> _declineCommand;
        private ReactiveCommand<Unit, Unit> _mergeCommand;
        private ReactiveCommand<Unit, Unit> _confirmationMergeCommand;
        private ReactiveCommand<Unit, Unit> _confirmationDeclineCommand;
        private ReactiveCommand<Unit, Unit> _refreshPullRequestCommand;

        private GitPullRequest _pullRequest;
        private string _mainSectionCommandText;
        private Theme _currentTheme;
        private ReactiveList<PullRequestActionModel> _actionCommands;
        private bool _hasAuthorApproved;
        private IEventAggregatorService _eventAggregatorService;
        private IDataNotifier _dataNotifier;

        public string PageTitle => "Pull Request Details";

        public string MainSectionCommandText
        {
            get => _mainSectionCommandText;
            set => this.RaiseAndSetIfChanged(ref _mainSectionCommandText, value);
        }

        public GitPullRequest PullRequest
        {
            get => _pullRequest;
            set => this.RaiseAndSetIfChanged(ref _pullRequest, value);
        }

        public Theme CurrentTheme
        {
            get => _currentTheme;
            set => this.RaiseAndSetIfChanged(ref _currentTheme, value);
        }

        public ReactiveList<PullRequestActionModel> ActionCommands
        {
            get => _actionCommands;
            set => this.RaiseAndSetIfChanged(ref _actionCommands, value);
        }

        public bool HasAuthorApproved
        {
            get => _hasAuthorApproved;
            set => this.RaiseAndSetIfChanged(ref _hasAuthorApproved, value);
        }

        public IPullRequestDiffViewModel PullRequestDiffViewModel { get; set; }

        public IEnumerable<ReactiveCommand> ThrowableCommands => new[] { _initializeCommand, _mergeCommand, _approveCommand, _disapproveCommand, _declineCommand }.Concat(PullRequestDiffViewModel.ThrowableCommands);
        public IEnumerable<ReactiveCommand> LoadingCommands => new[] { _initializeCommand, _approveCommand, _disapproveCommand, _declineCommand, _mergeCommand };

        public string ErrorMessage
        {
            get => _errorMessage;
            set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => this.RaiseAndSetIfChanged(ref _isLoading, value);
        }

        [ImportingConstructor]
        public PullRequestsDetailViewModel(
            IGitClientService gitClientService,
            IGitService gitService,
            ICommandsService commandsService,
            IUserInformationService userInformationService,
            IEventAggregatorService eventAggregatorService,
            IMessageBoxService messageBoxService,
            IDataNotifier dataNotifier,
            IPullRequestDiffViewModel pullRequestDiffViewModel
            )
        {
            _gitClientService = gitClientService;
            _userInformationService = userInformationService;
            _messageBoxService = messageBoxService;
            _eventAggregatorService = eventAggregatorService;
            _dataNotifier = dataNotifier;

            CurrentTheme = userInformationService.CurrentTheme;

            PullRequestDiffViewModel = pullRequestDiffViewModel;

        }

        protected override IEnumerable<IDisposable> SetupObservables()
        {
            yield return _eventAggregatorService.GetEvent<ThemeChangedEvent>().Subscribe(ev =>
            {
                CurrentTheme = ev.Theme;
            });

            this.WhenAnyObservable(
                x => x._approveCommand,
                x => x._declineCommand,
                x => x._disapproveCommand,
                x => x._mergeCommand,
                x => x._refreshPullRequestCommand
                )
                .Where(x => PullRequest != null)
                .Select(x => PullRequest.Id)
                .InvokeCommand(_initializeCommand);

            this.WhenAnyValue(x => x.PullRequest).Where(x => x != null).Subscribe(_ => { this.RaisePropertyChanged(nameof(PageTitle)); });
        }

        public ICommand InitializeCommand => _initializeCommand;
        public ICommand RefreshPullRequestCommand => _refreshPullRequestCommand;

        public void InitializeCommands()
        {
            _initializeCommand = ReactiveCommand.CreateFromTask<long>(LoadPullRequestData);
            _refreshPullRequestCommand = ReactiveCommand.Create(() => { });
            _approveCommand = ReactiveCommand.CreateFromTask(async _ =>
            {
                await _gitClientService.ApprovePullRequest(PullRequest.Id);
                _dataNotifier.ShouldUpdate = true;
            });

            _disapproveCommand = ReactiveCommand.CreateFromTask(async _ =>
            {
                await _gitClientService.DisapprovePullRequest(PullRequest.Id);
                _dataNotifier.ShouldUpdate = true;
            });

            _declineCommand = ReactiveCommand.CreateFromTask(async _ =>
            {
                await _gitClientService.DeclinePullRequest(PullRequest.Id, PullRequest.Version);
                _dataNotifier.ShouldUpdate = true;
            });

            _mergeCommand = ReactiveCommand.CreateFromTask(async _ =>
            {
                await MergePullRequest();
                _dataNotifier.ShouldUpdate = true;
            });
            _confirmationMergeCommand = ReactiveCommand.CreateFromTask(_ => RunMergeConfirmation());
            _confirmationDeclineCommand = ReactiveCommand.CreateFromTask(_ => RunDeclineConfirmation());
        }

        private Task RunDeclineConfirmation()
        {
            _messageBoxService.ExecuteCommandWithConfirmation(
               "Declining Pull Request",
               "Do you really want to decline this pull request?",
               _declineCommand
               );

            return Task.CompletedTask;
        }

        private Task RunMergeConfirmation()
        {
            _messageBoxService.ExecuteCommandWithConfirmation(
               "Merging Pull Request",
               "Do you really want to merge this pull request?",
               _mergeCommand
               );

            return Task.CompletedTask;
        }


        private async Task LoadPullRequestData(long id)
        {
            var tasks = new[]
            {
                GetPullRequestInfo(id),
                CreateCommits(id),
                CreateComments(id),
                CreateDiffContent(id)
            };

            await Task.WhenAll(tasks);

            PullRequestDiffViewModel.FromCommit = PullRequest.SourceBranch.Target.Hash;
            PullRequestDiffViewModel.ToCommit = PullRequest.DestinationBranch.Target.Hash;

            UpdateCommentsCountInFiles(PullRequestDiffViewModel.FilesTree);

        }

        private void UpdateCommentsCountInFiles(IEnumerable<ITreeFile> files)
        {
            foreach (var treeFile in files)
            {
                treeFile.Comments = PullRequestDiffViewModel.CommentViewModel.Comments.Where(x=>!x.IsDeleted).Count(x => x.Inline != null && treeFile.FileDiff != null && x.Inline.Path == treeFile.FileDiff.DisplayFileName);
                UpdateCommentsCountInFiles(treeFile.Files);
            }
        }

        private async Task GetPullRequestInfo(long id)
        {
            var pullRequest = await _gitClientService.GetPullRequest(id);
            CreatePullRequestCommands(pullRequest);
            PullRequest = pullRequest;
        }

        private async Task CreateComments(long id)
        {
            await PullRequestDiffViewModel.UpdateComments(id);
        }

        private async Task CreateCommits(long id)
        {
            var commits = (await _gitClientService.GetPullRequestCommits(id)).ToList();
            PullRequestDiffViewModel.AddCommits(commits);
        }

        private async Task CreateDiffContent(long id)
        {
            var fileDiffs = (await _gitClientService.GetPullRequestDiff(id)).ToList();

            PullRequestDiffViewModel.AddFileDiffs(fileDiffs);
        }

        private void CreatePullRequestCommands(GitPullRequest pullRequest)
        {
            var isApproved = true;
            bool isApproveAvailable = false;

            foreach (var reviewer in pullRequest.Reviewers)
                if (reviewer.Key.Uuid == _userInformationService.ConnectionData.Id)
                {
                    isApproveAvailable = true;
                    isApproved = reviewer.Value;
                }

            var approvesCount = pullRequest.Reviewers.Count(x => x.Value);
            var author = pullRequest.Reviewers.FirstOrDefault(x => x.Key.Uuid == pullRequest.Author.Uuid);
            HasAuthorApproved = author.Value;
            if (author.Key != null)
                pullRequest.Reviewers.Remove(author.Key);

            ActionCommands = new ReactiveList<PullRequestActionModel>
            {
                new PullRequestActionModel("Merge", _confirmationMergeCommand),
                new PullRequestActionModel("Decline", _confirmationDeclineCommand),
                !isApproveAvailable || !isApproved
                    ? new PullRequestActionModel($"Approve ({approvesCount})", _approveCommand)
                    : new PullRequestActionModel($"Unapprove ({approvesCount})", _disapproveCommand)
            };
        }


        private async Task MergePullRequest()
        {
            var gitMergeRequest = new GitMergeRequest()
            {
                CloseSourceBranch = PullRequest.CloseSourceBranch,
                Id = PullRequest.Id,
                MergeStrategy = "merge_commit",
                Version = PullRequest.Version
            };

            await _gitClientService.MergePullRequest(gitMergeRequest);
        }
    }
}
