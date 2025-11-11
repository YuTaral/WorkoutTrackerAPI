using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Models;
using FitnessAppAPI.Data.Services.Exercises;
using FitnessAppAPI.Data.Services.Exercises.Models;
using FitnessAppAPI.Data.Services.Teams;
using FitnessAppAPI.Data.Services.TrainingPrograms;
using FitnessAppAPI.Data.Services.Workouts.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net;
using static FitnessAppAPI.Common.Constants;


namespace FitnessAppAPI.Data.Services.WorkoutTemplates
{
    /// <summary>
    ///     Workout Temaplte service class to implement IWorkoutTemplateService interface.
    /// </summary>
    public class WorkoutTemplateService(FitnessAppAPIContext DB, IExerciseService exService, 
                                        ITeamService tService, ITrainingPlanService tpService) : BaseService(DB), IWorkoutTemplateService
    {
        private readonly IExerciseService exerciseService = exService;
        private readonly ITeamService teamService = tService;
        private readonly ITrainingPlanService trainingProgramService = tpService;

        public async Task<ServiceActionResult<WorkoutModel>> AddWorkoutTemplate(Dictionary<string, string> requestData, string userId)
        {
            // Validate template
            var validationResult = ValidateTemplateData(requestData);

            if (!validationResult.IsSuccess() || validationResult.Data.Count == 0)
            {
                return validationResult;
            }

            // Validation passed, create the template
            var workoutData = validationResult.Data[0];

            // Create Workout record, with Template = "Y"
            var template = new Workout
            {
                Name = workoutData.Name,
                UserId = userId,
                StartDateTime = null,
                FinishDateTime = null,
                Template = "Y",
                DurationSeconds = null,
                Notes = workoutData.Notes,
                WeightUnit = workoutData.WeightUnit,
                AssignedFromWorkoutId = null
            };

            await DBAccess.Workouts.AddAsync(template);
            await DBAccess.SaveChangesAsync();

            // Add the Exercises and sets
            if (workoutData.Exercises != null)
            {

                foreach (ExerciseModel exerciseData in workoutData.Exercises)
                {
                    var result = await exerciseService.AddExerciseToWorkout(exerciseData, template.Id);

                    if (!result.IsSuccess())
                    {
                        return new ServiceActionResult<WorkoutModel>((HttpStatusCode)result.Code, result.Message);
                    }
                }
            }

            return new ServiceActionResult<WorkoutModel>(HttpStatusCode.Created);
        }

        public async Task<ServiceActionResult<WorkoutModel>> UpdateWorkoutTemplate(Dictionary<string, string> requestData, string userId)
        {
            // Validate template
            var validationResult = ValidateTemplateData(requestData);

            if (!validationResult.IsSuccess() || validationResult.Data.Count == 0)
            {
                return validationResult;
            }

            // Validation passed, update the template
            var workoutData = validationResult.Data[0];

            var template = await DBAccess.Workouts.Where(w => w.Id == workoutData.Id && w.Template == "Y").FirstOrDefaultAsync();

            if (template == null)
            {
                return new ServiceActionResult<WorkoutModel>(HttpStatusCode.NotFound, MSG_TEMPLATE_DOES_NOT_EXIST);
            }

            template.Name = workoutData.Name;
            template.Notes = workoutData.Notes;
            template.WeightUnit = workoutData.WeightUnit;

            DBAccess.Entry(template).State = EntityState.Modified;
            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult<WorkoutModel>(HttpStatusCode.OK);
        }

        public async Task<ServiceActionResult<WorkoutModel>> DeleteWorkoutTemplate(long templateId)
        {
            // Check if the neccessary data is provided
            if (templateId <= 0)
            {
                return new ServiceActionResult<WorkoutModel>(HttpStatusCode.BadRequest, MSG_EXERCISE_DELETE_FAIL_NO_ID);
            }

            var template = await DBAccess.Workouts.Where(w => w.Id == templateId && w.Template == "Y").FirstOrDefaultAsync();

            if (template == null)
            {
                return new ServiceActionResult<WorkoutModel>(HttpStatusCode.NotFound, MSG_TEMPLATE_DOES_NOT_EXIST);
            }

            // First delete assigned workouts and workout to training days records because of constraint errors
            await teamService.DeleteAssignedWorkouts(templateId);
            await trainingProgramService.DeleteWorkoutToTrainingDayRecs(templateId);

            DBAccess.Workouts.Remove(template);
            await DBAccess.SaveChangesAsync();
           
            return new ServiceActionResult<WorkoutModel>(HttpStatusCode.OK);
        }

        public async Task<ServiceActionResult<WorkoutModel>> GetWorkoutTemplates(string userId)
        {
            // Fetch the templats asynchonously
            var templates = await DBAccess.Workouts.Where(w => w.UserId == userId && w.Template == "Y")
                                            .OrderByDescending(w => w.Id)
                                            .ToListAsync();


            // Create the list asynchonously
            var templateModels = new List<WorkoutModel>();
            foreach (var template in templates)
            {
                var workoutModel = await ModelMapper.MapToWorkoutModel(template, DBAccess);
                templateModels.Add(workoutModel);
            }

            return new ServiceActionResult<WorkoutModel>(HttpStatusCode.OK, MSG_SUCCESS, templateModels);
        }

        public async Task<ServiceActionResult<WorkoutModel>> GetWorkoutTemplate(long assignedWorkoutId)
        {
            // Check if the neccessary data is provided
            if (assignedWorkoutId <= 0)
            {
                return new ServiceActionResult<WorkoutModel>(HttpStatusCode.BadRequest, MSG_ASSIGNED_WORKOUT_ID_NOT_PROVIDED);
            }

            var assignedWorkoutRecord = await DBAccess.AssignedWorkouts.Where(a => a.Id == assignedWorkoutId).FirstOrDefaultAsync();
            if (assignedWorkoutRecord == null)
            {
                return new ServiceActionResult<WorkoutModel>(HttpStatusCode.BadRequest, MSG_ASSIGNED_WORKOUT_ID_NOT_PROVIDED);
            }

            var template = await DBAccess.Workouts.Where(w => w.Id == assignedWorkoutRecord.TemplateId).FirstOrDefaultAsync();
            if (template == null)
            {
                return new ServiceActionResult<WorkoutModel>(HttpStatusCode.NotFound, MSG_WORKOUT_DOES_NOT_EXIST);
            }

            return new ServiceActionResult<WorkoutModel>(HttpStatusCode.OK, MSG_SUCCESS, [await ModelMapper.MapToWorkoutModel(template, DBAccess)]);
        }

        /// <summary>
        ///    Perform validations whether the provided template data is valid
        ///    Return workout model if valid, otherwise Bad Request
        /// </summary>
        /// <param name="requestData">
        ///     The request data - must contain serialized template
        /// </param>
        private static ServiceActionResult<WorkoutModel> ValidateTemplateData(Dictionary<string, string> requestData)
        {
            // Check if the neccessary data is provided
            if (!requestData.TryGetValue("workout", out string? serializedWorkout))
            {
                return new ServiceActionResult<WorkoutModel>(HttpStatusCode.BadRequest, MSG_TEMPLATE_ADD_FAIL_NO_DATA);
            }

            WorkoutModel? workoutData = JsonConvert.DeserializeObject<WorkoutModel>(serializedWorkout);
            if (workoutData == null)
            {
                return new ServiceActionResult<WorkoutModel>(HttpStatusCode.BadRequest, string.Format(MSG_WORKOUT_FAILED_TO_DESERIALIZE_OBJ, "WorkoutModel"));
            }

            string validationErrors = Utils.ValidateModel(workoutData);
            if (!string.IsNullOrEmpty(validationErrors))
            {
                return new ServiceActionResult<WorkoutModel>(HttpStatusCode.BadRequest, validationErrors);
            }

            return new ServiceActionResult<WorkoutModel>(HttpStatusCode.OK, validationErrors, [workoutData]);
        }
    }
}
