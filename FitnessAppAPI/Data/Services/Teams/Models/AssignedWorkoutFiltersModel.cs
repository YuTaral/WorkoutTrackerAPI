namespace FitnessAppAPI.Data.Services.Teams.Models
{
    /// <summary>
    ///     AssignedWorkoutFiltersModel class representing the filter used when fetching assigned workouts.
    ///     Must correspond with client-side AssignedWorkoutFiltersModel class
    /// </summary>
    public class AssignedWorkoutFiltersModel
    {
        public required DateTime StartDate { get; set; }
        public required long TeamId { get; set; }
        public required string Progress { get; set; }
    }
}
