using System.Globalization;
using AutoMapper;
using BitBucket.REST.API.Helpers;
using BitBucket.REST.API.Models.Enterprise;
using BitBucket.REST.API.Models.Standard;

namespace BitBucket.REST.API.Mappings.Converters
{
    public class EnterpriseCommentTypeConverter : ITypeConverter<EnterpriseComment, Comment>
    {
        public Comment Convert(EnterpriseComment source, Comment destination, ResolutionContext context)
        {
            var comment = new Comment()
            {
                User = source.User.MapTo<User>(),
                Content = new Content() { Html = source.Text },
                CreatedOn = source.CreatedOn.FromUnixTimeStamp().ToString(CultureInfo.InvariantCulture),
                UpdatedOn = source.UpdatedOn.FromUnixTimeStamp().ToString(CultureInfo.InvariantCulture),
                Id = source.Id,
                Parent = source.Parent != null ? new Parent() { Id = source.Parent.Id } : null,
            };

            if (source.Anchor != null)
            {
                comment.Inline = new Inline()
                {
                    Path = source.Anchor.Path,
                    From = source.Anchor.FileType == FileDiffType.From ? source.Anchor.Line : null,
                    To = source.Anchor.FileType == FileDiffType.To ? source.Anchor.Line : null,
                };
            }

            comment.User.Links.Avatar = new Link() { Href = comment.User.Links.Self.Href + "/avatar.png" };

            return comment;
        }
    }
}