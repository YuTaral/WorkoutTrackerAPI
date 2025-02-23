using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Models;
using FitnessAppAPI.Data.Services.Exercises.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net;
using static FitnessAppAPI.Common.Constants;

namespace FitnessAppAPI.Data.Services.Exercises
{
    /// <summary>
    ///     Exercise service class to implement IWorkoutService interface.
    /// </summary>
    public class ExerciseService(FitnessAppAPIContext DB) : BaseService(DB), IExerciseService
    {
        public async Task<ServiceActionResult<long>> AddExerciseToWorkout(Dictionary<string, string> requestData)
        {
            // Check if the neccessary data is provided
            if (!requestData.TryGetValue("exercise", out string? serializedExercise) || 
                !requestData.TryGetValue("workoutId", out string? workoutIdString))
            {
                return new ServiceActionResult<long>(HttpStatusCode.BadRequest, Constants.MSG_EXERCISE_ADD_FAIL_NO_DATA);
            }

            ExerciseModel? exerciseData = JsonConvert.DeserializeObject<ExerciseModel>(serializedExercise);
            if (exerciseData == null)
            {
                return new ServiceActionResult<long>(HttpStatusCode.BadRequest, string.Format(Constants.MSG_WORKOUT_FAILED_TO_DESERIALIZE_OBJ, "ExerciseModel"));
            }

            string validationErrors = Utils.ValidateModel(exerciseData);
            if (!string.IsNullOrEmpty(validationErrors))
            {
                return new ServiceActionResult<long>(HttpStatusCode.BadRequest, validationErrors);
            }

            return await AddExerciseToWorkout(exerciseData, long.Parse(workoutIdString));
        }

        public async Task<ServiceActionResult<long>> AddExerciseToWorkout(MGExerciseModel MGExerciseData, long workoutId)
        {
            // Find the muscle group
            var muscleGroup = await DBAccess.MuscleGroups.Where(mg => mg.Id == MGExerciseData.MuscleGroupId)
                                                    .Select(mg => ModelMapper.MapToMuscleGroupModel(mg))
                                                    .FirstOrDefaultAsync();

            if (muscleGroup == null || muscleGroup.Id == 0)
            {
                return new ServiceActionResult<long>(HttpStatusCode.NotFound, Constants.MSG_UNEXPECTED_ERROR);
            }

            // Create the Exercise Model
            var exerciseToAdd = new ExerciseModel
            {
                Id = 0,
                Name = MGExerciseData.Name,
                MuscleGroup = muscleGroup,
                Sets = [],
                MGExerciseId = MGExerciseData.Id,
                Notes = ""
            };

            return await AddExerciseToWorkout(exerciseToAdd, workoutId);
        }

        public async Task<ServiceActionResult<long>> UpdateExerciseFromWorkout(Dictionary<string, string> requestData)
        {
            // Check if the neccessary data is provided
            if (!requestData.TryGetValue("exercise", out string? serializedExercise) || !requestData.TryGetValue("workoutId", out string? workoutIdString))
            {
                return new ServiceActionResult<long>(HttpStatusCode.BadRequest, Constants.MSG_EXERCISE_UPDATE_FAIL_NO_DATA);
            }

            ExerciseModel? exerciseData = JsonConvert.DeserializeObject<ExerciseModel>(serializedExercise);
            if (exerciseData == null)
            {
                return new ServiceActionResult<long>(HttpStatusCode.BadRequest, string.Format(Constants.MSG_WORKOUT_FAILED_TO_DESERIALIZE_OBJ, "ExerciseModel"));
            }

            string validationErrors = Utils.ValidateModel(exerciseData);
            if (!string.IsNullOrEmpty(validationErrors))
            {
                return new ServiceActionResult<long>(HttpStatusCode.BadRequest, validationErrors);
            }

            long workoutId = long.Parse(workoutIdString);

            // Fetch the exact exercise and it's sets
            var exercise = await CheckExerciseExists(exerciseData.Id);
            if (exercise == null)
            {
                return new ServiceActionResult<long>(HttpStatusCode.NotFound, Constants.MSG_EXERCISE_NOT_FOUND);
            }

            var sets = await DBAccess.Sets.Where(s => s.ExerciseId == exerciseData.Id).ToListAsync();

            if (!exercise.Notes.Equals(exerciseData.Notes))
            {
                // Update the exercise Notes if it has been changed
                exercise.Notes = exerciseData.Notes;
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
                            Rest = s.Rest,
                            ExerciseId = exerciseData.Id
                        };

                        await DBAccess.Sets.AddAsync(set);
                    }
                    else
                    {
                        // Update the set
                        var set = await DBAccess.Sets.FindAsync(s.Id);

                        if (set != null)
                        {
                            set.Completed = s.Completed;
                            set.Reps = s.Reps;
                            set.Rest = s.Rest;
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
                var setsToRemove = await DBAccess.Sets.Where(s => s.ExerciseId == exercise.Id).ToListAsync();

                foreach (Set set in setsToRemove)
                {
                    DBAccess.Sets.Remove(set);
                }

            }

            // Save all changes
            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult<long>(HttpStatusCode.OK, Constants.MSG_EX_UPDATED, [workoutId]);
        }

        public async Task<ServiceActionResult<long>> DeleteExerciseFromWorkout(long exerciseId)
        {
            // Check if the neccessary data is provided
            if (exerciseId <= 0)
            {
                return new ServiceActionResult<long>(HttpStatusCode.BadRequest, Constants.MSG_OBJECT_ID_NOT_PROVIDED);
            }

            var exercise = await CheckExerciseExists(exerciseId);
            if (exercise == null)
            {
                return new ServiceActionResult<long>(HttpStatusCode.NotFound, Constants.MSG_EXERCISE_NOT_FOUND);
            }

            DBAccess.Exercises.Remove(exercise);
            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult<long>(HttpStatusCode.OK, Constants.MSG_EX_DELETED, [exercise.WorkoutId]);
        }

        public async Task<ServiceActionResult<MGExerciseModel>> AddExercise(Dictionary<string, string> requestData, string userId)
        {
            // Check if the neccessary data is provided
            if (!requestData.TryGetValue("exercise", out string? serializedExercise))
            {
                return new ServiceActionResult<MGExerciseModel>(HttpStatusCode.BadRequest, Constants.MSG_EXERCISE_ADD_FAIL_NO_DATA);
            }

            MGExerciseModel? exerciseData = JsonConvert.DeserializeObject<MGExerciseModel>(serializedExercise);
            if (exerciseData == null)
            {
                 return new ServiceActionResult<MGExerciseModel>(HttpStatusCode.BadRequest, string.Format(Constants.MSG_WORKOUT_FAILED_TO_DESERIALIZE_OBJ, "MGExerciseModel"));
            }

            string validationErrors = Utils.ValidateModel(exerciseData);
            if (!string.IsNullOrEmpty(validationErrors))
            {
                 return new ServiceActionResult<MGExerciseModel>(HttpStatusCode.BadRequest, validationErrors);
            }

            requestData.TryGetValue("checkExistingEx", out string? checkExistingEx);
            checkExistingEx ??= "Y";

            if (checkExistingEx == "Y")
            {
                // Check if exercise with the same name already exists
                var existingExercise = await DBAccess.MGExercises.Where(e => e.Name.ToLower().Equals(exerciseData.Name.ToLower()) &&
                                                                        e.MuscleGroupId == exerciseData.MuscleGroupId &&
                                                                        e.UserId == userId)
                                                                        .FirstOrDefaultAsync();
                if (existingExercise != null)
                {
                    // If it exists, ask the user whether to override the description or create a new one
                    return new ServiceActionResult<MGExerciseModel>(CustomHttpStatusCode.EXERCISE_ALREADY_EXISTS, Constants.MSG_EX_ALREADY_EXISTS,
                                    [ModelMapper.MapToMGExerciseModel(existingExercise)]);
                }
            }
               
            // Create new exercise
            var MGExercise = new MGExercise
            {
                Name = exerciseData.Name,
                Description = exerciseData.Description,
                MuscleGroupId = exerciseData.MuscleGroupId,
                UserId = userId
            };

            await DBAccess.MGExercises.AddAsync(MGExercise);
            await DBAccess.SaveChangesAsync();

            // Get the MGExercises results and convert them to Enumerable, to avoid errors that the Entity Framerwork
            // cannot translate the method into SQL when MapToExerciseModel() is called.
            // Return the newly added MGExercise
            var MGExercisesEnum = DBAccess.MGExercises.Where(e => e.Id == MGExercise.Id).AsEnumerable();
            var model = MGExercisesEnum.Select(e => ModelMapper.MapToMGExerciseModel(e)).ToList();

            return new ServiceActionResult<MGExerciseModel>(HttpStatusCode.Created, Constants.MSG_EX_ADDED, model);
        }

        public async Task<ServiceActionResult<MGExerciseModel>> UpdateExercise(Dictionary<string, string> requestData) {
            // Check if the neccessary data is provided
            if (!requestData.TryGetValue("exercise", out string? serializedExercise))
            {
                return new ServiceActionResult<MGExerciseModel>(HttpStatusCode.BadRequest, Constants.MSG_EXERCISE_ADD_FAIL_NO_DATA);
            }

            MGExerciseModel? exerciseData = JsonConvert.DeserializeObject<MGExerciseModel>(serializedExercise);
            if (exerciseData == null)
            {
                return new ServiceActionResult<MGExerciseModel>(HttpStatusCode.BadRequest, string.Format(Constants.MSG_WORKOUT_FAILED_TO_DESERIALIZE_OBJ, "MGExerciseModel"));
            }

            string validationErrors = Utils.ValidateModel(exerciseData);
            if (!string.IsNullOrEmpty(validationErrors))
            {
                return new ServiceActionResult<MGExerciseModel>(HttpStatusCode.BadRequest, validationErrors);
            }

            var mgExercise = await CheckMGExerciseExists(exerciseData.Id);
            if (mgExercise == null)
            {
                return new ServiceActionResult<MGExerciseModel>(HttpStatusCode.NotFound, Constants.MSG_EXERCISE_NOT_FOUND);
            }

            // Change the data
            mgExercise.Name = exerciseData.Name;
            mgExercise.Description = exerciseData.Description;

            DBAccess.Entry(mgExercise).State = EntityState.Modified;
            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult<MGExerciseModel>(HttpStatusCode.OK, Constants.MSG_EX_UPDATED, [exerciseData]);
        }

        public async Task<ServiceActionResult<long>> DeleteExercise(long MGExerciseId, string userId)
        {
            // Check if the neccessary data is provided
            if (MGExerciseId <= 0)
            {
                return new ServiceActionResult<long>(HttpStatusCode.BadRequest, Constants.MSG_EXERCISE_DELETE_FAIL_NO_ID);
            }

            var MGExercise = await CheckMGExerciseExists(MGExerciseId);
            if (MGExercise == null)
            {
                return new ServiceActionResult<long>(HttpStatusCode.NotFound, Constants.MSG_EXERCISE_NOT_FOUND);
            }

            if (MGExercise.UserId == null)
            {
                return new ServiceActionResult<long>(HttpStatusCode.BadRequest, Constants.MSG_CANNOT_DELETE_DEFAULT_ERROR);
            }

            DBAccess.MGExercises.Remove(MGExercise);

            // Delete all records for default values for this muscle group exercise
            DBAccess.UserDefaultValues.RemoveRange(DBAccess.UserDefaultValues.Where(u => u.MGExeciseId == MGExerciseId && u.UserId == userId));

            // Set all Exercise records for this MGExercise to have Exercise.MGExerciseId = null
            var exercises = await DBAccess.Exercises.Where(e => e.MGExerciseId == MGExerciseId).ToListAsync();

            foreach (var exercise in exercises)
            {
                exercise.MGExerciseId = null;
                DBAccess.Entry(exercise).State = EntityState.Modified;
            }

            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult<long>(HttpStatusCode.OK, Constants.MSG_EX_DELETED, [MGExercise.MuscleGroupId]);
        }

        public async Task<ServiceActionResult<BaseModel>> CompleteSet(Dictionary<string, string> requestData)
        {
            // Check if the neccessary data is provided
            if (!requestData.TryGetValue("id", out string? idString))
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, Constants.MSG_OBJECT_ID_NOT_PROVIDED);
            }

            if (!long.TryParse(idString, out long setId))
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, Constants.MSG_OBJECT_ID_NOT_PROVIDED);
            }

            var set = await DBAccess.Sets.Where(s => s.Id == setId).FirstOrDefaultAsync();

            if (set == null) {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.NotFound, Constants.MSG_SET_DOES_NOT_EXIST);
            }

            set.Completed = true;
            DBAccess.Entry(set).State = EntityState.Modified;
            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult<BaseModel>(HttpStatusCode.OK);
        }

        public async Task<ServiceActionResult<MGExerciseModel>> GetMGExercise(long mGExerciseId)
        {

            var mgExercise = await DBAccess.MGExercises.Where(mg => mg.Id == mGExerciseId).FirstOrDefaultAsync();

            if (mgExercise == null)
            {
                return new ServiceActionResult<MGExerciseModel>(HttpStatusCode.NotFound, Constants.MSG_EXERCISE_NOT_FOUND);
            }

            return new ServiceActionResult<MGExerciseModel>(HttpStatusCode.OK, Constants.MSG_SUCCESS, 
                                            [ModelMapper.MapToMGExerciseModel(mgExercise)]);
        }

        public async Task<ServiceActionResult<MGExerciseModel>> GetExercisesForMG(long muscleGroupId, string onlyForUser, string userId)
        {
            var returnData = new List<MGExerciseModel>();

            if (muscleGroupId == 0 || onlyForUser == "")
            {
                return new ServiceActionResult<MGExerciseModel>(HttpStatusCode.BadRequest, Constants.MSG_OBJECT_ID_NOT_PROVIDED);
            }

            if (onlyForUser == "Y")
            {

                returnData = await DBAccess.MGExercises.Where(e => e.MuscleGroupId == muscleGroupId && e.UserId == userId)
                                                    .Select(e => ModelMapper.MapToMGExerciseModel(e))
                                                    .ToListAsync();
            }
            else
            {
                returnData = await DBAccess.MGExercises.Where(e => e.MuscleGroupId == muscleGroupId && (e.UserId == null || e.UserId == userId))
                                                    .Select(e => ModelMapper.MapToMGExerciseModel(e))
                                                    .ToListAsync();
            }

            return new ServiceActionResult<MGExerciseModel>(HttpStatusCode.OK, Constants.MSG_SUCCESS, returnData);
        }

        public async Task<ServiceActionResult<long>> AddExerciseToWorkout(ExerciseModel exerciseData, long workoutId)
        {
            var exercise = new Exercise
            {
                Name = exerciseData.Name,
                WorkoutId = workoutId,
                MuscleGroupId = exerciseData.MuscleGroup.Id,
                MGExerciseId = exerciseData.MGExerciseId,
                Notes = exerciseData.Notes
            };

            await DBAccess.Exercises.AddAsync(exercise);
            await DBAccess.SaveChangesAsync();

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
                        Rest = s.Rest,
                        ExerciseId = exercise.Id
                    };

                    await DBAccess.Sets.AddAsync(set);
                }

                await DBAccess.SaveChangesAsync();
            }

            // Return the workout
            return new ServiceActionResult<long>(HttpStatusCode.Created, Constants.MSG_EX_ADDED, [ workoutId]);
        }

        /// <summary>
        ///     Perform a check whether the exercise exists, returns exercise object if it exists,
        ///     null otherwise
        /// </summary>
        /// <param name="id">
        ///     The exercise id
        /// </param>
        private async Task<Exercise?> CheckExerciseExists(long id)
        {
            return await DBAccess.Exercises.Where(e => e.Id == id).FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Perform a check whether the muscle group exercise exists, returns muslce group exercise object if it exists,
        ///     null otherwise
        /// </summary>
        /// <param name="id">
        ///     The muscle group exercise id
        /// </param>
        private async Task<MGExercise?> CheckMGExerciseExists(long id)
        {
            return await DBAccess.MGExercises.Where(e => e.Id == id).FirstOrDefaultAsync();
        }
    }
}
