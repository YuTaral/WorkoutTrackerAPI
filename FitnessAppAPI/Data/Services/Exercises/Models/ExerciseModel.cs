using System.ComponentModel.DataAnnotations;
using FitnessAppAPI.Common;


namespace FitnessAppAPI.Data.Services.Exercises.Models
{
    /// <summary>
    ///     ExerciseModel class representing an exercise, part of workout.
    ///     Must correspond with client-side ExerciseModel class
    /// </summary>
    public class ExerciseModel
    {
        public long Id { get; set; }

        [MinLength(Constants.DBConstants.MinLen1, ErrorMessage = Constants.ValidationErrors.NAME_REQUIRED)]
        [MaxLength(Constants.DBConstants.MaxLen50, ErrorMessage = Constants.ValidationErrors.NAME_MAX_LEN_50)]
        public required string Name { get; set; }
        public List<SetModel>? Sets { get; set; }

    }
}
