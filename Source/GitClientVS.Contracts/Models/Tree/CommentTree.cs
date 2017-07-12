using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using GitClientVS.Contracts.Models.GitClientModels;
using ReactiveUI;

namespace GitClientVS.Contracts.Models.Tree
{
    public class CommentTree : ReactiveObject, ICommentTree
    {
        public ReactiveList<ICommentTree> Comments { get; set; }
        public GitComment Comment { get; set; }
        public bool IsExpanded { get; set; }
        public bool AllDeleted
        {
            get => Comments.All(x => x.AllDeleted) && Comment.IsDeleted;
        }

        public CommentTree()
        {
            Comments = new ReactiveList<ICommentTree> { ChangeTrackingEnabled = true };
        }

        public CommentTree(GitComment comment) : this()
        {
            Comment = comment;
            IsExpanded = true;
        }

        public void DeleteCurrentComment()
        {
            Comment.IsDeleted = true;
        }

        public void AddComment(GitComment comment)
        {
            Comments.Add(new CommentTree(comment));
            this.RaisePropertyChanged(nameof(AllDeleted));
        }
    }
}