using GitClientVS.Contracts.Models.GitClientModels;

namespace GitClientVS.Contracts.Models
{
    public class ObjectTree
    {
        public string Path { get; set; }
        public GitComment GitComment { get; set; }

        public ObjectTree(string path, GitComment gitComment)
        {
            Path = path;
            GitComment = gitComment;
        }
    }
}