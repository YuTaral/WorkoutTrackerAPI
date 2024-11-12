using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using FitnessAppAPI.Common;

namespace FitnessAppAPI.Data.Models
{
    /// <summary>
    ///    Muscle Group Exercise class to represent a row of database table MuscleGroupExercises.
    /// </summary>
    public class MuscleGroupExercise
    {
        [Key]
        public long Id { get; set; }

        [MinLength(Constants.DBConstants.MinLen1, ErrorMessage = Constants.ValidationErrors.NAME_REQUIRED)]
        [MaxLength(Constants.DBConstants.MaxLen50, ErrorMessage = Constants.ValidationErrors.NAME_MAX_LEN_50)]
        public required string Name { get; set; }

        [MinLength(Constants.DBConstants.MinLen1, ErrorMessage = Constants.ValidationErrors.DESCRIPTION_REQUIRED)]
        [MaxLength(Constants.DBConstants.MaxLen4000, ErrorMessage = Constants.ValidationErrors.DESCRIPTION_MAX_LEN_4000)]
        public string? Description { get; set; }

        [ForeignKey("UserId")]
        public string? UserId { get; set; }

        [ForeignKey("MuscleGroupId")]
        public long MuscleGroupId { get; set; }
    }
}
