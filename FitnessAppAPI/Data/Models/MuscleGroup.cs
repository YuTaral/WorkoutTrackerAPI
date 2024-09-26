using FitnessAppAPI.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessAppAPI.Data.Models
{
    /// <summary>
    ///    Muscle Group class to represent a row of database table muscle_groups.
    /// </summary>
    public class MuscleGroup
    {
        [Required]
        [Key]
        public long Id { get; set; }

        [Required]
        [MaxLength(Constants.DBConstants.MuscleGroupMaxLen)]
        public required string Name { get; set; }

        [ForeignKey("UserId")]
        public string? UserId { get; set; }
    }
}
