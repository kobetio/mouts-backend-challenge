using Ambev.DeveloperEvaluation.Application.Sales;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Common.Caching;
using Ambev.DeveloperEvaluation.Common.Events;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Services;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class UpdateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IEventPublisher _eventPublisher;
    private readonly ICacheService _cacheService;
    private readonly UpdateSaleHandler _handler;

    public UpdateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _eventPublisher = Substitute.For<IEventPublisher>();
        _cacheService = Substitute.For<ICacheService>();
        _handler = new UpdateSaleHandler(
            _saleRepository,
            _mapper,
            new TieredDiscountPolicy(),
            _eventPublisher,
            _cacheService);
    }

    [Fact(DisplayName = "Given existing sale When updating Then persists, publishes events and invalidates cache")]
    public async Task Handle_ExistingSale_UpdatesPublishesAndInvalidatesCache()
    {
        var sale = SaleTestData.GenerateValidSale();
        var item = sale.Items[0];
        var command = new UpdateSaleCommand
        {
            Id = sale.Id,
            CustomerId = sale.Customer.Id,
            CustomerName = sale.Customer.Name,
            BranchId = sale.Branch.Id,
            BranchName = sale.Branch.Name,
            SaleDate = sale.SaleDate,
            Items =
            [
                new UpdateSaleItemCommand
                {
                    Id = item.Id,
                    ProductId = item.Product.Id,
                    ProductName = item.Product.Name,
                    Quantity = 5,
                    UnitPrice = item.UnitPrice
                }
            ]
        };
        var result = new SaleResult { Id = sale.Id };

        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<SaleResult>(sale).Returns(result);

        var response = await _handler.Handle(command, CancellationToken.None);

        response.Should().BeSameAs(result);
        sale.Items.Should().HaveCount(1);
        sale.Items[0].Quantity.Should().Be(5);
        await _eventPublisher.Received(1).PublishAsync(Arg.Any<object>(), Arg.Any<CancellationToken>());
        await _cacheService.Received(1).RemoveAsync(SaleCacheKeys.Item(sale.Id), Arg.Any<CancellationToken>());
        await _cacheService.Received(1).RemoveByPrefixAsync(SaleCacheKeys.ListPrefix, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given missing sale When updating Then throws KeyNotFoundException")]
    public async Task Handle_MissingSale_ThrowsKeyNotFoundException()
    {
        var command = new UpdateSaleCommand
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            CustomerName = "Customer",
            BranchId = Guid.NewGuid(),
            BranchName = "Branch",
            SaleDate = DateTime.UtcNow,
            Items =
            [
                new UpdateSaleItemCommand
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Product",
                    Quantity = 1,
                    UnitPrice = 10m
                }
            ]
        };

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
