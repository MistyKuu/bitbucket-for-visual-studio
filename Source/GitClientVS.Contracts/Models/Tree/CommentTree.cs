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
        private bool _isReplyExpanded;
        private bool _isEditExpanded;
        private string _editContent;
        private string _replyContent;
        private bool _isExpanded;
        public ReactiveList<ICommentTree> Comments { get; set; }
        public GitComment Comment { get; set; }

        public bool IsExpanded
        {
            get => _isExpanded;
            set => this.RaiseAndSetIfChanged(ref _isExpanded, value);
        }

        public bool IsEditExpanded
        {
            get => _isEditExpanded;
            set => this.RaiseAndSetIfChanged(ref _isEditExpanded, value);
        }

        public bool IsReplyExpanded
        {
            get => _isReplyExpanded;
            set => this.RaiseAndSetIfChanged(ref _isReplyExpanded, value);
        }

        public string EditContent
        {
            get => _editContent;
            set => this.RaiseAndSetIfChanged(ref _editContent, value);
        }

        public string ReplyContent
        {
            get => _replyContent;
            set => this.RaiseAndSetIfChanged(ref _replyContent, value);
        }

        public bool AllDeleted => Comments.All(x => x.AllDeleted) && Comment.IsDeleted;

        public CommentTree(GitComment comment) 
        {
            Comment = comment;
            IsExpanded = true;
            EditContent = comment?.Content?.Html;
            Comments = new ReactiveList<ICommentTree>();
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