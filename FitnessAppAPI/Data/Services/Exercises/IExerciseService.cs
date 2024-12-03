using FitnessAppAPI.Data.Services.Exercises.Models;

namespace FitnessAppAPI.Data.Services.Exercises
{
    /// <summary>
    ///     Exercise service interface to define the logic for exercise CRUD operations.
    /// </summary>
    public interface IExerciseService
    {
        public ServiceActionResult AddExerciseToWorkout(ExerciseModel exerciseData, long workoutId, string userId);
        public ServiceActionResult UpdateExerciseFromWorkout(ExerciseModel exerciseData, long workoutId, string userId);
        public ServiceActionResult DeleteExerciseFromWorkout(long exerciseId, string userId);
        public ServiceActionResult AddExercise(MGExerciseModel MGExerciseData, string userId, string checkExistingEx);
        public ServiceActionResult UpdateExercise(MGExerciseModel exerciseData, string userId);
        public ServiceActionResult DeleteExercise(long MGExerciseId, string userId);
        public ServiceActionResult GetExercisesForMG(long muscleGroupId, string userId, string onlyForUser);
        public ServiceActionResult AddExerciseToWorkout(MGExerciseModel MGExerciseData, long workoutId, string userId);
    }
}
