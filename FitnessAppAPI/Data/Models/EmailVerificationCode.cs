using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessAppAPI.Data.Models
{
    /// <summary>
    ///     EmailVerificationCode class to represent a row of database account verification code.
    /// </summary>
    public class EmailVerificationCode
    {
        [Key]
        public long Id { get; set; }

        public required string HashedCode { get; set; }

        [ForeignKey("AspNetUser")]
        public required string UserId { get; set; }

    }
}
