using FitnessAppAPI.Data.Services.SystemLogs;
using Microsoft.AspNetCore.Mvc;
using static FitnessAppAPI.Common.Constants;

namespace FitnessAppAPI.Controllers
{
    /// <summary>
    ///     System logs controller
    /// </summary>
    [ApiController]
    [Route(RequestEndPoints.SYSTEM_LOGS)]
    public class SystemLogController(ISystemLogService s) : BaseController
    {
        /// <summary>
        //      SystemLogService instance
        /// </summary>
        private readonly ISystemLogService service = s;

        /// <summary>
        //      POST request to store system log
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> Add([FromBody] Dictionary<string, string> requestData)
        {
            return await SendResponse(await service.Add(requestData, GetUserId()));
        }
    }
}
