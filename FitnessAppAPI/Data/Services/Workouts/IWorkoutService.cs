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
        public WorkoutModel? GetWorkoutFromExercise(long exerciseId);
        public WorkoutModel? GetTodayWorkout(string userId);
        public bool AddExercise(ExerciseModel set, long workoutId);
        public bool UpdateExercise(ExerciseModel exercise, long workoutId);
        public long DeleteExercise(long exerciseId);
    }
}
