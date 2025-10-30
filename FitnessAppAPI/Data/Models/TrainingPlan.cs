using FitnessAppAPI.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace FitnessAppAPI.Data.Models
{
    /// <summary>
    ///     TrainingPlan class to represent a row of database table training plan.
    /// </summary>
    public class TrainingPlan
    {
        [Key]
        public long Id { get; set; }

        [ForeignKey("AspNetUser")]
        public required string UserId { get; set; }

        [MinLength(Constants.DBConstants.Len1, ErrorMessage = Constants.ValidationErrors.NAME_REQUIRED)]
        [MaxLength(Constants.DBConstants.Len50, ErrorMessage = Constants.ValidationErrors.NAME_MAX_LEN_50)]
        public required string Name { get; set; }

        [MaxLength(Constants.DBConstants.Len4000, ErrorMessage = Constants.ValidationErrors.DESCRIPTION_MAX_LEN_4000)]
        public required string Description { get; set; }
    }
}
