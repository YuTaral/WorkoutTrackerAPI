using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessAppAPI.Data.Models
{
    /// <summary>
    ///     AssignedTrainingPlan class to represent a row of database table assigned training plan.
    /// </summary>
    public class AssignedTrainingPlan
    {
        [Key]
        public long Id { get; set; }

        public required DateTime ScheduledStartDate { get; set; }
        public required DateTime? StartDate { get; set; }

        [ForeignKey("TrainingPlan")]
        public required long TrainingPlanId { get; set; }

        [ForeignKey("TeamMember")]
        public required long TeamMemberId { get; set; }
    }
}
