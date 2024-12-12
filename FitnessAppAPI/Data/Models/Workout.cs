using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessAppAPI.Data.Models
{
    /// <summary>
    ///     Workout class to represent a row of database table workouts.
    /// </summary>
    public class Workout
    {
        [Key]
        public long Id { get; set; }

        public required string Name { get; set; }

        [ForeignKey("AspNetUser")]
        public required string UserId { get; set; }

        public DateTime? StartDateTime { get; set; }

        public DateTime? FinishDateTime { get; set; }

        public required string Template { get; set; }

        public int? DurationSeconds { get; set; }
    }
}
