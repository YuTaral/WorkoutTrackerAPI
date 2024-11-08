using FitnessAppAPI.Data.Services.Workouts.Models;

namespace FitnessAppAPI.Data.Services.WorkoutTemplates
{
    /// <summary>
    ///     Workout template service interface to define the logic for workout CRUD operations.
    /// </summary>
    public interface IWorkoutTemplateService
    {
        public bool AddWorkoutTemplate(WorkoutModel data, string userId);
        public bool DeleteWorkoutTemplate(long templateId);
        public List<WorkoutModel> GetWorkoutTemplates(string userId);
    }
}
