using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander.Playground.Tests.Contexts.Category
{
    public sealed class AddProductCommand : Command
    {
        public string Name { get; set; }

        public override bool ValidateThis()
        {
           CommandValidation = new ProductCommandValidator().Validate(this);
            return CommandValidation.IsSuccess;
        }
    }
}
