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
        [MinLength(Constants.DBConstants.Len1, ErrorMessage = Constants.ValidationErrors.NAME_REQUIRED)]
        [MaxLength(Constants.DBConstants.Len50, ErrorMessage = Constants.ValidationErrors.NAME_MAX_LEN_50)]
        public required string Name { get; set; }

        public required string Image { get; set; }

        [MaxLength(Constants.DBConstants.Len4000, ErrorMessage = Constants.ValidationErrors.DESCRIPTION_MAX_LEN_4000)]
        public required string Description { get; set; }

        public required string ViewTeamAs { get; set; }
    }
}
