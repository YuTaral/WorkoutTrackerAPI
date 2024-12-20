using System.ComponentModel.DataAnnotations;
using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Models;

namespace FitnessAppAPI.Data.Services.MuscleGroups.Models
{
    /// <summary>
    ///     MuscleGroupModel class representing a muscle group.
    ///     Must correspond with client-side MuscleGroupModel class
    /// </summary>
    public class MuscleGroupModel: BaseModel
    {
        [MinLength(Constants.DBConstants.Len1, ErrorMessage = Constants.ValidationErrors.NAME_REQUIRED)]
        [MaxLength(Constants.DBConstants.Len50, ErrorMessage = Constants.ValidationErrors.NAME_MAX_LEN_50)]
        public required string Name { get; set; }

        public required string ImageName { get; set; }
    }
}
