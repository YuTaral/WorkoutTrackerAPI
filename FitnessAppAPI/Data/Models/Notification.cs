using FitnessAppAPI.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessAppAPI.Data.Models
{
    /// <summary>
    ///    Notification class to represent a row of database table Notifications.
    /// </summary>
    public class Notification
    {
        [Key]
        public long Id { get; set; }

        [MaxLength(Constants.DBConstants.Len50)]
        [EnumDataType(typeof(Constants.NotificationType))]
        public required string NotificationType { get; set; }   

        [ForeignKey("AspNetUser")]
        public string? SenderUserId { get; set; }

        [ForeignKey("AspNetUser")]
        public required string ReceiverUserId { get; set; }

        [MaxLength(Constants.DBConstants.Len200)]
        public required string NotificationText { get; set; }

        public required DateTime DateTime { get; set; }

        public required bool IsActive { get; set; }

        [ForeignKey("Team")]
        public required long? TeamId { get; set; }
    }
}
