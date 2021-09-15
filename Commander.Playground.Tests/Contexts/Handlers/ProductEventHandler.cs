using Commander.Playground.Tests.Contexts.Product.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander.Playground.Tests
{
    public class ProductEventHandler : IEventHandler<ProductAddedEvent>
    {
        public async ValueTask Publish(ProductAddedEvent request)
        {
            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}
