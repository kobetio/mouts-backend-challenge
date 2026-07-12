using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Microsoft.Extensions.Primitives;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ListSales;

/// <summary>
/// Parses raw HTTP query-string parameters into a structured <see cref="ListSalesCommand"/>,
/// implementing the general API conventions (§3.7): <c>_page</c>, <c>_size</c>, <c>_order</c>,
/// field filters, <c>*</c> wildcards, and <c>_min</c>/<c>_max</c> range prefixes.
/// </summary>
public static class SaleListQueryParser
{
    private static readonly HashSet<string> ReservedKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "_page", "_size", "_order"
    };

    /// <summary>
    /// Builds a <see cref="ListSalesCommand"/> from the incoming request's query collection.
    /// </summary>
    public static ListSalesCommand Parse(IQueryCollection query)
    {
        var command = new ListSalesCommand();

        if (query.TryGetValue("_page", out StringValues pageValue) && int.TryParse(pageValue, out var page))
        {
            command.Page = Math.Max(1, page);
        }

        if (query.TryGetValue("_size", out StringValues sizeValue) && int.TryParse(sizeValue, out var size))
        {
            command.Size = Math.Clamp(size, 1, 100);
        }

        if (query.TryGetValue("_order", out StringValues orderValue))
        {
            command.OrderBy = orderValue.ToString();
        }

        foreach (var parameter in query)
        {
            var key = parameter.Key;
            var value = parameter.Value.ToString();

            if (string.IsNullOrWhiteSpace(value) || ReservedKeys.Contains(key))
            {
                continue;
            }

            if (TryApplyRangeFilter(key, value, command))
            {
                continue;
            }

            ApplyFieldFilter(key, value, command);
        }

        return command;
    }

    private static bool TryApplyRangeFilter(string key, string value, ListSalesCommand command)
    {
        if (key.Equals("_minTotalAmount", StringComparison.OrdinalIgnoreCase)
            && decimal.TryParse(value, out var minTotal))
        {
            command.MinTotalAmount = minTotal;
            return true;
        }

        if (key.Equals("_maxTotalAmount", StringComparison.OrdinalIgnoreCase)
            && decimal.TryParse(value, out var maxTotal))
        {
            command.MaxTotalAmount = maxTotal;
            return true;
        }

        if (key.Equals("_minDate", StringComparison.OrdinalIgnoreCase)
            && DateTime.TryParse(value, out var minDate))
        {
            command.MinSaleDate = minDate;
            return true;
        }

        if (key.Equals("_maxDate", StringComparison.OrdinalIgnoreCase)
            && DateTime.TryParse(value, out var maxDate))
        {
            command.MaxSaleDate = maxDate;
            return true;
        }

        return false;
    }

    private static void ApplyFieldFilter(string key, string value, ListSalesCommand command)
    {
        switch (key.ToLowerInvariant())
        {
            case "cancelled":
                if (bool.TryParse(value, out var cancelled))
                {
                    command.IsCancelled = cancelled;
                }
                break;

            case "customername":
            case "customer":
                command.CustomerName = value;
                break;

            case "branchname":
            case "branch":
                command.BranchName = value;
                break;

            case "customerid":
                if (Guid.TryParse(value, out var customerId))
                {
                    command.CustomerId = customerId;
                }
                break;

            case "branchid":
                if (Guid.TryParse(value, out var branchId))
                {
                    command.BranchId = branchId;
                }
                break;
        }
    }
}
