using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace FitnessAppAPI.Data.Models
{
    /// <summary>
    ///     WorkoutToTrainingDay class to represent a row of database table training day to workout.
    /// </summary>
    public class WorkoutToTrainingDay
    {
        [Key]
        public long Id { get; set; }

        [ForeignKey("TrainingDay")]
        public required long TrainingDayId { get; set; }

        [ForeignKey("Workout")]
        public required long WorkoutId { get; set; }
    }
}
