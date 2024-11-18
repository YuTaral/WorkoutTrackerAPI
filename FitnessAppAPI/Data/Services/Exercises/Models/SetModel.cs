using FitnessAppAPI.Data.Models;

namespace FitnessAppAPI.Data.Services.Exercises.Models
{
    /// <summary>
    ///     SetModel class representing a set, part of exercise.
    ///     Must correspond with client-side SetModel class
    /// </summary>
    public class SetModel: BaseModel
    {
        public int Reps { get; set; }
        public double Weight { get; set; }
        public bool Completed { get; set; }

    }
}
