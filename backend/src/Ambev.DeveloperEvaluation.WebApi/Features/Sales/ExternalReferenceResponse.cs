namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

/// <summary>
/// API response shape for an external reference (Customer/Branch/Product), mirroring the domain's
/// "External Identities" pattern (Id + denormalized name captured at the time of the sale).
/// </summary>
public class ExternalReferenceResponse
{
    /// <summary>
    /// The external entity's unique identifier, owned by another domain.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Denormalized copy of the external entity's descriptive name at the time of the sale.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
