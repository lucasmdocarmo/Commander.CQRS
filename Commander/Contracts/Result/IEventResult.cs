using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Commander.Contracts.Result
{
    public interface IEventResult
    {
        public HttpStatusCode StatusCode { get; }
        public string Message { get; }
        public bool IsSuccess { get;  }
    }
}
