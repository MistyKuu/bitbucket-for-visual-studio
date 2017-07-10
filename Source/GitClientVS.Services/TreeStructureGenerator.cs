using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitClientVS.Contracts.Interfaces;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Models;
using GitClientVS.Contracts.Models.GitClientModels;
using GitClientVS.Contracts.Models.Tree;
using ParseDiff;

namespace GitClientVS.Services
{
    [Export(typeof(ITreeStructureGenerator))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class TreeStructureGenerator : ITreeStructureGenerator
    {
        public IEnumerable<ICommentTree> CreateCommentTree(IEnumerable<GitComment> gitComments, Theme currentTheme, char separator = '/')
        {
            Dictionary<long, GitComment> searchableGitComments = new Dictionary<long, GitComment>();

            foreach (var comment in gitComments)
            {
                WrapComment(comment, currentTheme);

                searchableGitComments.Add(comment.Id, comment);
            }

            Dictionary<int, List<ObjectTree>> result = new Dictionary<int, List<ObjectTree>>();

            var maxLevel = -1;
            foreach (var comment in gitComments)
            {
                var level = 0;
                List<long> ids = new List<long>();
                StringBuilder path = new StringBuilder();

                ids.Add(comment.Id);
                var tmpComment = comment;
                while (tmpComment.Parent != null)
                {
                    ids.Add(tmpComment.Parent.Id);
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
                    UpdatedOn = comment.UpdatedOn,
                    IsInline = comment.IsInline,
                    Path = comment.Path,
                    From = comment.From,
                    To = comment.To,
                    IsDeleted = comment.IsDeleted
                }));
            }



            ICommentTree entryComment = new CommentTree();
            for (var i = 0; i <= maxLevel; i++)
            {
                List<ObjectTree> preparedComments = result[i];
                foreach (var objectTree in preparedComments)
                {
                    ICommentTree currentComment = entryComment;
                    var pathChunks = objectTree.Path.Split(separator);
                    foreach (var pathChunk in pathChunks)
                    {
                        var tmp = currentComment.Comments.Where(x => x.Comment.Id.Equals(long.Parse(pathChunk)));
                        if (tmp.Any())
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

            return entryComment.Comments;
        }

        public IEnumerable<ITreeFile> CreateFileTree(IEnumerable<FileDiff> fileDiffs, string rootFileName = "test", char separator = '/')
        {
            var entryFile = new TreeDirectory(rootFileName);

            foreach (var fileDiff in fileDiffs.Where(x => !string.IsNullOrEmpty(x.DisplayFileName.Trim())))
            {
                ITreeFile currentFile = entryFile;

                var pathChunks = fileDiff.DisplayFileName.Split(separator);
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
            ExpandTree(entryFile.Files);

            return entryFile.Files;
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

        private void WrapComment(GitComment comment, Theme currentTheme)
        {
            var foregroundColor = currentTheme == Theme.Light ? "black" : "white";
            comment.Content.DisplayHtml = $"<body style='background-color:transparent;color:{foregroundColor};font-size:13px'>" + comment.Content.Html + "</body>";
        }
    }
}
