using FitnessAppAPI.Data.Models;

namespace FitnessAppAPI.Data.Services.Teams.Models
{

    /// <summary>
    ///     TeamMemberModel class representing team member.
    ///     Must correspond with client-side TeamMemberModel class
    /// </summary>
    public class TeamMemberModel: BaseModel
    {
        public required string UserId { get; set; }

        public required long TeamId { get; set; }

        public required string FullName { get; set; }

        public required string Image { get; set; }

        public required string TeamState { get; set; }
    }
}
