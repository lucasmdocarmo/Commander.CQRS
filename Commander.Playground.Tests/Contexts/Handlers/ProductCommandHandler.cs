using Commander.Playground.Tests.Contexts.Category;

namespace Commander.Playground.Tests;

public sealed class ProductCommandHandler : ICommandHandler<AddProductCommand, Product>
{
    public ValueTask<ICommandResult<Product>> Execute(AddProductCommand request, CancellationToken cancellationToken = default)
        => CommandResult<Product>.SuccessAsync(new Product { Name = request.Name });
}
