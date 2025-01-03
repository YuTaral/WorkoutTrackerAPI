using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Services.UserProfile.Models;
using System.ComponentModel.DataAnnotations;

namespace FitnessAppAPI.Data.Services.User.Models
{
    /// <summary>
    ///     UserModel class representing the logged in user.
    ///     Must correspond with client-side UserModel class
    /// </summary>
    /// 
    public class UserModel
    {
        public required string Id { get; set; }

        public required string Email { get; set; }

        [MinLength(Constants.DBConstants.Len1, ErrorMessage = Constants.ValidationErrors.NAME_REQUIRED)]
        [MaxLength(Constants.DBConstants.Len100, ErrorMessage = Constants.ValidationErrors.NAME_MAX_LEN_100)]
        public required string FullName { get; set; }

        public required string ProfileImage { get; set; }

        public required UserDefaultValuesModel DefaultValues { get; set; }
    }
}
