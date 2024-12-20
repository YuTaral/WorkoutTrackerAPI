using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace FitnessAppAPI.Data.Services.Teams.Models
{
    /// <summary>
    ///     TeamModel class representing a team.
    ///     Must correspond with client-side TeamModel class
    /// </summary>
    public class TeamModel: BaseModel
    {
        [MinLength(Constants.DBConstants.MinLen1, ErrorMessage = Constants.ValidationErrors.NAME_REQUIRED)]
        [MaxLength(Constants.DBConstants.MaxLen50, ErrorMessage = Constants.ValidationErrors.NAME_MAX_LEN_50)]
        public required string Name { get; set; }
        public required string Image { get; set; }
        public required string Description { get; set; }
        public required string PrivateNote { get; set; }
    }
}
