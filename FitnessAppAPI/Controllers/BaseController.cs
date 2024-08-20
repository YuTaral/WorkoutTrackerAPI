using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Services.User.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FitnessAppAPI.Controllers
{
    /// <summary>
    ///     Base Controller class. Holds the logic common logic for all Controllers.
    /// </summary>
    public class BaseController : ControllerBase
    {
        /// <summary>
        ///     Returns a JsonResult with all the data needed for a Response
        ///     Returned fields must correspond with client-side CustomResponse class:
        ///         - responseCode
        ///         - userMessage
        ///         - returnData
        /// </summary>
        public ActionResult ReturnResponse(bool badRequest, Constants.ResponseCode responseCode, string userMessage, List<string> returnData)
        {
            if (badRequest)
            {
                return BadRequest(new { 
                    responseCode,
                    userMessage,
                    returnData
                });
            }

            return Ok(new { 
                responseCode, 
                userMessage, 
                returnData 
            });
        }
    }
}
