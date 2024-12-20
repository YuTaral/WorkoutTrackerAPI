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
        public Task Add(Exception exception, string userId);
    }
}
