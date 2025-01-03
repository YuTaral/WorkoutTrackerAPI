using FitnessAppAPI.Data.Models;
using FitnessAppAPI.Data.Services.User.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using static FitnessAppAPI.Common.Constants;

namespace FitnessAppAPI.Common
{
    /// <summary>
    ///     Helper class with frequently used functions
    /// </summary>
    public static class Utils
    {
        /// <summary>
        ///     Check whether the provided email is valid
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
        ///     Construct a comma separated text with the description of the IdentityErrors
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
        ///     Validates whether the provided model and its nested objects are valid. Returns empty string if 
        ///     the model is valid, otherwise comma-separated list of errors
        /// </summary>
        /// <param name="model">The model to validate, inheriting from BaseModel.</param>
        public static string ValidateModel(BaseModel model)
        {
            var validationResults = new List<ValidationResult>();
            ValidateObjectRecursive(model, validationResults);

            if (validationResults.Count == 0)
            {
                return "";
            }

            return string.Join(", ", validationResults.Select(vr => vr.ErrorMessage));
        }

        /// <summary>
        ///     Recursively validates an object, including its nested objects and collections.
        /// </summary>
        /// <param name="model">
        ///     The object to validate.
        /// </param>
        /// <param name="results">
        ///     A list to collect validation results from the object and its nested properties.
        /// </param>
        private static void ValidateObjectRecursive(object model, List<ValidationResult> results)
        {
            var validationContext = new ValidationContext(model, null, null);

            // Validate the current model, if there is error it is added to results
            Validator.TryValidateObject(model, validationContext, results, true);

            // Go throught all properties of the model
            foreach (var property in model.GetType().GetProperties())
            {
                // Get the current value
                var value = property.GetValue(model);

                if (value is IEnumerable<object> enumerable)
                {
                    // If the current value is enumerable, validate all items
                    foreach (var item in enumerable)
                    {
                        ValidateObjectRecursive(item, results);
                    }
                }
                else if (value is BaseModel)
                {
                    // Validate the value if it's another model
                    ValidateObjectRecursive(value, results);
                }
            }
        }

        /// <summary>
        ///     Validates whether the provided user model and its nested objects are valid. Returns empty string if 
        ///     the user model is valid, otherwise comma-separated list of errors
        /// </summary>
        /// <param name="model">
        ///     The model to validate.
        /// </param>
        public static string ValidateModel(UserModel model)
        {
            var validationResults = new List<ValidationResult>();
            ValidateObjectRecursive(model, validationResults);

            if (validationResults.Count == 0)
            {
                return "";
            }

            return string.Join(", ", validationResults.Select(vr => vr.ErrorMessage));
        }

        /// <summary>
        ///     Create response object
        ///     Returned fields must correspond with client-side CustomResponse class:
        ///         - Code
        ///         - Message
        ///         - Data
        ///         - Notification
        /// </summary>
        public static System.Object CreateResponseObject(ResponseCode Code, string Message, List<string> Data, bool Notification = false)
        {
            return (new
            {
                Code,
                Message,
                Data,
                Notification
            });
        }

        /// <summary>
        ///     Decode base64 string into byte array
        /// </summary>
        /// <param name="base64String">
        ///     The string
        /// </param>
        public static byte[] DecodeBase64ToByteArray(string base64String)
        {
            // Strip metadata prefix if it exists (e.g., "data:image/png;base64,")
            if (base64String.Contains(","))
            {
                base64String = base64String.Substring(base64String.IndexOf(",") + 1);
            }

            // Convert Base64 to byte array
            return Convert.FromBase64String(base64String);
        }

        /// <summary>
        ///     Encode byte array into base64 string, return empty string if the array is empty
        /// </summary>
        /// <param name="imageBytes">
        ///     The by array
        /// </param>     
        public static string EncodeByteArrayToBase64Image(byte[] imageBytes)
        {
            // Convert the byte array to a Base64 string
            return Convert.ToBase64String(imageBytes);
        }

        /// <summary>
        ///     Format the date object to the default date time format for the app
        /// </summary>
        /// <param name="dateTime">
        ///     The date time
        /// </param>
        public static string FormatDefaultDateTime(DateTime dateTime) {
            return dateTime.ToString("dd MMM yyyy, HH:mm");
        }
    }
}
