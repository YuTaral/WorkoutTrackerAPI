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
        ///     Adds new workout from the provided WorkoutModel data
        /// </summary>
        /// <param name="data">
        ///     The workout data
        /// </param>
        /// <param name="userId">
        ///     The user who is adding the workout
        /// </param>
        public ServiceActionResult AddWorkout(WorkoutModel data, string userId)
        {
            return ExecuteServiceAction(userId => {
                // Verify user with this id exists
                if (!UserExists(DBAccess, userId))
                {
                    return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_USER_DOES_NOT_EXISTS);
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

                return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_WORKOUT_ADDED,
                                                CreateReturnData(ModelMapper.MapToWorkoutModel(workout, DBAccess)));
            }, userId);
        }

        /// <summary>
        ///     Edits the workout from the provided WorkoutModel data
        /// </summary>
        /// <param name="data">
        ///     The workout data
        /// </param>
        /// <param name="userId">
        ///     The user who is updating the workout
        /// </param>
        public ServiceActionResult EditWorkout(WorkoutModel data, string userId)
        {
            return ExecuteServiceAction(userId => {
                // Verify user with this id exists
                if (!UserExists(DBAccess, userId))
                {
                    return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_USER_DOES_NOT_EXISTS);
                }

                var workout = CheckWorkoutExists(data.Id);
                if (workout == null)
                {
                    return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_WORKOUT_DOES_NOT_EXIST);
                }

                // Change the name
                workout.Name = data.Name;

                DBAccess.Entry(workout).State = EntityState.Modified;
                DBAccess.SaveChanges();

                return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_WORKOUT_UPDATED,
                                                 CreateReturnData(ModelMapper.MapToWorkoutModel(workout, DBAccess)));
            }, userId);
        }

        /// <summary>
        ///     Deletes the workout with the provided id
        /// </summary>
        ///  /// <param name="workoutId">
        ///     The workout id
        /// </param>
        /// <param name="userId">
        ///     The user who is deleting the workout
        /// </param>
        public ServiceActionResult DeleteWorkout(long workoutId, string userId) {
            return ExecuteServiceAction(userId => {
                var workout = CheckWorkoutExists(workoutId);
                if (workout == null)
                {
                    return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_WORKOUT_DOES_NOT_EXIST);
                }

                // Delete the workout
                DBAccess.Workouts.Remove(workout);
                DBAccess.SaveChanges();

                return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_WORKOUT_DELETED);
            }, userId);
        }

        /// <summary>
        ///     Tries to fetch the last workout for the user with the provided id
        /// </summary>
        /// <param name="userId">
        ///     The user id
        /// </param>

        public ServiceActionResult GetLastWorkout(string userId) {
            return ExecuteServiceAction(userId => {
                var workout = DBAccess.Workouts.Where(w => w.UserId == userId && w.Template == "N")
                                          .OrderByDescending(w => w.Date)
                                          .FirstOrDefault();

                if (workout == null)
                {
                    return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_USER_HAS_NO_WORKOUT);
                }

                return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS,
                                                CreateReturnData(ModelMapper.MapToWorkoutModel(workout, DBAccess)));
            }, userId);
        }

        /// <summary>
        ///     Fetches the workout with the provided id and returns WorkoutModel
        /// </summary>
        /// <param name="id">
        ///     The workout id
        /// </param>
        /// <param name="userId">
        ///     The user owner of the workout
        /// </param>
        public ServiceActionResult GetWorkout(long id, string userId) {
            return ExecuteServiceAction(userId => {
                var workout = CheckWorkoutExists(id);
                if (workout == null)
                {
                    return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_WORKOUT_DOES_NOT_EXIST);
                }

                return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS,
                                                CreateReturnData(ModelMapper.MapToWorkoutModel(workout, DBAccess)));
            }, userId);
        }

        /// <summary>
        ///     Fetches the latest workout for the user and returns WorkoutModel list
        /// </summary>
        /// <param name="userId">
        ///     The user id
        /// </param>
        public ServiceActionResult GetLatestWorkouts(string userId) {
            return ExecuteServiceAction(userId => {
                var workouts = DBAccess.Workouts.Where(w => w.UserId == userId && w.Template == "N")
                                                .OrderByDescending(w => w.Date)
                                                .Select(w => (BaseModel)ModelMapper.MapToWorkoutModel(w, DBAccess))
                                                .ToList();

                if (workouts.Count == 0)
                {
                    return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_USER_HAS_NO_WORKOUT);
                }

                return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS, workouts);
            }, userId);
        }

        /// <summary>
        ///     Performs a check whether the workout exists, returns workout object if it exists,
        ///     null otherwise
        /// </summary>
        /// <param name="id">
        ///     The workout id
        /// </param>

        private Workout? CheckWorkoutExists(long id)
        {
            return DBAccess.Workouts.Where(w => w.Id == id).FirstOrDefault();
        }
    }
}
