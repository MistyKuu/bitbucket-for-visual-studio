using AutoMapper;
using GitClientVS.Contracts.Models.GitClientModels;
using GitClientVS.Infrastructure.Extensions;
using GitLab.NET.ResponseModels;

namespace GitClientVS.Infrastructure.Mappings.Gitlab
{
    public class GitlabCommentConverter : ITypeConverter<Comment, GitComment>
    {
        private static long _id = 0;

        public GitComment Convert(Comment source, GitComment destination, ResolutionContext context)
        {
            return new GitComment()
            {
                Id = _id++,
                Content = new GitCommentContent() { Html = source.Note },
                User = source.Author.MapTo<GitUser>(),
            };
        }
    }
}