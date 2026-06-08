namespace Commander;

/// <summary>
/// Base type for state-changing commands.
/// </summary>
public abstract class Command : Message
{
    /// <summary>Optional snapshot of the validation result associated with this command.</summary>
    public ICommandResult? Validations { get; set; }

    /// <summary>
    /// Hook for handler-side ad-hoc validation. Prefer registering an <see cref="ICommandValidator{T}"/>
    /// (or the FluentValidation adapter) so the validation pipeline behavior runs automatically.
    /// </summary>
    public virtual bool ValidateThis() => true;
}
