using FitnessAppAPI.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessAppAPI.Data.Models
{
    /// <summary>
    ///     AssignedWorkoutcs class to represent a row of database table assigned workout.
    /// </summary>
    public class AssignedWorkout
    {
        [Key]
        public long Id { get; set; }

        [ForeignKey("Workout")]
        public required long WorkoutId { get; set; }

        [ForeignKey("TeamMember")]
        public required long TeamMemberId { get; set; }

        [MaxLength(Constants.DBConstants.Len50)]
        [EnumDataType(typeof(Constants.AssignedWorkoutState))]
        public required string State { get; set; }
    }
}
