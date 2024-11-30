using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessAppAPI.Data.Models
{
    /// <summary>
    ///     UserDefaultValue class to represent a row of database table UserDefaultValues.
    /// </summary>
    public class UserDefaultValue
    {
        [Key]
        public long Id { get; set; }

        public int Sets { get; set; }

        public int Reps { get; set; }

        public double Weight { get; set; }

        public required string WeightUnitCode { get; set; }

        [ForeignKey("AspNetUser")]
        public required string UserId { get; set; }

        [ForeignKey("MGExercise")]
        public long? MGExeciseId { get; set; }
    }
}
