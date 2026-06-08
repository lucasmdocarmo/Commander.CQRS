namespace Commander;

/// <summary>
/// Provider-agnostic validator contract. Plug in FluentValidation, DataAnnotations,
/// or a hand-rolled validator by implementing this interface.
/// </summary>
public interface ICommandValidator<in TRequest> where TRequest : Command
{
    ValueTask<ICommandResult> ValidateAsync(TRequest request, CancellationToken cancellationToken = default);
}
