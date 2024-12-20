using FitnessAppAPI.Common;
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

        [MaxLength(Constants.DBConstants.Len2000)]
        public required string ExceptionDescription { get; set; }

        [MaxLength(Constants.DBConstants.Len2000)]
        public required string ExceptionStackTrace { get; set; }

        public DateTime Date { get; set; }

        public string? UserId { get; set; }
    }
}
