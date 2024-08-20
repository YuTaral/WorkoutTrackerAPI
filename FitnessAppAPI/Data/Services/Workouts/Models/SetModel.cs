namespace FitnessAppAPI.Data.Services.Workouts.Models
{
    /// <summary>
    ///     SetModel class representing a set, part of exercise.
    ///     Must correspond with client-side SetModel class
    /// </summary>
    public class SetModel
    {
        public long Id { get; set; }
        public int Reps { get; set; }
        public double Weight { get; set; }
        public bool Completed { get; set; }

    }
}
