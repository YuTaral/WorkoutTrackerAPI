using Microsoft.AspNetCore.Mvc;
using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Services.Notifications;
using Newtonsoft.Json;
using System.Net;

namespace FitnessAppAPI.Controllers
{
    /// <summary>
    ///     User Controller
    /// </summary>
    [ApiController]
    [Route(Constants.RequestEndPoints.NOTIFICATIONS)]
    public class NotificationController(INotificationService s) : BaseController
    {
        /// <summary>
        //      NotificationService instance
        /// </summary>
        private readonly INotificationService service = s;

        /// <summary>
        //      Patch request to mark the notification as reviewed
        /// </summary>
        [HttpPatch]
        public async Task<ActionResult> NotificationReviewed([FromBody] Dictionary<string, string> requestData)
        {
            return SendResponse(await service.UpdateNotification(requestData, false));
        }

        /// <summary>
        //      Delete request to delete the notification
        /// </summary>
        [HttpDelete]
        public async Task<ActionResult> DeleteNotification([FromQuery] long notificationId)
        {
            var userId = GetUserId();

            var result = await service.DeleteNotification(notificationId, userId);
            if (!result.IsSuccess())
            {
                return SendResponse(result);
            }

            // Return the notifications on success
            return SendResponse(await service.GetNotifications(userId));
        }

        /// <summary>
        //      Get request to get user notifications
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> GetNotifications()
        {
            return SendResponse(await service.GetNotifications(GetUserId()));
        }

        /// <summary>
        //      Get request to get join team notification details
        /// </summary>
        [HttpGet(Constants.RequestEndPoints.JOIN_TEAM_NOTIFICATION_DETAILS)]
        public async Task<ActionResult> GetJoinTeamNotificationDetails([FromQuery] long notificationId)
        {
            return SendResponse(await service.GetJoinTeamNotificationDetails(notificationId));
        }

        /// <summary>
        //      Get request to refresh notifications
        /// </summary>
        [HttpGet(Constants.RequestEndPoints.REFRESH_NOTIFICATIONS)]
        public ActionResult RefreshNotifications()
        {
            // Just return custom response, it will automatically refresh the notification
            return SendResponse(HttpStatusCode.OK, Constants.MSG_SUCCESS);
        }
    }
}
