using Microsoft.AspNetCore.Mvc;
using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Services.Notifications;
using FitnessAppAPI.Data.Services.User.Models;

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
    }
}
