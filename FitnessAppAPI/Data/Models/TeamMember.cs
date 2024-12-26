using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessAppAPI.Data.Models
{
    /// <summary>
    ///     TeamMembers class to represent a row of database table team members.
    /// </summary>
    public class TeamMember
    {
        [Key]
        public long Id { get; set; }

        [ForeignKey("Team")]
        public required long TeamId { get; set; }

        [ForeignKey("AspNetUser")]
        public required string UserId { get; set; }

        public required string State { get; set; }
    }
}
