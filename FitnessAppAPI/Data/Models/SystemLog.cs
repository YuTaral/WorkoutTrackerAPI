using System.ComponentModel.DataAnnotations;

namespace FitnessAppAPI.Data.Models
{
    /// <summary>
    ///     SystemLog class to represent a row of database table SystemLogs.
    /// </summary>
    public class SystemLog
    {
        [Key]
        public long Id { get; set; }
        public required string ActionName { get; set; }
        public required string ExceptionType { get; set; }
        public required string ExceptionDescription { get; set; }
        public DateTime Date { get; set; }
        public string? UserId { get; set; }
    }
}
