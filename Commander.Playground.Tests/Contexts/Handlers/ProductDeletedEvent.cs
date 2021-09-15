using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander.Playground.Tests.Contexts.Handlers
{
    public class ProductDeletedEvent:Event
    {
        public ProductDeletedEvent(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}
