using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace FitnessAppAPI.Data.Models
{
    /// <summary>
    ///     TrainingDay class to represent a row of database table training day as part of program.
    /// </summary>
    public class TrainingDay
    {
        [Key]
        public long Id { get; set; }

        [ForeignKey("TrainingProgram")]
        public required long TrainingProgramId { get; set; }
    }
}
