using FluentValidation;

namespace Commander.FluentValidation;

/// <summary>
/// FluentValidation-based base class. Inherit and use the standard <c>RuleFor(...)</c> DSL.
/// The validator is automatically picked up by the validation pipeline behavior when registered.
/// </summary>
public abstract class CommanderValidator<T> : AbstractValidator<T>, ICommandValidator<T> where T : Command
{
    private string? _customMessage;

    public new ICommandResult Validate(T instance)
    {
        if (instance is null)
        {
            return CommandResult.IsFailed(_customMessage ?? "Request was null.");
        }

        var result = base.Validate(instance);
        return result.IsValid
            ? CommandResult.Success()
            : CommandResult.IsFailed(_customMessage ?? result.ToString());
    }

    public new async ValueTask<ICommandResult> ValidateAsync(T request, CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            return CommandResult.IsFailed(_customMessage ?? "Request was null.");
        }

        var result = await base.ValidateAsync(request, cancellationToken).ConfigureAwait(false);
        return result.IsValid
            ? CommandResult.Success()
            : CommandResult.IsFailed(_customMessage ?? result.ToString());
    }

    /// <summary>Override the failure message returned by validation.</summary>
    public void CustomMessage(string message) => _customMessage = message;
}
