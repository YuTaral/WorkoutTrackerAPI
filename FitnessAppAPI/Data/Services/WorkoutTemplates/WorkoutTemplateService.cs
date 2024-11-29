using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Models;
using FitnessAppAPI.Data.Services.Exercises;
using FitnessAppAPI.Data.Services.Exercises.Models;
using FitnessAppAPI.Data.Services.Workouts.Models;

namespace FitnessAppAPI.Data.Services.WorkoutTemplates
{
    /// <summary>
    ///     Workout Temaplte service class to implement IWorkoutTemplateService interface.
    /// </summary>
    public class WorkoutTemplateService(FitnessAppAPIContext DB, IExerciseService exService) : BaseService(DB), IWorkoutTemplateService 
    {
        private readonly IExerciseService exerciseService = exService;

        /// <summary>
        ///     Add new workout template from the provided WorkoutModel data
        /// </summary>
        /// <param name="data">
        ///     The template data
        /// </param>
        /// <param name="userId">
        ///     The user who is adding the template
        /// </param>
        public ServiceActionResult AddWorkoutTemplate(WorkoutModel data, string userId) {
            return ExecuteServiceAction(userId =>
            {
                // Create Workout record, with Template = "Y"
                var template = new Workout
                {
                    Name = data.Name,
                    UserId = userId,
                    Date = DateTime.Now,
                    Template = "Y"
                };

                DBAccess.Workouts.Add(template);
                DBAccess.SaveChanges();

                // Add the Exercises and sets
                if (data.Exercises != null)
                {

                    foreach (ExerciseModel exerciseData in data.Exercises)
                    {
                        var result = exerciseService.AddExerciseToWorkout(exerciseData, template.Id, userId);

                        if (!result.IsSuccess())
                        {
                            return result;
                        }
                    }

                }

                return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_TEMPLATE_ADDED);
            }, userId);
        }

        /// <summary>
        ///    Delete the template with the provided id
        /// </summary>
        /// <param name="templateId">
        ///     The template id
        /// </param>
        /// <param name="userId">
        ///     The user who is deleting the template
        /// </param>
        public ServiceActionResult DeleteWorkoutTemplate(long templateId, string userId) {
            return ExecuteServiceAction(userId => {
                var template = DBAccess.Workouts.Where(w => w.Id == templateId && w.Template == "Y").FirstOrDefault();

                if (template == null)
                {
                    return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_TEMPLATE_DOES_NOT_EXIST);
                }

                DBAccess.Workouts.Remove(template);
                DBAccess.SaveChanges();

                return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_TEMPLATE_DELETED);
            }, userId);
        }

        /// <summary>
        ///     Return list of all workout templates created by the user with the provided id
        /// </summary>
        /// <param name="userId">
        ///     The user who is fetching the templates
        /// </param>
        public ServiceActionResult GetWorkoutTemplates(string userId)
        {
            return ExecuteServiceAction(userId => {
                var templates = DBAccess.Workouts.Where(w => w.UserId == userId && w.Template == "Y")
                                                .OrderByDescending(w => w.Date)
                                                .Select(t => (BaseModel)ModelMapper.MapToWorkoutModel(t, DBAccess))
                                                .ToList();

                if (templates.Count == 0)
                {
                    return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_NO_TEMPLATES);
                }

                return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS, templates);
            }, userId);
        }
    }
}
