using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Commander
{
    [Serializable]
    public class EventException : Exception
    {
        public string OriginalMessage { get; }
        public HttpStatusCode ResponseCode { get; set; }
        public EventException(string originalMessage) : base(originalMessage)
        {
            OriginalMessage = originalMessage;
        }
        public EventException(string originalMessage, Exception orignalException) : base(originalMessage, orignalException)
        {
            OriginalMessage = originalMessage;
        }
        public EventException(string originalMessage, Exception orignalException, HttpStatusCode responseCode) : base(originalMessage, orignalException)
        {
            OriginalMessage = originalMessage;
            ResponseCode = responseCode;
        }
    }
}
