using FitnessAppAPI.Data.Models;
using FitnessAppAPI.Data.Services.Exercises.Models;

namespace FitnessAppAPI.Data.Services.Exercises
{
    /// <summary>
    ///     Exercise service interface to define the logic for exercise CRUD operations.
    /// </summary>
    public interface IExerciseService
    {
        public Task<ServiceActionResult> AddExerciseToWorkout(ExerciseModel exerciseData, long workoutId);
        public Task<ServiceActionResult> UpdateExerciseFromWorkout(ExerciseModel exerciseData, long workoutId);
        public Task<ServiceActionResult> DeleteExerciseFromWorkout(long exerciseId);
        public Task<ServiceActionResult> AddExercise(MGExerciseModel MGExerciseData, string userId, string checkExistingEx);
        public Task<ServiceActionResult> UpdateExercise(MGExerciseModel exerciseData);
        public Task<ServiceActionResult> DeleteExercise(long MGExerciseId, string userId);
        public Task<ServiceActionResult> GetExercisesForMG(long muscleGroupId, string userId, string onlyForUser);
        public Task<ServiceActionResult> AddExerciseToWorkout(MGExerciseModel MGExerciseData, long workoutId);
        public Task<ServiceActionResult> GetMGExercise(long mGExerciseId);
    }
}
