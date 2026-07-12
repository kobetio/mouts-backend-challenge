namespace Ambev.DeveloperEvaluation.Domain.ValueObjects;

/// <summary>
/// External reference to a Customer (owned by the Customer domain), with a denormalized name.
/// </summary>
public class CustomerReference : ExternalReference
{
    public CustomerReference()
    {
    }

    public CustomerReference(Guid id, string name) : base(id, name)
    {
    }
}
