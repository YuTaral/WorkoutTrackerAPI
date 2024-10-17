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
        [ForeignKey("Workouts")]
        public required long WorkoutId { get; set; }

        [ForeignKey("MuscleGroups")]
        public required long MuscleGroupId { get; set; }
    }
}
