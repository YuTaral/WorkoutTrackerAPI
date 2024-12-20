using FitnessAppAPI.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessAppAPI.Data.Models
{
    /// <summary>
    ///    Muscle Group class to represent a row of database table MuscleGroups.
    /// </summary>
    public class MuscleGroup
    {
        [Key]
        public long Id { get; set; }

        [MinLength(Constants.DBConstants.Len1, ErrorMessage = Constants.ValidationErrors.NAME_REQUIRED)]
        [MaxLength(Constants.DBConstants.Len50, ErrorMessage = Constants.ValidationErrors.NAME_MAX_LEN_50)]
        public required string Name { get; set; }

        [ForeignKey("AspNetUser")]
        public string? UserId { get; set; }

        public required string ImageName { get; set; }

        [MaxLength(Constants.DBConstants.Len1)]
        public required string Default { get; set; }
    }
}
