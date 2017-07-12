using AutoMapper;
using BitBucket.REST.API.Models.Standard;

namespace BitBucket.REST.API.Mappings
{
    public class CommentV1TypeConverter : ITypeConverter<CommentV1, Comment>
    {
        public Comment Convert(CommentV1 source, Comment destination, ResolutionContext context)
        {
            var comment = new Comment()
            {
                Content = new Content() { Html = source.ContentRendered },
                CreatedOn = source.CreatedOn,
                UpdatedOn = source.UpdatedOn,
                Id = source.CommentId.Value,
                IsDeleted = source.Deleted,
                Parent = source.ParentId.HasValue ? new Parent() { Id = source.ParentId.Value } : null,
                User = new User()
                {
                    Username = source.AuthorInfo?.UserName,
                    DisplayName = source.AuthorInfo?.DisplayName,
                    Links = new Links()
                    {
                        Self = new Link() { Href = source.AuthorInfo?.AvatarUrl },
                        Avatar = new Link() { Href = source.AuthorInfo?.AvatarUrl },
                    }
                },
            };

            if (source.FileName != null)
            {
                comment.Inline = new Inline()
                {
                    Path = source.FileName,
                    From = source.LineFrom,
                    To = source.LineTo,
                };
            }
            return comment;
        }
    }
}