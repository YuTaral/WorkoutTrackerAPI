using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FitnessAppAPI.Data.Models
{
    /// <summary>
    ///     UserProfile class to represent a row of database table UserProfiles.
    /// </summary>
    public class UserProfile
    {
        [Key]
        public long Id { get; set; }

        public required string FullName { get; set; }
        
        public required byte[] ProfileImage { get; set; }

        [ForeignKey("AspNetUser")]
        public required string UserId { get; set; }
    }
}
