namespace Ambev.DeveloperEvaluation.Domain.ValueObjects;

/// <summary>
/// External reference to a Branch (owned by the Branch domain), with a denormalized name.
/// </summary>
public class BranchReference : ExternalReference
{
    public BranchReference()
    {
    }

    public BranchReference(Guid id, string name) : base(id, name)
    {
    }
}
