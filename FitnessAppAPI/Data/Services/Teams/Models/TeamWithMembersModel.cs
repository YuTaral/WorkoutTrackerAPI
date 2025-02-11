namespace FitnessAppAPI.Data.Services.Teams.Models
{
    /// <summary>
    /// TeamWithMembersModel class representing a team and it's members.
    ///  Must correspond with client-side TeamWithMembersModel
    /// </summary>
    public class TeamWithMembersModel: TeamModel
    {
        public required List<TeamMemberModel> Members { get; set; }
    }
}
