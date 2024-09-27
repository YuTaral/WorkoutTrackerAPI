namespace FitnessAppAPI.Data.Services.Exercises.Models
{
    /// <summary>
    ///     ExerciseModel class representing an exercise, part of workout.
    ///     Must correspond with client-side ExerciseModel class
    /// </summary>
    public class ExerciseModel
    {
        public long Id { get; set; }
        public required string Name { get; set; }
        public List<SetModel>? Sets { get; set; }

    }
}
