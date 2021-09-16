using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander
{
    public abstract class Event: Message
    {
        public Guid AggreggateId { get; private set; }
        public void SetAggreggateId(Guid id)
        {
            this.AggreggateId = id;
        }
        protected Event()
        {
            Timestamp = DateTime.Now;
        }
    }
}
