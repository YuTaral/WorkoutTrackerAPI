using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Models;
using FitnessAppAPI.Data.Services.Exercises.Models;
using Microsoft.EntityFrameworkCore;

namespace FitnessAppAPI.Data.Services.Exercises
{
    /// <summary>
    ///     Exercise service class to implement IWorkoutService interface.
    /// </summary>
    public class ExerciseService(FitnessAppAPIContext DB) : BaseService(DB), IExerciseService
    {
        /// <summary>
        ///     Adds the exercise to the workout with the provided id
        /// </summary>
        /// <param name="exerciseData">
        ///     The exercise
        /// </param>
        /// <param name="workoutId">
        ///     The workout id
        /// </param>
        /// <param name="userId">
        ///     The user who is adding the exercse
        /// </param>
        public ServiceActionResult AddExerciseToWorkout(ExerciseModel exerciseData, long workoutId, string userId)
        {
            return ExecuteServiceAction(userId => {
                var exercise = new Exercise
                {
                    Name = exerciseData.Name,
                    WorkoutId = workoutId,
                    MuscleGroupId = exerciseData.MuscleGroup.Id,
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

                return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_EX_ADDED);
            }, userId);
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
        /// <param name="userId">
        ///     The user who is updating the exercse
        /// </param>
        public ServiceActionResult UpdateExerciseFromWorkout(ExerciseModel exerciseData, long workoutId, string userId)
        {
            return ExecuteServiceAction(userId => {
                // Fetch the exact exercise and it's sets
                var exercise = DBAccess.Exercises.Find(exerciseData.Id);

                if (exercise == null)
                {
                    return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_EXERCISE_NOT_FOUND);
                }

                var sets = DBAccess.Sets.Where(s => s.ExerciseId == exerciseData.Id).ToList();

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

                        }
                        else
                        {
                            // Update the set
                            var set = DBAccess.Sets.Find(s.Id);

                            if (set != null)
                            {
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

                }
                else
                {
                    // Delete all sets for this exercise
                    var setsToRemove = DBAccess.Sets.Where(s => s.ExerciseId == exercise.Id).ToList();

                    foreach (Set set in setsToRemove)
                    {
                        DBAccess.Sets.Remove(set);
                    }

                }

                // Save all changes
                DBAccess.SaveChanges();

                return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_EX_UPDATED);
            }, userId);
        }

        /// <summary>
        ///     Deletes the exercise with the provided id
        /// </summary>
        /// <param name="exerciseId">
        ///     The exercise id
        /// </param>
        /// <param name="userId">
        ///     The user who is deleting the exercse
        /// </param>
        public ServiceActionResult DeleteExerciseFromWorkout(long exerciseId, string userId)
        {
            return ExecuteServiceAction((userId) => {
                var exercise = DBAccess.Exercises.Where(e => e.Id == exerciseId).FirstOrDefault();
                if (exercise == null)
                {
                    return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_EXERCISE_NOT_FOUND);
                }

                DBAccess.Exercises.Remove(exercise);
                DBAccess.SaveChanges();

                return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_EX_DELETED,
                                                  CreateReturnData(new BaseModel { Id = exercise.WorkoutId }));
            }, userId);
        }

        /// <summary>
        ///     Adds the exercise to specific muscle group
        /// </summary>
        /// <param name="exerciseData">
        ///     The exercise
        /// </param>
        /// <param name="userId">
        ///     The user id who adding the exercise
        /// </param>
        /// <param name="checkExistingEx">
        ///     "Y" if we need to check whether exercise with this name already exists,
        ///     "N" to skip the check
        /// </param>
        public ServiceActionResult AddExercise(MGExerciseModel exerciseData, string userId, string checkExistingEx)
        {
            return ExecuteServiceAction(userId => {

                if (checkExistingEx == "Y")
                {
                    // Check if exercise with the same name already exists
                    var existingExercise = DBAccess.MGExercises.Where(e => e.Name.ToLower().Equals(exerciseData.Name.ToLower()) &&
                                                                           e.MuscleGroupId == exerciseData.MuscleGroupId &&
                                                                           e.UserId == userId)
                                                                           .FirstOrDefault();

                    if (existingExercise != null)
                    {
                        // If it exists, ask the user whether to override the description or create a new one
                        return new ServiceActionResult(Constants.ResponseCode.EXERCISE_ALREADY_EXISTS, Constants.MSG_EX_ALREADY_EXISTS,
                            CreateReturnData(ModelMapper.MapToMGExerciseModel(existingExercise)));
                    }
                }
               
                // Create new exercise
                var exercise = new MGExercise
                {
                    Name = exerciseData.Name,
                    Description = exerciseData.Description,
                    MuscleGroupId = exerciseData.MuscleGroupId,
                    UserId = userId
                };

                DBAccess.MGExercises.Add(exercise);
                DBAccess.SaveChanges();

                // Get the MGExercises results and convert them to Enumerable, to avoid errors that the Entity Framerwork
                // cannot translate the method into SQL when MapToExerciseModel() is called
                var MGExercisesEnum = DBAccess.MGExercises.Where(e => e.Id == exercise.Id).AsEnumerable();
                var model = MGExercisesEnum.Select(e => ModelMapper.MapToExerciseModel(e, DBAccess))
                                            .FirstOrDefault(ModelMapper.GetEmptyExerciseModel());

                return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_EX_ADDED, CreateReturnData(model));
            }, userId);
        }

        /// <summary>
        ///     Adds the exercise to specific muscle group
        /// </summary>
        /// <param name="exerciseData">
        ///     The exercise
        /// </param>
        /// <param name="userId">
        ///     The user id who adding the exercise
        /// </param>
        public ServiceActionResult UpdateExercise(MGExerciseModel exerciseData, string userId) {
            return ExecuteServiceAction(userId => {
                var mgExercise = DBAccess.MGExercises.Where(mg => mg.Id == exerciseData.Id).FirstOrDefault();

                if (mgExercise == null)
                {
                    return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_EXERCISE_NOT_FOUND);
                }

                // Change the data
                mgExercise.Name = exerciseData.Name;
                mgExercise.Description = exerciseData.Description;

                DBAccess.Entry(mgExercise).State = EntityState.Modified;
                DBAccess.SaveChanges();

                return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_EX_UPDATED);
            }, userId);
        }

        /// <summary>
        ///     Deletes the muscle group exercise with the provided id
        /// </summary>
        /// <param name="MGExerciseId">
        ///     The exercise id
        /// </param>
        /// <param name="userId">
        ///     The user id who deleting the exercise
        /// </param>
        /// 
        public ServiceActionResult DeleteExercise(long MGExerciseId, string userId)
        {
            return ExecuteServiceAction(userId => {
                var MGExercise = DBAccess.MGExercises.Where(e => e.Id == MGExerciseId).FirstOrDefault();

                if (MGExercise == null)
                {
                    return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_EXERCISE_NOT_FOUND);
                }

                if (MGExercise.UserId == null)
                {
                    return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_CANNOT_DELETE_DEFAULT_ERROR);
                }

                DBAccess.MGExercises.Remove(MGExercise);
                DBAccess.SaveChanges();

                return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_EX_DELETED,
                                CreateReturnData(new BaseModel { Id = MGExercise.MuscleGroupId }));
            }, userId);
        }


        /// <summary>
        ///     Fetches the exercises for the muscle group
        /// </summary>
        /// <param name="muscleGroupId">
        ///     The muscle group id
        /// </param>
        /// <param name="userId">
        ///     The the logged in user id
        /// </param>
        /// <param name="onlyForUser">
        ///     If "Y" the method will return only the user defined exercises for this muscle group,
        ///     which are considered editable and can be deleted / updated
        ///     If "N" the method will return all default and user defined exercises for this muscle group
        /// </param>
        public ServiceActionResult GetExercisesForMG(long muscleGroupId, string userId, string onlyForUser) {
            return ExecuteServiceAction(userId => {
                var returnData = new List<BaseModel>();

                if (onlyForUser == "Y")
                {

                    returnData = DBAccess.MGExercises.Where(e => e.MuscleGroupId == muscleGroupId && e.UserId == userId)
                                                       .Select(e => (BaseModel)ModelMapper.MapToMGExerciseModel(e))
                                                       .ToList();
                }
                else
                {
                    returnData = DBAccess.MGExercises.Where(e => e.MuscleGroupId == muscleGroupId && (e.UserId == null || e.UserId == userId))
                                                        .Select(e => (BaseModel)ModelMapper.MapToMGExerciseModel(e))
                                                        .ToList();
                }

                return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS, returnData);
            }, userId);
        }
    }
}
