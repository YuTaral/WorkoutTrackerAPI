using Microsoft.AspNetCore.Http;

namespace FitnessAppAPI.Common
{
    /// <summary>
    ///     Constant class to hold all constant used on the server-side.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        ///     Enum to hold all custom defined response codes. Must correspond with client-side ResponseCode enum
        /// </summary>
        public enum ResponseCode
        {
            SUCCESS,
            FAIL,
            UNEXPECTED_ERROR
        }

        /// <summary>
        ///     User messages to show on the client-side.
        /// </summary>
        public const string MSG_SUCCESS = "Success";
        public const string MSG_NOT_ENOUGH_DATA = "Not enough data provided to process the request";
        public const string MSG_REG_FAIL = "Registration failed. Invalid email or password";
        public const string MSG_REG_FAIL_EMAIL = "Registration failed. Invalid email";
        public const string MSG_LOGIN_FAILED = "Invalid email or password.";
        public const string MSG_WORKOUT_ADD_FAIL_NO_DATA = "Add workout failed. Name or user not provided";
        public const string MSG_EXERCISE_ADD_FAIL_NO_DATA = "Add exercise failed. Workout id or exercise data not provided";
        public const string MSG_EXERCISE_UPDATE_FAIL_NO_DATA = "Update exercise failed. Workout id or exercise data not provided";
        public const string MSG_EXERCISE_DELETE_FAIL_NO_ID = "Delete exercise failed. Exercise id not provided";
        public const string MSG_WORKOUT_FAILED_TO_DESERIALIZE_OBJ = "Request failed. Failed to deseriazlied object %s";
        public const string MSG_USER_NOT_LOGGED_IN = "User not logged in";
        public const string MSG_UNEXPECTED_ERROR = "Unexpected error occurred while proccessing your request. Please try again";
        public const string MSG_OBJECT_ID_NOT_PROVIDED = "Id not provided";

        /// <summary>
        ///     Class containing the DB constants
        /// </summary>
        public static class DBConstants
        {
            public const int MinLen1 = 1;
            public const int MaxLen50 = 50;
        }

        /// <summary>
        ///     Class containing the Validatoin error messages
        /// </summary>
        public static class ValidationErrors
        {
            public const string NAME_REQUIRED = "Name is required";
            public const string NAME_MAX_LEN_50 = "Name maximum length is 50";
        }
    }
}
