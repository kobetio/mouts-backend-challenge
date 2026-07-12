namespace Ambev.DeveloperEvaluation.Domain.ValueObjects;

/// <summary>
/// External reference to a Product (owned by the Product domain), with a denormalized name.
/// </summary>
public class ProductReference : ExternalReference
{
    public ProductReference()
    {
    }

    public ProductReference(Guid id, string name) : base(id, name)
    {
    }
}
