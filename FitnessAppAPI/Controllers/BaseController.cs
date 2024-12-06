using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Services;
using FitnessAppAPI.Data.Models;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using System.Security.Claims;

namespace FitnessAppAPI.Controllers
{
    /// <summary>
    ///     Base Controller class. Holds the logic common logic for all Controllers.
    /// </summary>
    public class BaseController : ControllerBase
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
            return Ok(Utils.CreateResponseObject(Code, Message, Data));
        }

        public OkObjectResult CustomResponse(Constants.ResponseCode Code, string Message, List<string> Data)
        {
            return SendCustomResponse(Code, Message, Data);
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
