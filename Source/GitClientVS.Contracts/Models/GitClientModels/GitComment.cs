using System;
using System.Net;

namespace GitClientVS.Contracts.Models.GitClientModels
{
    public class GitComment
    {
        public GitUser User { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public GitCommentContent Content { get; set; }
        public long Id { get; set; }
        public GitCommentParent Parent { get; set; }
        public GitCommentInline Inline { get; set; }
        public bool IsDeleted { get; set; }
        public long Version { get; set; }
    }

    public class GitCommentInline
    {
        public long? From { get; set; }
        public long? To { get; set; }
        public string Path { get; set; }
    }

    public class GitCommentParent
    {
        public long Id { get; set; }
    }
}