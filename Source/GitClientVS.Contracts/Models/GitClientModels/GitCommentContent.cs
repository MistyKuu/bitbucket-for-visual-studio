namespace GitClientVS.Contracts.Models.GitClientModels
{
    public class GitCommentContent
    {
        public string Raw { get; set; }

        public string Markup { get; set; }

        public string Html { get; set; }

        public string DisplayHtml { get; set; }
    }
}