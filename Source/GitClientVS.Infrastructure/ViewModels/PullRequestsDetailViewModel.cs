using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Models;
using GitClientVS.Contracts.Models.GitClientModels;
using GitClientVS.Infrastructure.Extensions;
using ParseDiff;
using ReactiveUI;

namespace GitClientVS.Infrastructure.ViewModels
{
    [Export(typeof(IPullRequestsDetailViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class PullRequestsDetailViewModel : ViewModelBase, IPullRequestsDetailViewModel
    {
        private readonly IGitClientService _gitClientService;
        private readonly IGitService _gitService;
        private readonly ICommandsService _commandsService;
        private readonly IDiffFileParser _diffFileParser;
        private string _errorMessage;
        private bool _isLoading;
        private ReactiveCommand<Unit> _initializeCommand;
        private IEnumerable<GitCommit> _commits;
        private IEnumerable<GitComment> _comments;
        private ReactiveCommand<Unit> _showDiffCommand;
        private IEnumerable<FileDiff> _fileDiffs;
        private List<ITreeFile> _filesTree;
        private List<ICommentTree> _commentTree;

        [ImportingConstructor]
        public PullRequestsDetailViewModel(
            IGitClientService gitClientService,
            IGitService gitService,
            ICommandsService commandsService,
            IDiffFileParser diffFileParser
            )
        {
            _gitClientService = gitClientService;
            _gitService = gitService;
            _commandsService = commandsService;
            _diffFileParser = diffFileParser;
        }

        public ICommand InitializeCommand => _initializeCommand;
        public ICommand ShowDiffCommand => _showDiffCommand;

        public void InitializeCommands()
        {
            _initializeCommand = ReactiveCommand.CreateAsyncTask(Observable.Return(true), x => LoadPullRequestData((GitPullRequest)x));
            _showDiffCommand = ReactiveCommand.CreateAsyncTask(Observable.Return(true), (x) => ShowDiff((FileDiff)x));
        }

        private async Task ShowDiff(FileDiff diff)
        {
            _commandsService.ShowDiffWindow(diff);
        }

        public IEnumerable<GitComment> Comments
        {
            get { return _comments; }
            set { this.RaiseAndSetIfChanged(ref _comments, value); }
        }

        public IEnumerable<GitCommit> Commits
        {
            get { return _commits; }
            set { this.RaiseAndSetIfChanged(ref _commits, value); }
        }

        public IEnumerable<FileDiff> FileDiffs
        {
            get { return _fileDiffs; }
            set { this.RaiseAndSetIfChanged(ref _fileDiffs, value); }
        }

        public List<ICommentTree> CommentTree
        {
            get { return _commentTree; }
            set { this.RaiseAndSetIfChanged(ref _commentTree, value); }
        }

        public List<ITreeFile> FilesTree
        {
            get { return _filesTree; }
            set { this.RaiseAndSetIfChanged(ref _filesTree, value); }
        }

        private async Task LoadPullRequestData(GitPullRequest pr)
        {
            var id = pr.Id;
            var currentRepository = _gitService.GetActiveRepository();
            Commits = await _gitClientService.GetPullRequestCommits("atlassian-rest", "atlassian", id);
            Comments = await _gitClientService.GetPullRequestComments("atlassian-rest", "atlassian", id);
            var diff = await _gitClientService.GetPullRequestDiff("atlassian-rest", "atlassian", id);
            FileDiffs = _diffFileParser.Parse(diff).ToList();
            CreateFileTree(FileDiffs.ToList());
            CreateCommentTree(Comments.ToList());
        }

        public void CreateCommentTree(List<GitComment> gitComments)
        {
            Dictionary<long, GitComment> searchableGitComments = new Dictionary<long, GitComment>();
            foreach (var comment in gitComments)
            {
                searchableGitComments.Add(comment.Id, comment);
            }

            Dictionary<int, List<ObjectTree>> result = new Dictionary<int, List<ObjectTree>>();

            var separator = '/';
            var maxLevel = 0;
            foreach (var comment in gitComments)
            {
                var level = 0;
                List<long> ids = new List<long>();
                StringBuilder path = new StringBuilder();

                ids.Add(comment.Id);
               // path.Append(comment.Id);

              
                var tmpComment = comment;
                while (tmpComment.Parent != null)
                {
                   // path.Append(separator);
                    ids.Add(tmpComment.Parent.Id);
                   // path.Append(tmpComment.Parent.Id);
                    level++;

                    tmpComment = searchableGitComments[tmpComment.Parent.Id];
                }

                if (!result.ContainsKey(level))
                {
                    result[level] = new List<ObjectTree>();
                    if (level > maxLevel)
                    {
                        maxLevel = level;
                    }
                }

                for (var pathIndex = ids.Count - 1; pathIndex > -1; pathIndex -= 1)
                {
                    path.Append(ids[pathIndex]);

                    if (pathIndex > 0)
                    {
                        path.Append(separator);
                    }
                }

                result[level].Add(new ObjectTree(path.ToString(), new GitComment()
                {
                    Content = comment.Content,
                    CreatedOn = comment.CreatedOn,
                    Id = comment.Id,
                    Parent = comment.Parent,
                    User = comment.User,
                    UpdatedOn = comment.UpdatedOn
                }));
            }



            ICommentTree entryComment = new CommentTree();
            for (var i = 0; i < maxLevel; i++)
            {
                List<ObjectTree> preparedComments = result[i];
             
                foreach (var objectTree in preparedComments)
                {

                    ICommentTree currentComment = entryComment;
                    var pathChunks = objectTree.Path.Split(separator);
                 
                    foreach (var pathChunk in pathChunks)
                    {
                        var tmp = currentComment.Comments.Where(x => x.Comment.Id.Equals(long.Parse(pathChunk)));
                        if (tmp.Count() > 0)
                        {
                            currentComment = tmp.Single();
                        }
                        else
                        {
                            ICommentTree newItem = new CommentTree(objectTree.GitComment);
                            currentComment.Comments.Add(newItem);
                            currentComment = newItem;
                        }
                    }

                }
            }

            CommentTree = entryComment.Comments;

        }

  

        public void CreateFileTree(List<FileDiff> fileDiffs, string rootFileName = "test", char separator = '/')
        {
            var entryFile = new TreeDirectory(rootFileName);

            foreach (var fileDiff in fileDiffs.Where(x => !string.IsNullOrEmpty(x.From.Trim())))
            {
                ITreeFile currentFile = entryFile;

                var pathChunks = fileDiff.From.Split(separator);
                var lastItem = pathChunks.Last();
                foreach (var pathChunk in pathChunks)
                {
                    var tmp = currentFile.Files.Where(x => x.Name.Equals(pathChunk));
                    if (tmp.Count() > 0)
                    {
                        currentFile = tmp.Single();
                    }
                    else
                    {
                        ITreeFile newItem;
                        if (lastItem.Equals(pathChunk))
                        {
                            newItem = new TreeFile(pathChunk, fileDiff);
                        }
                        else
                        {
                            newItem = new TreeDirectory(pathChunk);
                        }

                        currentFile.Files.Add(newItem);
                        currentFile = newItem;
                    }
                }
            }


            FilesTree = entryFile.Files;
            ExpandTree(FilesTree);
        }

        private void ExpandTree(List<ITreeFile> files)
        {
            foreach (var treeFile in files)
            {
                if (treeFile.Files.Any())
                    ExpandTree(treeFile.Files);

                treeFile.IsExpanded = true;
            }
        }


        public IEnumerable<IReactiveCommand> ThrowableCommands => new[] { _initializeCommand, _showDiffCommand };
        public IEnumerable<IReactiveCommand> LoadingCommands => new[] { _initializeCommand, _showDiffCommand };

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                this.RaiseAndSetIfChanged(ref _errorMessage, value);
            }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set { this.RaiseAndSetIfChanged(ref _isLoading, value); }
        }

    }
}
