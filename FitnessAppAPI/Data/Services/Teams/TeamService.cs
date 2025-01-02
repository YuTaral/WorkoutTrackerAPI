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

        public async Task<ServiceActionResult> InviteMember(long teamId, string userId)
        {
            var teamMember = new TeamMember
            {
                State = Constants.MemberTeamState.INVITED.ToString(),
                UserId = userId,
                TeamId = teamId
            };

            DBAccess.TeamMembers.Add(teamMember);
            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_MEMBER_INVITE);
        }

        public async Task<ServiceActionResult> RemoveMember(long recordId)
        {
            var record = await DBAccess.TeamMembers.Where(tm => tm.Id == recordId).FirstOrDefaultAsync();

            if (record == null)
            {
                return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_MEMBER_IS_NOT_IN_TEAM);
            }

            DBAccess.TeamMembers.Remove(record);
            await DBAccess.SaveChangesAsync();

            // Return the team id on success
            return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_MEMBER_REMOVED, [new BaseModel { Id = record.TeamId }]);
        }

        public async Task<ServiceActionResult> AcceptInvite(string userId, long teamId)
        {
            var model = new BaseModel
            {
                Id = 0
            };

            var record = await DBAccess.TeamMembers.Where(tm => tm.UserId == userId && tm.TeamId == teamId).FirstOrDefaultAsync();

            if (record == null) {
                return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_FAILED_TO_JOIN_TEAM);
            }

            // Update the team members record
            record.State = Constants.MemberTeamState.ACCEPTED.ToString();

            DBAccess.Entry(record).State = EntityState.Modified;
            await DBAccess.SaveChangesAsync();

            // Return the notification id
            var notification = await DBAccess.Notifications.Where(n => n.ReceiverUserId == userId &&
                                                                 n.TeamId == teamId &&
                                                                 n.NotificationType == Constants.NotificationType.INVITED_TO_TEAM.ToString())
                                                            .FirstOrDefaultAsync();
            if (notification != null)
            {
                model.Id = notification.Id;
            }

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_JOINED_TEAM, [model]);
        }

        public async Task<ServiceActionResult> GetMyTeams(string userId)
        {
            var teams = await DBAccess.Teams.Where(t => t.UserId == userId)
                                .Select(t => (BaseModel) ModelMapper.MapToTeamModel(t)).ToListAsync();

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS, teams);
        }

        public async Task<ServiceActionResult> GetUsersToInvite(string name, long teamId, string userId)
        {
            // Trim and convert to lower
            name = name.Trim().ToLower();

            var members = new List<BaseModel>();

            // Fetch users with this name
            var users = await DBAccess.UserProfiles.Where(u => u.UserId != userId && u.FullName.ToLower().Contains(name) &&
                                                        !DBAccess.TeamMembers.Any(tm => tm.TeamId == teamId && tm.UserId == u.UserId))
                                                    .ToListAsync();

            if (users.Count > 0) {

                foreach (var user in users) {

                    // Create member record and map it to model
                    var member = new TeamMember
                    {
                        Id = 0,
                        UserId = user.UserId,
                        TeamId = teamId,
                        State = Constants.MemberTeamState.NOT_INVITED.ToString()
                    };

                    members.Add(await ModelMapper.MapToTeamMemberModel(member, DBAccess));
                }
            }

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS, members);
        }

        public async Task<ServiceActionResult> GetTeamMembers(long teamId)
        {
            var memberModels = new List<BaseModel>();
            var members = await DBAccess.TeamMembers.Where(tm => tm.TeamId == teamId).OrderBy(tm => tm.State).ToListAsync();

            foreach (var member in members) 
            {
                memberModels.Add(await ModelMapper.MapToTeamMemberModel(member, DBAccess));
            }

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS, memberModels);
        }
    }
}
