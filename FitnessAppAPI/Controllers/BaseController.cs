using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Services;
using FitnessAppAPI.Data.Models;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using System.Security.Claims;
using FitnessAppAPI.Data.Services.Notifications;

namespace FitnessAppAPI.Controllers
{
    /// <summary>
    ///     Base Controller class. Implement the logic common logic for all Controllers.
    /// </summary>
    public class BaseController: ControllerBase
    {

        /// <summary>
        ///     Return status 200OK Response with the provided data
        ///     Returned fields must correspond with client-side CustomResponse class:
        ///         - Code
        ///         - Message
        ///         - Data
        /// </summary>
        private OkObjectResult SendCustomResponse(Constants.ResponseCode Code, string Message, List<string> Data)
        {
            var hasNotification = false;

            var notificationService = HttpContext.RequestServices.GetService<INotificationService>();

            if (notificationService != null)
            {
               hasNotification = notificationService.HasNotification(GetUserId()).Result;
            }

            return Ok(Utils.CreateResponseObject(Code, Message, Data, hasNotification));
        }

        public OkObjectResult CustomResponse(Constants.ResponseCode Code, string Message, List<string> Data)
        {
            return SendCustomResponse(Code, Message, Data);
        }

        public OkObjectResult CustomResponse(Constants.ResponseCode Code)
        {
            return SendCustomResponse(Code, Constants.MSG_SUCCESS, []);
        }

        public OkObjectResult CustomResponse(Constants.ResponseCode Code, string Message)
        {
            return SendCustomResponse(Code, Message, []);
        }

        public OkObjectResult CustomResponse(Constants.ResponseCode Code, string Message, List<BaseModel> DataVal)
        {
            var Data = new List<String>();

            if (DataVal.Count > 0)
            {
                Data.AddRange(DataVal.Select(m => m.ToJson()));
            }

            return SendCustomResponse(Code, Message, Data);
           
        }

        public OkObjectResult CustomResponse(ServiceActionResult result)
        {
            var ResponseData = new List<String>();

            if (result.Data.Count > 0) {
                ResponseData.AddRange(result.Data.Select(m => m.ToJson()));
            }

            return SendCustomResponse(result.Code, result.Message, ResponseData);
        }


        /// <summary>
        ///     Used to get the logged in user id
        /// </summary>
        /// <returns>
        ///     UserId if user is logged in, empty string otherwise
        /// </returns>
        public string GetUserId() {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        }
    }
}
