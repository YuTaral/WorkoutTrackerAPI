using Microsoft.AspNetCore.Mvc;
using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Services.Notifications;
using FitnessAppAPI.Data.Services.Exercises.Models;
using Newtonsoft.Json;
using FitnessAppAPI.Data.Services.Notifications.Models;

namespace FitnessAppAPI.Controllers
{
    /// <summary>
    ///     User Controller
    /// </summary>
    [ApiController]
    [Route(Constants.RequestEndPoints.NOTIFICATION)]
    public class NotificationController(INotificationService s) : BaseController
    {
        /// <summary>
        //      NotificationService instance
        /// </summary>
        private readonly INotificationService service = s;

        /// <summary>
        //      Post request to mark the notification as reviewed
        /// </summary>
        [HttpPost(Constants.RequestEndPoints.NOTIFICATION_REVIEWED)]
        public async Task<ActionResult> NotificationReviewed([FromQuery] long id)
        {
            // Check if the neccessary data is provided
            if (id < 1)
            {
                return CustomResponse(Constants.ResponseCode.FAIL, Constants.MSG_FAILED_TO_GET_NOTIFICATION_DETAILS);
            }

            return CustomResponse(await service.UpdateNotification(id, false));
        }

        /// <summary>
        //      Post request to delete the notification
        /// </summary>
        [HttpPost(Constants.RequestEndPoints.DELETE_NOTIFICATION)]
        public async Task<ActionResult> DeleteNotification([FromBody] Dictionary<string, string> requestData)
        {
            // Check if the neccessary data is provided
            if (!requestData.TryGetValue("notification", out string? serializednotification))
            {
                return CustomResponse(Constants.ResponseCode.FAIL, Constants.MSG_DELETE_NOTIFICATION_FAILED);
            }

            NotificationModel? notification = JsonConvert.DeserializeObject<NotificationModel>(serializednotification);
            if (notification == null)
            {
                return CustomResponse(Constants.ResponseCode.FAIL, string.Format(Constants.MSG_WORKOUT_FAILED_TO_DESERIALIZE_OBJ, "NotificationModel"));
            }

            var userId = GetUserId();

            var result = await service.DeleteNotification(notification, userId);
            if (!result.IsSuccess())
            {
                return CustomResponse(result);
            }

            // Return the notifications on success
            return CustomResponse(await service.GetNotifications(userId));
        }

        /// <summary>
        //      Get request to get user notifications
        /// </summary>
        [HttpGet(Constants.RequestEndPoints.GET_NOTIFICATIONS)]
        public async Task<ActionResult> GetNotifications()
        {
            return CustomResponse(await service.GetNotifications(GetUserId()));
        }

        /// <summary>
        //      Get request to get join team notification details
        /// </summary>
        [HttpGet(Constants.RequestEndPoints.GET_JOIN_TEAM_NOTIFICATION_DETAILS)]
        public async Task<ActionResult> GetJoinTeamNotificationDetails([FromQuery] long id)
        {
            // Check if the neccessary data is provided
            if (id < 1)
            {
                return CustomResponse(Constants.ResponseCode.FAIL, Constants.MSG_FAILED_TO_GET_NOTIFICATION_DETAILS);
            }

            return CustomResponse(await service.GetJoinTeamNotificationDetails(id));
        }
    }
}
