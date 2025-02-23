using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Services.MuscleGroups.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using static FitnessAppAPI.Common.Constants;

namespace FitnessAppAPI.Data.Services.MuscleGroups
{
    /// <summary>
    ///     Muscle group service class to implement IMuscleGroup interface.
    /// </summary>
    public class MuscleGroupService(FitnessAppAPIContext DB) : BaseService(DB), IMuscleGroupService
    {
        public async Task<ServiceActionResult<MuscleGroupModel>> GetMuscleGroups(String userId)
        {
            var returnData = await DBAccess.MuscleGroups.Where(m => m.UserId == userId || m.UserId == null)
                                                .OrderBy(m => m.Id)
                                                .Select(m => ModelMapper.MapToMuscleGroupModel(m))
                                                .ToListAsync();

            if (returnData.Count > 0)
            {
                return new ServiceActionResult<MuscleGroupModel>(HttpStatusCode.OK, MSG_SUCCESS, returnData);
            }

            // Should not happen as there are always default muscle groups
            return new ServiceActionResult<MuscleGroupModel>(HttpStatusCode.NotFound, MSG_NO_MUSCLE_GROUPS_FOUND);
        }
    }
}
