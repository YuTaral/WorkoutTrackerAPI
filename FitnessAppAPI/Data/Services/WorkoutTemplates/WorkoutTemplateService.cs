using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Models;
using FitnessAppAPI.Data.Services.Exercises;
using FitnessAppAPI.Data.Services.Exercises.Models;
using FitnessAppAPI.Data.Services.Workouts.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;

namespace FitnessAppAPI.Data.Services.WorkoutTemplates
{
    /// <summary>
    ///     Workout Temaplte service class to implement IWorkoutTemplateService interface.
    /// </summary>
    public class WorkoutTemplateService(FitnessAppAPIContext DB, IExerciseService exService) : BaseService(DB), IWorkoutTemplateService 
    {
        private readonly IExerciseService exerciseService = exService;

        public async Task<ServiceActionResult<BaseModel>> AddWorkoutTemplate(Dictionary<string, string> requestData, string userId) {
            // Check if the neccessary data is provided
            if (!requestData.TryGetValue("workout", out string? serializedWorkout))
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, Constants.MSG_TEMPLATE_ADD_FAIL_NO_DATA);
            }

            WorkoutModel? workoutData = JsonConvert.DeserializeObject<WorkoutModel>(serializedWorkout);
            if (workoutData == null)
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, string.Format(Constants.MSG_WORKOUT_FAILED_TO_DESERIALIZE_OBJ, "WorkoutModel"));
            }

            string validationErrors = Utils.ValidateModel(workoutData);
            if (!string.IsNullOrEmpty(validationErrors))
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, validationErrors);
            }

            // Create Workout record, with Template = "Y"
            var template = new Workout
            {
                Name = workoutData.Name,
                UserId = userId,
                StartDateTime = null,
                FinishDateTime = null,
                Template = "Y",
                DurationSeconds = null,
                Notes = workoutData.Notes
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
                        return new ServiceActionResult<BaseModel>((HttpStatusCode) result.Code, result.Message);
                    }
                }
            }

            return new ServiceActionResult<BaseModel>(HttpStatusCode.Created, Constants.MSG_TEMPLATE_ADDED);
        }

        public async Task<ServiceActionResult<BaseModel>> UpdateWorkoutTemplate(Dictionary<string, string> requestData, string userId)
        {
            // Check if the neccessary data is provided
            if (!requestData.TryGetValue("workout", out string? serializedWorkout))
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, Constants.MSG_TEMPLATE_UPDATE_FAIL_NO_DATA);
            }

            WorkoutModel? workoutData = JsonConvert.DeserializeObject<WorkoutModel>(serializedWorkout);
            if (workoutData == null)
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, string.Format(Constants.MSG_WORKOUT_FAILED_TO_DESERIALIZE_OBJ, "WorkoutModel"));
            }

            string validationErrors = Utils.ValidateModel(workoutData);
            if (!string.IsNullOrEmpty(validationErrors))
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, validationErrors);
            }

            var template = await DBAccess.Workouts.Where(w => w.Id == workoutData.Id && w.Template == "Y").FirstOrDefaultAsync();

            if (template == null)
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.NotFound, Constants.MSG_TEMPLATE_DOES_NOT_EXIST);
            }

            if (template.Name != workoutData.Name)
            {
                template.Name = workoutData.Name;
            }

            if (template.Notes != workoutData.Notes) 
            {
                template.Notes = workoutData.Notes;
            }

            DBAccess.Entry(template).State = EntityState.Modified;
            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult<BaseModel>(HttpStatusCode.OK, Constants.MSG_TEMPLATE_UPDATED);
        }

        public async Task<ServiceActionResult<BaseModel>> DeleteWorkoutTemplate(long templateId) {
            // Check if the neccessary data is provided
            if (templateId <= 0)
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, Constants.MSG_EXERCISE_DELETE_FAIL_NO_ID);
            }

            var template = await DBAccess.Workouts.Where(w => w.Id == templateId && w.Template == "Y").FirstOrDefaultAsync();

            if (template == null)
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.NotFound, Constants.MSG_TEMPLATE_DOES_NOT_EXIST);
            }

            DBAccess.Workouts.Remove(template);
            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult<BaseModel>(HttpStatusCode.OK, Constants.MSG_TEMPLATE_DELETED);
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

            return new ServiceActionResult<WorkoutModel>(HttpStatusCode.OK, Constants.MSG_SUCCESS, templateModels);
        }
    }
}
