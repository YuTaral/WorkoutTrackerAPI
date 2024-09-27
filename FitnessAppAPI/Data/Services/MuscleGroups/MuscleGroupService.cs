using FitnessAppAPI.Data.Services.MuscleGroups.Models;

namespace FitnessAppAPI.Data.Services.MuscleGroups
{
    /// <summary>
    ///     Muscle group service class to implement IMuscleGroup interface.
    /// </summary>
    public class MuscleGroupService(FitnessAppAPIContext DB) : IMuscleGroupService
    {
        private readonly FitnessAppAPIContext DBAccess = DB;

        /// <summary>
        ///     Fetches the default Muscle Groups and the user defined Muscle Groups
        /// </summary>
        /// <param name="userId">
        ///     The user id
        /// </param>
        public List<MuscleGroupModel>? GetMuscleGroups(String userId)
        {
            return DBAccess.MuscleGroups.Where(m => m.UserId == userId || m.UserId == null)
                                        .OrderBy(m => m.Id)
                                        .Select(m => new MuscleGroupModel
                                        {
                                            Id = m.Id,
                                            Name = m.Name,
                                            Checked = false
                                        }).ToList();
        }
    }
}
