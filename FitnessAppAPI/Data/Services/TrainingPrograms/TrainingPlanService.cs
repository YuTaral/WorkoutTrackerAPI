using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Models;
using FitnessAppAPI.Data.Services.Notifications;
using FitnessAppAPI.Data.Services.Teams;
using FitnessAppAPI.Data.Services.Teams.Models;
using FitnessAppAPI.Data.Services.TrainingPrograms.Models;
using FitnessAppAPI.Data.Services.Workouts;
using FitnessAppAPI.Data.Services.Workouts.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net;
using static FitnessAppAPI.Common.Constants;

namespace FitnessAppAPI.Data.Services.TrainingPrograms
{
    public class TrainingPlanService(FitnessAppAPIContext DB, INotificationService nService, ITeamService tService, IWorkoutService wService) : BaseService(DB), ITrainingPlanService
    {

        /// <summary>
        ///     Notification service instance
        /// </summary>
        private readonly INotificationService notificationService = nService;

        /// <summary>
        ///     Team service instance
        /// </summary>
        private readonly ITeamService teamService = tService;

        /// <summary>
        ///     Workout service instance
        /// </summary>
        private readonly IWorkoutService workoutService = wService;

        public async Task<ServiceActionResult<TrainingPlanModel>> AddTrainingPlan(Dictionary<string, string> requestData, string userId)
        {
            // Validate training program data
            var validationResult = ValidateTrainingProgram(requestData);

            if (!validationResult.IsSuccess() || validationResult.Data.Count == 0)
            {
                return validationResult;
            }

            // Validation passed, create the training program
            var trainingProgramData = validationResult.Data[0];

            var trainingProgram = new TrainingPlan
            {
                UserId = userId,
                Name = trainingProgramData.Name,
                Description = trainingProgramData.Description,
            };

            await DBAccess.TrainingPlans.AddAsync(trainingProgram);
            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult<TrainingPlanModel>(HttpStatusCode.Created, MSG_SUCCESS,
                [await ModelMapper.MapToTrainingPlanModel(trainingProgram, null, null, DBAccess)]);
        }

        public async Task<ServiceActionResult<TrainingPlanModel>> DeleteTrainingPlan(long trainingProgramId, string userId)
        {
            // Check if the neccessary data is provided
            if (trainingProgramId <= 0)
            {
                return new ServiceActionResult<TrainingPlanModel>(HttpStatusCode.BadRequest, MSG_OBJECT_ID_NOT_PROVIDED);
            }

            var trainingProgram = await CheckTrainingPlanExists(trainingProgramId, userId);
            if (trainingProgram == null)
            {
                return new ServiceActionResult<TrainingPlanModel>(HttpStatusCode.NotFound, MSG_TRAINING_PROGRAM_NOT_FOUND);
            }

            DBAccess.TrainingPlans.Remove(trainingProgram);
            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult<TrainingPlanModel>(HttpStatusCode.OK);
        }

        public async Task<ServiceActionResult<TrainingPlanModel>> GetTrainingPlans(string userId)
        {
            // Start the query
            var trainingPrograms = await DBAccess.TrainingPlans.Where(t => t.UserId == userId)
                                               .OrderByDescending(t => t.Id)
                                               .ToListAsync();

            // Create the list asynchonously
            var returnData = new List<TrainingPlanModel>();
            foreach (var tp in trainingPrograms)
            {
                var trainingProgramModel = await ModelMapper.MapToTrainingPlanModel(tp, null, null, DBAccess);
                returnData.Add(trainingProgramModel);
            }

            return new ServiceActionResult<TrainingPlanModel>(HttpStatusCode.OK, MSG_SUCCESS, returnData);
        }

        public async Task<ServiceActionResult<TrainingPlanModel>> UpdateTrainingPlan(Dictionary<string, string> requestData, string userId)
        {
            // Validate training program data
            var validationResult = ValidateTrainingProgram(requestData);

            if (!validationResult.IsSuccess() || validationResult.Data.Count == 0)
            {
                return validationResult;
            }

            // Validation passed, update the training program
            var trainingProgramData = validationResult.Data[0];

            var record = await CheckTrainingPlanExists(trainingProgramData.Id, userId);
            if (record == null)
            {
                return new ServiceActionResult<TrainingPlanModel>(HttpStatusCode.NotFound, MSG_TRAINING_PROGRAM_NOT_FOUND);
            }

            record.Name = trainingProgramData.Name;
            record.Description = trainingProgramData.Description;

            DBAccess.Entry(record).State = EntityState.Modified;
            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult<TrainingPlanModel>(HttpStatusCode.OK, MSG_SUCCESS, 
                [await ModelMapper.MapToTrainingPlanModel(record, null, null, DBAccess)]);
        }

        public async Task<ServiceActionResult<TrainingPlanModel>> AddTrainingDayToPlan(Dictionary<string, string> requestData, string userId)
        {
            // Validate training program data
            var validationResult = ValidateTrainingDay(requestData);

            if (!validationResult.IsSuccess() || validationResult.Data.Count == 0)
            {
                return new ServiceActionResult<TrainingPlanModel>(validationResult.Code, validationResult.Message, []);
            }

            // Validation passed, create the training day
            var trainingDayData = validationResult.Data[0];

            var trainingDay = new TrainingDay
            {
                TrainingPlanId = trainingDayData.TrainingPlanId,
            };

            await DBAccess.TrainingDays.AddAsync(trainingDay);
            await DBAccess.SaveChangesAsync();

            // Add records for all templates
            if (trainingDayData.Workouts.Count > 0)
            {
                foreach (WorkoutModel w in trainingDayData.Workouts)
                {
                    var recrod = new WorkoutToTrainingDay
                    {
                        TrainingDayId = trainingDay.Id,
                        WorkoutId = w.Id,
                    };

                    await DBAccess.WorkoutToTrainingDays.AddAsync(recrod);
                }

                await DBAccess.SaveChangesAsync();
            }

            var trainingProgram = await CheckTrainingPlanExists(trainingDay.TrainingPlanId, userId);
            return new ServiceActionResult<TrainingPlanModel>(HttpStatusCode.Created, MSG_SUCCESS, 
                [await ModelMapper.MapToTrainingPlanModel(trainingProgram, null, null, DBAccess)]);
        }

        public async Task<ServiceActionResult<TrainingPlanModel>> UpdateTrainingDayToPlan(Dictionary<string, string> requestData, string userId)
        {
            // Validate training program data
            var validationResult = ValidateTrainingDay(requestData);

            if (!validationResult.IsSuccess() || validationResult.Data.Count == 0)
            {
                return new ServiceActionResult<TrainingPlanModel>(validationResult.Code, validationResult.Message, []);
            }

            // Validation passed, check whether the training day exists
            var trainingDayData = validationResult.Data[0];
            var record = await CheckTrainingDayExists(trainingDayData.Id);
            if (record == null)
            {
                return new ServiceActionResult<TrainingPlanModel>(HttpStatusCode.NotFound, MSG_TRAINING_DAY_NOT_FOUND);
            }

            // Delete all workouts linked to the training day
            var existingWorkouts = DBAccess.WorkoutToTrainingDays.Where(wt => wt.TrainingDayId == record.Id);
            DBAccess.WorkoutToTrainingDays.RemoveRange(existingWorkouts);

            // Insert all new workouts linked to the training day
            if (trainingDayData.Workouts.Count > 0)
            {
                foreach (WorkoutModel w in trainingDayData.Workouts)
                {
                    var recrod = new WorkoutToTrainingDay
                    {
                        TrainingDayId = record.Id,
                        WorkoutId = w.Id,
                    };
                    await DBAccess.WorkoutToTrainingDays.AddAsync(recrod);
                }
            }

            await DBAccess.SaveChangesAsync();


            var trainingProgram = await CheckTrainingPlanExists(trainingDayData.TrainingPlanId, userId);
            return new ServiceActionResult<TrainingPlanModel>(HttpStatusCode.Created, MSG_SUCCESS, 
                [await ModelMapper.MapToTrainingPlanModel(trainingProgram, null, null, DBAccess)]);

        }
        public async Task<ServiceActionResult<TrainingPlanModel>> DeleteTrainingDay(long trainingDayId, string userId)
        {
            // Check if the neccessary data is provided
            if (trainingDayId <= 0)
            {
                return new ServiceActionResult<TrainingPlanModel>(HttpStatusCode.BadRequest, MSG_OBJECT_ID_NOT_PROVIDED);
            }

            var trainingDay = await CheckTrainingDayExists(trainingDayId);
            if (trainingDay == null)
            {
                return new ServiceActionResult<TrainingPlanModel>(HttpStatusCode.NotFound, MSG_TRAINING_DAY_NOT_FOUND);
            }

            DBAccess.TrainingDays.Remove(trainingDay);
            await DBAccess.SaveChangesAsync();

            var trainingProgram = await CheckTrainingPlanExists(trainingDay.TrainingPlanId, userId);
            return new ServiceActionResult<TrainingPlanModel>(HttpStatusCode.Created, MSG_SUCCESS,
                [await ModelMapper.MapToTrainingPlanModel(trainingProgram, null, null, DBAccess)]);
        }

        public async Task<ServiceActionResult<BaseModel>> DeleteWorkoutToTrainingDayRecs(long templateId)
        {
            var records = DBAccess.WorkoutToTrainingDays.Where(wt => wt.WorkoutId == templateId);
            DBAccess.WorkoutToTrainingDays.RemoveRange(records);
            await DBAccess.SaveChangesAsync();
            return new ServiceActionResult<BaseModel>(HttpStatusCode.OK);
        }

        public async Task<ServiceActionResult<BaseModel>> AssignTrainingPlan(Dictionary<string, string> requestData, string coachId)
        {
            // Check if the neccessary data is provided
            if (!requestData.TryGetValue("trainingPlanId", out string? trainingPlanIdString))
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, MSG_TRAINING_PROGRAM_ID_NOT_PROVIDED);
            }

            if (!long.TryParse(trainingPlanIdString, out long trainingPlanId))
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, MSG_TRAINING_PROGRAM_ID_NOT_PROVIDED);
            }

            if (!requestData.TryGetValue("memberIds", out string? memberIdsString))
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, MSG_MEMBER_IDS_NOT_PROVIDED);
            }

            if (!requestData.TryGetValue("startDate", out string? startDateString))
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, MSG_INVALID_DATE_FORMAT);
            }

            if (!DateTime.TryParse(startDateString, out DateTime startDate))
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, MSG_INVALID_DATE_FORMAT);
            }

            var trainingPlan = await CheckTrainingPlanExists(trainingPlanId, coachId);
            if (trainingPlan == null)
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.NotFound, MSG_TRAINING_PROGRAM_NOT_FOUND);
            }

            var trainingDays = await DBAccess.TrainingDays.Where(td => td.TrainingPlanId == trainingPlan.Id).ToListAsync();

            List<long> teamMemberIds = JsonConvert.DeserializeObject<List<long>>(memberIdsString!)!;

            // Go through each member and assign the training plan
            foreach (long teamMemberId in teamMemberIds)
            {
                var assignedTrainingPlan = new AssignedTrainingPlan
                {
                    ScheduledStartDate = startDate,
                    StartDate = null,
                    TeamMemberId = teamMemberId,
                    TrainingPlanId = trainingPlanId,
                };

                await DBAccess.AssignedTrainingPlans.AddAsync(assignedTrainingPlan);
                await DBAccess.SaveChangesAsync();

                var teamMemberRec = await DBAccess.TeamMembers.Where(tm => tm.Id == teamMemberId).FirstOrDefaultAsync();
                if (teamMemberRec == null)
                {
                    // MUST NOT happen, but just in case
                    continue;
                }

                // Send notification to the team member
                await notificationService.AddTrainingPlanAssignedNotification(coachId, teamMemberRec.TeamId, teamMemberRec.UserId, assignedTrainingPlan.Id);
            }

            return new ServiceActionResult<BaseModel>(HttpStatusCode.OK, MSG_TRAINING_PLAN_ASSIGNED);
        }

        public async Task<ServiceActionResult<TrainingPlanModel>> GetTrainingPlan(long assignedTrainingPlanId)
        {
            if (assignedTrainingPlanId <= 0)
            {
                return new ServiceActionResult<TrainingPlanModel>(HttpStatusCode.BadRequest, MSG_OBJECT_ID_NOT_PROVIDED);
            }

            var assignedPlanRecord = await DBAccess.AssignedTrainingPlans.Where(p => p.Id == assignedTrainingPlanId).FirstOrDefaultAsync();
            if (assignedPlanRecord == null)
            {
                return new ServiceActionResult<TrainingPlanModel>(HttpStatusCode.BadRequest, MSG_OBJECT_ID_NOT_PROVIDED);
            }

            var trainingPlanRecord = await CheckTrainingPlanExists(assignedPlanRecord.TrainingPlanId);
            if (trainingPlanRecord == null)
            {
                return new ServiceActionResult<TrainingPlanModel>(HttpStatusCode.NotFound, MSG_TRAINING_PROGRAM_NOT_FOUND);
            }

            return new ServiceActionResult<TrainingPlanModel>(HttpStatusCode.OK, MSG_SUCCESS, 
                [await ModelMapper.MapToTrainingPlanModel(trainingPlanRecord, assignedPlanRecord.ScheduledStartDate, assignedTrainingPlanId, DBAccess)]);
        }

        public async Task<ServiceActionResult<TrainingPlanModel>> StartTrainingPlan(Dictionary<string, string> requestData, string userId)
        {
            // Validate training plan is provided
            var validationResult = ValidateTrainingProgram(requestData);
            if (!validationResult.IsSuccess() || validationResult.Data.Count == 0)
            {
                return validationResult;
            }

            var trainingPlan = validationResult.Data[0];

            // Validate member record and assigned training plan exist for the user
            var teamMemberRecord = await DBAccess.TeamMembers.Where(t => t.UserId == userId).FirstOrDefaultAsync();
            if (teamMemberRecord == null)
            {
                return new ServiceActionResult<TrainingPlanModel>(HttpStatusCode.BadRequest, MSG_MEMBER_NOT_FOUND);
            }

            var assignedTrainingPlanRecord = await DBAccess.AssignedTrainingPlans
                                        .Where(p => p.Id == trainingPlan.AssignedTrainingPlanId)
                                        .FirstOrDefaultAsync();

            if (assignedTrainingPlanRecord == null) {
                return new ServiceActionResult<TrainingPlanModel>(HttpStatusCode.BadRequest, MSG_ASSIGNED_TRAINING_PLAN_NOT_FOUND);
            }

            var teamRecord = await DBAccess.Teams.Where(t => t.Id == teamMemberRecord.TeamId).FirstOrDefaultAsync();
            if (teamRecord == null)
            {
                return new ServiceActionResult<TrainingPlanModel>(HttpStatusCode.BadRequest, MSG_TEAM_DOES_NOT_EXIST);
            }

            // Mark the training plan as started and create all workouts as assigned workout records
            assignedTrainingPlanRecord.StartDate = DateTime.UtcNow;
            DBAccess.Entry(assignedTrainingPlanRecord).State = EntityState.Modified;

            var scheduledForDate = trainingPlan.ScheduledStartDate ?? throw new InvalidOperationException(MSG_INVALID_DATE_FORMAT);

            foreach (var trainingDay in trainingPlan.TrainingDays)
            {
                foreach (var workout in trainingDay.Workouts)
                {
                    // Assign the workout
                    var result = await teamService.AssingWorkoutToMember(teamMemberRecord.Id, workout.Id, teamRecord.UserId, scheduledForDate, false);
                   
                    if (result.IsSuccess() && result.Data.Count > 0)
                    {
                        // Auto create workout
                        await workoutService.AddWorkout(workout, scheduledForDate, result.Data[0], teamMemberRecord.UserId);
                    }
                }

                // Add one day
                scheduledForDate = scheduledForDate.AddDays(1);
            }

            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult<TrainingPlanModel>(HttpStatusCode.OK);
        }

        /// <summary>
        ///    Perform validations whether the provided workout training data is valid
        ///    Return training data model if valid, otherwise Bad Request
        /// </summary>
        /// <param name="requestData">
        ///     The request data - must contain serialized training program data
        /// </param>
        private static ServiceActionResult<TrainingPlanModel> ValidateTrainingProgram(Dictionary<string, string> requestData)
        {
            // Check if the neccessary data is provided
            if (!requestData.TryGetValue("trainingPlan", out string? serializedTrainingProgram))
            {
                return new ServiceActionResult<TrainingPlanModel>(HttpStatusCode.BadRequest, MSG_TRAINING_DATA_ADD_FAIL_NO_DATA);
            }

            TrainingPlanModel? trainingProgramData = JsonConvert.DeserializeObject<TrainingPlanModel>(serializedTrainingProgram);
            if (trainingProgramData == null)
            {
                return new ServiceActionResult<TrainingPlanModel>(HttpStatusCode.BadRequest, string.Format(MSG_WORKOUT_FAILED_TO_DESERIALIZE_OBJ, "TrainingProgramModel"));
            }

            string validationErrors = Utils.ValidateModel(trainingProgramData);
            if (!string.IsNullOrEmpty(validationErrors))
            {
                return new ServiceActionResult<TrainingPlanModel>(HttpStatusCode.BadRequest, validationErrors);
            }

            return new ServiceActionResult<TrainingPlanModel>(HttpStatusCode.OK, validationErrors, [trainingProgramData]);
        }

        /// <summary>
        ///    Perform validations whether the provided training day data is valid
        ///    Return training data model if valid, otherwise Bad Request
        /// </summary>
        /// <param name="requestData">
        ///     The request data - must contain serialized training day data
        /// </param>
        private static ServiceActionResult<TrainingDayModel> ValidateTrainingDay(Dictionary<string, string> requestData)
        {
            // Check if the neccessary data is provided
            if (!requestData.TryGetValue("trainingDay", out string? trainingDaySerialized))
            {
                return new ServiceActionResult<TrainingDayModel>(HttpStatusCode.BadRequest, MSG_TRAINING_DAY_ADD_FAIL_NO_DATA);
            }

            TrainingDayModel? trainingDayData = JsonConvert.DeserializeObject<TrainingDayModel>(trainingDaySerialized);
            if (trainingDayData == null)
            {
                return new ServiceActionResult<TrainingDayModel>(HttpStatusCode.BadRequest, string.Format(MSG_WORKOUT_FAILED_TO_DESERIALIZE_OBJ, "TrainingDayModel"));
            }

            string validationErrors = Utils.ValidateModel(trainingDayData);
            if (!string.IsNullOrEmpty(validationErrors))
            {
                return new ServiceActionResult<TrainingDayModel>(HttpStatusCode.BadRequest, validationErrors);
            }

            return new ServiceActionResult<TrainingDayModel>(HttpStatusCode.OK, validationErrors, [trainingDayData]);
        }

        /// <summary>
        ///     Perform a check whether the training exists, returns training program object if it exists,
        ///     null otherwise
        /// </summary>
        /// <param name="id">
        ///     The workout id
        /// </param>
        /// <param name="userId">
        ///     The userId owner of the training program
        /// </param>
        private async Task<TrainingPlan?> CheckTrainingPlanExists(long id, string userId)
        {
            return await DBAccess.TrainingPlans.Where(t => t.Id == id && t.UserId == userId).FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Perform a check whether the training exists, returns training program object if it exists,
        ///     null otherwise
        /// </summary>
        /// <param name="id">
        ///     The workout id
        /// </param>
        private async Task<TrainingPlan?> CheckTrainingPlanExists(long id)
        {
            return await DBAccess.TrainingPlans.Where(t => t.Id == id).FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Perform a check whether the training day exists, returns training day object if it exists,
        ///     null otherwise
        /// </summary>
        /// <param name="id">
        ///     The workout id
        /// </param>
        private async Task<TrainingDay?> CheckTrainingDayExists(long id)
        {
            return await DBAccess.TrainingDays.Where(t => t.Id == id).FirstOrDefaultAsync();
        }
    }
}
