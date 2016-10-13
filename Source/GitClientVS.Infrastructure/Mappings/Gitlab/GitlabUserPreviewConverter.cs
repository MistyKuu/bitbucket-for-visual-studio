using AutoMapper;
using GitClientVS.Contracts.Models.GitClientModels;
using GitLab.NET.ResponseModels;

namespace GitClientVS.Infrastructure.Mappings.Gitlab
{
    public class GitlabUserPreviewConverter : ITypeConverter<UserPreview, GitUser>
    {
        public GitUser Convert(UserPreview source, GitUser destination, ResolutionContext context)
        {
            return new GitUser()
            {
                DisplayName = source.Name,
                Username = source.Username,
                Links = new GitLinks()
                {
                    Avatar = new GitLink() { Href = source.AvatarUrl },
                    Self = new GitLink() { Href = source.WebUrl }
                }
            };
        }

    }

    public class GitlabUserConverter : ITypeConverter<User, GitUser>
    {
        public GitUser Convert(User source, GitUser destination, ResolutionContext context)
        {
            return new GitUser()
            {
                DisplayName = source.Name,
                Username = source.Username,
                Links = new GitLinks()
                {
                    Avatar = new GitLink() { Href = source.AvatarUrl },
                    Self = new GitLink() { Href = source.WebUrl }
                }
            };
        }

    }
}