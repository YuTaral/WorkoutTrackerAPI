using FitnessAppAPI.Data.Models;
using FitnessAppAPI.Data.Services.TrainingPrograms.Models;

namespace FitnessAppAPI.Data.Services.TrainingPrograms
{
    /// <summary>
    ///     Training plan service interface to define the logic for training plans CRUD operations.
    /// </summary>
    public interface ITrainingPlanService
    {
        /// <summary>
        ///     Add new training program from the provided TrainingProgramModel data
        /// </summary>
        /// <param name="requestData">
        ///     The request data (training program)
        /// </param>
        /// <param name="userId">
        ///     The user who is adding the training program
        /// </param>
        public Task<ServiceActionResult<TrainingPlanModel>> AddTrainingPlan(Dictionary<string, string> requestData, string userId);

        /// <summary>
        ///     Edit the training program from the provided TrainingProgramModel data
        /// </summary>
        /// <param name="requestData">
        ///     The request data (training program)
        /// </param>
        /// <param name="userId">
        ///     The user who is updating the training program
        /// </param>
        public Task<ServiceActionResult<TrainingPlanModel>> UpdateTrainingPlan(Dictionary<string, string> requestData, string userId);

        /// <summary>
        ///     Delete the training program with the provided id
        /// </summary>
        /// <param name="trainingProgramId">
        ///     The training program id
        /// </param>
        /// <param name="userId">
        ///     The user who is deleting the training program
        /// </param>
        public Task<ServiceActionResult<TrainingPlanModel>> DeleteTrainingPlan(long trainingProgramId, string userId);

        /// <summary>
        ///     Fetch the training programs for the user and returns TrainingProgramModel list
        /// </summary>
        /// <param name="userId">
        ///     The user id
        /// </param>
        public Task<ServiceActionResult<TrainingPlanModel>> GetTrainingPlans(string userId);

        /// <summary>
        ///     Add new training day to the program program from the provided TrainingDayModel model
        /// </summary>
        /// <param name="requestData">
        ///     The request data (training day model)
        /// </param>
        /// <param name="userId">
        ///     The user who is adding the day
        /// </param>
        public Task<ServiceActionResult<TrainingPlanModel>> AddTrainingDayToPlan(Dictionary<string, string> requestData, string userId);

        /// <summary>
        ///     Add update the training day from the provided TrainingDayModel model
        /// </summary>
        /// <param name="requestData">
        ///     The request data (training day model)
        /// </param>
        /// <param name="userId">
        ///     The user who is adding the day
        /// </param>
        public Task<ServiceActionResult<TrainingPlanModel>> UpdateTrainingDayToPlan(Dictionary<string, string> requestData, string userId);

        /// <summary>
        ///     Delete the training day with the provided id
        /// </summary>
        /// <param name="trainingDayId">
        ///     The training day id
        /// </param>
        /// <param name="userId">
        ///     The user who is deleting the training day
        /// </param>
        public Task<ServiceActionResult<TrainingPlanModel>> DeleteTrainingDay(long trainingDayId, string userId);

        /// <summary>
        ///     Delete all workout to training day records for the provided template id
        /// </summary>
        /// <param name="templateId">
        ///     The template id
        /// </param>
        public Task<ServiceActionResult<BaseModel>> DeleteWorkoutToTrainingDayRecs(long templateId);

        /// <summary>
        ///     Assign the training plan to the members
        /// </summary>
        /// <param name="requestData">
        ///     The request data (training plan id and member ids)
        /// </param>
        /// <param name="coachId">
        ///     The coach id
        /// </param>
        public Task<ServiceActionResult<BaseModel>> AssignTrainingPlan(Dictionary<string, string> requestData, string coachId);

        /// <summary>
        ///     Get the training plan by the provided assigned training plan id
        /// </summary>
        /// <param name="assignedTrainingPlanId">
        ///     The assigned training plan id
        /// </param>
        public Task<ServiceActionResult<TrainingPlanModel>> GetTrainingPlan(long assignedTrainingPlanId);

        /// <summary>
        ///     Start the training plan
        /// </summary>
        /// <param name="requestData">
        ///     The request data
        /// </param>
        /// <param name="userId">
        ///     The user id
        /// </param>
        public Task<ServiceActionResult<TrainingPlanModel>> StartTrainingPlan(Dictionary<string, string> requestData, string userId);
    }
}
