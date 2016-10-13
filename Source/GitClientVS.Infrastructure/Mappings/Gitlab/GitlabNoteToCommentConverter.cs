using AutoMapper;
using GitClientVS.Contracts.Models.GitClientModels;
using GitClientVS.Infrastructure.Extensions;
using GitLab.NET.ResponseModels;

namespace GitClientVS.Infrastructure.Mappings.Gitlab
{
    public class GitlabNoteToCommentConverter : ITypeConverter<Note, GitComment>
    {

        public GitComment Convert(Note source, GitComment destination, ResolutionContext context)
        {
            return new GitComment()
            {
                Id =  source.Id,
                Content = new GitCommentContent() { Html = source.Body },
                User = source.Author.MapTo<GitUser>(),
                CreatedOn = source.CreatedAt,
                UpdatedOn = source.UpdatedAt,
            };
        }
    }
}