using FitnessAppAPI.Data.Services.Workouts.Models;

namespace FitnessAppAPI.Data.Services.Workouts
{
    /// <summary>
    ///     Workout service interface to define the logic for workout CRUD operations.
    /// </summary>
    public interface IWorkoutService
    {
        public ServiceActionResult AddWorkout(WorkoutModel data, string userId);
        public ServiceActionResult EditWorkout(WorkoutModel data, string userId);
        public ServiceActionResult DeleteWorkout(long workoutId, string userId);
        public ServiceActionResult GetWorkout(long id, string userId);
        public ServiceActionResult GetLatestWorkouts(string userId);
        public ServiceActionResult GetLastWorkout(string userId);
    }
}
