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

        public required int Sets { get; set; }

        public required int Reps { get; set; }

        public required double Weight { get; set; }

        public required double Rest { get; set; }

        public required bool Completed { get; set; }

        public required string WeightUnit { get; set; }

        [ForeignKey("AspNetUser")]
        public required string UserId { get; set; }

        [ForeignKey("MGExercise")]
        public required long MGExeciseId { get; set; }
    }
}
