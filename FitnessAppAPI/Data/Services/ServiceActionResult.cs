using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Models;

namespace FitnessAppAPI.Data.Services
  
{
    /// <summary>
    ///    Class to define the result of service action.
    /// </summary>
    public class ServiceActionResult
    {
        public Constants.ResponseCode ResponseCode { get; set; }
        public string ResponseMessage { get; set; }    
        public List<BaseModel> ResponseData { get; set; }

        public ServiceActionResult(Constants.ResponseCode ResponseCodeVal, string ResponseMessageVal) {
            ResponseCode = ResponseCodeVal;
            ResponseMessage = ResponseMessageVal;
            ResponseData = new List<BaseModel>();
        }

        public ServiceActionResult(Constants.ResponseCode ResponseCodeVal, string ResponseMessageVal, List<BaseModel> ResponseDataVal)
        {
            ResponseCode = ResponseCodeVal;
            ResponseMessage = ResponseMessageVal;
            ResponseData = ResponseDataVal;
        }

        /// <summary>
        ///   True if the ResponseCode is SUCCESS, false otherwise
        /// </summary>
        public bool IsSuccess()
        {
            return ResponseCode == Constants.ResponseCode.SUCCESS;
        }

    }
}
