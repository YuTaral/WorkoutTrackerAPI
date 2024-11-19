using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Models;

namespace FitnessAppAPI.Data.Services.MuscleGroups
{
    /// <summary>
    ///     Muscle group service class to implement IMuscleGroup interface.
    /// </summary>
    public class MuscleGroupService(FitnessAppAPIContext DB) : BaseService, IMuscleGroupService
    {
        private readonly FitnessAppAPIContext DBAccess = DB;

        /// <summary>
        ///     Fetches the default Muscle Groups and the user defined Muscle Groups
        /// </summary>
        /// <param name="userId">
        ///     The user id
        /// </param>
        public ServiceActionResult GetMuscleGroups(String userId)
        {
            return ExecuteServiceAction(() => {
                var returnData = DBAccess.MuscleGroups.Where(m => m.UserId == userId || m.UserId == null)
                                                    .OrderBy(m => m.Id)
                                                    .Select(m => (BaseModel)ModelMapper.MapToMuscleGroupModel(m))
                                                    .ToList();

                if (returnData.Count > 0)
                {
                    return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS, returnData);
                }

                // Should not happen as there are always default muscle groups
                return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_NO_MUSCLE_GROUPS_FOUND, returnData);
            });
        }
    }
}
