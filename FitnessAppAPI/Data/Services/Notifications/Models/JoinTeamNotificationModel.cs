using FitnessAppAPI.Data.Models;

namespace FitnessAppAPI.Data.Services.Notifications.Models
{
    /// <summary>
    ///     JoinTeamNotificationModel class the show notification about team invitation.
    ///     Must correspond with client-side JoinTeamNotificationModel class
    /// </summary>
    public class JoinTeamNotificationModel: BaseModel
    {
        public required string TeamName { get; set; }

        public required string Description { get; set; }

        public required string TeamImage { get; set; }

        public required string NotificationType { get; set; }

        public required long TeamId { get; set; }
    }
}
