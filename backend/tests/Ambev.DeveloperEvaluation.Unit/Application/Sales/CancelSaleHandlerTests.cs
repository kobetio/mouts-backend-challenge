using Ambev.DeveloperEvaluation.Application.Sales;
using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
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

public class CancelSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IEventPublisher _eventPublisher;
    private readonly ICacheService _cacheService;
    private readonly CancelSaleHandler _handler;

    public CancelSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _eventPublisher = Substitute.For<IEventPublisher>();
        _cacheService = Substitute.For<ICacheService>();
        _handler = new CancelSaleHandler(_saleRepository, _mapper, _eventPublisher, _cacheService);
    }

    [Fact(DisplayName = "Given existing sale When cancelling Then updates sale, publishes events and invalidates cache")]
    public async Task Handle_ExistingSale_CancelsPublishesAndInvalidatesCache()
    {
        var sale = SaleTestData.GenerateValidSale();
        var saleId = sale.Id;
        var result = new SaleResult { Id = saleId, Status = SaleStatus.Cancelled };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<SaleResult>(sale).Returns(result);

        var response = await _handler.Handle(new CancelSaleCommand(saleId), CancellationToken.None);

        response.Status.Should().Be(SaleStatus.Cancelled);
        sale.Status.Should().Be(SaleStatus.Cancelled);
        await _eventPublisher.Received(1).PublishAsync(Arg.Any<object>(), Arg.Any<CancellationToken>());
        await _cacheService.Received(1).RemoveAsync(SaleCacheKeys.Item(saleId), Arg.Any<CancellationToken>());
        await _cacheService.Received(1).RemoveByPrefixAsync(SaleCacheKeys.ListPrefix, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given missing sale When cancelling Then throws KeyNotFoundException")]
    public async Task Handle_MissingSale_ThrowsKeyNotFoundException()
    {
        var saleId = Guid.NewGuid();
        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        var act = () => _handler.Handle(new CancelSaleCommand(saleId), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
