using FitnessAppAPI.Data.Services.Workouts.Models;

namespace FitnessAppAPI.Data.Services.WorkoutTemplates
{
    /// <summary>
    ///     Workout template service interface to define the logic for workout CRUD operations.
    /// </summary>
    public interface IWorkoutTemplateService
    {

        /// <summary>
        ///     Add new workout template from the provided WorkoutModel data
        /// </summary>
        /// <param name="requestData">
        ///     The request data (template data)
        /// </param>
        /// <param name="userId">
        ///     The user who is adding the template
        /// </param>
        public Task<ServiceActionResult<WorkoutModel>> AddWorkoutTemplate(Dictionary<string, string> requestData, string userId);

        /// <summary>
        ///    Update workout template
        /// </summary>
        /// <param name="requestData">
        ///     The request data (template data)
        /// </param>
        /// <param name="userId">
        ///     The user who is adding the template
        /// </param>
        public Task<ServiceActionResult<WorkoutModel>> UpdateWorkoutTemplate(Dictionary<string, string> requestData, string userId);

        /// <summary>
        ///    Delete the template with the provided id
        /// </summary>
        /// <param name="templateId">
        ///     The emplate id
        /// </param>
        public Task<ServiceActionResult<WorkoutModel>> DeleteWorkoutTemplate(long templateId);

        /// <summary>
        ///     Return list of all workout templates created by the user with the provided id
        /// </summary>
        /// <param name="userId">
        ///     The user who is fetching the templates
        /// </param>
        public Task<ServiceActionResult<WorkoutModel>> GetWorkoutTemplates(string userId);

        /// <summary>
        ///     Get the workout template data by the provided assigned workout id
        /// </summary>
        /// <param name="assignedWorkoutId">
        ///     The assigned workout id
        /// </param>
        public Task<ServiceActionResult<WorkoutModel>> GetWorkoutTemplate(long assignedWorkoutId);
    }
}
