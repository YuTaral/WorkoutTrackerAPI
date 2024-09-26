using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessAppAPI.Data.Models
{
    /// <summary>
    ///     MuscleGroupToWorkout class to represent a row of database table MuscleGroupToWorkout - contains
    ///     the relation between muscle groups assigned to workout.
    /// </summary>
    public class MuscleGroupToWorkout
    {
        [Required]
        [ForeignKey("Workouts")]
        public long WorkoutId { get; set; }

        [Required]
        [ForeignKey("MuscleGroups")]
        public long MuscleGroupId { get; set; }
    }
}
