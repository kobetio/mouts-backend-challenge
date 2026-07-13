using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ListSales;

/// <summary>
/// Query-string parameters for <c>GET /api/sales</c>. Bound from the request query and mapped
/// to <see cref="ListSalesCommand"/> for the application layer.
/// </summary>
public class SaleListQueryRequest
{
    /// <summary>
    /// Page number (1-based). Default: 1.
    /// </summary>
    [FromQuery(Name = "_page")]
    public int? Page { get; set; }

    /// <summary>
    /// Items per page. Default: 10. Maximum: 100.
    /// </summary>
    [FromQuery(Name = "_size")]
    public int? Size { get; set; }

    /// <summary>
    /// Sort order using response field names, e.g. <c>saleDate desc, saleNumber asc</c>.
    /// </summary>
    [FromQuery(Name = "_order")]
    public string? OrderBy { get; set; }

    /// <summary>
    /// Filter by customer name. Supports <c>*</c> wildcard (e.g. <c>John*</c>).
    /// </summary>
    [FromQuery(Name = "customerName")]
    public string? CustomerName { get; set; }

    /// <summary>
    /// Alias for <see cref="CustomerName"/>.
    /// </summary>
    [FromQuery(Name = "customer")]
    public string? Customer { get; set; }

    /// <summary>
    /// Filter by branch name. Supports <c>*</c> wildcard (e.g. <c>Downtown*</c>).
    /// </summary>
    [FromQuery(Name = "branchName")]
    public string? BranchName { get; set; }

    /// <summary>
    /// Alias for <see cref="BranchName"/>.
    /// </summary>
    [FromQuery(Name = "branch")]
    public string? Branch { get; set; }

    /// <summary>
    /// Filter by cancellation status (<c>true</c> = cancelled, <c>false</c> = active).
    /// </summary>
    [FromQuery(Name = "cancelled")]
    public bool? Cancelled { get; set; }

    /// <summary>
    /// Filter by external customer Id.
    /// </summary>
    [FromQuery(Name = "customerId")]
    public Guid? CustomerId { get; set; }

    /// <summary>
    /// Filter by external branch Id.
    /// </summary>
    [FromQuery(Name = "branchId")]
    public Guid? BranchId { get; set; }

    /// <summary>
    /// Minimum sale total amount (inclusive).
    /// </summary>
    [FromQuery(Name = "_minTotalAmount")]
    public decimal? MinTotalAmount { get; set; }

    /// <summary>
    /// Maximum sale total amount (inclusive).
    /// </summary>
    [FromQuery(Name = "_maxTotalAmount")]
    public decimal? MaxTotalAmount { get; set; }

    /// <summary>
    /// Minimum sale date (inclusive).
    /// </summary>
    [FromQuery(Name = "_minDate")]
    public DateTime? MinDate { get; set; }

    /// <summary>
    /// Maximum sale date (inclusive).
    /// </summary>
    [FromQuery(Name = "_maxDate")]
    public DateTime? MaxDate { get; set; }

    /// <summary>
    /// Maps this request to a <see cref="ListSalesCommand"/>.
    /// </summary>
    public ListSalesCommand ToCommand()
    {
        var command = new ListSalesCommand
        {
            Page = Page.HasValue ? Math.Max(1, Page.Value) : 1,
            Size = Size.HasValue ? Math.Clamp(Size.Value, 1, 100) : 10,
            OrderBy = OrderBy,
            CustomerId = CustomerId,
            BranchId = BranchId,
            CustomerName = CustomerName ?? Customer,
            BranchName = BranchName ?? Branch,
            IsCancelled = Cancelled,
            MinTotalAmount = MinTotalAmount,
            MaxTotalAmount = MaxTotalAmount,
            MinSaleDate = MinDate,
            MaxSaleDate = MaxDate
        };

        return command;
    }
}
