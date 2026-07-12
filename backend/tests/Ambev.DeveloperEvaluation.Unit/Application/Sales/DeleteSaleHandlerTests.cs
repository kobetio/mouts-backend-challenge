using Ambev.DeveloperEvaluation.Application.Sales;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Common.Caching;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class DeleteSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly ICacheService _cacheService;
    private readonly DeleteSaleHandler _handler;

    public DeleteSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _cacheService = Substitute.For<ICacheService>();
        _handler = new DeleteSaleHandler(_saleRepository, _cacheService);
    }

    [Fact(DisplayName = "Given successful delete When deleting sale Then invalidates item and list caches")]
    public async Task Handle_SuccessfulDelete_InvalidatesSalesCaches()
    {
        var saleId = Guid.NewGuid();

        _saleRepository.DeleteAsync(saleId, Arg.Any<CancellationToken>()).Returns(true);

        var result = await _handler.Handle(new DeleteSaleCommand(saleId), CancellationToken.None);

        result.Success.Should().BeTrue();
        await _cacheService.Received(1).RemoveAsync(SaleCacheKeys.Item(saleId), Arg.Any<CancellationToken>());
        await _cacheService.Received(1).RemoveByPrefixAsync(SaleCacheKeys.ListPrefix, Arg.Any<CancellationToken>());
    }
}
