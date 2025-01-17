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
        /// <param name="data">
        ///     The template data
        /// </param>
        /// <param name="userId">
        ///     The user who is adding the template
        /// </param>
        public Task<ServiceActionResult> AddWorkoutTemplate(WorkoutModel data, string userId);

        /// <summary>
        ///    Update workout template
        /// </summary>
        /// <param name="data">
        ///     The template data
        /// </param>
        /// <param name="userId">
        ///     The user who is adding the template
        /// </param>
        public Task<ServiceActionResult> UpdateWorkoutTemplate(WorkoutModel data, string userId);

        /// <summary>
        ///    Delete the template with the provided id
        /// </summary>
        /// <param name="templateId">
        ///     The template id
        /// </param>
        public Task<ServiceActionResult> DeleteWorkoutTemplate(long templateId);

        /// <summary>
        ///     Return list of all workout templates created by the user with the provided id
        /// </summary>
        /// <param name="userId">
        ///     The user who is fetching the templates
        /// </param>
        public Task<ServiceActionResult> GetWorkoutTemplates(string userId);
    }
}
