using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander.Playground.Tests.Contexts.Category
{
    public sealed class ProductCommandValidator : CommanderValidator<AddProductCommand>
    {
        public ProductCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}
