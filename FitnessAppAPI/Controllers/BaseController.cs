using FitnessAppAPI.Common;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        public ActionResult ReturnResponse(Constants.ResponseCode responseCode, string userMessage, List<string> returnData)
        {
            return Ok(new 
            { 
                responseCode, 
                userMessage, 
                returnData 
            });
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
