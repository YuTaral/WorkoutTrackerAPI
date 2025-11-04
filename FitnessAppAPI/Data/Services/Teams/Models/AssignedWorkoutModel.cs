using FitnessAppAPI.Data.Models;
using FitnessAppAPI.Data.Services.Workouts.Models;

namespace FitnessAppAPI.Data.Services.Teams.Models
{
    /// <summary>
    ///     TeamModel class representing an assigned workout.
    ///     Must correspond with client-side AssignedWorkoutModel class
    /// </summary>
    public class AssignedWorkoutModel: BaseModel
    {
        public required WorkoutModel WorkoutModel { get; set; }
        public required string TeamName { get; set; }
        public required string TeamImage { get; set; }
        public required long TeamId { get; set; }
        public required string UserFullName { get; set; }
        public required DateTime ScheduledForDate { get; set; }
        public DateTime? DateTimeCompleted { get; set; }
    }
}
