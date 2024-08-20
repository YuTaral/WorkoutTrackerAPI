using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FitnessAppAPI.Common;

namespace FitnessAppAPI.Data.Models
{
    /// <summary>
    ///     Exercise class to represent a row of database table exercises.
    /// </summary>
    public class Exercise
    {
        [Required]
        [Key]
        public long Id { get; set; }

        [Required]
        [MinLength(Constants.DBConstants.ExercisetNameMinLen)]
        [MaxLength(Constants.DBConstants.ExerciseNameMaxLen)]
        public required string Name { get; set; }

        [Required]
        [ForeignKey("Workout")]
        public long WorkoutId { get; set; }
    }
}
