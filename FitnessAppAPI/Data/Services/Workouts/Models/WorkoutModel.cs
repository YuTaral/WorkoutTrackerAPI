namespace FitnessAppAPI.Data.Services.Workouts.Models
{
    /// <summary>
    ///     WorkoutModel class representing a workout.
    ///     Must correspond with client-side WorkoutModel class
    /// </summary>
    public class WorkoutModel
    {
        public long Id { get; set; }
        public required string Name { get; set; }
        public required DateTime Date { get; set; }
        public List<ExerciseModel>? Exercises { get; set; }

    }
}
