using Commander.Contracts.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Commander
{
    public class EventResult: IEventResult
    {
        public EventResult() => IsSuccess = true;

        public EventResult(string message, bool success)
        {
            IsSuccess = success;
            Message = message;
        }
        public EventResult(string message, bool success, HttpStatusCode statusCode)
        {
            IsSuccess = success;
            Message = message;
            StatusCode = statusCode;
        }
        public HttpStatusCode StatusCode { get; protected set; }
        public string Message { get; protected set; }
        public bool IsSuccess { get; protected set; }

        public static IEventResult Success() => new EventResult();

        public static IEventResult Success(string message) => new EventResult(message, true);

        public static ValueTask<IEventResult> SuccessAsync() => ValueTask.FromResult(Success());

        public static ValueTask<IEventResult> SuccessAsync(string message) => ValueTask.FromResult(Success(message));

        public static IEventResult IsFailed(string message) => new EventResult(message, false);

        public static ValueTask<IEventResult> FailAsync(string message) => ValueTask.FromResult(IsFailed(message));
    }
}
