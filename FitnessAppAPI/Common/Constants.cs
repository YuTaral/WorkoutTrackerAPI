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
            UNEXPECTED_ERROR,
            EXERCISE_ALREADY_EXISTS
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
        public const string MSG_UNEXPECTED_DB_ERROR = "Unexpected database error occurred while proccessing your request. Please try again";
        public const string MSG_OBJECT_ID_NOT_PROVIDED = "Id not provided";
        public const string MSG_NO_TEMPLATES = "You don\'t have any workout templates yet. Use the top right menu button to create template";
        public const string MSG_USER_REGISTER_SUCCESS = "User registered successfully";
        public const string MSG_WORKOUT_DELETED = "Workout deleted";
        public const string MSG_WORKOUT_ADDED = "Workout added";
        public const string MSG_WORKOUT_UPDATED = "Workout updated";
        public const string MSG_EX_DELETED = "Exercise deleted";
        public const string MSG_EX_ADDED = "Exercise added";
        public const string MSG_EX_UPDATED = "Exercise updated";
        public const string MSG_TEMPLATE_ADDED = "Workout template added";
        public const string MSG_TEMPLATE_DELETED = "Workout template deleted";
        public const string MSG_GET_EXERCISES_FOR_MG_FAILED = "Fetching exercises for muscle group failed. No muscle group id provided";
        public const string MSG_CANNOT_DELETE_DEFAULT_ERROR = "Cannot delete non user defined exercise";
        public const string MSG_USER_DOES_NOT_EXISTS = "User does not exist";
        public const string MSG_WORKOUT_DOES_NOT_EXIST = "Workout does not exist";
        public const string MSG_USER_HAS_NO_WORKOUT = "You don\'t have any workouts";
        public const string MSG_EXERCISE_NOT_FOUND = "Exercise does not exist";
        public const string MSG_NO_MUSCLE_GROUPS_FOUND = "No muscle groups found";
        public const string MSG_TEMPLATE_DOES_NOT_EXIST = "Tempalte does not exist";
        public const string MSG_EX_ALREADY_EXISTS = "Exercise with the same name already exists";
        public const string MSG_CHANGE_PASS_FAIL = "Password not provided";
        public const string MSG_PASSWORD_CHANGED = "Password changed successfully";
        public const string MSG_CHANGE_USER_DEF_VALUES = "User default values not provided";
        public const string MSG_DEF_VALUES_UPDATED = "Default values updated";
        public const string MSG_FAILED_TO_FETCH_WEIGHT_UNITS = "Failed to fetch weight units";
        public const string MSG_FAILED_TO_FETCH_DEFAULT_VALUES = "Failed to fetch default values";
        public const string MSG_FAILED_TO_UPDATE_DEFAULT_VALUES = "Failed to fetch update values";
        public const string MSG_TOKEN_VALIDATION_FAILED = "Token validation failed";


        /// <summary>
        ///     Class containing the DB constants
        /// </summary>
        public static class DBConstants
        {
            public const int MinLen1 = 1;
            public const int MaxLen50 = 50;
            public const int MaxLen4000 = 4000;
            public const string ExceptionTypeDB = "DB";
            public const string ExceptionTypeUnexpected = "UNEXPECTED";
            public const string KG = "Kg";
            public const string LB = "Lb";
        }

        /// <summary>
        ///     Class containing the Validatoin error messages
        /// </summary>
        public static class ValidationErrors
        {
            public const string NAME_REQUIRED = "Name is required";
            public const string NAME_MAX_LEN_50 = "Name maximum length is 50";
            public const string DESCRIPTION_REQUIRED = "Description is required";
            public const string DESCRIPTION_MAX_LEN_4000 = "Description maximum length is 4000";
            public const string SETS_MUST_BE_POSITIVE = "Sets must be positive number";
            public const string REPS_MUST_BE_POSITIVE = "Reps must be positive number";
            public const string WEIGHT_MUST_BE_POSITIVE = "Weight must be positive number";

        }
    }
}
