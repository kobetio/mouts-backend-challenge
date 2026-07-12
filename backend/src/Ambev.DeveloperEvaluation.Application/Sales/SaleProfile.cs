using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales;

/// <summary>
/// Shared AutoMapper profile for Sale/SaleItem read-model mappings. Kept in one place (instead
/// of duplicated per operation, as with the Users feature) because every Sales use case returns
/// the exact same <see cref="SaleResult"/> shape.
/// </summary>
public class SaleProfile : Profile
{
    public SaleProfile()
    {
        CreateMap<CustomerReference, ExternalReferenceResult>();
        CreateMap<BranchReference, ExternalReferenceResult>();
        CreateMap<ProductReference, ExternalReferenceResult>();

        CreateMap<SaleItem, SaleItemResult>();
        CreateMap<Sale, SaleResult>();
    }
}
