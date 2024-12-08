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
        ///     Add the exercise to the workout with the provided id
        /// </summary>
        /// <param name="exerciseData">
        ///     The exercise
        /// </param>
        /// <param name="workoutId">
        ///     The workout id
        /// </param>
        public ServiceActionResult AddExerciseToWorkout(ExerciseModel exerciseData, long workoutId)
        {
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
        }

        /// <summary>
        ///     Add the exercise to the workout with the provided id.
        ///     Used when new muscle group exercise has been added and we
        ///     need to auto add it to the current workout as exercise to workout
        /// </summary>
        /// <param name="exerciseData">
        ///     The exercise
        /// </param>
        /// <param name="workoutId">
        ///     The workout id
        /// </param>
        public ServiceActionResult AddExerciseToWorkout(MGExerciseModel MGExerciseData, long workoutId)
        {
            // Find the muscle group
            var muscleGroup = DBAccess.MuscleGroups.Where(mg => mg.Id == MGExerciseData.MuscleGroupId)
                                                    .Select(mg => ModelMapper.MapToMuscleGroupModel(mg))
                                                    .FirstOrDefault();

            if (muscleGroup == null || muscleGroup.Id == 0)
            {
                return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_UNEXPECTED_ERROR);
            }

            // Create the Exercise Model
            var exerciseToAdd = new ExerciseModel
            {
                Id = 0,
                Name = MGExerciseData.Name,
                MuscleGroup = muscleGroup,
                Sets = []
            };

            // Reuse add exercise to workout
            return AddExerciseToWorkout(exerciseToAdd, workoutId);
        }

        /// <summary>
        ///     Update the provided exercise
        /// </summary>
        /// <param name="exercise">
        ///     The exercise
        /// </param>
        /// <param name="workoutId">
        ///     The workout id
        /// </param>
        public ServiceActionResult UpdateExerciseFromWorkout(ExerciseModel exerciseData, long workoutId)
        {
            // Fetch the exact exercise and it's sets
            var exercise = CheckExerciseExists(exerciseData.Id);
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
        }

        /// <summary>
        ///     Delete the exercise with the provided id
        /// </summary>
        /// <param name="exerciseId">
        ///     The exercise id
        /// </param>
        public ServiceActionResult DeleteExerciseFromWorkout(long exerciseId)
        {
            var exercise = CheckExerciseExists(exerciseId);
            if (exercise == null)
            {
                return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_EXERCISE_NOT_FOUND);
            }

            DBAccess.Exercises.Remove(exercise);
            DBAccess.SaveChanges();

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_EX_DELETED,
                                                [new BaseModel { Id = exercise.WorkoutId }]);
        }

        /// <summary>
        ///     Add the exercise to specific muscle group
        /// </summary>
        /// <param name="MGExerciseData">
        ///     The exercise
        /// </param>
        /// <param name="userId">
        ///     The user id who adding the exercise
        /// </param>
        /// <param name="checkExistingEx">
        ///     "Y" if we need to check whether exercise with this name already exists,
        ///     "N" to skip the check
        /// </param>
        public ServiceActionResult AddExercise(MGExerciseModel MGExerciseData, string userId, string checkExistingEx)
        {
            if (checkExistingEx == "Y")
            {
                // Check if exercise with the same name already exists
                var existingExercise = DBAccess.MGExercises.Where(e => e.Name.ToLower().Equals(MGExerciseData.Name.ToLower()) &&
                                                                        e.MuscleGroupId == MGExerciseData.MuscleGroupId &&
                                                                        e.UserId == userId)
                                                                        .FirstOrDefault();

                if (existingExercise != null)
                {
                    // If it exists, ask the user whether to override the description or create a new one
                    return new ServiceActionResult(Constants.ResponseCode.EXERCISE_ALREADY_EXISTS, Constants.MSG_EX_ALREADY_EXISTS,
                        [ModelMapper.MapToMGExerciseModel(existingExercise)]);
                }
            }
               
            // Create new exercise
            var MGExercise = new MGExercise
            {
                Name = MGExerciseData.Name,
                Description = MGExerciseData.Description,
                MuscleGroupId = MGExerciseData.MuscleGroupId,
                UserId = userId
            };

            DBAccess.MGExercises.Add(MGExercise);
            DBAccess.SaveChanges();

            // Get the MGExercises results and convert them to Enumerable, to avoid errors that the Entity Framerwork
            // cannot translate the method into SQL when MapToExerciseModel() is called.
            // Return the newly added MGExercise
            var MGExercisesEnum = DBAccess.MGExercises.Where(e => e.Id == MGExercise.Id).AsEnumerable();
            var model = MGExercisesEnum.Select(e => (BaseModel) ModelMapper.MapToMGExerciseModel(e)).ToList();

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_EX_ADDED, model);
        }

        /// <summary>
        ///     Add the exercise to specific muscle group
        /// </summary>
        /// <param name="exerciseData">
        ///     The exercise
        /// </param>
        public ServiceActionResult UpdateExercise(MGExerciseModel exerciseData) {
            var mgExercise = CheckMGExerciseExists(exerciseData.Id);
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
        }

        /// <summary>
        ///     Delete the muscle group exercise with the provided id
        /// </summary>
        /// <param name="MGExerciseId">
        ///     The exercise id
        /// </param>
        /// <param name="userId">
        ///     The user id who deleting the exercise
        /// </param>
        public ServiceActionResult DeleteExercise(long MGExerciseId, string userId)
        {
            var MGExercise = CheckMGExerciseExists(MGExerciseId);
            if (MGExercise == null)
            {
                return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_EXERCISE_NOT_FOUND);
            }

            if (MGExercise.UserId == null)
            {
                return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_CANNOT_DELETE_DEFAULT_ERROR);
            }

            DBAccess.MGExercises.Remove(MGExercise);

            // Delete all records for default values for this muscle group exercise
            DBAccess.UserDefaultValues.RemoveRange(DBAccess.UserDefaultValues.Where(u => u.MGExeciseId == MGExerciseId && u.UserId == userId));

            DBAccess.SaveChanges();

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_EX_DELETED,
                            [new BaseModel { Id = MGExercise.MuscleGroupId }]);
        }


        /// <summary>
        ///     Fetch the exercises for the muscle group
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
        }

        /// <summary>
        ///     Perform a check whether the exercise exists, returns exercise object if it exists,
        ///     null otherwise
        /// </summary>
        /// <param name="id">
        ///     The exercise id
        /// </param>
        /// <param name="workoutId">
        ///     The workout id
        /// </param>
        private Exercise? CheckExerciseExists(long id)
        {
            return DBAccess.Exercises.Where(e => e.Id == id).FirstOrDefault();
        }

        /// <summary>
        ///     Perform a check whether the muscle group exercise exists, returns muslce group exercise object if it exists,
        ///     null otherwise
        /// </summary>
        /// <param name="id">
        ///     The muscle group exercise id
        /// </param>
        private MGExercise? CheckMGExerciseExists(long id)
        {
            return DBAccess.MGExercises.Where(e => e.Id == id).FirstOrDefault();
        }
    }
}
