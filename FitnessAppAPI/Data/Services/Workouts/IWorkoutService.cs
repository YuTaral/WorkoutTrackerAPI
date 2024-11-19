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
        public ServiceActionResult DeleteWorkout(long workoutId);
        public ServiceActionResult GetWorkout(long id);
        public ServiceActionResult GetLatestWorkouts(String userId);
        public ServiceActionResult GetLastWorkout(string userId);
    }
}
