using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Models;
using FitnessAppAPI.Data.Services.Exercises;
using FitnessAppAPI.Data.Services.Exercises.Models;
using FitnessAppAPI.Data.Services.Workouts.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.Collections.Generic;

namespace FitnessAppAPI.Data.Services.WorkoutTemplates
{
    /// <summary>
    ///     Workout Temaplte service class to implement IWorkoutTemplateService interface.
    /// </summary>
    public class WorkoutTemplateService(FitnessAppAPIContext DB, IExerciseService exService) : BaseService(DB), IWorkoutTemplateService 
    {
        private readonly IExerciseService exerciseService = exService;

        public async Task<ServiceActionResult> AddWorkoutTemplate(WorkoutModel data, string userId) {
            // Create Workout record, with Template = "Y"
            var template = new Workout
            {
                Name = data.Name,
                UserId = userId,
                StartDateTime = null,
                FinishDateTime = null,
                Template = "Y",
                DurationSeconds = null,
                Notes = data.Notes
            };

            await DBAccess.Workouts.AddAsync(template);
            await DBAccess.SaveChangesAsync();

            // Add the Exercises and sets
            if (data.Exercises != null)
            {

                foreach (ExerciseModel exerciseData in data.Exercises)
                {
                    var result = await exerciseService.AddExerciseToWorkout(exerciseData, template.Id);

                    if (!result.IsSuccess())
                    {
                        return result;
                    }
                }

            }

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_TEMPLATE_ADDED);
        }

        public async Task<ServiceActionResult> UpdateWorkoutTemplate(WorkoutModel data, string userId)
        {
            var template = await DBAccess.Workouts.Where(w => w.Id == data.Id && w.Template == "Y").FirstOrDefaultAsync();

            if (template == null)
            {
                return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_TEMPLATE_DOES_NOT_EXIST);
            }

            if (template.Name != data.Name)
            {
                template.Name = data.Name;
            }

            if (template.Notes != data.Notes) 
            {
                template.Notes = data.Notes;
            }

            DBAccess.Entry(template).State = EntityState.Modified;
            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_TEMPLATE_UPDATED);
        }

        public async Task<ServiceActionResult> DeleteWorkoutTemplate(long templateId) {
            var template = await DBAccess.Workouts.Where(w => w.Id == templateId && w.Template == "Y").FirstOrDefaultAsync();

            if (template == null)
            {
                return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_TEMPLATE_DOES_NOT_EXIST);
            }

            DBAccess.Workouts.Remove(template);
            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_TEMPLATE_DELETED);
        }

        public async Task<ServiceActionResult> GetWorkoutTemplates(string userId)
        {
            // Fetch the templats asynchonously
            var templates = await DBAccess.Workouts.Where(w => w.UserId == userId && w.Template == "Y")
                                            .OrderByDescending(w => w.Id)
                                            .ToListAsync();


            // Create the list asynchonously
            var templateModels = new List<BaseModel>();
            foreach (var template in templates)
            {
                var workoutModel = await ModelMapper.MapToWorkoutModel(template, DBAccess);
                templateModels.Add(workoutModel);
            }

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS, templateModels);
        }
    }
}
