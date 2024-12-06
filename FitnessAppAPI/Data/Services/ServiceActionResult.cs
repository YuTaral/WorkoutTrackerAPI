using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Models;

namespace FitnessAppAPI.Data.Services
  
{
    /// <summary>
    ///    Class to define the result of service action.
    /// </summary>
    public class ServiceActionResult
    {
        public Constants.ResponseCode Code { get; set; }
        public string Message { get; set; }    
        public List<BaseModel> Data { get; set; }

        public ServiceActionResult(Constants.ResponseCode CodeVal)
        {
            Code = CodeVal;
            Message = Constants.MSG_SUCCESS;
            Data = [];
        }

        public ServiceActionResult(Constants.ResponseCode CodeVal, string MessageVal) {
            Code = CodeVal;
            Message = MessageVal;
            Data = [];
        }

        public ServiceActionResult(Constants.ResponseCode CodeVal, string MessageVal, List<BaseModel> DataVal)
        {
            Code = CodeVal;
            Message = MessageVal;
            Data = DataVal;
        }

        /// <summary>
        ///   True if the ResponseCode is SUCCESS, false otherwise
        /// </summary>
        public bool IsSuccess()
        {
            return Code == Constants.ResponseCode.SUCCESS;
        }

        /// <summary>
        ///   True if the ResponseCode is REFRESH_TOKEN, false otherwise
        /// </summary>
        public bool IsRefreshToken()
        {
            return Code == Constants.ResponseCode.REFRESH_TOKEN;
        }

        /// <summary>
        ///   True if the ResponseCode is TOKEN_EXPIRED, false otherwise
        /// </summary>
        public bool IsTokenExpired()
        {
            return Code == Constants.ResponseCode.TOKEN_EXPIRED;
        }
    }
}
