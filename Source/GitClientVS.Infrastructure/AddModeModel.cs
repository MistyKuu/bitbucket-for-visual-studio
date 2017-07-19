using GitClientVS.Contracts.Models.GitClientModels;

namespace GitClientVS.Infrastructure
{
    public class AddModeModel
    {
        public GitCommentInline Inline { get; }

        public AddModeModel(GitCommentInline inline)
        {
            Inline = inline;
        }
    }
}