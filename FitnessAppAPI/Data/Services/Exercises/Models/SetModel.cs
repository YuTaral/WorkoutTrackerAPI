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
        public int Reps { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = Constants.ValidationErrors.WEIGHT_MUST_BE_POSITIVE)]
        public double Weight { get; set; }

        public int Rest { get; set; }

        public bool Completed { get; set; }
    }
}
