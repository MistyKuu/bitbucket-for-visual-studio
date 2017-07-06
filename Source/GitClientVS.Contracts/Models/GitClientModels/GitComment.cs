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
        public bool IsInline { get; set; }
        public int? From { get; set; }
        public int? To { get; set; }
        public string Path { get; set; }
    }

    public class GitCommentParent
    {
        public long Id { get; set; }
    }
}