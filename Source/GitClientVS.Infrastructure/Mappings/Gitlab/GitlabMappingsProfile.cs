using AutoMapper;
using GitClientVS.Contracts.Models.GitClientModels;
using GitLab.NET;
using GitLab.NET.ResponseModels;

namespace GitClientVS.Infrastructure.Mappings.Gitlab
{
    public class GitlabMappingsProfile : Profile
    {
        public GitlabMappingsProfile()
        {
            CreateMap<Project, GitRepository>().ConvertUsing<GitlabRepositoryConverter>();

            CreateMap(typeof(PaginatedResult<>), typeof(PageIterator<>)).ConvertUsing(typeof(GitlabPaginatedConverter<,>));

            CreateMap<MergeRequest, GitPullRequest>().ConvertUsing<GitlabMergeRequestTypeConverter>();
            CreateMap<MergeRequestState, GitPullRequestStatus>().ConvertUsing<GitlabMergeRequestStateConverter>();
            CreateMap<UserPreview, GitUser>().ConvertUsing<GitlabUserPreviewConverter>();
            CreateMap<User, GitUser>().ConvertUsing<GitlabUserConverter>();
            CreateMap<Commit, GitCommit>().ConvertUsing<GitlabCommitConverter>();
            CreateMap<Comment, GitComment>().ConvertUsing<GitlabCommentConverter>();
            CreateMap<Branch, GitBranch>().ConvertUsing<GitlabBranchConverter>();
            CreateMap<Note, GitComment>().ConvertUsing<GitlabNoteToCommentConverter>();
            CreateMap<Namespace, GitTeam>().ConvertUsing<GitlabNamespaceConverter>();
        }
    }
}