using System.ComponentModel.DataAnnotations;
using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Models;

namespace FitnessAppAPI.Data.Services.Exercises.Models
{
    /// <summary>
    ///     MGExerciseModel class representing a exercise of specific muscle group.
    ///     Must correspond with client-side MGExerciseModel class
    /// </summary>
    public class MGExerciseModel: BaseModel
    {
        [MinLength(Constants.DBConstants.Len1, ErrorMessage = Constants.ValidationErrors.NAME_REQUIRED)]
        [MaxLength(Constants.DBConstants.Len50, ErrorMessage = Constants.ValidationErrors.NAME_MAX_LEN_50)]
        public required string Name { get; set; }

        [MaxLength(Constants.DBConstants.Len4000, ErrorMessage = Constants.ValidationErrors.DESCRIPTION_MAX_LEN_4000)]
        public string? Description { get; set; }

        public required long MuscleGroupId { get; set; }
    }
}
