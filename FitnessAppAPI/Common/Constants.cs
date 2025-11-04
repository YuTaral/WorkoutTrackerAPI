using System.CodeDom;

namespace FitnessAppAPI.Common
{
    /// <summary>
    ///     Constant class with all constants used on the server-side.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        ///     Custom response codes
        /// </summary>
        public class CustomHttpStatusCode
        {
            public const int ACCOUNT_NOT_VERIFIED = 498;
            public const int EXERCISE_ALREADY_EXISTS = 499;
        }

        /// <summary>
        ///     User messages to show on the client-side.
        /// </summary>
        public const string MSG_SUCCESS = "Success";
        public const string MSG_REG_FAIL = "Email or password not provided";
        public const string MSG_LOGIN_FAIL = "Email or password not provided";
        public const string MSG_INVALID_EMAIL = "Invalid email";
        public const string MSG_LOGIN_FAILED = "Invalid email or password";
        public const string MSG_GOOGLE_SIGN_IN_FAILED = "Unexpected error occurred while using Google Sign-In. Please try again";
        public const string MSG_WORKOUT_ADD_FAIL_NO_DATA = "Workout data not provided";
        public const string MSG_EXERCISE_ADD_FAIL_NO_DATA = "Workout id or exercise data not provided";
        public const string MSG_EXERCISE_FAIL_NO_DATA = "Exercise data not provided";
        public const string MSG_EXERCISE_DELETE_FAIL_NO_ID = "Exercise id not provided";
        public const string MSG_WORKOUT_FAILED_TO_DESERIALIZE_OBJ = "Failed to deseriazlied object %s";
        public const string MSG_UNEXPECTED_ERROR = "Unexpected error occurred while proccessing your request. Please try again";
        public const string MSG_UNEXPECTED_DB_ERROR = "Unexpected database error occurred while proccessing your request. Please try again";
        public const string MSG_OBJECT_ID_NOT_PROVIDED = "Id not provided";
        public const string MSG_CANNOT_DELETE_DEFAULT_ERROR = "Cannot delete non user defined exercise";
        public const string MSG_USER_DOES_NOT_EXISTS = "User does not exist";
        public const string MSG_WORKOUT_DOES_NOT_EXIST = "Workout does not exist";
        public const string MSG_EXERCISE_NOT_FOUND = "Exercise does not exist";
        public const string MSG_NO_MUSCLE_GROUPS_FOUND = "No muscle groups found";
        public const string MSG_TEMPLATE_DOES_NOT_EXIST = "Template does not exist";
        public const string MSG_EX_ALREADY_EXISTS = "Exercise with the same name already exists";
        public const string MSG_PASSWORD_NOT_PROVIDED = "Password not provided";
        public const string MSG_CHANGE_USER_DEF_VALUES = "User default values not provided";
        public const string MSG_FAILED_TO_FETCH_WEIGHT_UNITS = "Failed to fetch weight units";
        public const string MSG_FAILED_TO_FETCH_DEFAULT_VALUES = "Failed to fetch default values";
        public const string MSG_FAILED_TO_UPDATE_DEFAULT_VALUES = "Failed to update values";
        public const string MSG_TOKEN_VALIDATION_FAILED = "Authorization failed";
        public const string MSG_TOKEN_EXPIRED = "Session expired. Please login again";
        public const string MSG_FAILED_TO_UPDATE_USER_PROFILE = "Failed to update user profile";
        public const string MSG_TEAM_FAIL_NO_DATA = "Team data not provided";
        public const string MSG_TEAM_DOES_NOT_EXIST = "Team does not exist";
        public const string MSG_EDIT_TEAM_NOT_ALLOWED = "You are not allowed to edit this team";
        public const string MSG_DELETE_TEAM_NOT_ALLOWED = "You are not allowed to delete this team";
        public const string MSG_LEAVE_TEAM_NOT_ALLOWED = "You are not allowed to leave this team";
        public const string MSG_SEARCH_NAME_NOT_PROVIDED = "Search name or team id not provided";
        public const string MSG_MEMBER_IS_NOT_IN_TEAM = "The member is not in the team";
        public const string MSG_FAILED_TO_GET_NOTIFICATION_DETAILS = "Failed to fetch notification details";
        public const string MSG_FAILED_TO_JOIN_DECLINE_TEAM = "Invalid user or team id provided.";
        public const string MSG_FAILED_TO_TEAM_OWNER = "Failed to fetch team coach";
        public const string MSG_DELETE_NOTIFICATION_FAILED = "Notification data not provided";
        public const string MSG_SET_DOES_NOT_EXIST = "Set does not exist";
        public const string MSG_INVALID_TEAM_TYPE = "Invalid team type value provided";
        public const string MSG_TEMPLATE_ADD_FAIL_NO_DATA = "Template data not provided";
        public const string MSG_INVALID_DATE_FORMAT = "Invalid date format provided";
        public const string MSG_WOKOUT_ID_NOT_PROVIDED = "Workout id not provided";
        public const string MSG_TEAM_ID_NOT_PROVIDED = "Team id not provided";
        public const string MSG_MEMBER_IDS_NOT_PROVIDED = "Member ids not provided";
        public const string MSG_WORKOUT_ASSIGNED = "Workout assigned to members";
        public const string MSG_ASSIGNED_WORKOUT_ID_NOT_PROVIDED = "Assigned workout id not provided";
        public const string MSG_NO_TEAMS = "You don't have any teams";
        public const string MSG_NO_TEAM_MEMBERS = "You don't have any members in your teams";
        public const string MSG_WORKOUT_ASSIGNMENTS = "You don't have any assigned workouts with the selected filters";
        public const string MSG_CHECK_EMAIL = "Verification code has been sent to the email";
        public const string MSG_INVALID_CODE = "Invalid code";
        public const string MSG_PASSWORD_RESET_SUCCESS = "Your password has been updated succesfully";
        public const string MSG_UNEXPECTED_ERROR_WHILE_SENDING_EMAIL = "Unexpected error occurred while sending code to email. Please try again";
        public const string MSG_ACC_NOT_VERIFILED = "Please verify your email";
        public const string MSG_ACC_VERIFICATIO_FAILED = "Email verification failed. Please try again";
        public const string MSG_ACC_VERIFILED = "Email verification successful";
        public const string MSG_TRAINING_DATA_ADD_FAIL_NO_DATA = "Training plan data not provided";
        public const string MSG_TRAINING_PROGRAM_NOT_FOUND = "Training plan not found";
        public const string MSG_TRAINING_DAY_ADD_FAIL_NO_DATA = "Training day data not provided";
        public const string MSG_TRAINING_DAY_NOT_FOUND = "Training day not found";
        public const string MSG_TRAINING_PROGRAM_ID_NOT_PROVIDED = "Training plan id not provided";



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
            public const string WorkoutAssigned = "You have new workout assignment";
            public const string WorkoutAssignmentDeclined = "{0} declined your workout assignment";
        }

        /// <summary>
        ///     Class containing the notification texts to display when dealin with notifications
        ///     (e.g accept team invitatoin)
        /// </summary>
        public static class NotificationText
        {
            public const string AskTeamInviteAccept = "{0} invited you to join the team on {1}.\nDo you accept?";
            public const string AskTeamInviteAcceptNoSender = "You have received invitation to join the team on {0}.\nDo you accept?";
            public const string WorkoutAssignmentFinished = "{0} completed your workout assignment";
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
            public const string SETS_MUST_BE_POSITIVE = "Sets must be positive number";
            public const string REPS_MUST_BE_POSITIVE = "Reps must be positive number";
            public const string WEIGHT_MUST_BE_POSITIVE = "Weight must be positive number";
        }

        /// <summary>
        ///    Request end point values
        /// </summary>
        public static class RequestEndPoints
        {
            public const string API = "/api";
            public const string FAVICON = "/favicon.ico";
            public const string USERS = $"{API}/users";
            public const string EXERCISES = $"{API}/exercises";
            public const string USER_PROFILES = $"{API}/user-profiles";
            public const string WORKOUT_TEMPLATES = $"{API}/workout-templates";
            public const string TEAMS = $"{API}/teams";
            public const string NOTIFICATIONS = $"{API}/notifications";
            public const string WORKOUTS = $"{API}/workouts";
            public const string MUSCLE_GROUPS = $"{API}/muscle-groups";
            public const string SYSTEM_LOGS = $"{API}/system-logs";
            public const string TRAINING_PLANS = $"{API}/training-plans";

            public const string REGISTER = $"{USERS}/register";
            public const string LOGIN = $"{USERS}/login";
            public const string LOGOUT = $"{USERS}/logout";
            public const string CHANGE_PASSWORD = $"{USERS}/change-password";
            public const string RESET_PASSWORD = $"{USERS}/reset-password";
            public const string VALIDATE_TOKEN = $"{USERS}/validate-token";
            public const string GOOGLE_SIGN_IN = $"{USERS}/google-sign-in";
            public const string SEND_CODE = $"{USERS}/send-code";
            public const string VERIFY_CODE = $"{USERS}/verify-code";

            public const string TO_WORKOUT = $"{EXERCISES}/to-workout";
            public const string EXERCISE_FROM_WORKOUT = $"{EXERCISES}/exercise-from-workout";
            public const string COMPLETE_SET = $"{EXERCISES}/complete-set";
            public const string EXERCISES_FOR_MG = $"{EXERCISES}/by-mg-id";
            public const string MG_EXERCISE = $"{EXERCISES}/mg-exercise";

            public const string DEFAULT_VALUES = $"{USER_PROFILES}/default-values";

            public const string LEAVE_TEAM = $"{TEAMS}/leave";
            public const string INVITE_MEMBER = $"{TEAMS}/invite-member";
            public const string REMOVE_MEMBER = $"{TEAMS}/remove-member";
            public const string ACCEPT_TEAM_INVITE = $"{TEAMS}/accept-invite";
            public const string DECLINE_TEAM_INVITE = $"{TEAMS}/decline-invite";
            public const string MY_TEAMS = $"{TEAMS}/my-teams";
            public const string MY_TEAMS_WITH_MEMBERS = $"{TEAMS}/my-teams-with-members";
            public const string USERS_TO_INVITE = $"{TEAMS}/users-to-invite";
            public const string MY_TEAM_MEMBERS= $"{TEAMS}/my-team-members";
            public const string JOINED_TEAM_MEMBERS = $"{TEAMS}/joined-team-members";
            public const string ASSIGN_WORKOUT = $"{TEAMS}/assign-workout";
            public const string ASSIGNED_WORKOUTS = $"{TEAMS}/assigned-workouts";
            public const string ASSIGNED_WORKOUT = $"{TEAMS}/assigned-workout";

            public const string JOIN_TEAM_NOTIFICATION_DETAILS = $"{NOTIFICATIONS}/join-team-notification-details";
            public const string REFRESH_NOTIFICATIONS = $"{NOTIFICATIONS}/refresh-notifications";

            public const string GET_WORKOUT_TEMPLATE = $"{WORKOUT_TEMPLATES}/get-template-by-assigned-workout";

            public const string FINISH_WORKOUT = $"{WORKOUTS}/finish";

            public const string TRAINING_DAYS = $"{TRAINING_PLANS}/days";
            public const string TRAINING_PLANS_ASSIGN = $"{TRAINING_PLANS}/assign";
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
            DECLINED_TEAM_INVITATION,
            WORKOUT_ASSIGNED,
            WORKOUT_ASSIGNMENT_COMPLETED,
            WORKOUT_ASSIGNMENT_DECLINED,
        }

        /// <summary>
        ///     Enum with team types when fetching teams
        /// </summary>
        public enum ViewTeamAs
        {
            COACH,
            MEMBER
        }

        /// <summary>
        ///     User -> Assigned workout state - whether the user has not started / started / completed the assigned workout
        /// </summary>
        public enum AssignedWorkoutState
        {
            ASSIGNED,
            STARTED,
            COMPLETED
        }
    }
}
