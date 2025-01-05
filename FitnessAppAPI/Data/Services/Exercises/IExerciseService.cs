using FitnessAppAPI.Data.Models;
using FitnessAppAPI.Data.Services.Exercises.Models;

namespace FitnessAppAPI.Data.Services.Exercises
{
    /// <summary>
    ///     Exercise service interface to define the logic for exercise CRUD operations.
    /// </summary>
    public interface IExerciseService
    {
        /// <summary>
        ///     Add the exercise to the workout with the provided id
        /// </summary>
        /// <param name="exerciseData">
        ///     The exercise
        /// </param>
        /// <param name="workoutId">
        ///     The workout id
        /// </param>
        public Task<ServiceActionResult> AddExerciseToWorkout(ExerciseModel exerciseData, long workoutId);

        /// <summary>
        ///     Add the exercise to the workout with the provided id.
        ///     Used when new muscle group exercise has been added and we
        ///     need to auto add it to the current workout as exercise to workout
        /// </summary>
        /// <param name="exerciseData">
        ///     The exercise
        /// </param>
        /// <param name="workoutId">
        ///     The workout id
        /// </param>
        public Task<ServiceActionResult> AddExerciseToWorkout(MGExerciseModel MGExerciseData, long workoutId);

        /// <summary>
        ///     Update the provided exercise
        /// </summary>
        /// <param name="exercise">
        ///     The exercise
        /// </param>
        /// <param name="workoutId">
        ///     The workout id
        /// </param>
        public Task<ServiceActionResult> UpdateExerciseFromWorkout(ExerciseModel exerciseData, long workoutId);

        /// <summary>
        ///     Delete the exercise with the provided id
        /// </summary>
        /// <param name="exerciseId">
        ///     The exercise id
        /// </param>
        public Task<ServiceActionResult> DeleteExerciseFromWorkout(long exerciseId);

        /// <summary>
        ///     Add the exercise to specific muscle group
        /// </summary>
        /// <param name="MGExerciseData">
        ///     The exercise
        /// </param>
        /// <param name="userId">
        ///     The user id who adding the exercise
        /// </param>
        /// <param name="checkExistingEx">
        ///     "Y" if we need to check whether exercise with this name already exists,
        ///     "N" to skip the check
        /// </param>
        public Task<ServiceActionResult> AddExercise(MGExerciseModel MGExerciseData, string userId, string checkExistingEx);

        /// <summary>
        ///     Add the exercise to specific muscle group
        /// </summary>
        /// <param name="exerciseData">
        ///     The exercise
        /// </param>
        public Task<ServiceActionResult> UpdateExercise(MGExerciseModel exerciseData);

        /// <summary>
        ///     Delete the muscle group exercise with the provided id
        /// </summary>
        /// <param name="MGExerciseId">
        ///     The exercise id
        /// </param>
        /// <param name="userId">
        ///     The user id who deleting the exercise
        /// </param>
        public Task<ServiceActionResult> DeleteExercise(long MGExerciseId, string userId);

        /// <summary>
        ///     Mark the set as completed
        /// </summary>
        /// <param name="id">
        ///     The set id
        /// </param>
        public Task<ServiceActionResult> CompleteSet(long id);

        /// <summary>
        ///     Fetch the exercises for the muscle group
        /// </summary>
        /// <param name="muscleGroupId">
        ///     The muscle group id
        /// </param>
        /// <param name="userId">
        ///     The the logged in user id
        /// </param>
        /// <param name="onlyForUser">
        ///     If "Y" the method will return only the user defined exercises for this muscle group,
        ///     which are considered editable and can be deleted / updated
        ///     If "N" the method will return all default and user defined exercises for this muscle group
        /// </param>
        public Task<ServiceActionResult> GetExercisesForMG(long muscleGroupId, string userId, string onlyForUser);

        /// <summary>
        ///     Fetch the muscle group exercise with the provided id
        /// </summary>
        /// <param name="mGExerciseId">
        ///     The muscle group exercise id
        /// </param>
        public Task<ServiceActionResult> GetMGExercise(long mGExerciseId);
    }
}
