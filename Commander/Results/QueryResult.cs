using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Commander
{
    public class QueryResult<T> : IQueryResult<T>
    {
        private QueryResult(T data, bool success)
        {
            IsSuccess = success;
            Data = data;
        }
        public QueryResult(string message, bool success)
        {
            IsSuccess = success;
            Message = message;
        }
        public QueryResult(T data, string message, bool success)
        {
            IsSuccess = success;
            Message = message;
            Data = data;
        }
        public string Message { get; protected set; }
        public bool IsSuccess { get; protected set; }
        public T Data { get; }
        public HttpStatusCode StatusCode { get; protected set; }

        public static IQueryResult<T> Success(T data) => new QueryResult<T>(data, true);
        public static IQueryResult<T> Success(string message, bool success) => new QueryResult<T>(message, success);
        public static IQueryResult<T> Success(T data, string message, bool success) => new QueryResult<T>(data, message, success);
        public static ValueTask<IQueryResult<T>> SuccessAsync(T data) => ValueTask.FromResult(Success(data));
        public static ValueTask<IQueryResult<T>> SuccessAsync(T data, string message, bool success) => ValueTask.FromResult(Success(data, message, success));
        public new static IQueryResult<T> IsFailed(T data, bool success) => new QueryResult<T>(data, success);
        public new static ValueTask<IQueryResult<T>> FailAsync(T data, bool success) => ValueTask.FromResult(IsFailed(data, success));
    }
}
