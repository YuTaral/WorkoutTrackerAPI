using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using FitnessAppAPI.Common;

namespace FitnessAppAPI.Data.Models
{
    /// <summary>
    ///     UserProfile class to represent a row of database table UserProfiles.
    /// </summary>
    public class UserProfile
    {
        [Key]
        public long Id { get; set; }

        [MinLength(Constants.DBConstants.Len1, ErrorMessage = Constants.ValidationErrors.NAME_REQUIRED)]
        [MaxLength(Constants.DBConstants.Len100, ErrorMessage = Constants.ValidationErrors.NAME_MAX_LEN_100)]
        public required string FullName { get; set; }
        
        public required byte[] ProfileImage { get; set; }

        [ForeignKey("AspNetUser")]
        public required string UserId { get; set; }
    }
}
