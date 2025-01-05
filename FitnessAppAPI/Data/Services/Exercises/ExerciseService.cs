using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Models;
using FitnessAppAPI.Data.Services.Exercises.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace FitnessAppAPI.Data.Services.Exercises
{
    /// <summary>
    ///     Exercise service class to implement IWorkoutService interface.
    /// </summary>
    public class ExerciseService(FitnessAppAPIContext DB) : BaseService(DB), IExerciseService
    {
        public async Task<ServiceActionResult> AddExerciseToWorkout(ExerciseModel exerciseData, long workoutId)
        {
            var exercise = new Exercise
            {
                Name = exerciseData.Name,
                WorkoutId = workoutId,
                MuscleGroupId = exerciseData.MuscleGroup.Id,
                MGExerciseId = exerciseData.MGExerciseId
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

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_EX_ADDED);
        }

        public async Task<ServiceActionResult> AddExerciseToWorkout(MGExerciseModel MGExerciseData, long workoutId)
        {
            // Find the muscle group
            var muscleGroup = await DBAccess.MuscleGroups.Where(mg => mg.Id == MGExerciseData.MuscleGroupId)
                                                    .Select(mg => ModelMapper.MapToMuscleGroupModel(mg))
                                                    .FirstOrDefaultAsync();

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
                Sets = [],
                MGExerciseId = MGExerciseData.Id
            };

            // Reuse add exercise to workout
            return await AddExerciseToWorkout(exerciseToAdd, workoutId);
        }

        public async Task<ServiceActionResult> UpdateExerciseFromWorkout(ExerciseModel exerciseData, long workoutId)
        {
            // Fetch the exact exercise and it's sets
            var exercise = await CheckExerciseExists(exerciseData.Id);
            if (exercise == null)
            {
                return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_EXERCISE_NOT_FOUND);
            }

            var sets = await DBAccess.Sets.Where(s => s.ExerciseId == exerciseData.Id).ToListAsync();

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

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_EX_UPDATED);
        }

        public async Task<ServiceActionResult> DeleteExerciseFromWorkout(long exerciseId)
        {
            var exercise = await CheckExerciseExists(exerciseId);
            if (exercise == null)
            {
                return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_EXERCISE_NOT_FOUND);
            }

            DBAccess.Exercises.Remove(exercise);
            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_EX_DELETED,
                                                [new BaseModel { Id = exercise.WorkoutId }]);
        }

        public async Task<ServiceActionResult> AddExercise(MGExerciseModel MGExerciseData, string userId, string checkExistingEx)
        {
            if (checkExistingEx == "Y")
            {
                // Check if exercise with the same name already exists
                var existingExercise = await DBAccess.MGExercises.Where(e => e.Name.ToLower().Equals(MGExerciseData.Name.ToLower()) &&
                                                                        e.MuscleGroupId == MGExerciseData.MuscleGroupId &&
                                                                        e.UserId == userId)
                                                                        .FirstOrDefaultAsync();

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

            await DBAccess.MGExercises.AddAsync(MGExercise);
            await DBAccess.SaveChangesAsync();

            // Get the MGExercises results and convert them to Enumerable, to avoid errors that the Entity Framerwork
            // cannot translate the method into SQL when MapToExerciseModel() is called.
            // Return the newly added MGExercise
            var MGExercisesEnum = DBAccess.MGExercises.Where(e => e.Id == MGExercise.Id).AsEnumerable();
            var model = MGExercisesEnum.Select(e => (BaseModel) ModelMapper.MapToMGExerciseModel(e)).ToList();

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_EX_ADDED, model);
        }

        public async Task<ServiceActionResult> UpdateExercise(MGExerciseModel exerciseData) {
            var mgExercise = await CheckMGExerciseExists(exerciseData.Id);
            if (mgExercise == null)
            {
                return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_EXERCISE_NOT_FOUND);
            }

            // Change the data
            mgExercise.Name = exerciseData.Name;
            mgExercise.Description = exerciseData.Description;

            DBAccess.Entry(mgExercise).State = EntityState.Modified;
            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_EX_UPDATED);
        }

        public async Task<ServiceActionResult> DeleteExercise(long MGExerciseId, string userId)
        {
            var MGExercise = await CheckMGExerciseExists(MGExerciseId);
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

            // Set all Exercise records for this MGExercise to have Exercise.MGExerciseId = null
            var exercises = await DBAccess.Exercises.Where(e => e.MGExerciseId == MGExerciseId).ToListAsync();

            foreach (var exercise in exercises)
            {
                exercise.MGExerciseId = null;
                DBAccess.Entry(exercise).State = EntityState.Modified;
            }

            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_EX_DELETED,
                            [new BaseModel { Id = MGExercise.MuscleGroupId }]);
        }

        public async Task<ServiceActionResult> GetExercisesForMG(long muscleGroupId, string userId, string onlyForUser) {
            var returnData = new List<BaseModel>();

            if (onlyForUser == "Y")
            {

                returnData = await DBAccess.MGExercises.Where(e => e.MuscleGroupId == muscleGroupId && e.UserId == userId)
                                                    .Select(e => (BaseModel)ModelMapper.MapToMGExerciseModel(e))
                                                    .ToListAsync();
            }
            else
            {
                returnData = await DBAccess.MGExercises.Where(e => e.MuscleGroupId == muscleGroupId && (e.UserId == null || e.UserId == userId))
                                                    .Select(e => (BaseModel)ModelMapper.MapToMGExerciseModel(e))
                                                    .ToListAsync();
            }

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS, returnData);
        }

        public async Task<ServiceActionResult> GetMGExercise(long mGExerciseId)
        {
            var mgExercise = await DBAccess.MGExercises.Where(mg => mg.Id == mGExerciseId).FirstOrDefaultAsync();

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS, 
                                            [ModelMapper.MapToMGExerciseModel(mgExercise)]);
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
