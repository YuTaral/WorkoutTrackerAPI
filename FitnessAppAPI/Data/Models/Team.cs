using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using FitnessAppAPI.Common;

namespace FitnessAppAPI.Data.Models
{
    /// <summary>
    ///     Team class to represent a row of database table teams.
    /// </summary>
    public class Team
    {
        [Key]
        public long Id { get; set; }

        public required byte[] Image { get; set; }

        [MinLength(Constants.DBConstants.MinLen1, ErrorMessage = Constants.ValidationErrors.NAME_REQUIRED)]
        [MaxLength(Constants.DBConstants.MaxLen50, ErrorMessage = Constants.ValidationErrors.NAME_MAX_LEN_50)]
        public required string Name { get; set; }

        public required string Description { get; set; }

        public required string PrivateNote { get; set; }

        [ForeignKey("AspNetUser")]
        public required string UserId { get; set; }
    }
}
