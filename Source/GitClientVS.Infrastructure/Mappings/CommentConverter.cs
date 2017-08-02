using AutoMapper;
using BitBucket.REST.API.Models.Standard;
using GitClientVS.Contracts.Models.GitClientModels;
using GitClientVS.Infrastructure.Extensions;
using GitClientVS.Infrastructure.Utils;

namespace GitClientVS.Infrastructure.Mappings
{
    public class CommentConverter : ITypeConverter<Comment, GitComment>
    {
        public GitComment Convert(Comment source, GitComment destination, ResolutionContext context)
        {
            GitComment result = new GitComment
            {
                Content = source.Content.MapTo<GitCommentContent>(),
                CreatedOn = TimeConverter.GetDate(source.CreatedOn),
                UpdatedOn = TimeConverter.GetDate(source.UpdatedOn),
                Id = source.Id,
                Parent = source.Parent.MapTo<GitCommentParent>(),
                User = source.User.MapTo<GitUser>(),
                Inline = source.Inline != null ? new GitCommentInline()
                {
                    From = source.Inline.From,
                    To = source.Inline.To,
                    Path = source.Inline.Path,
                } : null,
                IsDeleted = source.IsDeleted,
                Version = source.Version
            };


            return result;
        }
    }
}