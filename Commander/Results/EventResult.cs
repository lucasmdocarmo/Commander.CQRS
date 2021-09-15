using Commander.Contracts.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Commander.Results
{
    public class EventResult: IEventResult
    {
        public EventResult() => IsSuccess = true;

        protected EventResult(string message, bool success)
        {
            IsSuccess = success;
            Message = message;
        }
        protected EventResult(string message, bool success, HttpStatusCode statusCode)
        {
            IsSuccess = success;
            Message = message;
            StatusCode = statusCode;
        }
        public HttpStatusCode StatusCode { get; protected set; }
        public string Message { get; protected set; }
        public bool IsSuccess { get; protected set; }
    }
}
