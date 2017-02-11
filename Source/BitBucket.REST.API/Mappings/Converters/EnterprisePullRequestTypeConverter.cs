using System;
using System.Globalization;
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
            return new PullRequest()
            {
                Description = source.Description,
                Links = source.Links.MapTo<Links>(),
                Author = source.Author.User.MapTo<User>(),
                Title = source.Title,
                Source = source.Source.MapTo<Source>(),
                State = source.State.MapTo<PullRequestOptions>(),
                CreatedOn = source.CreatedOn.FromUnixTimeStamp().ToString(CultureInfo.InvariantCulture),
                UpdatedOn = source.UpdatedOn.FromUnixTimeStamp().ToString(CultureInfo.InvariantCulture),
                Id = source.Id
            };
        }

        public EnterprisePullRequest Convert(PullRequest source, EnterprisePullRequest destination, ResolutionContext context)
        {
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
                State = source.State.MapTo<EnterprisePullRequestOptions>(),
                CreatedOn = DateTime.Parse(source.CreatedOn).ToUnixTimeStamp(),
                UpdatedOn = DateTime.Parse(source.UpdatedOn).ToUnixTimeStamp(),
                Id = source.Id
            };
        }
    }
}