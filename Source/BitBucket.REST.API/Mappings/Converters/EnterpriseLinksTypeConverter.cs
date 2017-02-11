using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BitBucket.REST.API.Models.Enterprise;
using BitBucket.REST.API.Models.Standard;

namespace BitBucket.REST.API.Mappings.Converters
{
    public class EnterpriseLinksTypeConverter : ITypeConverter<EnterpriseLinks, Links>, ITypeConverter<Links, EnterpriseLinks>
    {
        public Links Convert(EnterpriseLinks source, Links destination, ResolutionContext context)
        {
            return new Links()
            {
                Self = source.Self?.FirstOrDefault()?.MapTo<Link>(),
                Repositories = source.Repositories?.FirstOrDefault()?.MapTo<Link>(),
                Link = source.Link?.FirstOrDefault()?.MapTo<Link>(),
                Followers = source.Followers?.FirstOrDefault()?.MapTo<Link>(),
                Avatar = source.Avatar?.FirstOrDefault()?.MapTo<Link>(),
                Html = source.Html?.FirstOrDefault()?.MapTo<Link>(),
                Following = source.Following?.FirstOrDefault()?.MapTo<Link>(),
                Clone = source.Clone?.MapTo<List<Link>>(),
            };
        }

        public EnterpriseLinks Convert(Links source, EnterpriseLinks destination, ResolutionContext context)
        {
            return new EnterpriseLinks()
            {
                Self = new List<EnterpriseLink>() { source.Self.MapTo<EnterpriseLink>() },
                Repositories = new List<EnterpriseLink>() { source.Repositories.MapTo<EnterpriseLink>() },
                Link = new List<EnterpriseLink>() { source.Link.MapTo<EnterpriseLink>() },
                Followers = new List<EnterpriseLink>() { source.Followers.MapTo<EnterpriseLink>() },
                Avatar = new List<EnterpriseLink>() { source.Avatar.MapTo<EnterpriseLink>() },
                Html = new List<EnterpriseLink>() { source.Html.MapTo<EnterpriseLink>() },
                Following = new List<EnterpriseLink>() { source.Following.MapTo<EnterpriseLink>() },
                Clone = source.Clone.MapTo<List<EnterpriseLink>>(),
            };
        }
    }
}