using Microsoft.EntityFrameworkCore;
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

        public int Reps { get; set; }

        public double Weight { get; set; }

        public required bool Completed { get; set; }

        [ForeignKey("Exercise")]
        public required long ExerciseId { get; set; }

    }
}
