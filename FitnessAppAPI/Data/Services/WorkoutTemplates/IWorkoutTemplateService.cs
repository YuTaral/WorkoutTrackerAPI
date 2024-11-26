using FitnessAppAPI.Data.Services.Workouts.Models;

namespace FitnessAppAPI.Data.Services.WorkoutTemplates
{
    /// <summary>
    ///     Workout template service interface to define the logic for workout CRUD operations.
    /// </summary>
    public interface IWorkoutTemplateService
    {
        public ServiceActionResult AddWorkoutTemplate(WorkoutModel data, string userId);
        public ServiceActionResult DeleteWorkoutTemplate(long templateId, string userId);
        public ServiceActionResult GetWorkoutTemplates(string userId);
    }
}
