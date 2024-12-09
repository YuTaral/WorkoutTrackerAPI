using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Models;
using FitnessAppAPI.Data.Services.Workouts.Models;
using Microsoft.EntityFrameworkCore;

namespace FitnessAppAPI.Data.Services.Workouts
{
    /// <summary>
    ///     Workout service class to implement IWorkoutService interface.
    /// </summary>

    public class WorkoutService(FitnessAppAPIContext DB) : BaseService(DB), IWorkoutService
    {
        /// <summary>
        ///     Add new workout from the provided WorkoutModel data
        /// </summary>
        /// <param name="data">
        ///     The workout data
        /// </param>
        /// <param name="userId">
        ///     The user who is adding the workout
        /// </param>
        public async Task<ServiceActionResult> AddWorkout(WorkoutModel data, string userId)
        {
            var workout = new Workout
            {
                Name = data.Name,
                UserId = userId,
                Date = DateTime.Now,
                Template = "N"
            };

            await DBAccess.Workouts.AddAsync(workout);
            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_WORKOUT_ADDED,
                                                [await ModelMapper.MapToWorkoutModel(workout, DBAccess)]);
        }

        /// <summary>
        ///     Edit the workout from the provided WorkoutModel data
        /// </summary>
        /// <param name="data">
        ///     The workout data
        /// </param>
        /// <param name="userId">
        ///     The user who is updating the workout
        /// </param>
        public async Task<ServiceActionResult> UpdateWorkout(WorkoutModel data, string userId)
        {
            var workout = await CheckWorkoutExists(data.Id, userId);
            if (workout == null)
            {
                return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_WORKOUT_DOES_NOT_EXIST);
            }

            // Change the name
            workout.Name = data.Name;

            DBAccess.Entry(workout).State = EntityState.Modified;
            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_WORKOUT_UPDATED,
                                                [await ModelMapper.MapToWorkoutModel(workout, DBAccess)]);
        }

        /// <summary>
        ///     Delete the workout with the provided id
        /// </summary>
        ///  /// <param name="workoutId">
        ///     The workout id
        /// </param>
        /// <param name="userId">
        ///     The user who is deleting the workout
        /// </param>
        public async Task<ServiceActionResult> DeleteWorkout(long workoutId, string userId) {
            var workout = await CheckWorkoutExists(workoutId, userId);
            if (workout == null)
            {
                return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_WORKOUT_DOES_NOT_EXIST);
            }

            // Delete the workout
            DBAccess.Workouts.Remove(workout);
            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_WORKOUT_DELETED);
        }

        /// <summary>
        ///     Try to fetch the last workout for the user with the provided id
        /// </summary>
        /// <param name="userId">
        ///     The user id
        /// </param>

        public async Task<ServiceActionResult> GetLastWorkout(string userId) {
            var workout = await DBAccess.Workouts.Where(w => w.UserId == userId && w.Template == "N")
                                        .OrderByDescending(w => w.Date)
                                        .FirstOrDefaultAsync();

            if (workout == null)
            {
                return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_USER_HAS_NO_WORKOUT, []);
            }

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS,
                                               [await ModelMapper.MapToWorkoutModel(workout, DBAccess)]);
        }

        /// <summary>
        ///     Fetch the workout with the provided id and returns WorkoutModel
        /// </summary>
        /// <param name="id">
        ///     The workout id
        /// </param>
        /// <param name="userId">
        ///     The user owner of the workout
        /// </param>
        public async Task<ServiceActionResult> GetWorkout(long id, string userId) {
            var workout = await CheckWorkoutExists(id, userId);
            if (workout == null)
            {
                return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_WORKOUT_DOES_NOT_EXIST, []);
            }

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS,
                                                [await ModelMapper.MapToWorkoutModel(workout, DBAccess)]);
        }

        /// <summary>
        ///     Fetch the latest workout for the user and returns WorkoutModel list
        /// </summary>
        /// <param name="userId">
        ///     The user id
        /// </param>
        public async Task<ServiceActionResult> GetLatestWorkouts(string userId) {
            // Fetch all workouts asynchonously
            var workouts = await DBAccess.Workouts
                                 .Where(w => w.UserId == userId && w.Template == "N")
                                 .OrderByDescending(w => w.Date)
                                 .ToListAsync();

            // Create the list asynchonously
            var workoutModels = new List<BaseModel>();
            foreach (var workout in workouts)
            {
                var workoutModel = await ModelMapper.MapToWorkoutModel(workout, DBAccess);
                workoutModels.Add(workoutModel);
            }

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS, workoutModels);
        }

        /// <summary>
        ///     Return the weight units
        /// </summary>
        public async Task<ServiceActionResult> GetWeightUnits()
        {
            var units = await DBAccess.WeightUnits.Select(w => (BaseModel)ModelMapper.MapToWeightUnitModel(w)).ToListAsync();

            if (units.Count == 0)
            {
                return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_FAILED_TO_FETCH_WEIGHT_UNITS);
            }

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS, units);

        }

        /// <summary>
        ///     Perform a check whether the workout exists, returns workout object if it exists,
        ///     null otherwise
        /// </summary>
        /// <param name="id">
        ///     The workout id
        /// </param>
        /// <param name="userId">
        ///     The userId owner of the workout
        /// </param>
        private async Task<Workout?> CheckWorkoutExists(long id, string userId)
        {
            return await DBAccess.Workouts.Where(w => w.Id == id && w.UserId == userId).FirstOrDefaultAsync();
        }
    }
}
