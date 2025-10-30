using FitnessAppAPI.Data.Services.TrainingPrograms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static FitnessAppAPI.Common.Constants;

namespace FitnessAppAPI.Controllers
{
    /// <summary>
    ///     Training program Controller
    /// </summary>
    [ApiController]
    [Route(RequestEndPoints.TRAINING_PROGRAMS)]
    public class TrainingPlanController(ITrainingPlanService s) : BaseController
    {
        /// <summary>
        //      TrainingService instance
        /// </summary>
        private readonly ITrainingPlanService service = s;

        /// <summary>
        //      POST request to create a new workout
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Add([FromBody] Dictionary<string, string> requestData)
        {
            return await SendResponse(await service.AddTrainingProgram(requestData, GetUserId()));
        }

        /// <summary>
        //      Patch request to edit a training program
        /// </summary>
        [HttpPatch]
        [Authorize]
        public async Task<ActionResult> Update([FromBody] Dictionary<string, string> requestData)
        {
            return await SendResponse(await service.UpdateTrainingProgram(requestData, GetUserId()));
        }

        /// <summary>
        //      Delete request to delete the training program with the provided id
        /// </summary>
        [HttpDelete]
        [Authorize]
        public async Task<ActionResult> Delete([FromQuery] long trainingProgramId)
        {
            return await SendResponse(await service.DeleteTrainingProgram(trainingProgramId, GetUserId()));
        }

        /// <summary>
        //      GET request to fetch the training programs for the user
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GetLatestWorkouts()
        {
            return await SendResponse(await service.GetTrainingPrograms(GetUserId()));
        }

        /// <summary>
        //      POST request to add training day to program
        /// </summary>
        [HttpPost(RequestEndPoints.TRAINING_DAYS)]
        [Authorize]
        public async Task<ActionResult> AddTrainingDayToProgram([FromBody] Dictionary<string, string> requestData)
        {
            return await SendResponse(await service.AddTrainingDayToProgram(requestData, GetUserId()));
        }


        /// <summary>
        //      PATCH request to update the training day in program
        /// </summary>
        [HttpPatch(RequestEndPoints.TRAINING_DAYS)]
        [Authorize]
        public async Task<ActionResult> UpdateTrainingDayToProgram([FromBody] Dictionary<string, string> requestData)
        {
            return await SendResponse(await service.UpdateTrainingDayToProgram(requestData, GetUserId()));
        }

        /// <summary>
        //      Delete request to delete the training program with the provided id
        /// </summary>
        [HttpDelete(RequestEndPoints.TRAINING_DAYS)]
        [Authorize]
        public async Task<ActionResult> DeleteTrainingDay([FromQuery] long trainingDayId)
        {
            return await SendResponse(await service.DeleteTrainingDay(trainingDayId, GetUserId()));
        }



    }
}
