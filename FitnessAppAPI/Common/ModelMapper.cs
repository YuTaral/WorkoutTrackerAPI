using FitnessAppAPI.Data.Models;
using FitnessAppAPI.Data.Services.Exercises.Models;
using FitnessAppAPI.Data.Services.MuscleGroups.Models;
using FitnessAppAPI.Data;
using FitnessAppAPI.Data.Services.Workouts.Models;
using Microsoft.EntityFrameworkCore;
using FitnessAppAPI.Data.Services.Teams.Models;
using FitnessAppAPI.Data.Services.Notifications.Models;
using static FitnessAppAPI.Common.Constants;
using FitnessAppAPI.Data.Services.UserProfiles.Models;
using FitnessAppAPI.Data.Services.Users.Models;

namespace FitnessAppAPI.Common
{
    /// <summary>
    ///     Class to implement the models mappings
    /// </summary>
    public static class ModelMapper
    {
        /// <summary>
        ///     Map the Workout to WorkoutModel
        /// </summary>
        public static async Task<WorkoutModel> MapToWorkoutModel(Workout workout, FitnessAppAPIContext DBAccess)
        {

            if (workout == null)
            {
                return GetEmptyWorkoutModel();
            }

            if (workout.Template == "Y")
            {
                return new WorkoutModel
                {
                    Id = workout.Id,
                    Name = workout.Name,
                    Template = workout.Template == "Y",
                    Exercises = await DBAccess.Exercises.Where(e => e.WorkoutId == workout.Id)
                                                 .Select(e => MapToExerciseModel(e, DBAccess))
                                                 .ToListAsync(),
                    DurationSeconds = workout.DurationSeconds,
                    Notes = workout.Notes,
                    WeightUnit = workout.WeightUnit
                };
            } 
            else
            {
                return new WorkoutModel
                {
                    Id = workout.Id,
                    Name = workout.Name,
                    StartDateTime = DateTime.SpecifyKind(workout.StartDateTime.GetValueOrDefault(), DateTimeKind.Utc),
                    FinishDateTime = workout.FinishDateTime.HasValue ? DateTime.SpecifyKind(workout.FinishDateTime.Value, DateTimeKind.Utc) : null,
                    Template = workout.Template == "Y",
                    Exercises = await DBAccess.Exercises.Where(e => e.WorkoutId == workout.Id)
                                                                 .Select(e => MapToExerciseModel(e, DBAccess))
                                                                 .ToListAsync(),
                    DurationSeconds = workout.DurationSeconds,
                    Notes = workout.Notes,
                    WeightUnit = workout.WeightUnit
                };
            }
        }

        /// <summary>
        ///     Map the Exercise to ExerciseModel
        /// </summary>
        public static ExerciseModel MapToExerciseModel(Exercise exercise, FitnessAppAPIContext DBAccess)
        {
            if (exercise == null)
            {
                return GetEmptyExerciseModel();
            }

            // Get the MuscleGroups results and convert them to Enumerable, to avoid errors that the Entity Framerwork
            // cannot translate the method into SQL when MapToMuscleGroupModel() is called
            var muscleGroupEnum = DBAccess.MuscleGroups.Where(mg => mg.Id == exercise.MuscleGroupId).AsEnumerable();
            var muscleGroup = muscleGroupEnum.Select(mg => MapToMuscleGroupModel(mg)).FirstOrDefault();

            muscleGroup ??= GetEmptyMuscleGroupModel();

            // Get the Sets results and convert them to Enumerable, to avoid errors that the Entity Framerwork
            // cannot translate the method into SQL when MapToSetModel() is called
            var setsEnumerable = DBAccess.Sets.Where(s => s.ExerciseId == exercise.Id).AsEnumerable();
            var sets = setsEnumerable.Select(s => MapToSetModel(s)).ToList();

            return new ExerciseModel
            {
                Id = exercise.Id,
                Name = exercise.Name,
                MuscleGroup = muscleGroup,
                Sets = sets,
                MGExerciseId = exercise.MGExerciseId,
                Notes = exercise.Notes
            };
        }

        /// <summary>
        ///     Map the MGExercises to MGExerciseModel
        /// </summary>
        public static MGExerciseModel MapToMGExerciseModel(MGExercise? MGExercise)
        {
            if (MGExercise == null)
            {
                return GetEmptyMGExerciseModel();
            }

            return new MGExerciseModel
            {
                Id = MGExercise.Id,
                Name = MGExercise.Name,
                Description = MGExercise.Description,
                MuscleGroupId = MGExercise.MuscleGroupId
            };
        }

        /// <summary>
        ///     Map the MuscleGroup to MuscleGroupModel
        /// </summary>
        public static MuscleGroupModel MapToMuscleGroupModel(MuscleGroup muscleGroup)
        {
            if (muscleGroup == null)
            {
                return GetEmptyMuscleGroupModel();
            }

            return new MuscleGroupModel
            {
                Id = muscleGroup.Id,
                Name = muscleGroup.Name,
                ImageName = muscleGroup.ImageName
            };
        }

        /// <summary>
        ///     Map the MuscleGroup to MuscleGroupModel
        /// </summary>
        public static SetModel MapToSetModel(Set set)
        {
            if (set == null)
            {
                return GetEmptySetModel();
            }

            return new SetModel
            {
                Id = set.Id,
                Reps = set.Reps,
                Weight = set.Weight,
                Rest = set.Rest,
                Completed = set.Completed
            };
        }

        /// <summary>
        ///     Map the provided to to UserModel
        /// </summary>
        public static UserModel MapToUserModel(User user, UserDefaultValue? defaultValues, UserProfile? profile)
        {
            var defaultValuesModel = GetEmptyUserDefaultValuesModel();
            var fullName = "";
            var profileImage = "";

            if (defaultValues != null) {
                defaultValuesModel.Id = defaultValues.Id;
                defaultValuesModel.Sets = defaultValues.Sets;
                defaultValuesModel.Reps = defaultValues.Reps;
                defaultValuesModel.Rest = defaultValues.Rest;
                defaultValuesModel.Completed = defaultValues.Completed;
                defaultValuesModel.Weight = defaultValues.Weight;
                defaultValuesModel.WeightUnit = defaultValues.WeightUnit;
                defaultValuesModel.MGExerciseId = defaultValues.MGExeciseId;
            }

            if (profile != null)
            {
                fullName = profile.FullName;
                profileImage = Utils.EncodeByteArrayToBase64Image(profile.ProfileImage);
            }

            return new UserModel
            {
                Id = user.Id,
                Email = user.Email,
                FullName = fullName,
                ProfileImage = profileImage,
                DefaultValues = defaultValuesModel
            };
        }

        /// <summary>
        ///     Map the provided to to UserModel
        /// </summary>
        public static UserModel MapToUserModel(string email)
        { 
            return new UserModel
            {
                Id = "",
                Email = email,
                FullName = "",
                ProfileImage = "",
                DefaultValues = GetEmptyUserDefaultValuesModel()
            };
        }

        /// <summary>
        ///     Map the UserDefaultValue to UserDefaultValuesModel
        /// </summary>
        public static UserDefaultValuesModel MapToUserDefaultValuesModel(UserDefaultValue? defaultValues, FitnessAppAPIContext DBAccess)
        {
            if (defaultValues == null)
            {
                return GetEmptyUserDefaultValuesModel();
            }

            return new UserDefaultValuesModel
            {
                Id = defaultValues.Id,
                Sets = defaultValues.Sets,
                Reps = defaultValues.Reps,
                Weight = defaultValues.Weight,
                Rest = defaultValues.Rest,
                Completed = defaultValues.Completed,
                WeightUnit = defaultValues.WeightUnit,
                MGExerciseId = defaultValues.MGExeciseId
            };
        }

        /// <summary>
        ///     Map the user profile to to TeamCoachModel
        /// </summary>
        public static TeamCoachModel MapToTeamCoachModel(UserProfile profile)
        { 
            return new TeamCoachModel { 
                Id = 0,
                FullName = profile.FullName,
                Image = Utils.EncodeByteArrayToBase64Image(profile.ProfileImage)
            };
        }

        /// <summary>
        ///    Return empty MuscleGroupModel
        /// </summary>
        public static ExerciseModel GetEmptyExerciseModel()
        {
            return new ExerciseModel
            {
                Id = 0,
                Name = "",
                MuscleGroup = GetEmptyMuscleGroupModel(),
                Sets = [],
                MGExerciseId = 0,
                Notes = ""
            };
        }

        /// <summary>
        ///    Return empty UserModel
        /// </summary>
        public static UserModel GetEmptyUserModel()
        {
            return new UserModel
            {
                Id = "",
                Email = "",
                FullName = "",
                ProfileImage = "",
                DefaultValues = GetEmptyUserDefaultValuesModel(),
            };
        }

        /// <summary>
        ///     Map the Team to TeamModel
        /// </summary>
        public static TeamModel MapToTeamModel(Team team, string viewTeamAs)
        {
            if (team == null)
            {
                return GetEmptyTeamModel();
            }

            return new TeamModel
            {
                Id = team.Id,
                Image = Utils.EncodeByteArrayToBase64Image(team.Image),
                Name = team.Name,
                Description = team.Description,
                ViewTeamAs = viewTeamAs
            };
        }

        /// <summary>
        ///     Map the Team to TeamWithMembersModel
        /// </summary>
        public static TeamWithMembersModel MapToTeamWithMembersModel(Team team, string viewTeamAs, List<TeamMemberModel> memberModels)
        {
            if (team == null)
            {
                return GetEmptyTeamWithMembersModelModel();
            }

            return new TeamWithMembersModel
            {
                Id = team.Id,
                Image = Utils.EncodeByteArrayToBase64Image(team.Image),
                Name = team.Name,
                Description = team.Description,
                ViewTeamAs = viewTeamAs,
                Members = memberModels
            };
        }

        /// <summary>
        ///     Map the team member record to TeamMemberModel
        /// </summary>
        public static async Task<TeamMemberModel> MapToTeamMemberModel(TeamMember teamMember, FitnessAppAPIContext DBAccess)
        {
            var userProfile = await DBAccess.UserProfiles.Where(p => p.UserId == teamMember.UserId).FirstOrDefaultAsync();

            if (userProfile == null)
            {
                return GetEmptyTeamMemberModel();
            }

            return new TeamMemberModel {
                Id = teamMember.Id,
                UserId = teamMember.UserId,
                TeamId = teamMember.TeamId,
                FullName = userProfile.FullName,
                Image = Utils.EncodeByteArrayToBase64Image(userProfile.ProfileImage),
                TeamState = teamMember.State
            };
        }

        /// <summary>
        ///     Map the team notification record to NotificationModel
        /// </summary>
        public static async Task<NotificationModel> MapToNotificationModel(Notification notification, FitnessAppAPIContext DBAccess)
        {
            var img = "";

            if (notification.TeamId != null)
            {
                // Show user sender image in case of invited to team / joined in team notification
                var user = await DBAccess.UserProfiles.Where(p => p.UserId == notification.SenderUserId).FirstOrDefaultAsync();

                if (user != null)
                {
                    img = Utils.EncodeByteArrayToBase64Image(user.ProfileImage);
                }

            } 

            return new NotificationModel
            {
                Id = notification.Id,
                NotificationText = notification.NotificationText,
                DateTime = DateTime.SpecifyKind(notification.DateTime, DateTimeKind.Utc),
                IsActive = notification.IsActive,
                Type = notification.NotificationType,
                Image = img,
                TeamId = notification.TeamId,
                ClickDisabled = NotificationNotClickable(notification.IsActive, notification.NotificationType),
                AssignedWorkoutId = notification.AssignedWorkoutId,
            };
        }


        /// <summary>
        ///     Map the assigned workout to assigned workout model
        /// </summary>
        public static async Task<AssignedWorkoutModel> MapToAssignedWorkoutModel(AssignedWorkout assignedWorkout, FitnessAppAPIContext DBAccess)
        {
            var teamMemberRecord = await DBAccess.TeamMembers.Where(t => t.Id == assignedWorkout.TeamMemberId).FirstOrDefaultAsync();
            if (teamMemberRecord == null)
            {
                return GetEmptyAssignedWorkoutModell();
            }

            var team = await DBAccess.Teams.Where(t => t.Id == teamMemberRecord.TeamId).FirstOrDefaultAsync();
            if (team == null)
            {
                return GetEmptyAssignedWorkoutModell();
            }

            var user = await DBAccess.UserProfiles.Where(t => t.UserId == teamMemberRecord.UserId).FirstOrDefaultAsync();
            if (user == null)
            {
                return GetEmptyAssignedWorkoutModell();
            }

            var workout = await DBAccess.Workouts.Where(w => w.Id == assignedWorkout.StartedWorkoutId).FirstOrDefaultAsync();
            var workoutModel = GetEmptyWorkoutModel();
            Workout? template;

            if (workout == null)
            {
                // Workout not yet started, fetch the data from the template id
                template = await DBAccess.Workouts.Where(w => w.Id == assignedWorkout.TemplateId).FirstOrDefaultAsync();
                if (template != null)
                {
                    workoutModel = await MapToWorkoutModel(template, DBAccess);
                } 
                else
                {
                    return GetEmptyAssignedWorkoutModell();
                }
            } 
            else
            {
                workoutModel = await MapToWorkoutModel(workout, DBAccess);
            }

            // Set template to false so the workout is displayed as workout instead of as template on the client
            workoutModel.Template = false;

            return new AssignedWorkoutModel
            {
                Id = assignedWorkout.Id,
                WorkoutModel = workoutModel,
                TeamName = team.Name,
                TeamImage = Utils.EncodeByteArrayToBase64Image(team.Image),
                TeamId = team.Id,
                UserFullName = user.FullName,
                
                DateTimeAssigned = DateTime.SpecifyKind(assignedWorkout.DateAssigned, DateTimeKind.Utc)
            };
        }

        /// <summary>
        ///    Return empty TeamModel
        /// </summary>
        public static TeamModel GetEmptyTeamModel()
        {
            return new TeamModel
            {
                Id = 0,
                Image = "",
                Name = "",
                Description = "",
                ViewTeamAs = ""
            };
        }

        /// <summary>
        ///    Return empty TeamWithMembersModel
        /// </summary>
        private static TeamWithMembersModel GetEmptyTeamWithMembersModelModel()
        {
            return new TeamWithMembersModel
            {
                Id = 0,
                Image = "",
                Name = "",
                Description = "",
                ViewTeamAs = "",
                Members = []
            };
        }

        /// <summary>
        ///    Return empty MuscleGroupModel
        /// </summary>
        private static MGExerciseModel GetEmptyMGExerciseModel()
        {
            return new MGExerciseModel
            {
                Id = 0,
                Name = "",
                Description = "",
                MuscleGroupId = 0
            };
        }

        /// <summary>
        ///    Return empty MuscleGroupModel
        /// </summary>
        private static MuscleGroupModel GetEmptyMuscleGroupModel()
        {
            return new MuscleGroupModel
            {
                Id = 0,
                Name = "",
                ImageName = ""
            };
        }

        /// <summary>
        ///    Return empty SetModel
        /// </summary>
        private static SetModel GetEmptySetModel()
        {
            return new SetModel
            {
                Id = 0,
                Reps = 0,
                Weight = 0,
                Rest = 0,
                Completed = false
            };
        }

        /// <summary>
        ///    Return empty WorkoutModel
        /// </summary>
        private static WorkoutModel GetEmptyWorkoutModel()
        {
            return new WorkoutModel {
                Id = 0,
                Name = "",
                DurationSeconds = 0,
                StartDateTime = DateTime.UtcNow,
                Template = false,
                Exercises = { },
                Notes = "",
                WeightUnit = ""
            };
        }

        /// <summary>
        ///    Return empty WorkoutModel
        /// </summary>
        private static UserDefaultValuesModel GetEmptyUserDefaultValuesModel()
        {
            return new UserDefaultValuesModel
            {
                Id = 0,
                Sets = 0,
                Reps = 0,
                Weight = 0,
                Rest = 0,
                Completed = false,
                WeightUnit = "",
                MGExerciseId = 0,
            };
        }

        /// <summary>
        ///    Return empty GetEmptyTeamMemberModel
        /// </summary>
        private static TeamMemberModel GetEmptyTeamMemberModel()
        {
            return new TeamMemberModel
            {
                Id = 0,
                UserId = "",
                TeamId = 0,
                FullName = "",
                Image = "",
                TeamState = ""
            };
        }

        /// <summary>
        ///     Return true if the notification is not clickable on the client side, false otherwise
        /// </summary>
        /// <param name="isActive">
        ///     True if the notification is active, false otherwise
        /// </param>
        /// <param name="type">
        ///     The notification type - Constants.NotificationType
        /// </param>
        private static bool NotificationNotClickable(bool isActive, string type)
        {
            // Notification is not clickable (won't execute action on click on the clinet) when
            // it's inactive and is of type INVITED_TO_TEAM
            return (!isActive && type == Constants.NotificationType.INVITED_TO_TEAM.ToString());
        }

        /// <summary>
        ///    Return empty GetEmptyTeamMemberModel
        /// </summary>
        private static AssignedWorkoutModel GetEmptyAssignedWorkoutModell()
        {
            return new AssignedWorkoutModel
            {
                Id = 0,
                WorkoutModel = GetEmptyWorkoutModel(),
                TeamName = "",
                TeamImage = "",
                TeamId = 0L,
                UserFullName = "",
                DateTimeAssigned = DateTime.UtcNow,
            };
        }
    } 
}
