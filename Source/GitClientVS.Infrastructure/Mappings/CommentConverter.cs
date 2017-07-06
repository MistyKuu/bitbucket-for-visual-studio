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
            GitComment result;

            if (source.Inline != null)
            {
                result = new InlineGitComment()
                {
                    From = source.Inline.From,
                    To = source.Inline.To,
                    Path = source.Inline.Path,
                    IsInline = true
                };
            }
            else
            {
                result = new GitComment() { IsInline = false };
            }

            result.Content = source.Content.MapTo<GitCommentContent>();
            result.CreatedOn = TimeConverter.GetDate(source.CreatedOn);
            result.UpdatedOn = TimeConverter.GetDate(source.UpdatedOn);
            result.Id = source.Id;
            result.Parent = source.MapTo<GitCommentParent>();
            result.User = source.User.MapTo<GitUser>();
            result.IsFile = source.Inline != null; //TODO THIS IS CLEARLY WRONG

            return result;
        }
    }
}