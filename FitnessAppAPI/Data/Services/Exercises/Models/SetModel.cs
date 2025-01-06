using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace FitnessAppAPI.Data.Services.Exercises.Models
{
    /// <summary>
    ///     SetModel class representing a set, part of exercise.
    ///     Must correspond with client-side SetModel class
    /// </summary>
    public class SetModel: BaseModel
    {
        [Range(0, int.MaxValue, ErrorMessage = Constants.ValidationErrors.REPS_MUST_BE_POSITIVE)]
        public required int Reps { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = Constants.ValidationErrors.WEIGHT_MUST_BE_POSITIVE)]
        public required double Weight { get; set; }

        public required int Rest { get; set; }

        public required bool Completed { get; set; }
    }
}
