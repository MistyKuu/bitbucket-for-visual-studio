namespace GitClientVS.Contracts.Models.GitClientModels
{
    public class GitComment
    {
        public GitUser User { get; set; }
        public string CreatedOn { get; set; }
        public string UpdatedOn { get; set; }
        public GitCommentContent Content { get; set; }
    }
}