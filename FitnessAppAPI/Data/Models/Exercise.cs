using FitnessAppAPI.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessAppAPI.Data.Models
{
    /// <summary>
    ///     Exercise class to represent a row of database table exercises.
    /// </summary>
    public class Exercise
    {
        [Key]
        public long Id { get; set; }

        [MinLength(Constants.DBConstants.Len1, ErrorMessage = Constants.ValidationErrors.NAME_REQUIRED)]
        [MaxLength(Constants.DBConstants.Len50, ErrorMessage = Constants.ValidationErrors.NAME_MAX_LEN_50)]
        public required string Name { get; set; }

        [ForeignKey("Workout")]
        public required long WorkoutId { get; set; }

        [ForeignKey("MuscleGroup")]
        public required long? MuscleGroupId { get; set; }

        [ForeignKey("MGExercise")]
        public required long? MGExerciseId { get; set; }
    }
}
