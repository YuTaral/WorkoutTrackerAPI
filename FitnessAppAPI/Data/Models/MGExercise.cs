using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using FitnessAppAPI.Common;

namespace FitnessAppAPI.Data.Models
{
    /// <summary>
    ///    Muscle Group Exercise class to represent a row of database table MuscleGroupExercises.
    /// </summary>
    public class MGExercise
    {
        [Key]
        public long Id { get; set; }

        [MinLength(Constants.DBConstants.Len1, ErrorMessage = Constants.ValidationErrors.NAME_REQUIRED)]
        [MaxLength(Constants.DBConstants.Len50, ErrorMessage = Constants.ValidationErrors.NAME_MAX_LEN_50)]
        public required string Name { get; set; }

        [MaxLength(Constants.DBConstants.Len4000, ErrorMessage = Constants.ValidationErrors.DESCRIPTION_MAX_LEN_4000)]
        public string? Description { get; set; }

        [ForeignKey("AspNetUser")]
        public string? UserId { get; set; }

        [ForeignKey("MuscleGroup")]
        public required long MuscleGroupId { get; set; }
    }
}
