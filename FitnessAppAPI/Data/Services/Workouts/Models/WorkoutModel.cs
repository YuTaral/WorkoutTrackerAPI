using FitnessAppAPI.Data.Services.Exercises.Models;
using System.ComponentModel.DataAnnotations;
using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Models;

namespace FitnessAppAPI.Data.Services.Workouts.Models
{
    /// <summary>
    ///     WorkoutModel class representing a workout.
    ///     Must correspond with client-side WorkoutModel class
    /// </summary>
    public class WorkoutModel: BaseModel
    {
        [MinLength(Constants.DBConstants.MinLen1, ErrorMessage = Constants.ValidationErrors.NAME_REQUIRED)]
        [MaxLength(Constants.DBConstants.MaxLen50, ErrorMessage = Constants.ValidationErrors.NAME_MAX_LEN_50)]
        public required string Name { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? FinishDateTime { get; set; }
        public required bool Template { get; set; }
        public List<ExerciseModel>? Exercises { get; set; }
        public int? DurationSeconds { get; set; }
    }
}
