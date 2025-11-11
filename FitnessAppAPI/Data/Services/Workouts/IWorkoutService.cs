using FitnessAppAPI.Data.Models;
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
        /// <param name="requestData">
        ///     The request data (workout)
        /// </param>
        /// <param name="userId">
        ///     The user who is adding the workout
        /// </param>
        public Task<ServiceActionResult<WorkoutModel>> AddWorkout(Dictionary<string, string> requestData, string userId);

        /// <summary>
        ///     Add new workout from the provided WorkoutModel data
        /// </summary>
        /// <param name="workoutData">
        ///     The workout data
        /// </param>
        /// <param name="scheduledDateTime">
        ///     The scheduled date time (may be null when the workout is not assigned)
        /// </param>
        /// <param name="assignedWorkoutId">
        ///     The assigned workout id (may be 0)
        /// </param>
        /// <param name="userId">
        ///     The user who is adding the workout
        /// </param>
        public Task<ServiceActionResult<WorkoutModel>> AddWorkout(WorkoutModel workoutData, DateTime? scheduledDateTime, long assignedWorkoutId, string userId);

        /// <summary>
        ///     Edit the workout from the provided WorkoutModel data
        /// </summary>
        /// <param name="requestData">
        ///     The request data (workout)
        /// </param>
        /// <param name="userId">
        ///     The user who is updating the workout
        /// </param>
        public Task<ServiceActionResult<WorkoutModel>> UpdateWorkout(Dictionary<string, string> requestData, string userId);


        /// <summary>
        ///     Mark the workout as finished
        /// </summary>
        /// <param name="workoutId">
        ///     The workout id
        /// </param>
        /// <param name="userId">
        ///     The user who is updating the workout
        /// </param>
        public Task<ServiceActionResult<WorkoutModel>> FinishWorkout(long workoutId, string userId);

        /// <summary>
        ///     Delete the workout with the provided id
        /// </summary>
        /// <param name="workoutId">
        ///     The workout id
        /// </param>
        /// <param name="userId">
        ///     The user who is deleting the workout
        /// </param>
        public Task<ServiceActionResult<BaseModel>> DeleteWorkout(long workoutId, string userId);

        /// <summary>
        ///     Fetch the workout with the provided id and returns WorkoutModel
        /// </summary>
        /// <param name="id">
        ///     The workout id
        /// </param>
        /// <param name="userId">
        ///     The user owner of the workout
        /// </param>
        public Task<ServiceActionResult<WorkoutModel>> GetWorkout(long id, string userId);

        /// <summary>
        ///     Fetch the latest workout for the user and returns WorkoutModel list
        /// </summary>
        /// <param name="startDate">
        ///     The start date
        /// </param>
        /// <param name="userId">
        ///     The user id
        /// </param>
        public Task<ServiceActionResult<WorkoutModel>> GetLatestWorkouts(string startDate, string userId);
    }
}
