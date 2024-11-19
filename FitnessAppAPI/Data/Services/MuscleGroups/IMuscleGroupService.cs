using FitnessAppAPI.Data.Services.MuscleGroups.Models;

namespace FitnessAppAPI.Data.Services.MuscleGroups
{
    /// <summary>
    ///     Muscle Groups service interface to define the logic for muscle groups CRUD operations.
    /// </summary>
    public interface IMuscleGroupService
    {
        public ServiceActionResult GetMuscleGroups(string userId);
    }
}
