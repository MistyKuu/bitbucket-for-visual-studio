using AutoMapper;
using AutoMapper.Execution;
using BitBucket.REST.API.Mappings.Converters;
using BitBucket.REST.API.Models.Enterprise;
using BitBucket.REST.API.Models.Standard;

namespace BitBucket.REST.API.Mappings
{
    public class EnterpriseToStandardMappingsProfile : Profile
    {
        public EnterpriseToStandardMappingsProfile()
        {
            CreateMap<RepositoryV1, Repository>().ConvertUsing<RepositoryV1TypeConverter>();
            CreateMap<CommentV1, Comment>().ConvertUsing<CommentV1TypeConverter>();
            CreateMap<EnterpriseLink, Link>();
            CreateMap<Link, EnterpriseLink>();

            CreateMap<EnterpriseUser, User>();
            CreateMap<User, EnterpriseUser>();

            CreateMap<EnterpriseLinks, Links>().ConvertUsing<EnterpriseLinksTypeConverter>();
            CreateMap<Links, EnterpriseLinks>().ConvertUsing<EnterpriseLinksTypeConverter>();
            CreateMap<EnterpriseBranch, Branch>().ConvertUsing<EnterpriseBranchTypeConverter>();
            CreateMap<EnterpriseParticipant, Participant>().ConvertUsing<EnterpriseParticipantTypeConverter>();

            CreateMap<EnterprisePullRequest, PullRequest>().ConvertUsing<EnterprisePullRequestTypeConverter>();
            CreateMap<PullRequest, EnterprisePullRequest>().ConvertUsing<EnterprisePullRequestTypeConverter>();
            CreateMap<EnterpriseBranchSource, Source>().ConvertUsing<EnterpriseBranchSourceTypeConverter>();
            CreateMap<Source, EnterpriseBranchSource>().ConvertUsing<EnterpriseBranchSourceTypeConverter>();

            CreateMap<EnterprisePullRequestOptions, PullRequestOptions>();
            CreateMap<PullRequestOptions, EnterprisePullRequestOptions>();
            CreateMap<EnterpriseUser, UserShort>();
            CreateMap<EnterpriseCommit, Commit>().ConvertUsing<EnterpriseCommitTypeConverter>();
            CreateMap<EnterpriseComment, Comment>().ConvertUsing<EnterpriseCommentTypeConverter>();

            CreateMap<EnterpriseRepository, Repository>().ConvertUsing<EnterpriseRepositoryTypeConverter>();
            CreateMap(typeof(IteratorBasedPage<>), typeof(IteratorBasedPage<>));
        }
    }
}