using Ambev.DeveloperEvaluation.Application.Sales;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Common.Caching;
using Ambev.DeveloperEvaluation.Common.Events;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Services;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class CreateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IDiscountPolicy _discountPolicy;
    private readonly IEventPublisher _eventPublisher;
    private readonly ICacheService _cacheService;
    private readonly CreateSaleHandler _handler;

    public CreateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _discountPolicy = new TieredDiscountPolicy();
        _eventPublisher = Substitute.For<IEventPublisher>();
        _cacheService = Substitute.For<ICacheService>();
        _handler = new CreateSaleHandler(
            _saleRepository,
            _mapper,
            _discountPolicy,
            _eventPublisher,
            _cacheService);
    }

    [Fact(DisplayName = "Given valid command When creating sale Then persists, publishes events and invalidates cache")]
    public async Task Handle_ValidCommand_PersistsPublishesAndInvalidatesCache()
    {
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var result = new SaleResult { Id = Guid.NewGuid(), SaleNumber = 1 };

        _saleRepository
            .CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Sale>());
        _mapper.Map<SaleResult>(Arg.Any<Sale>()).Returns(result);

        var response = await _handler.Handle(command, CancellationToken.None);

        response.Should().BeSameAs(result);
        await _saleRepository.Received(1).CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
        await _eventPublisher.Received(1).PublishAsync(
            Arg.Any<object>(),
            Arg.Any<CancellationToken>());
        await _cacheService.Received(1).RemoveAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _cacheService.Received(1).RemoveByPrefixAsync(SaleCacheKeys.ListPrefix, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given invalid command When creating sale Then throws validation exception")]
    public async Task Handle_InvalidCommand_ThrowsValidationException()
    {
        var command = new CreateSaleCommand();

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact(DisplayName = "Given quantity above 20 When creating sale Then throws DomainException")]
    public async Task Handle_QuantityAboveTwenty_ThrowsDomainException()
    {
        var command = CreateSaleHandlerTestData.GenerateCommandWithQuantity(21);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*Cannot sell more than 20 identical items*");
    }
}
