namespace FitnessAppAPI.Data.Services.SystemLogs
{
    /// <summary>
    ///     ISystemLogService interface to define the logic for system logs.
    /// </summary>
    public interface ISystemLogService
    {
        public void Add(Exception exception, string userId);
    }
}
