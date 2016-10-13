using AutoMapper;
using BitBucket.REST.API.Models;
using GitClientVS.Contracts.Models.GitClientModels;

namespace GitClientVS.Infrastructure.Mappings.Bitbucket
{
    public class BitbucketTeamTypeConverter : ITypeConverter<Team, GitTeam>
    {
        public GitTeam Convert(Team source, GitTeam destination, ResolutionContext context)
        {
            return new GitTeam()
            {
                Name = source.Username
            };
        }
    }

    public class BitbucketReverseTeamTypeConverter : ITypeConverter<GitTeam, Team>
    {
        public Team Convert(GitTeam source, Team destination, ResolutionContext context)
        {
            return new Team()
            {
                Username = source.Name
            };
        }
    }
}