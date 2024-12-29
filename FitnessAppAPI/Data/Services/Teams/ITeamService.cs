using FitnessAppAPI.Data.Services.Teams.Models;

namespace FitnessAppAPI.Data.Services.Teams
{
    /// <summary>
    ///     Team service service interface to define the logic for teams CRUD operations.
    /// </summary>
    public interface ITeamService
    {
        /// <summary>
        ///     Add the team with the provided data. The user is owner of the team
        /// </summary>
        /// <param name="data">
        ///     The team data
        /// </param>
        /// <param name="userId">
        ///     The user owner of the team
        /// </param>
        public Task<ServiceActionResult> AddTeam(TeamModel data, string userId);

        /// <summary>
        ///     Update the team 
        /// </summary>
        /// <param name="data">
        ///     The team data
        /// </param>
        public Task<ServiceActionResult> UpdateTeam(TeamModel data);

        /// <summary>
        ///     Delete the team with the provided id
        /// </summary>
        ///  /// <param name="teamId">
        ///     The team id
        /// </param>
        /// <param name="userId">
        ///     The user who is deleting the team
        /// </param>
        public Task<ServiceActionResult> DeleteTeam(long teamId, string userId);

        /// <summary>
        ///     Invite member to the team
        /// </summary>
        ///  /// <param name="teamId">
        ///     The team id
        /// </param>
        /// <param name="userId">
        ///     The user (member) id
        /// </param>
        public Task<ServiceActionResult> InviteMember(long teamId, string userId);

        /// <summary>
        ///     Remove member from the team
        /// </summary>
        ///  /// <param name="recordId">
        ///     The record id to remove
        /// </param>
        public Task<ServiceActionResult> RemoveMember(long recordId);

        /// <summary>
        ///     Return all teams created by the user
        /// </summary>
        /// <param name="userId">
        ///     The user owner of the team
        /// </param>
        public Task<ServiceActionResult> GetMyTeams(string userId);

        /// <summary>
        ///     Return filtered users by the specified name which are valid for team invitation
        /// </summary>
        /// <param name="name">
        ///     The name to search for
        /// </param>
        /// <param name="teamId">
        ///     The team id
        /// </param>
        /// <param name="userId">
        ///     The logged in user
        /// </param>
        public Task<ServiceActionResult> GetUsersToInvite(string name, long teamId, string userId);

        /// <summary>
        ///     Get team members
        /// </summary>
        /// <param name="teamId">
        ///     The team id
        /// </param>
        public Task<ServiceActionResult> GetTeamMembers(long teamId);
    }
}
