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
            EXERCISE_ALREADY_EXISTS,
            TOKEN_EXPIRED,
            REFRESH_TOKEN
        }

        /// <summary>
        ///     User messages to show on the client-side.
        /// </summary>
        public const string MSG_SUCCESS = "Success";
        public const string MSG_NOT_ENOUGH_DATA = "Not enough data provided to process the request";
        public const string MSG_REG_FAIL = "Registration failed. Email or password not provided";
        public const string MSG_LOGIN_FAIL = "Login failed. Email or password not provided";
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
        public const string MSG_TOKEN_VALIDATION_FAILED = "Authorization failed";
        public const string MSG_TOKEN_EXPIRED = "Session expired. Please login again";
        public const string MSG_WORKOUT_FINISHED = "Workout finished";
        public const string MSG_FAILED_TO_UPDATE_USER_PROFILE = "Failed to update user profile";
        public const string MSG_USER_PROFILE_UPDATED = "User profile updated";
        public const string MSG_TEAM_FAIL_NO_DATA = "Add team failed. Data not provided";
        public const string MSG_TEAM_ADDED = "Team added";
        public const string MSG_TEAM_DOES_NOT_EXIST = "Team does not exist";
        public const string MSG_TEAM_DELETED = "Team deleted";
        public const string MSG_TEAM_UPDATED = "Team updated";
        public const string MSG_UPDATE_TEAM_FAIL_NO_DATA = "Update team failed. No data provided";
        public const string MSG_SEARCH_NAME_NOT_PROVIDED = "Search name not provided";
        public const string MSG_MEMBER_INVITE = "Member invited";
        public const string MSG_MEMBER_REMOVED = "Member removed";
        public const string MSG_MEMBER_IS_NOT_IN_TEAM = "The member is not in the team";
        public const string MSG_FAILED_TO_SEND_NOTIFICATION = "Failed to send notification";
        public const string MSG_FAILED_TO_GET_NOTIFICATION_DETAILS = "Failed to fetch notification details";
        public const string MSG_FAILED_TO_JOIN_DECLINE_TEAM = "Unexpected error occurred, invalid user or team id provided.";
        public const string MSG_JOINED_TEAM = "Joined the team successfully";
        public const string MSG_FAILED_TO_TEAM_OWNER = "Failed to fetch team owner";
        public const string MSG_TEAM_INVITATION_DECLINED = "Team invitation declined";

        /// <summary>
        ///     Class containing the DB constants
        /// </summary>
        public static class DBConstants
        {
            public const int Len1 = 1;
            public const int Len50 = 50;
            public const int Len100 = 100;
            public const int Len200 = 200;
            public const int Len2000 = 2000;
            public const int Len4000 = 4000;
            public const string KG = "Kg";
            public const string LB = "Lb";
            public const string InviteToTeamNotification = "You have received invitation to join team \"{0}\"";
            public const string AcceptTeamInvitationNotification = "{0} joined team {1}";
            public const string DeclineTeamInvitationNotification = "{0} declined your request to join team {1}";

        }

        /// <summary>
        ///     Class containing the notification texts to display when dealin with notifications
        ///     (e.g accept team invitatoin)
        /// </summary>
        public static class NotificationText
        {
            public const string AskTeamInviteAccept = "{0} invited you to join the team on {1}.\nDo you accept?";
            public const string AskTeamInviteAcceptNoSender = "You have received invitation to join the team on {0}.\nDo you accept?";
        }

        /// <summary>
        ///     Class containing the Validatoin error messages
        /// </summary>
        public static class ValidationErrors
        {
            public const string NAME_REQUIRED = "Name is required";
            public const string NAME_MAX_LEN_50 = "Name maximum length is 50";
            public const string NAME_MAX_LEN_100 = "Name maximum length is 100";
            public const string DESCRIPTION_REQUIRED = "Description is required";
            public const string DESCRIPTION_MAX_LEN_4000 = "Description maximum length is 4000";
            public const string PRIVATE_NOTE_MAX_LEN_2000 = "Private note maximum length is 2000";
            public const string SETS_MUST_BE_POSITIVE = "Sets must be positive number";
            public const string REPS_MUST_BE_POSITIVE = "Reps must be positive number";
            public const string WEIGHT_MUST_BE_POSITIVE = "Weight must be positive number";
            public const string MSG_UPDATE_TEAM_FAIL_NO_DATA = "Update team failed. Data not provided";
        }

        /// <summary>
        ///    Request end point values
        /// </summary>
        public static class RequestEndPoints
        {
            public const string API = "/api";
            public const string FAVICON = "/favicon.ico";

            public const string USER = $"{API}/user";
            public const string WORKOUT = $"{API}/workout";
            public const string EXERCISE = $"{API}/exercise";
            public const string MUSCLE_GROUP = $"{API}/muscle-group";
            public const string USER_PROFILE = $"{API}/user-profile";
            public const string WORKOUT_TEMPLATE = $"{API}/workout-template";
            public const string TEAM = $"{API}/team";
            public const string NOTIFICATION = $"{API}/notification";

            public const string REGISTER = $"{USER}/register";
            public const string LOGIN = $"{USER}/login";
            public const string LOGOUT = $"{USER}/logout";
            public const string CHANGE_PASSWORD = $"{USER}/change-password";
            public const string VALIDATE_TOKEN = $"{USER}/validate-token";

            public const string ADD_WORKOUT = $"{WORKOUT}/add";
            public const string UPDATE_WORKOUT = $"{WORKOUT}/update";
            public const string DELETE_WORKOUT = $"{WORKOUT}/delete";
            public const string GET_WORKOUTS = $"{WORKOUT}/get-workouts";
            public const string GET_WORKOUT = $"{WORKOUT}/get-workout";
            public const string GET_WEIGHT_UNITS = $"{WORKOUT}/get-weight-units";

            public const string ADD_EXERCISE_TO_WORKOUT = $"{EXERCISE}/add-to-workout";
            public const string UPDATE_EXERCISE_FROM_WORKOUT = $"{EXERCISE}/update-exercise-from-workout";
            public const string DELETE_EXERCISE_FROM_WORKOUT = $"{EXERCISE}/delete-exercise-from-workout";
            public const string ADD_EXERCISE = $"{EXERCISE}/add";
            public const string UPDATE_EXERCISE = $"{EXERCISE}/update";
            public const string DELETE_EXERCISE = $"{EXERCISE}/delete";
            public const string GET_EXERCISES_FOR_MG = $"{EXERCISE}/get-by-mg-id";
            public const string GET_MG_EXERCISE = $"{EXERCISE}/get-mg-exercise";

            public const string GET_MUSCLE_GROUPS_FOR_USER = $"{MUSCLE_GROUP}/get-by-user";

            public const string UPDATE_USER_DEFAULT_VALUES = $"{USER_PROFILE}/update-default-values";
            public const string UPDATE_USER_PROFILE = $"{USER_PROFILE}/update-profile";
            public const string GET_USER_DEFAULT_VALUES = $"{USER_PROFILE}/get-default-values";

            public const string ADD_WORKOUT_TEMPLATE = $"{WORKOUT_TEMPLATE}/add";
            public const string DELETE_WORKOUT_TEMPLATE = $"{WORKOUT_TEMPLATE}/delete";
            public const string GET_WORKOUT_TEMPLATES= $"{WORKOUT_TEMPLATE}/get-templates";

            public const string ADD_TEAM= $"{TEAM}/add";
            public const string UPDATE_TEAM = $"{TEAM}/update";
            public const string DELETE_TEAM = $"{TEAM}/delete";
            public const string INVITE_MEMBER = $"{TEAM}/invite-member";
            public const string REMOVE_MEMBER = $"{TEAM}/remove-member";
            public const string ACCEPT_TEAM_INVITE = $"{TEAM}/accept-invite";
            public const string DECLINE_TEAM_INVITE = $"{TEAM}/decline-invite";
            public const string GET_USERS_TO_INVITE = $"{TEAM}/get-users-to-invite";
            public const string GET_MY_TEAMS = $"{TEAM}/my-teams";
            public const string GET_TEAM_MEMBERS= $"{TEAM}/get-team-members";

            public const string NOTIFICATION_REVIEWED = $"{NOTIFICATION}/reviewed";
            public const string GET_NOTIFICATIONS = $"{NOTIFICATION}/get-notifications";
            public const string GET_JOIN_TEAM_NOTIFICATION_DETAILS = $"{NOTIFICATION}/get-join-team-notification-details";
        }

        /// <summary>
        ///     Filter values when fetching workouts
        /// </summary>
        public static class WorkoutFilters
        {
            public const string ALL = "ALL";
            public const string IN_PROGRESS = "IN_PROGRESS";
            public const string COMPLETED = "COMPLETED";
        }

        /// <summary>
        ///     User -> Team state - whether the user has been not invited, invited or accepted to join team
        /// </summary>
        public enum MemberTeamState
        {
            NOT_INVITED,
            INVITED,
            ACCEPTED,
            DECLINED
        }

        /// <summary>
        ///     Notification Types
        /// </summary>
        public enum NotificationType
        {
            INVITED_TO_TEAM,
            JOINED_TEAM,
            REMOVED_FROM_TEAM,
            DECLINED_TEAM_INVITATION
        }
    }
}
