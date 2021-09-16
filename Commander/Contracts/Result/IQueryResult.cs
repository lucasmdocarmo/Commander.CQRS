using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Commander
{
    public interface IQueryResult<out T>: IQueryResult
    {
        public T Data { get; }
    }
    public interface IQueryResult
    {
        public HttpStatusCode StatusCode { get; }
        public string Message { get; }
        public bool IsSuccess { get; }
    }
}
