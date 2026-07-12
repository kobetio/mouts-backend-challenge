using Ambev.DeveloperEvaluation.Application.Sales;
using Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;
using Ambev.DeveloperEvaluation.Common.Caching;
using Ambev.DeveloperEvaluation.Common.Events;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class CancelSaleItemHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IEventPublisher _eventPublisher;
    private readonly ICacheService _cacheService;
    private readonly CancelSaleItemHandler _handler;

    public CancelSaleItemHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _eventPublisher = Substitute.For<IEventPublisher>();
        _cacheService = Substitute.For<ICacheService>();
        _handler = new CancelSaleItemHandler(_saleRepository, _mapper, _eventPublisher, _cacheService);
    }

    [Fact(DisplayName = "Given existing item When cancelling Then updates sale, publishes events and invalidates cache")]
    public async Task Handle_ExistingItem_CancelsPublishesAndInvalidatesCache()
    {
        var sale = SaleTestData.GenerateValidSaleWithItems((2, 10m), (3, 20m));
        var itemId = sale.Items[0].Id;
        var result = new SaleResult { Id = sale.Id };

        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<SaleResult>(sale).Returns(result);

        await _handler.Handle(new CancelSaleItemCommand(sale.Id, itemId), CancellationToken.None);

        sale.Items[0].Status.Should().Be(SaleItemStatus.Cancelled);
        await _eventPublisher.Received(1).PublishAsync(Arg.Any<object>(), Arg.Any<CancellationToken>());
        await _cacheService.Received(1).RemoveAsync(SaleCacheKeys.Item(sale.Id), Arg.Any<CancellationToken>());
        await _cacheService.Received(1).RemoveByPrefixAsync(SaleCacheKeys.ListPrefix, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given missing sale When cancelling item Then throws KeyNotFoundException")]
    public async Task Handle_MissingSale_ThrowsKeyNotFoundException()
    {
        var saleId = Guid.NewGuid();
        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        var act = () => _handler.Handle(new CancelSaleItemCommand(saleId, Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
