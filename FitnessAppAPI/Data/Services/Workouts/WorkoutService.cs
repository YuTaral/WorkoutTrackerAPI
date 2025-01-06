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
        public async Task<ServiceActionResult> AddWorkout(WorkoutModel data, string userId)
        {
            var workout = new Workout
            {
                Name = data.Name,
                UserId = userId,
                StartDateTime = DateTime.Now,
                FinishDateTime = null,
                Template = "N",
                DurationSeconds = 0,
                Notes = data.Notes
            };

            await DBAccess.Workouts.AddAsync(workout);
            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_WORKOUT_ADDED,
                                                [await ModelMapper.MapToWorkoutModel(workout, DBAccess)]);
        }

        public async Task<ServiceActionResult> UpdateWorkout(WorkoutModel data, string userId)
        {
            var workout = await CheckWorkoutExists(data.Id, userId);
            if (workout == null)
            {
                return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_WORKOUT_DOES_NOT_EXIST);
            }

            // Change the data
            workout.Name = data.Name;
            workout.FinishDateTime = data.FinishDateTime;
            workout.DurationSeconds = data.DurationSeconds;
            workout.Notes = data.Notes;

            DBAccess.Entry(workout).State = EntityState.Modified;
            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_WORKOUT_UPDATED,
                                                [await ModelMapper.MapToWorkoutModel(workout, DBAccess)]);
        }

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

        public async Task<ServiceActionResult> GetWorkout(long id, string userId) {
            var workout = await CheckWorkoutExists(id, userId);
            if (workout == null)
            {
                return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_WORKOUT_DOES_NOT_EXIST, []);
            }

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS,
                                                [await ModelMapper.MapToWorkoutModel(workout, DBAccess)]);
        }

        public async Task<ServiceActionResult> GetLatestWorkouts(string filterBy, string userId) {
            List<Workout>? workouts;

            // Start the query
            var query = DBAccess.Workouts.Where(w => w.UserId == userId && w.Template == "N");

            // Apply the appropriate filter to the query
            if (filterBy == Constants.WorkoutFilters.IN_PROGRESS)
            {
                query = query.Where(w => w.FinishDateTime == null);
            }
            else if (filterBy == Constants.WorkoutFilters.COMPLETED)
            {
                query = query.Where(w => w.FinishDateTime != null);
            }

            // Fetch the workouts
            workouts = await query.OrderByDescending(w => w.StartDateTime).ToListAsync();

            // Create the list asynchonously
            var workoutModels = new List<BaseModel>();
            foreach (var workout in workouts)
            {
                var workoutModel = await ModelMapper.MapToWorkoutModel(workout, DBAccess);
                workoutModels.Add(workoutModel);
            }

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS, workoutModels);
        }

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
