using FitnessAppAPI.Data.Services.Workouts.Models;

namespace FitnessAppAPI.Data.Services.Workouts
{
    /// <summary>
    ///     Workout service interface to define the logic for workout CRUD operations.
    /// </summary>
    public interface IWorkoutService
    {
        public Task<ServiceActionResult> AddWorkout(WorkoutModel data, string userId);
        public Task<ServiceActionResult> UpdateWorkout(WorkoutModel data, string userId);
        public Task<ServiceActionResult> DeleteWorkout(long workoutId, string userId);
        public Task<ServiceActionResult> GetWorkout(long id, string userId);
        public Task<ServiceActionResult> GetLatestWorkouts(string userId);
        public Task<ServiceActionResult> GetLastWorkout(string userId);
        public Task<ServiceActionResult> GetWeightUnits();
    }
}
