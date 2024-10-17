using System.ComponentModel.DataAnnotations;
using FitnessAppAPI.Common;

namespace FitnessAppAPI.Data.Services.MuscleGroups.Models
{
    /// <summary>
    ///     MuscleGroupModel class representing a muscle group.
    ///     Must correspond with client-side MuscleGroupModel class
    /// </summary>
    public class MuscleGroupModel
    {
        public long Id { get; set; }

        [MinLength(Constants.DBConstants.MinLen1, ErrorMessage = Constants.ValidationErrors.NAME_REQUIRED)]
        [MaxLength(Constants.DBConstants.MaxLen50, ErrorMessage = Constants.ValidationErrors.NAME_MAX_LEN_50)]
        public required string Name { get; set; }
        public required bool Checked { get; set; }

    }
}
