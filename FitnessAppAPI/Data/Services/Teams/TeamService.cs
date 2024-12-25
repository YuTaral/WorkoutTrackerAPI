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

        public async Task<ServiceActionResult> UpdateTeam(TeamModel data)
        {
            var team = await DBAccess.Teams.Where(t => t.Id == data.Id).FirstOrDefaultAsync();
            if (team == null)
            {
                return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_TEAM_DOES_NOT_EXIST);
            }

            team.Image = Utils.DecodeBase64ToByteArray(data.Image);
            team.Name = data.Name;
            team.Description = data.Description;
            team.PrivateNote = data.PrivateNote;

            DBAccess.Entry(team).State = EntityState.Modified;
            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_TEAM_UPDATED);
        }

        public async Task<ServiceActionResult> DeleteTeam(long teamId, string userId)
        {
            var team = await DBAccess.Teams.Where(t => t.Id == teamId).FirstOrDefaultAsync();
            if (team == null)
            {
                return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_TEAM_DOES_NOT_EXIST);
            }

            // Delete the team
            DBAccess.Teams.Remove(team);
            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_TEAM_DELETED);
        }

        public async Task<ServiceActionResult> GetMyTeams(string userId)
        {
            var teams = await DBAccess.Teams.Where(t => t.UserId == userId)
                                .Select(t => (BaseModel) ModelMapper.MapToTeamModel(t)).ToListAsync();

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS, teams);
        }
    }
}
