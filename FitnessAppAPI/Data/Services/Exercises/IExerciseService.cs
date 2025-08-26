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
        /// <param name="requestData">
        ///     The request data (exercise and workout id)
        /// </param>
        public Task<ServiceActionResult<long>> AddExerciseToWorkout(Dictionary<string, string> requestData);

        /// <summary>
        ///     Save the Exercise when adding exercse to workout
        /// </summary>
        /// <param name="exerciseData">
        ///     The exercise data
        /// </param>
        /// <param name="workoutId">
        ///     The workout id
        /// </param>
        public Task<ServiceActionResult<long>> AddExerciseToWorkout(ExerciseModel exerciseData, long workoutId);

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
        public Task<ServiceActionResult<long>> AddExerciseToWorkout(MGExerciseModel MGExerciseData, long workoutId);

        /// <summary>
        ///     Update the provided exercise
        /// </summary>
        /// <param name="requestData">
        ///     The request data (exercise and workout id)
        /// </param>
        public Task<ServiceActionResult<long>> UpdateExerciseFromWorkout(Dictionary<string, string> requestData);

        /// <summary>
        ///     Delete the exercise with the provided id
        /// </summary>
        /// <param name="exerciseId">
        ///     The exercise id to delete
        /// </param>
        public Task<ServiceActionResult<long>> DeleteExerciseFromWorkout(long exerciseId);

        /// <summary>
        ///     Add the exercise to specific muscle group
        /// </summary>
        /// <param name="requestData">
        ///     The request data (the exercise)
        /// </param>
        /// <param name="userId">
        ///     The user id who adding the exercise
        /// </param>
        public Task<ServiceActionResult<MGExerciseModel>> AddExercise(Dictionary<string, string> requestData, string userId);

        /// <summary>
        ///     Add the exercise to specific muscle group
        /// </summary>
        /// <param name="requestData">
        ///      The request data (exercise)
        /// </param>
        public Task<ServiceActionResult<MGExerciseModel>> UpdateExercise(Dictionary<string, string> requestData);

        /// <summary>
        ///     Delete the muscle group exercise with the provided id
        /// </summary>
        /// <param name="MGExerciseId">
        ///     The exercise id
        /// </param>
        /// <param name="userId">
        ///     The user id who deleting the exercise
        /// </param>
        public Task<ServiceActionResult<long>> DeleteExercise(long MGExerciseId, string userId);

        /// <summary>
        ///     Mark the set as completed
        /// </summary>
        /// <param name="requestData">
        ///     The request data (set id)
        /// </param>
        public Task<ServiceActionResult<BaseModel>> CompleteSet(Dictionary<string, string>  requestData);

        /// <summary>
        ///    Fetch the exercises for the specified muscle group
        /// </summary>
        /// <param name="muscleGroupId">
        ///     The muscle group id
        /// </param>
        /// <param name="onlyForUser">
        ///     "Y" if the exercises should be only the ones created by the user,
        ///     "N" if all
        /// </param>
        ///  <param name="userId">
        ///     The logged in user id
        /// </param>
        public Task<ServiceActionResult<MGExerciseModel>> GetExercisesForMG(long muscleGroupId, string onlyForUser, string userId);

        /// <summary>
        ///     Fetch the muscle group exercise with the provided id
        /// </summary>
        /// <param name="mGExerciseId">
        ///     The muscle group exercise id
        /// </param>
        public Task<ServiceActionResult<MGExerciseModel>> GetMGExercise(long mGExerciseId);
    }
}
