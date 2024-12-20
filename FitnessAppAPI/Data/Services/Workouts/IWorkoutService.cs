using FitnessAppAPI.Data.Services.Workouts.Models;

namespace FitnessAppAPI.Data.Services.Workouts
{
    /// <summary>
    ///     Workout service interface to define the logic for workout CRUD operations.
    /// </summary>
    public interface IWorkoutService
    {
        /// <summary>
        ///     Add new workout from the provided WorkoutModel data
        /// </summary>
        /// <param name="data">
        ///     The workout data
        /// </param>
        /// <param name="userId">
        ///     The user who is adding the workout
        /// </param>
        public Task<ServiceActionResult> AddWorkout(WorkoutModel data, string userId);

        /// <summary>
        ///     Edit the workout from the provided WorkoutModel data
        /// </summary>
        /// <param name="data">
        ///     The workout data
        /// </param>
        /// <param name="userId">
        ///     The user who is updating the workout
        /// </param>
        public Task<ServiceActionResult> UpdateWorkout(WorkoutModel data, string userId);

        /// <summary>
        ///     Delete the workout with the provided id
        /// </summary>
        ///  /// <param name="workoutId">
        ///     The workout id
        /// </param>
        /// <param name="userId">
        ///     The user who is deleting the workout
        /// </param>
        public Task<ServiceActionResult> DeleteWorkout(long workoutId, string userId);

        /// <summary>
        ///     Fetch the workout with the provided id and returns WorkoutModel
        /// </summary>
        /// <param name="id">
        ///     The workout id
        /// </param>
        /// <param name="userId">
        ///     The user owner of the workout
        /// </param>
        public Task<ServiceActionResult> GetWorkout(long id, string userId);

        /// <summary>
        ///     Fetch the latest workout for the user and returns WorkoutModel list
        /// </summary>
        /// <param name="filterBy">
        ///     The filter value - ALL / IN_PROGRESS / COMPLETED
        /// </param>
        /// <param name="userId">
        ///     The user id
        /// </param>
        public Task<ServiceActionResult> GetLatestWorkouts(string filterBy, string userId);

        /// <summary>
        ///     Fetch the weight units
        /// </summary>
        public Task<ServiceActionResult> GetWeightUnits();
    }
}
