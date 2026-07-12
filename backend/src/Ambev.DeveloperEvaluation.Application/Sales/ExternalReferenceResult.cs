namespace Ambev.DeveloperEvaluation.Application.Sales;

/// <summary>
/// Read-model shape for an external reference (Customer/Branch/Product), mirroring the
/// domain's <c>ExternalReference</c> value object (Id + denormalized name).
/// </summary>
public class ExternalReferenceResult
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
