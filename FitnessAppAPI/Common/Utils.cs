using FitnessAppAPI.Data;
using FitnessAppAPI.Data.Models;
using FitnessAppAPI.Data.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace FitnessAppAPI.Common
{
    /// <summary>
    ///     Class to hold frequently used functions
    /// </summary>
    public static class Utils
    {
        /// <summary>
        ///     Checks whether the provided email is valid
        /// </summary>
        public static bool IsValidEmail(string email)
        {
            var valid = true;

            try
            {
                var emailAddress = new MailAddress(email);
            }
            catch
            {
                valid = false;
            }

            return valid;
        }

        /// <summary>
        ///     Constructs a comma separated text with the description of the IdentityErrors
        /// </summary>
        public static string UserErrorsToString(IEnumerable<IdentityError> errors)
        {
            string result = "";

            foreach (var error in errors)
            {
                result += error.Description + ", ";
            }

            return result.Substring(0, result.Length - 2);
        }

        /// <summary>
        ///     Checks whether a user with the provided id exists and returns true / false
        /// </summary>
        public static bool UserExists(FitnessAppAPIContext DBAccess, string userId) {
            return DBAccess.Users.Find(userId) != null;
        }

        /// <summary>
        ///     Validates whether the model data is valid
        /// </summary>
        /// <returns>
        /// Empty string if it is valid, otherwise returns the errors
        /// </returns>
        public static string ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model, null, null);
            bool isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            if (isValid)
            {
                return "";
            }

            return string.Join(", ", validationResults.Select(vr => vr.ErrorMessage));
        }
    }
}
