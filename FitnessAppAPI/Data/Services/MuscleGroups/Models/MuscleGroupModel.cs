namespace FitnessAppAPI.Data.Services.MuscleGroups.Models
{
    /// <summary>
    ///     MuscleGroupModel class representing a muscle group.
    ///     Must correspond with client-side MuscleGroupModel class
    /// </summary>
    public class MuscleGroupModel
    {
        public long Id { get; set; }
        public required string Name { get; set; }
        public required bool Checked { get; set; }

    }
}
