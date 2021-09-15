using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander.Playground.Tests.Contexts.Product.Events
{
    public class ProductAddedEvent: Event
    {
        public ProductAddedEvent(string productName)
        {
            ProductName = productName;
        }

        public string ProductName { get; set; }
    }
}
