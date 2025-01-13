using FitnessAppAPI.Data.Models;

namespace FitnessAppAPI.Data.Services.Teams.Models
{
    /// <summary>
    ///     TeamCoachModel class representing team coach.
    ///     Must correspond with client-side TeamCoachModel class
    /// </summary>
    public class TeamCoachModel: BaseModel
    {
        public required string FullName { get; set; }

        public required string Image { get; set; }
    }
}
