using FitnessAppAPI.Data.Models;
using System.ComponentModel.DataAnnotations;


namespace FitnessAppAPI.Data.Services.User.Models
{
    /// <summary>
    ///     UserDefaultValuesModel class representing the user default values
    ///     for sets, reps, weight and other.
    ///     Must correspond with client-side UserDefaultValuesModel class
    /// </summary>
    public class UserDefaultValuesModel: BaseModel
    {
        [Range(0, int.MaxValue)]
        public required int Sets { get; set; }

        [Range(0, int.MaxValue)]
        public required int Reps { get; set; }

        [Range(0, double.MaxValue)]
        public required double Weight { get; set; }

        public required bool Completed {  get; set; }

        public required string WeightUnitText { get; set; }
    }
}
