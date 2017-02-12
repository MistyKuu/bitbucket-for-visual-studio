using System;
using System.Globalization;
using System.Linq;
using AutoMapper;
using BitBucket.REST.API.Helpers;
using BitBucket.REST.API.Models.Enterprise;
using BitBucket.REST.API.Models.Standard;

namespace BitBucket.REST.API.Mappings.Converters
{
    public class EnterprisePullRequestTypeConverter :
        ITypeConverter<EnterprisePullRequest, PullRequest>,
        ITypeConverter<PullRequest, EnterprisePullRequest>
    {
        public PullRequest Convert(EnterprisePullRequest source, PullRequest destination, ResolutionContext context)
        {
            var pullRequest= new PullRequest()
            {
                Description = source.Description,
                Links = source.Links.MapTo<Links>(),
                Author = source.Author.User.MapTo<User>(),
                Title = source.Title,
                Source = source.Source.MapTo<Source>(),
                Destination = source.Destination.MapTo<Source>(),
                State = source.State.MapTo<PullRequestOptions>(),
                CreatedOn = source.CreatedOn.FromUnixTimeStamp().ToString(CultureInfo.InvariantCulture),
                UpdatedOn = source.UpdatedOn.FromUnixTimeStamp().ToString(CultureInfo.InvariantCulture),
                Id = source.Id,
                CommentsCount = source.Properties.CommentsCount
            };
            pullRequest.Links.Html = pullRequest.Links.Html ?? new Link() {Href = source.Links.Self.First().Href};
            return pullRequest;
        }

        public EnterprisePullRequest Convert(PullRequest source, EnterprisePullRequest destination, ResolutionContext context)
        {//TODO THIS IS PROBABLY UNNECESSARY BECAUSE IT"S USED ONLY FOR CREATING SO DO IT MANUALLY
            return new EnterprisePullRequest()
            {
                Description = source.Description,
                Links = source.Links.MapTo<EnterpriseLinks>(),
                Author = new EnterpriseParticipant()
                {
                    User = source.Author.MapTo<EnterpriseUser>(),
                },
                Title = source.Title,
                Source = source.Source.MapTo<EnterpriseBranchSource>(),
                Destination = source.Destination.MapTo<EnterpriseBranchSource>(),
                State = source.State.MapTo<EnterprisePullRequestOptions>(),
                CreatedOn = DateTime.Parse(source.CreatedOn).ToUnixTimeStamp(),
                UpdatedOn = DateTime.Parse(source.UpdatedOn).ToUnixTimeStamp(),
                Id = source.Id
            };
        }
    }
}