using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander.Playground.Tests.Contexts.Category
{
    public sealed class AddProductCommand : IRequest
    {
        public string Name { get; set; }
    }
}
