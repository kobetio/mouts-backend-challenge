namespace Ambev.DeveloperEvaluation.Domain.ValueObjects;

/// <summary>
/// Base value object for the "External Identities" DDD pattern described in the project's
/// business rules: entities from other domains (Customer, Product, Branch) are never referenced
/// directly. Instead, the sale stores the external entity's Id plus a denormalized copy of its
/// descriptive data at the time of the sale, so the sale stays historically accurate even if the
/// original entity is later changed or removed.
/// </summary>
public abstract class ExternalReference
{
    /// <summary>
    /// The external entity's unique identifier, owned by another domain.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Denormalized copy of the external entity's descriptive name, captured at the time of the sale.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    protected ExternalReference()
    {
    }

    protected ExternalReference(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}
