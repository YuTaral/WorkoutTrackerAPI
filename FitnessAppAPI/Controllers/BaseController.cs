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
        ///     Return a JsonResult with all the data needed for a Response
        ///     Returned fields must correspond with client-side CustomResponse class:
        ///         - ResponseCode
        ///         - ResponseMessage
        ///         - ResponseData
        /// </summary>
        private OkObjectResult SendCustomResponse(Constants.ResponseCode ResponseCode, string ResponseMessage, List<string> ResponseData)
        {
            return Ok(new
            {
                ResponseCode,
                ResponseMessage,
                ResponseData
            });
        }

        public OkObjectResult CustomResponse(Constants.ResponseCode ResponseCode, string ResponseMessage, List<string> ResponseData)
        {
            return SendCustomResponse(ResponseCode, ResponseMessage, ResponseData);
        }

        public OkObjectResult CustomResponse(Constants.ResponseCode ResponseCode, string ResponseMessage)
        {
            return SendCustomResponse(ResponseCode, ResponseMessage, []);
        }

        public OkObjectResult CustomResponse(Constants.ResponseCode ResponseCode, string ResponseMessage, List<BaseModel> ResponseDataVal)
        {
            var ResponseData = new List<String>();

            if (ResponseDataVal.Count > 0)
            {
                ResponseData.AddRange(ResponseDataVal.Select(m => m.ToJson()));
            }

            return SendCustomResponse(ResponseCode, ResponseMessage, ResponseData);
           
        }

        public OkObjectResult CustomResponse(ServiceActionResult result)
        {
            var ResponseData = new List<String>();

            if (result.ResponseData.Count > 0) {
                ResponseData.AddRange(result.ResponseData.Select(m => m.ToJson()));
            }

            return SendCustomResponse(result.ResponseCode, result.ResponseMessage, ResponseData);
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
