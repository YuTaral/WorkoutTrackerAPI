using FitnessAppAPI.Data.Services.MuscleGroups.Models;

namespace FitnessAppAPI.Data.Services.MuscleGroups
{
    /// <summary>
    ///     Muscle Groups service interface to define the logic for muscle groups CRUD operations.
    /// </summary>
    public interface IMuscleGroupService
    {
        /// <summary>
        ///     Fetch the default Muscle Groups and the user defined Muscle Groups
        /// </summary>
        /// <param name="userId">
        ///     The user id
        /// </param>
        public Task<ServiceActionResult> GetMuscleGroups(string userId);
    }
}
