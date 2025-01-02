using FitnessAppAPI.Data.Models;

namespace FitnessAppAPI.Data.Services.Notifications.Models
{
    /// <summary>
    ///     NotificationModel class representing a notification.
    ///     Must correspond with client-side NotificationModel class
    /// </summary>
    public class NotificationModel: BaseModel
    {
        public required string NotificationText { get; set; }

        public required DateTime DateTime { get; set; }

        public required bool IsActive { get; set; }

        public required string Type { get; set; }

        public required string Image { get; set; }

        public long? TeamId { get; set; }
    }
}
