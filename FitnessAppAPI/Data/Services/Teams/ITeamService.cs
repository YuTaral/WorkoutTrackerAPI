using FitnessAppAPI.Data.Models;
using FitnessAppAPI.Data.Services.Notifications.Models;
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
        /// <param name="requestData">
        ///     The request data (team)
        /// </param>
        /// <param name="userId">
        ///     The user owner of the team
        /// </param>
        public Task<ServiceActionResult<TeamModel>> AddTeam(Dictionary<string, string> requestData, string userId);

        /// <summary>
        ///     Update the team 
        /// </summary>
        /// <param name="requestData">
        ///     The request data (team)
        /// </param>
        /// <param name="userId">
        ///     The logged in user id
        /// </param>
        public Task<ServiceActionResult<TeamModel>> UpdateTeam(Dictionary<string, string> requestData, string userId);

        /// <summary>
        ///     Delete the team with the provided id
        /// </summary>
        /// <param name="teamId">
        ///     The team id
        /// </param>
        /// <param name="userId">
        ///     The user who is deleting the team
        /// </param>
        public Task<ServiceActionResult<BaseModel>> DeleteTeam(long teamId, string userId);

        /// <summary>
        ///     Leave the team with the provided id
        /// </summary>
        /// <param name="requestData">
        ///     The request data (team id)
        /// </param>
        /// <param name="userId">s
        ///     The user who is leaving the team
        /// </param>
        public Task<ServiceActionResult<BaseModel>> LeaveTeam(Dictionary<string, string> requestData, string userId);

        /// <summary>
        ///     Invite member to the team
        /// </summary>
        /// <param name="requestData">
        ///     The request data (team and user id)
        /// </param>
        public Task<ServiceActionResult<long>> InviteMember(Dictionary<string, string> requestData, string senderUserId);

        /// <summary>
        ///     Remove member from the team
        /// </summary>
        ///  /// <param name="requestData">
        ///     The request data (team member model to remove (contains the record id))
        /// </param>
        public Task<ServiceActionResult<TeamMemberModel>> RemoveMember(Dictionary<string, string> requestData);

        /// <summary>
        ///     Change team member record state to accepted or declined.
        ///     Mark the invite notification as inactive and return
        ///     updated notifications
        /// </summary>
        /// <param name="requestData">
        ///     The request data (user and team id)
        /// </param>
        /// <param name="newState">
        ///     The new state
        /// </param>
        public Task<ServiceActionResult<NotificationModel>> AcceptDeclineInvite(Dictionary<string, string> requestData, string newState);

        /// <summary>
        ///     Return all teams created by the user
        /// </summary>
        /// <param name="teamType">
        ///     The team type
        /// </param>
        /// <param name="userId">
        ///     The user owner of the team
        /// </param>
        public Task<ServiceActionResult<TeamModel>> GetMyTeams(string teamType, string userId);

        /// <summary>
        ///     Return all teams created by the user, in which there are at least 1 member
        /// </summary>
        /// <param name="userId">
        ///     The user owner of the team
        /// </param>
        public Task<ServiceActionResult<TeamWithMembersModel>> GetMyTeamsWithMembers(string userId);

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
        public Task<ServiceActionResult<TeamMemberModel>> GetUsersToInvite(string name, long teamId, string userId);

        /// <summary>
        ///     Get team members when logged in user is coach
        /// </summary>
        /// <param name="teamId">
        ///     The team id
        /// </param>
        public Task<ServiceActionResult<TeamMemberModel>> GetMyTeamMembers(long teamId);

        /// <summary>
        ///     Get team members when logged in user is member
        /// </summary>
        /// <param name="teamId">
        ///     The team id
        /// </param>
        /// <param name="userId">
        ///     Logged in user id
        /// </param>

        public Task<ServiceActionResult<BaseModel>> GetJoinedTeamMembers(long teamId, string userId);

        /// <summary>
        ///     Assign the workout to the members
        /// </summary>
        /// <param name="requestData">
        ///     The request data (workout id, member ids and workout start date)
        /// </param>
        /// <param name="coachId">
        ///     The coach id
        /// </param>
        public Task<ServiceActionResult<long>> AssignWorkout(Dictionary<string, string> requestData, string coachId);

        /// <summary>
        ///    Delete the assigned workout records
        /// </summary>
        /// <param name="templateId">
        ///     The the template id which was deleted
        /// </param>
        public Task<ServiceActionResult<long>> DeleteAssignedWorkouts(long templateId);

        /// <summary>
        ///    Delete the assigned workout records
        /// </summary>
        /// <param name="startedWorkoutId">
        ///     The stared workout id value of the records to delete
        /// </param>
        public Task<ServiceActionResult<long>> DeleteAssignedWorkoutsByWorkoutId(long startedWorkoutId);

        /// <summary>
        ///     Mark the state of assigned workout as finished
        /// </summary>
        /// <param name="workoutId">
        ///     The workout id
        /// </param>
        public Task<ServiceActionResult<long>> FinishAssignedWorkout(long workoutId);

        /// <summary>
        ///     Mark the state of assigned workout as in progress and update started workout id
        /// </summary>
        /// <param name="assignedWorkoutId">
        ///     The assigned workout id record
        /// </param>
        public Task<ServiceActionResult<long>> UpdateAssignedWorkoutStarted(long assignedWorkoutId, long startedWorkoutId);

        /// <summary>
        ///     Get the assigned workouts
        /// </summary>
        /// <param name="coachId">
        ///     The coach id who assigned the workouts
        /// </param>
        /// <param name="startDate">
        ///     The start date filter
        /// </param>
        /// <param name="teamId">
        ///     The team id filter (0 if not applied)
        /// </param>
        public Task<ServiceActionResult<AssignedWorkoutModel>> GetAssignedWorkouts(string startDate, long teamId, string coachId);

        /// <summary>
        ///     Fetch the assigned workout with the specified id
        /// </summary>
        /// <param name="assignedWorkoutId">
        ///     The assigned workout id
        /// </param>
        public Task<ServiceActionResult<AssignedWorkoutModel>> GetAssignedWorkout(long assignedWorkoutId);
    }
}
