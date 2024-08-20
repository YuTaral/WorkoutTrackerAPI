using FitnessAppAPI.Data.Services.Workouts.Models;

namespace FitnessAppAPI.Data.Services.Workouts
{
    /// <summary>
    ///     Workout service interface define the logic for workout CRUD operations.
    /// </summary>
    public interface IWorkoutService
    {
        public WorkoutModel? AddWorkout(WorkoutModel data, string userId);
        public WorkoutModel? GetWorkout(long id);
        public WorkoutModel? GetTodayWorkout(string userId);
        public void AddExercise(ExerciseModel set, long workoutId);
        public bool UpdateExercise(ExerciseModel exercise, long workoutId);

    }
}
