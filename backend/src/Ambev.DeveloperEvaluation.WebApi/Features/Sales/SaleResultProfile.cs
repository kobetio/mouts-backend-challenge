using Ambev.DeveloperEvaluation.Application.Sales;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

/// <summary>
/// Shared AutoMapper profile mapping Application-layer read models to API response DTOs.
/// </summary>
public class SaleResultProfile : Profile
{
    public SaleResultProfile()
    {
        CreateMap<ExternalReferenceResult, ExternalReferenceResponse>();
        CreateMap<SaleItemResult, SaleItemResponse>();
        CreateMap<SaleResult, SaleResponse>();
    }
}
