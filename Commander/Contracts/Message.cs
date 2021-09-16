using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander
{
    public abstract class Message
    {
        public DateTime Timestamp { get; protected set; }
    }
}
