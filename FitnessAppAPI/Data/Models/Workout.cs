using FitnessAppAPI.Common;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FitnessAppAPI.Data.Models
{
    /// <summary>
    ///     Workout class to represent a row of database table workouts.
    /// </summary>
    public class Workout
    {
        [Required]
        [Key]
        public long Id { get; set; }

        [Required]
        [MinLength(Constants.DBConstants.WorkoutNameMinLen)]
        [MaxLength(Constants.DBConstants.WorkoutNameMaxLen)]
        public required string Name { get; set; }

        [Required]
        public required string UserId { get; set; }

        [Required]
        public DateTime Date { get; set; }
    }
}
