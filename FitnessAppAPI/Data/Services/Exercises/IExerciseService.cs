using FitnessAppAPI.Data.Services.Exercises.Models;
using FitnessAppAPI.Data.Services.MuscleGroups.Models;

namespace FitnessAppAPI.Data.Services.Exercises
{
    /// <summary>
    ///     Exercise service interface to define the logic for exercise CRUD operations.
    /// </summary>
    public interface IExerciseService
    {
        // Exercise
        public bool AddExerciseToWorkout(ExerciseModel set, long workoutId);
        public bool UpdateExercise(ExerciseModel exercise, long workoutId);
        public long DeleteExercise(long exerciseId);
        public List<MGExerciseModel> GetExercisesForMG(long muscleGroupId);
    }
}
