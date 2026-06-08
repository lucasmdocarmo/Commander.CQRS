using Commander.FluentValidation;
using FluentValidation;

namespace Commander.Playground.Tests.Contexts.Category;

public sealed class ProductCommandValidator : CommanderValidator<AddProductCommand>
{
    public ProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}
