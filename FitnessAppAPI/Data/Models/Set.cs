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
        [Required]
        [Key]
        public long Id { get; set; }

        public int Reps { get; set; }

        public double Weight { get; set; }

        [Required]
        public bool Completed { get; set; }

        [Required]
        [ForeignKey("Exercise")]
        public long ExerciseId { get; set; }

    }
}
