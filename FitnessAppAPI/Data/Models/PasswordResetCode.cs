using FitnessAppAPI.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessAppAPI.Data.Models
{
    /// <summary>
    ///     PasswordResetCode class to represent a row of database password reset code.
    /// </summary>
    public class PasswordResetCode
    {
        [Key]
        public long Id { get; set; }

        public required string HashedCode { get; set; }

        [ForeignKey("AspNetUser")]
        public required string UserId { get; set; }

        public required DateTime ExpirationDate { get; set; }
    }
}
