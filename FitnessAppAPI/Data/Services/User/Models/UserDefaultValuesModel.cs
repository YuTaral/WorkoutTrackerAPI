using FitnessAppAPI.Data.Models;

namespace FitnessAppAPI.Data.Services.User.Models
{
    /// <summary>
    ///     UserDefaultValuesModel class representing the user default values
    ///     for sets, reps, weight and other.
    ///     Must correspond with client-side UserDefaultValuesModel class
    /// </summary>
    public class UserDefaultValuesModel: BaseModel
    {
        public required int Sets { get; set; }

        public required int Reps { get; set; }

        public required double Weight { get; set; }

        public required string WeightUnit { get; set; }
    }
}
