namespace Commander;

/// <summary>
/// Base type for read-only queries.
/// </summary>
public abstract class Query : Message
{
    /// <summary>Optional snapshot of the validation result associated with this query.</summary>
    public IQueryResult? Validations { get; set; }

    /// <summary>Hook for handler-side ad-hoc validation. Default is no-op (valid).</summary>
    public virtual bool ValidateThis() => true;
}
