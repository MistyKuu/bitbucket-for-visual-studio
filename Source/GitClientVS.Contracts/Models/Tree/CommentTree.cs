using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GitClientVS.Contracts.Models.GitClientModels;
using ReactiveUI;

namespace GitClientVS.Contracts.Models.Tree
{
    public class CommentTree : ReactiveObject, ICommentTree
    {
        public ReactiveList<ICommentTree> Comments { get; set; }
        public GitComment Comment { get; set; }
        public bool IsExpanded { get; set; }
        public bool AllDeleted => Comments.All(x => x.AllDeleted) && Comment.IsDeleted;

        public CommentTree()
        {
            Comments = new ReactiveList<ICommentTree>();
            this.WhenAnyObservable(x => x.Comments.Changed).Subscribe(_ => { this.RaisePropertyChanged(nameof(AllDeleted)); });
        }

        public void DeleteCurrentComment()
        {
            Comment.IsDeleted = true;
            this.RaisePropertyChanged(nameof(AllDeleted));
            this.RaisePropertyChanged(nameof(Comment));
        }

        public void AddComment(GitComment comment)
        {
            Comments.Add(new CommentTree(comment));
        }

        public CommentTree(GitComment comment)
        {
            Comment = comment;
            Comments = new ReactiveList<ICommentTree>();
            IsExpanded = true;
        }
    }
}