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

        [MinLength(Constants.DBConstants.MinLen1, ErrorMessage = Constants.ValidationErrors.NAME_REQUIRED)]
        [MaxLength(Constants.DBConstants.MaxLen50, ErrorMessage = Constants.ValidationErrors.NAME_MAX_LEN_50)]
        public required string Name { get; set; }

        [ForeignKey("UserId")]
        public string? UserId { get; set; }

        public required string ImageName { get; set; }

        public required string Default { get; set; }
    }
}
