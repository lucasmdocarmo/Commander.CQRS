using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Commander
{
    [Serializable]
    public class CommandException:Exception
    {
        public string OriginalMessage { get; }
        public HttpStatusCode ResponseCode { get; set; }
        public CommandException(string originalMessage) : base(originalMessage)
        {
            OriginalMessage = originalMessage;
        }
        public CommandException(string originalMessage, Exception orignalException) : base(originalMessage, orignalException)
        {
            OriginalMessage = originalMessage;
        }
        public CommandException(string originalMessage, Exception orignalException, HttpStatusCode responseCode) : base(originalMessage, orignalException)
        {
            OriginalMessage = originalMessage;
            ResponseCode = responseCode;
        }
    }
}
