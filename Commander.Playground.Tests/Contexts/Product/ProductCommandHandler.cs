using Commander.Playground.Tests.Contexts.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander.Playground.Tests
{
    public class ProductCommandHandler : ICommandHandler<AddProductCommand, Product>
    {
        public async ValueTask<ICommandResult<Product>> Execute(AddProductCommand request)
        {
            return await CommandResult<Product>.SuccessAsync(new Product());
        }
    }
}

