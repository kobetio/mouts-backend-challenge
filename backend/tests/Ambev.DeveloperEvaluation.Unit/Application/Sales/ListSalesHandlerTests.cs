using Ambev.DeveloperEvaluation.Application.Sales;
using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Ambev.DeveloperEvaluation.Common.Caching;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class ListSalesHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;
    private readonly ListSalesHandler _handler;

    public ListSalesHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _cacheService = Substitute.For<ICacheService>();
        _handler = new ListSalesHandler(_saleRepository, _mapper, _cacheService);
    }

    [Fact(DisplayName = "Given cached list When listing sales Then returns cached result without hitting repository")]
    public async Task Handle_CacheHit_ReturnsCachedResultWithoutRepositoryCall()
    {
        var command = new ListSalesCommand { Page = 1, Size = 10 };
        var cacheKey = SaleCacheKeys.List(command);
        var cachedResult = new ListSalesResult
        {
            Items = [new SaleResult { Id = Guid.NewGuid(), SaleNumber = 1 }],
            TotalItems = 1,
            CurrentPage = 1,
            TotalPages = 1
        };

        _cacheService
            .GetAsync<ListSalesResult>(cacheKey, Arg.Any<CancellationToken>())
            .Returns(cachedResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeSameAs(cachedResult);
        await _saleRepository.DidNotReceive().ListAsync(Arg.Any<SaleListFilter>(), Arg.Any<CancellationToken>());
        await _cacheService.DidNotReceive().SetAsync(
            Arg.Any<string>(),
            Arg.Any<ListSalesResult>(),
            Arg.Any<TimeSpan?>(),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given cache miss When listing sales Then loads from repository and stores in cache")]
    public async Task Handle_CacheMiss_LoadsFromRepositoryAndStoresInCache()
    {
        var command = new ListSalesCommand { Page = 2, Size = 5, OrderBy = "saleDate desc" };
        var cacheKey = SaleCacheKeys.List(command);
        var mappedItems = new List<SaleResult> { new() { Id = Guid.NewGuid(), SaleNumber = 99 } };

        _cacheService
            .GetAsync<ListSalesResult>(cacheKey, Arg.Any<CancellationToken>())
            .Returns((ListSalesResult?)null);
        _saleRepository
            .ListAsync(Arg.Any<SaleListFilter>(), Arg.Any<CancellationToken>())
            .Returns(new PagedResult<Sale> { Items = [], TotalCount = 0 });
        _mapper.Map<IReadOnlyList<SaleResult>>(Arg.Any<IReadOnlyList<Sale>>()).Returns(mappedItems);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.TotalItems.Should().Be(0);
        result.CurrentPage.Should().Be(2);
        await _saleRepository.Received(1).ListAsync(Arg.Any<SaleListFilter>(), Arg.Any<CancellationToken>());
        await _cacheService.Received(1).SetAsync(
            cacheKey,
            Arg.Is<ListSalesResult>(r => r.CurrentPage == 2),
            SaleCacheKeys.ListTtl,
            Arg.Any<CancellationToken>());
    }
}
