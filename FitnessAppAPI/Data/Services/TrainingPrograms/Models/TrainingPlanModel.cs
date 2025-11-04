using System.ComponentModel.DataAnnotations;
using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Models;

namespace FitnessAppAPI.Data.Services.TrainingPrograms.Models
{
    /// <summary>
    ///     TrainingPlanModel class representing a training program.
    ///     Must correspond with client-side TrainingPlanModel class
    /// </summary>
    public class TrainingPlanModel : BaseModel
    {
        [MinLength(Constants.DBConstants.Len1, ErrorMessage = Constants.ValidationErrors.NAME_REQUIRED)]
        [MaxLength(Constants.DBConstants.Len50, ErrorMessage = Constants.ValidationErrors.NAME_MAX_LEN_50)]
        public required string Name { get; set; }

        [MaxLength(Constants.DBConstants.Len4000, ErrorMessage = Constants.ValidationErrors.DESCRIPTION_MAX_LEN_4000)]
        public required string Description { get; set; }

        public required List<TrainingDayModel> TrainingDays { get; set; }
    }
}
