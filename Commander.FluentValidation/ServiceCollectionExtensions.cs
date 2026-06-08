using Commander.Validation;

namespace Commander.FluentValidation;

/// <summary>
/// Registration helpers for the FluentValidation adapter.
/// </summary>
public static class CommanderFluentValidationExtensions
{
    /// <summary>
    /// Add the validation pipeline behavior so any registered <see cref="ICommandValidator{T}"/>
    /// (including FluentValidation-based <see cref="CommanderValidator{T}"/>) runs before each command handler.
    /// </summary>
    public static CommanderServiceConfiguration AddFluentValidation(this CommanderServiceConfiguration cfg)
    {
        cfg.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));
        return cfg;
    }
}
