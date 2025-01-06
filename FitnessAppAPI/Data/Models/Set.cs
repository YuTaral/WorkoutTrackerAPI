using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessAppAPI.Data.Models
{
    /// <summary>
    ///     Set class to represent a row of database table sets.
    /// </summary>
    public class Set
    {
        [Key]
        public long Id { get; set; }

        public required int Reps { get; set; }

        public required double Weight { get; set; }

        public required int Rest { get; set; }

        public required bool Completed { get; set; }

        [ForeignKey("Exercise")]
        public required long ExerciseId { get; set; }

    }
}
