using FitnessAppAPI.Data.Services.Workouts.Models;

namespace FitnessAppAPI.Data.Services.WorkoutTemplates
{
    /// <summary>
    ///     Workout template service interface to define the logic for workout CRUD operations.
    /// </summary>
    public interface IWorkoutTemplateService
    {
        public Task<ServiceActionResult> AddWorkoutTemplate(WorkoutModel data, string userId);
        public Task<ServiceActionResult> DeleteWorkoutTemplate(long templateId);
        public Task<ServiceActionResult> GetWorkoutTemplates(string userId);
    }
}
