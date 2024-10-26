using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Models;
using FitnessAppAPI.Data.Services.Exercises;
using FitnessAppAPI.Data.Services.Exercises.Models;
using FitnessAppAPI.Data.Services.Workouts;
using FitnessAppAPI.Data.Services.Workouts.Models;

namespace FitnessAppAPI.Data.Services.WorkoutTemplates
{
    /// <summary>
    ///     Workout Temaplte service class to implement IWorkoutTemplateService interface.
    /// </summary>
    public class WorkoutTemplateService(FitnessAppAPIContext DB, 
                                        IWorkoutService wService, 
                                        IExerciseService exService) : IWorkoutTemplateService
    {
        private readonly FitnessAppAPIContext DBAccess = DB;
        private readonly IWorkoutService workoutService = wService;
        private readonly IExerciseService exerciseService = exService;

        /// <summary>
        ///     Adds new workout template from the provided WorkoutModel data
        /// </summary>
        public bool AddWorkoutTemplate(WorkoutModel data, string userId) {
            // Verify user with this id exists
            if (!Utils.UserExists(DBAccess, userId))
            {
                return false;
            }

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

            // Add the Muscle Groups
            if (data.MuscleGroups != null)
            {
                foreach (var mg in data.MuscleGroups)
                {
                    DBAccess.MuscleGroupsToWorkout.Add(new MuscleGroupToWorkout
                    {
                        WorkoutId = template.Id,
                        MuscleGroupId = mg.Id
                    });
                }
            }

            DBAccess.SaveChanges();

            // Add the Exercises and sets
            if (data.Exercises != null) {
                foreach (ExerciseModel exerciseData in data.Exercises) {
                    exerciseService.AddExercise(exerciseData, template.Id);
                }
            }

            return true;
        }

        /// <summary>
        ///     Returns list of all workout templates created by the user with the provided id
        /// </summary>
        public List<WorkoutModel> GetWorkoutTemplates(string userId)
        {
            var templatesModel = new List<WorkoutModel>();
            var templates = DBAccess.Workouts.Where(w => w.UserId == userId && w.Template == "Y")
                                             .OrderByDescending(w => w.Date)
                                             .ToList();

            foreach (Workout t in templates) {
                templatesModel.Add(workoutService.GetWorkoutModelFromWorkout(t));
            }

            return templatesModel;
        }
    }
}
