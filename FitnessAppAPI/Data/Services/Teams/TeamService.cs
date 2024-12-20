using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Models;
using FitnessAppAPI.Data.Services.Teams.Models;
using Microsoft.EntityFrameworkCore;

namespace FitnessAppAPI.Data.Services.Teams
{
    /// <summary>
    ///     Team service class to implement ITeamService interface.
    /// </summary>
    public class TeamService(FitnessAppAPIContext DB) : BaseService(DB), ITeamService
    {
        public async Task<ServiceActionResult> AddTeam(TeamModel data, string userId)
        {
            var team = new Team
            {
                Image = Utils.DecodeBase64ToByteArray(data.Image),
                Name = data.Name,
                Description = data.Description,
                PrivateNote = data.PrivateNote,
                UserId = userId
            };

            await DBAccess.Teams.AddAsync(team);
            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_TEAM_ADDED);
        }

        public async Task<ServiceActionResult> GetMyTeams(string userId)
        {
            var teams = await DBAccess.Teams.Where(t => t.UserId == userId)
                                .Select(t => (BaseModel) ModelMapper.MapToTeamModel(t)).ToListAsync();

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS, teams);
        }
    }
}
