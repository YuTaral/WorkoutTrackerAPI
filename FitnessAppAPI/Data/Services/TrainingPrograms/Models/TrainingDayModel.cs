using FitnessAppAPI.Data.Models;
using FitnessAppAPI.Data.Services.Workouts.Models;

namespace FitnessAppAPI.Data.Services.TrainingPrograms.Models
{
    /// <summary>
    ///     TrainingDayModel class representing a day as part of training program.
    ///     Must correspond with client-side TrainingDayModel class
    /// </summary>
    public class TrainingDayModel : BaseModel
    {
        public required long TrainingPlanId { get; set; }

        public required List<WorkoutModel> Workouts { get; set; }
    }
}
