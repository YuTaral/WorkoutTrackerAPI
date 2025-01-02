using Microsoft.AspNetCore.Mvc;
using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Services.Notifications;

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
