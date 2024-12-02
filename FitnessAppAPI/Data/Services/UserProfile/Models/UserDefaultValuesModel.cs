using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Models;
using System.ComponentModel.DataAnnotations;


namespace FitnessAppAPI.Data.Services.UserProfile.Models
{
    /// <summary>
    ///     UserDefaultValuesModel class representing the user default values
    ///     for sets, reps, weight and other.
    ///     Must correspond with client-side UserDefaultValuesModel class
    /// </summary>
    public class UserDefaultValuesModel : BaseModel
    {
        [Range(0, int.MaxValue, ErrorMessage = Constants.ValidationErrors.SETS_MUST_BE_POSITIVE)]
        public required int Sets { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = Constants.ValidationErrors.REPS_MUST_BE_POSITIVE)]
        public required int Reps { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = Constants.ValidationErrors.WEIGHT_MUST_BE_POSITIVE)]
        public required double Weight { get; set; }

        public required bool Completed { get; set; }

        public required WeightUnitModel WeightUnit { get; set; }
    }
}
