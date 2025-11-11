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
    [Route(RequestEndPoints.TRAINING_PLANS)]
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
            return await SendResponse(await service.AddTrainingPlan(requestData, GetUserId()));
        }

        /// <summary>
        //      Patch request to edit a training program
        /// </summary>
        [HttpPatch]
        [Authorize]
        public async Task<ActionResult> Update([FromBody] Dictionary<string, string> requestData)
        {
            return await SendResponse(await service.UpdateTrainingPlan(requestData, GetUserId()));
        }

        /// <summary>
        //      Delete request to delete the training program with the provided id
        /// </summary>
        [HttpDelete]
        [Authorize]
        public async Task<ActionResult> Delete([FromQuery] long trainingProgramId)
        {
            return await SendResponse(await service.DeleteTrainingPlan(trainingProgramId, GetUserId()));
        }

        /// <summary>
        //      GET request to fetch the training programs for the user
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GetTrainingPlans()
        {
            return await SendResponse(await service.GetTrainingPlans(GetUserId()));
        }

        /// <summary>
        //      POST request to add training day to program
        /// </summary>
        [HttpPost(RequestEndPoints.TRAINING_DAYS)]
        [Authorize]
        public async Task<ActionResult> AddTrainingDayToTrainingPlan([FromBody] Dictionary<string, string> requestData)
        {
            return await SendResponse(await service.AddTrainingDayToPlan(requestData, GetUserId()));
        }


        /// <summary>
        //      PATCH request to update the training day in program
        /// </summary>
        [HttpPatch(RequestEndPoints.TRAINING_DAYS)]
        [Authorize]
        public async Task<ActionResult> UpdateTrainingDayToPlan([FromBody] Dictionary<string, string> requestData)
        {
            return await SendResponse(await service.UpdateTrainingDayToPlan(requestData, GetUserId()));
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

        /// <summary>
        //      POST request to assign the training plan
        /// </summary>
        [HttpPost(RequestEndPoints.TRAINING_PLANS_ASSIGN)]
        [Authorize]
        public async Task<ActionResult> AssignTrainingPlan([FromBody] Dictionary<string, string> requestData)
        {
            return await SendResponse(await service.AssignTrainingPlan(requestData, GetUserId()));
        }

        /// <summary>
        //      Get request to fetch the training plan by the provided id
        /// </summary>
        [HttpGet(RequestEndPoints.TRAINING_PLAN_BY_ASSIGNED_ID)]
        [Authorize]
        public async Task<ActionResult> GetTrainingPlan([FromQuery] long assignedTrainingPlanId)
        {
            return await SendResponse(await service.GetTrainingPlan(assignedTrainingPlanId));
        }

        /// <summary>
        //      POST request to start the training plan
        /// </summary>
        [HttpPost(RequestEndPoints.TRAINING_PLANS_START)]
        [Authorize]
        public async Task<ActionResult> StartrainingPlan([FromBody] Dictionary<string, string> requestData)
        {
            return await SendResponse(await service.StartTrainingPlan(requestData, GetUserId()));
        }
    }
}
