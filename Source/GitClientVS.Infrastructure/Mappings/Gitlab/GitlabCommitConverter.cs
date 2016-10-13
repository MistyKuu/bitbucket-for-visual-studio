using System;
using AutoMapper;
using GitClientVS.Contracts.Models.GitClientModels;
using GitLab.NET.ResponseModels;

namespace GitClientVS.Infrastructure.Mappings.Gitlab
{
    public class GitlabCommitConverter : ITypeConverter<Commit,GitCommit>
    {
        public GitCommit Convert(Commit source, GitCommit destination, ResolutionContext context)
        {
            return new GitCommit()
            {
                Author = new GitUser()
                {
                    DisplayName = source.AuthorName,
                    Username = source.AuthorName,
                    Links = new GitLinks()
                    {
                        Avatar = new GitLink()
                        {
                            
                        }
                    }
                },
                Message = source.Message,
                Date = source.CreatedAt ?? DateTime.MinValue
            };
        }
    }
}