using FitnessAppAPI.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessAppAPI.Data.Models
{
    /// <summary>
    ///     Workout class to represent a row of database table workouts.
    /// </summary>
    public class Workout
    {
        [Key]
        public long Id { get; set; }

        [MinLength(Constants.DBConstants.Len1, ErrorMessage = Constants.ValidationErrors.NAME_REQUIRED)]
        [MaxLength(Constants.DBConstants.Len50, ErrorMessage = Constants.ValidationErrors.NAME_MAX_LEN_50)]
        public required string Name { get; set; }

        [ForeignKey("AspNetUser")]
        public required string UserId { get; set; }

        public DateTime? StartDateTime { get; set; }

        public DateTime? FinishDateTime { get; set; }

        [MaxLength(Constants.DBConstants.Len1)]
        public required string Template { get; set; }

        public required int? DurationSeconds { get; set; }

        [MaxLength(Constants.DBConstants.Len4000, ErrorMessage = Constants.ValidationErrors.DESCRIPTION_MAX_LEN_4000)]
        public required string Notes { get; set; }
    }
}
