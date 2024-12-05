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
            ResponseData = [];
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

        /// <summary>
        ///   True if the ResponseCode is REFRESH_TOKEN, false otherwise
        /// </summary>
        public bool IsRefreshToken()
        {
            return ResponseCode == Constants.ResponseCode.REFRESH_TOKEN;
        }

        /// <summary>
        ///   True if the ResponseCode is TOKEN_EXPIRED, false otherwise
        /// </summary>
        public bool IsTokenExpired()
        {
            return ResponseCode == Constants.ResponseCode.TOKEN_EXPIRED;
        }
    }
}
