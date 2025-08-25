namespace FitnessAppAPI.Data.Services.SystemLogs
{
    /// <summary>
    ///     ISystemLogService interface to define the logic for system logs.
    /// </summary>
    public interface ISystemLogService
    {
        /// <summary>
        ///     Add record in the Systems logs table when error occured
        /// </summary>
        /// <param name="exception">
        ///     The exception
        /// </param>
        /// <param name="userId">
        ///     User id, may be empty if not logged in
        /// </param>
        public void Add(Exception exception, string userId);

        /// <summary>
        ///     Add record in the Systems logs table when error occured in the client
        /// </summary>
        /// <param name="requestData">
        ///     The request data (exception description and stack trace)
        /// </param>
        /// <param name="userId">
        ///     User id, may be empty if not logged in
        /// </param>
        public ServiceActionResult<string> Add(Dictionary<string, string> requestData, string userId);
    }
}
