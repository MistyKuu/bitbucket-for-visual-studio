using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BitBucket.REST.API.Models;
using BitBucket.REST.API.Models.Standard;
using GitClientVS.Contracts.Models.GitClientModels;

namespace GitClientVS.Infrastructure.Mappings
{
    public class TeamTypeConverter : ITypeConverter<Team, GitTeam>
    {
        public GitTeam Convert(Team source, GitTeam destination, ResolutionContext context)
        {
            return new GitTeam()
            {
                Name = source.Username
            };
        }
    }

    public class ReverseTeamTypeConverter : ITypeConverter<GitTeam, Team>
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