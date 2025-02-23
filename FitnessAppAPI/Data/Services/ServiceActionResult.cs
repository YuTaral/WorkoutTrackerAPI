using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Models;
using System.Net;

namespace FitnessAppAPI.Data.Services
  
{
    /// <summary>
    ///    Class to define the result of service action.
    /// </summary>
    public class ServiceActionResult<T>
    {
        public int Code { get; set; }
        public string Message { get; set; }    
        public List<T> Data { get; set; }

        public ServiceActionResult(HttpStatusCode CodeVal)
        {
            Code = (int) CodeVal;
            Message = Constants.MSG_SUCCESS;
            Data = [];
        }

        public ServiceActionResult(HttpStatusCode CodeVal, string MessageVal) {
            Code = (int) CodeVal;
            Message = MessageVal;
            Data = [];
        }

        public ServiceActionResult(HttpStatusCode CodeVal, string MessageVal, List<T> DataVal)
        {
            Code = (int) CodeVal;
            Message = MessageVal;
            Data = DataVal;
        }

        public ServiceActionResult(int CodeVal, string MessageVal, List<T> DataVal)
        {
            Code = CodeVal;
            Message = MessageVal;
            Data = DataVal;
        }

        /// <summary>
        ///   True if the HttpStatusCode is SUCCESS, false otherwise
        /// </summary>
        public bool IsSuccess()
        {
            return Code >= 200 && Code < 300;
        }
    }
}
