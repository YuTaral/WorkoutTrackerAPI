using FitnessAppAPI.Data.Services.Teams.Models;

namespace FitnessAppAPI.Data.Services.Teams
{
    /// <summary>
    ///     Team service service interface to define the logic for teams CRUD operations.
    /// </summary>
    public interface ITeamService
    {
        public Task<ServiceActionResult> AddTeam(TeamModel data, string userId);
        public Task<ServiceActionResult> GetMyTeams(string userId);
    }
}
