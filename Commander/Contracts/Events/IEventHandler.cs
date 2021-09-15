using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander
{
    public interface IEventHandler<in TRequest> where TRequest : Event
    {
        ValueTask Publish(TRequest request);
    }
}
