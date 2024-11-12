using System.ComponentModel.DataAnnotations;
using FitnessAppAPI.Common;

namespace FitnessAppAPI.Data.Services.MuscleGroups.Models
{
    /// <summary>
    ///     MGExerciseModel class representing a exercise of specific muscle group.
    ///     Must correspond with client-side MGExerciseModel class
    /// </summary>
    public class MGExerciseModel
    {
        public long Id { get; set; }

        [MinLength(Constants.DBConstants.MinLen1, ErrorMessage = Constants.ValidationErrors.NAME_REQUIRED)]
        [MaxLength(Constants.DBConstants.MaxLen50, ErrorMessage = Constants.ValidationErrors.NAME_MAX_LEN_50)]
        public required string Name { get; set; }

        [MinLength(Constants.DBConstants.MinLen1, ErrorMessage = Constants.ValidationErrors.DESCRIPTION_REQUIRED)]
        [MaxLength(Constants.DBConstants.MaxLen4000, ErrorMessage = Constants.ValidationErrors.DESCRIPTION_MAX_LEN_4000)]
        public string? Description { get; set; }
    }
}
