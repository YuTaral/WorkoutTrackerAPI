using FitnessAppAPI.Data.Services.Exercises.Models;
using FitnessAppAPI.Data.Services.MuscleGroups.Models;
using System.ComponentModel.DataAnnotations;
using FitnessAppAPI.Common;

namespace FitnessAppAPI.Data.Services.Workouts.Models
{
    /// <summary>
    ///     WorkoutModel class representing a workout.
    ///     Must correspond with client-side WorkoutModel class
    /// </summary>
    public class WorkoutModel
    {
        public long Id { get; set; }

        [MinLength(Constants.DBConstants.MinLen1, ErrorMessage = Constants.ValidationErrors.NAME_REQUIRED)]
        [MaxLength(Constants.DBConstants.MaxLen50, ErrorMessage = Constants.ValidationErrors.NAME_MAX_LEN_50)]
        public required string Name { get; set; }
        public required DateTime Date { get; set; }
        public List<ExerciseModel>? Exercises { get; set; }
        public List<MuscleGroupModel>? MuscleGroups { get; set; }

    }
}
