using System;

namespace GitClientVS.Contracts.Models.GitClientModels
{
    public class GitCommit
    {
        public string Hash { get; set; }

        public string Message { get; set; }

        public DateTime Date { get; set; }

        public GitUser Author { get; set; }
    }
}