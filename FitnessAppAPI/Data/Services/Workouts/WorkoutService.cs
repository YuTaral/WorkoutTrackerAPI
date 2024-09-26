using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Models;
using FitnessAppAPI.Data.Services.Workouts.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Collections.Generic;

namespace FitnessAppAPI.Data.Services.Workouts
{
    /// <summary>
    ///     Workout service class to implement IWorkoutService interface.
    /// </summary>

    public class WorkoutService(FitnessAppAPIContext DB) : IWorkoutService
    {
        private readonly FitnessAppAPIContext DBAccess = DB;

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
            DeleteMuscleGroupsToWorkoutRecords(workout.Id);

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
            DeleteMuscleGroupsToWorkoutRecords(workoutId);

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
            var workout = DBAccess.Workouts.Where(w => w.UserId == userId)
                                           .OrderByDescending(w => w.Date)
                                           .FirstOrDefault();

            if (workout == null)
            {
                return null;
            }

            return GetWorkoutModelFromWorkout(workout);
        }

        /// <summary>
        ///     Adds the exercise to the workout with the provided id
        /// </summary>
        /// <param name="exercise">
        ///     The exercise
        /// </param>
        /// <param name="workoutId">
        ///     The workout id
        /// </param>
        public bool AddExercise(ExerciseModel exerciseData, long workoutId) {
            var exercise = new Exercise
            {
                Name = exerciseData.Name,
                WorkoutId = workoutId
            };

            DBAccess.Exercises.Add(exercise);
            DBAccess.SaveChanges();

            // Check if we need to add sets
            if (exerciseData.Sets != null && exerciseData.Sets.Count > 0)
            {
                foreach (SetModel s in exerciseData.Sets)
                {
                    var set = new Set
                    {
                        Reps = s.Reps,
                        Weight = s.Weight,
                        Completed = s.Completed,
                        ExerciseId = exercise.Id
                    };

                    DBAccess.Sets.Add(set);
                    DBAccess.SaveChanges();
                }
            }

            return true;
        }

        /// <summary>
        ///     Updates the provided exercise
        /// </summary>
        /// <param name="exercise">
        ///     The exercise
        /// </param>
        /// <param name="workoutId">
        ///     The workout id
        /// </param>
        public bool UpdateExercise(ExerciseModel exerciseData, long workoutId) {
            // Fetch the exact exercise and it's sets
            var exercise = DBAccess.Exercises.Find(exerciseData.Id);
            var sets = DBAccess.Sets.Where(s => s.ExerciseId == exerciseData.Id).ToList();
            
            if (exercise == null) 
            {
                return false;
            }

            if (!exercise.Name.Equals(exerciseData.Name)) 
            {
                // Update the exercise name if it has been changed
                exercise.Name = exerciseData.Name;
            }

            if (exerciseData.Sets != null && exerciseData.Sets.Count > 0) 
            {
                // Update / add the sets
                foreach (SetModel s in exerciseData.Sets)
                {
                    if (s.Id == 0) 
                    {
                        // Add new set
                        var set = new Set
                        {
                            Reps = s.Reps,
                            Weight = s.Weight,
                            Completed = s.Completed,
                            ExerciseId = exerciseData.Id
                        };

                        DBAccess.Sets.Add(set);

                    } else 
                    {
                        // Update the set
                        var set = DBAccess.Sets.Find(s.Id);

                        if (set != null) {
                            set.Completed = s.Completed;
                            set.Reps = s.Reps;
                            set.Weight = s.Weight;
                        }
                    }
                }

                // Check if we need to remove any sets
                foreach (Set set in sets) 
                {
                    if (!exerciseData.Sets.Any(x => x.Id == set.Id)) 
                    {
                        DBAccess.Sets.Remove(set);
                    }
                }

            } else {
                // Delete all sets for this exercise
                var setsToRemove = DBAccess.Sets.Where(s => s.ExerciseId == exercise.Id).ToList();

                foreach(Set set in setsToRemove) 
                {
                    DBAccess.Sets.Remove(set);
                }
                
            }

            // Save all changes
            DBAccess.SaveChanges();
            return true;
        }

        /// <summary>
        ///     Deletes the exercise with the provided id
        /// </summary>
        /// <param name="exerciseId">
        ///     The exercise id
        /// </param>
        public long DeleteExercise(long exerciseId)
        {
            var exercise = DBAccess.Exercises.Where(e => e.Id == exerciseId).FirstOrDefault();
            if (exercise == null) 
            {
                return 0;
            }

            DBAccess.Exercises.Remove(exercise);
            DBAccess.SaveChanges();
            return exercise.WorkoutId;
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
            return DBAccess.Workouts.Where(w => w.UserId == userId)
                                    .OrderByDescending(w => w.Date)
                                    .ToList()
                                    .Select(GetWorkoutModelFromWorkout).ToList();
        }

        /// <summary>
        ///     Fetches the default Muscle Groups and the user defined Muscle Groups
        /// </summary>
        /// <param name="userId">
        ///     The user id
        /// </param>
        public List<MuscleGroupModel>? GetMuscleGroups(String userId)
        {
            return DBAccess.MuscleGroups.Where(m => m.UserId == userId || m.UserId == null)
                                        .OrderBy(m => m.Id)
                                        .Select(m => new MuscleGroupModel 
                                        { 
                                            Id = m.Id,
                                            Name = m.Name,
                                            Checked = false
                                        }).ToList();
        }

        /// <summary>
        ///     Returns WorkoutModel from the provided workout
        /// </summary>
        /// <param name="workout">
        ///     The workout
        /// </param>
        private WorkoutModel GetWorkoutModelFromWorkout(Workout workout)
        {
            var model = new WorkoutModel
            {
                Id = workout.Id,
                Name = workout.Name,
                Date = workout.Date,
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

        /// <summary>
        ///     Deletes records from MuscleGroupsToWorkout
        /// </summary>
        /// <param name="workout">
        ///     The workout
        /// </param>
        private void DeleteMuscleGroupsToWorkoutRecords(long workoutId)
        {
            DBAccess.MuscleGroupsToWorkout.RemoveRange(DBAccess.MuscleGroupsToWorkout.Where(mgw => mgw.WorkoutId == workoutId).ToList());
        }
    }
}
