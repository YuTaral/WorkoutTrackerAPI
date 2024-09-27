using FitnessAppAPI.Data.Services.MuscleGroups.Models;

namespace FitnessAppAPI.Data.Services.MuscleGroups
{
    /// <summary>
    ///     Muscle Groups service interface define the logic for muscle groups CRUD operations.
    /// </summary>
    public interface IMuscleGroupService
    {
        public List<MuscleGroupModel>? GetMuscleGroups(string userId);
    }
}
