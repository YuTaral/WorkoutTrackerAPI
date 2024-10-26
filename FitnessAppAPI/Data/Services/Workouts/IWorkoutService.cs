using FitnessAppAPI.Data.Models;
using FitnessAppAPI.Data.Services.Workouts.Models;

namespace FitnessAppAPI.Data.Services.Workouts
{
    /// <summary>
    ///     Workout service interface to define the logic for workout CRUD operations.
    /// </summary>
    public interface IWorkoutService
    {
        public WorkoutModel? AddWorkout(WorkoutModel data, string userId);
        public WorkoutModel? EditWorkout(WorkoutModel data, string userId);
        public bool DeleteWorkout(long workoutId);
        public WorkoutModel? GetWorkout(long id);
        public List<WorkoutModel>? GetLatestWorkouts(String userId);
        public WorkoutModel? GetLastWorkout(string userId);
        public WorkoutModel GetWorkoutModelFromWorkout(Workout workout);
    }
}
