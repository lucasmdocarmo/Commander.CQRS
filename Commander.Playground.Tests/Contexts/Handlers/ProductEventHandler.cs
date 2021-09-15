using Commander.Contracts.Result;
using Commander.Playground.Tests.Contexts.Handlers;
using Commander.Playground.Tests.Contexts.Product.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander.Playground.Tests
{
    public class ProductEventHandler : IEventHandler<ProductAddedEvent>, IEventHandler<ProductDeletedEvent>
    {
        public async ValueTask<IEventResult> Publish(ProductAddedEvent request)
        {
            return await EventResult.SuccessAsync();
        }

        public async ValueTask<IEventResult> Publish(ProductDeletedEvent request)
        {
            return new EventResult("Event Published!", true, System.Net.HttpStatusCode.OK);
        }
    }
}
