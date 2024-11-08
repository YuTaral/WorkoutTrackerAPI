using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Models;
using FitnessAppAPI.Data.Services.Workouts.Models;
using FitnessAppAPI.Data.Services.Exercises.Models;
using Microsoft.EntityFrameworkCore;
using FitnessAppAPI.Data.Services.MuscleGroups.Models;
using FitnessAppAPI.Data.Services.MuscleGroups;

namespace FitnessAppAPI.Data.Services.Workouts
{
    /// <summary>
    ///     Workout service class to implement IWorkoutService interface.
    /// </summary>

    public class WorkoutService(FitnessAppAPIContext DB, IMuscleGroupService mgService) : IWorkoutService
    {
        private readonly FitnessAppAPIContext DBAccess = DB;
        private readonly IMuscleGroupService musclegroupService = mgService;

        /// <summary>
        ///     Adds new workout from the provided WorkoutModel data
        /// </summary>
        public WorkoutModel? AddWorkout(WorkoutModel data, string userId)
        {
            // Verify user with this id exists
            if (!Utils.UserExists(DBAccess, userId)) 
            {
                return null;
            }

            var workout = new Workout 
            { 
                Name = data.Name,
                UserId = userId,
                Date = DateTime.Now,
                Template = "N"
            };

            DBAccess.Workouts.Add(workout);
            DBAccess.SaveChanges();

            // Add the Muscle Groups
            if (data.MuscleGroups != null) 
            {
                foreach (var mg in data.MuscleGroups)
                {
                    DBAccess.MuscleGroupsToWorkout.Add(new MuscleGroupToWorkout
                    {
                        WorkoutId = workout.Id,
                        MuscleGroupId = mg.Id
                    });
                }
            }

            DBAccess.SaveChanges();

            return GetWorkoutModelFromWorkout(workout);
        }

        /// <summary>
        ///     Edits the workout from the provided WorkoutModel data
        /// </summary>
        public WorkoutModel? EditWorkout(WorkoutModel data, string userId)
        {
            // Verify user with this id exists
            if (!Utils.UserExists(DBAccess, userId))
            {
                return null;
            }

            var workout = DBAccess.Workouts.Where(w => w.Id == data.Id).FirstOrDefault();

            if (workout == null)
            {
                return null;
            }

            // Change the name
            workout.Name = data.Name;

            // Remove the existing Muscle Groups
            musclegroupService.DeleteMuscleGroupsToWorkoutRecords(workout.Id);

            // Add the selected Muscle Groups
            if (data.MuscleGroups != null)
            {
                foreach (var mg in data.MuscleGroups)
                {
                    DBAccess.MuscleGroupsToWorkout.Add(new MuscleGroupToWorkout
                    {
                        WorkoutId = workout.Id,
                        MuscleGroupId = mg.Id
                    });
                }
            }

            DBAccess.Entry(workout).State = EntityState.Modified;
            DBAccess.SaveChanges();

            return GetWorkoutModelFromWorkout(workout);
        }

        /// <summary>
        ///     Deletes the workout with the provided id
        /// </summary>
        ///  /// <param name="workoutId">
        ///     The workout id
        /// </param>
        public bool DeleteWorkout(long workoutId) {
            var workout = DBAccess.Workouts.Where(w => w.Id == workoutId).FirstOrDefault();

            if (workout == null) {
                return false; 
            }

            // Delete all records from MuscleGroupsToWorkout for this workout
            musclegroupService.DeleteMuscleGroupsToWorkoutRecords(workoutId);

            // Delete the workout
            DBAccess.Workouts.Remove(workout);
            DBAccess.SaveChanges();

            return true;
        }

        /// <summary>
        ///     Tries to fetch the last workout for the user with the provided id
        /// </summary>
        /// <param name="userId">
        ///     The user id
        /// </param>

        public WorkoutModel? GetLastWorkout(string userId) {
            var workout = DBAccess.Workouts.Where(w => w.UserId == userId && w.Template == "N")
                                           .OrderByDescending(w => w.Date)
                                           .FirstOrDefault();

            if (workout == null)
            {
                return null;
            }

            return GetWorkoutModelFromWorkout(workout);
        }

        /// <summary>
        ///     Fetches the workout with the provided id and returns WorkoutModel
        /// </summary>
        /// <param name="id">
        ///     The workout id
        /// </param>
        public WorkoutModel? GetWorkout(long id) {
            var workout = DBAccess.Workouts.Where(w => w.Id == id).FirstOrDefault();

            if (workout == null) 
            {
                return null;
            }

            return GetWorkoutModelFromWorkout(workout);
        }

        /// <summary>
        ///     Fetches the latest workout for the user and returns WorkoutModel list
        /// </summary>
        /// <param name="userId">
        ///     The user id
        /// </param>
        public List<WorkoutModel>? GetLatestWorkouts(String userId) {
            return DBAccess.Workouts.Where(w => w.UserId == userId && w.Template == "N")
                                    .OrderByDescending(w => w.Date)
                                    .ToList()
                                    .Select(GetWorkoutModelFromWorkout).ToList();
        }

        /// <summary>
        ///     Returns WorkoutModel from the provided workout
        /// </summary>
        /// <param name="workout">
        ///     The workout
        /// </param>
        public WorkoutModel GetWorkoutModelFromWorkout(Workout workout)
        {
            var model = new WorkoutModel
            {
                Id = workout.Id,
                Name = workout.Name,
                Date = workout.Date,
                Template = workout.Template == "Y",
                Exercises = [.. DBAccess.Exercises
                            .Where(e => e.WorkoutId == workout.Id)
                            .Select(e => new ExerciseModel
                            {
                                Id = e.Id,
                                Name = e.Name,
                                Sets = DBAccess.Sets.Where(s => s.ExerciseId == e.Id)
                                                    .Select(s => new SetModel {
                                                        Id = s.Id,
                                                        Reps = s.Reps,
                                                        Weight = s.Weight,
                                                        Completed = s.Completed
                                                    }).ToList()
                            })],
                MuscleGroups = DBAccess.MuscleGroups.Select(mgm => new MuscleGroupModel
                                                    {
                                                        Id = mgm.Id,
                                                        Name = mgm.Name,
                                                        Checked = DBAccess.MuscleGroupsToWorkout
                                                                          .Any(mgw => mgw.WorkoutId == workout.Id && mgw.MuscleGroupId == mgm.Id)
                                                    })
                                                    .OrderByDescending(mgm => mgm.Checked)  
                                                    .ThenBy(mgm => mgm.Id)                  
                                                    .ToList()
            };

            return model;
        }
    }
}
