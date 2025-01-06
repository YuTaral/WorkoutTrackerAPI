using System.ComponentModel.DataAnnotations;
using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Models;
using FitnessAppAPI.Data.Services.MuscleGroups.Models;

namespace FitnessAppAPI.Data.Services.Exercises.Models
{
    /// <summary>
    ///     ExerciseModel class representing an exercise, part of workout.
    ///     Must correspond with client-side ExerciseModel class
    /// </summary>
    public class ExerciseModel: BaseModel
    {
        [MinLength(Constants.DBConstants.Len1, ErrorMessage = Constants.ValidationErrors.NAME_REQUIRED)]
        [MaxLength(Constants.DBConstants.Len50, ErrorMessage = Constants.ValidationErrors.NAME_MAX_LEN_50)]
        public required string Name { get; set; }

        public required MuscleGroupModel MuscleGroup { get; set; }

        public List<SetModel>? Sets { get; set; }

        public required long? MGExerciseId { get; set; }

        [MaxLength(Constants.DBConstants.Len4000, ErrorMessage = Constants.ValidationErrors.DESCRIPTION_MAX_LEN_4000)]
        public string? Notes { get; set; }
    }
}
