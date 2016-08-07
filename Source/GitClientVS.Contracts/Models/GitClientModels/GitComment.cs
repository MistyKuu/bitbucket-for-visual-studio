using System;

namespace GitClientVS.Contracts.Models.GitClientModels
{
    public class GitComment
    {
        public GitUser User { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public GitCommentContent Content { get; set; }
    }
}