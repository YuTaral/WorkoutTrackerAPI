using FitnessAppAPI.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace FitnessAppAPI.Data.Services.UserProfile.Models
{

    /// <summary>
    ///     WeightUnitModel class representing an weight unit.
    ///     Must correspond with client-side WeightUnitModel class
    /// </summary>
    public class WeightUnitModel: BaseModel
    {
        [MaxLength(10)]
        public required string Text { get; set; }
    }
}
