using FitnessAppAPI.Common;
using Microsoft.AspNetCore.Mvc;

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
            if (responseCode == Constants.ResponseCode.BAD_REQUEST) 
            {
                return BadRequest(new 
                { 
                    responseCode,
                    userMessage,
                    returnData
                });
            }

            return Ok(new 
            { 
                responseCode, 
                userMessage, 
                returnData 
            });
        }
    }
}
