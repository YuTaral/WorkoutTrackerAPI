using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Services;
using FitnessAppAPI.Data.Models;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using System.Security.Claims;
using FitnessAppAPI.Data.Services.Notifications;
using System.Net;
using FitnessAppAPI.Data.Services.User.Models;

namespace FitnessAppAPI.Controllers
{
    /// <summary>
    ///     Base Controller class. Implement the logic common logic for all Controllers.
    /// </summary>
    public class BaseController: ControllerBase
    {

        /// <summary>
        ///     Return new ObjectResult
        ///     Returned fields must correspond with client-side CustomResponse class:
        ///         - Code
        ///         - Message
        ///         - Data
        /// </summary>
        private ObjectResult SendResponse(int Code, string Message, List<string> Data)
        {
            var hasNotification = false;

            var notificationService = HttpContext.RequestServices.GetService<INotificationService>();

            if (notificationService != null)
            {
                hasNotification = notificationService.HasNotification(GetUserId()).Result;
            }

            return Utils.CreateResponseObject(Code, Message, Data, hasNotification);
        }

        public ObjectResult SendResponse(HttpStatusCode Code)
        {
            return SendResponse((int) Code, Constants.MSG_SUCCESS, []);
        }

        public ObjectResult SendResponse(HttpStatusCode Code, string Message)
        {
            return SendResponse((int) Code, Message, []);
        }

        public ObjectResult SendResponse(HttpStatusCode Code, string Message, TokenResponseModel tokenRespModel)
        {
            var Data = new List<String>
            {
                tokenRespModel.Result.Data[0].ToJson(),
                tokenRespModel.Token
            };

            return SendResponse((int)Code, Message, Data);
        }

        public ObjectResult SendResponse<T>(HttpStatusCode Code, string Message, List<T> DataVal)
        {
            var Data = new List<String>();

            if (DataVal.Count > 0)
            {
                Data.AddRange(DataVal.Select(m => m.ToJson()));
            }

            return SendResponse((int) Code, Message, Data);
        }

        public ObjectResult SendResponse<T>(ServiceActionResult<T> result)
        {
            var ResponseData = new List<String>();

            if (result.Data.Count > 0) {
                ResponseData.AddRange(result.Data.Select(m => m.ToJson()));
            }

            return SendResponse(result.Code, result.Message, ResponseData);
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
