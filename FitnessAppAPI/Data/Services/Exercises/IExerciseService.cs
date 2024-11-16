using FitnessAppAPI.Data.Services.Exercises.Models;

namespace FitnessAppAPI.Data.Services.Exercises
{
    /// <summary>
    ///     Exercise service interface to define the logic for exercise CRUD operations.
    /// </summary>
    public interface IExerciseService
    {
        public bool AddExerciseToWorkout(ExerciseModel exerciseData, long workoutId);
        public bool UpdateExerciseFromWorkout(ExerciseModel exerciseData, long workoutId);
        public long DeleteExerciseFromWorkout(long exerciseId);
        public ExerciseModel? AddExercise(MGExerciseModel exerciseData, string userId);
        public bool UpdateExercise(MGExerciseModel exerciseData);
        public long DeleteExercise(long MGExerciseId);
        public List<MGExerciseModel> GetExercisesForMG(long muscleGroupId, string userId, string onlyForUser);

    }
}
