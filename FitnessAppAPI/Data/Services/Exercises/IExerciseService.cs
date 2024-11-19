using FitnessAppAPI.Data.Services.Exercises.Models;

namespace FitnessAppAPI.Data.Services.Exercises
{
    /// <summary>
    ///     Exercise service interface to define the logic for exercise CRUD operations.
    /// </summary>
    public interface IExerciseService
    {
        public ServiceActionResult AddExerciseToWorkout(ExerciseModel exerciseData, long workoutId);
        public ServiceActionResult UpdateExerciseFromWorkout(ExerciseModel exerciseData, long workoutId);
        public ServiceActionResult DeleteExerciseFromWorkout(long exerciseId);
        public ServiceActionResult AddExercise(MGExerciseModel exerciseData, string userId);
        public ServiceActionResult UpdateExercise(MGExerciseModel exerciseData);
        public ServiceActionResult DeleteExercise(long MGExerciseId);
        public ServiceActionResult GetExercisesForMG(long muscleGroupId, string userId, string onlyForUser);

    }
}
