using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Ambev.DeveloperEvaluation.WebApi;

/// <summary>
/// Swashbuckle/OpenAPI configuration helpers.
/// </summary>
public static class SwaggerExtensions
{
    /// <summary>
    /// Registers Swagger generation with XML doc comments for every endpoint, parameter,
    /// request/response schema, and error response (§3.5).
    /// </summary>
    public static IServiceCollection AddSalesApiSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Sales API",
                Version = "v1",
                Description =
                    "REST API for managing sales records. Supports CRUD, cancellation " +
                    "(entire sale or individual items), paginated/sorted/filtered listing, and " +
                    "tiered quantity-based discounts (0% below 4 units, 10% for 4–9, 20% for 10–20; " +
                    "more than 20 units per product line is rejected).\n\n" +
                    "**List query parameters** (`GET /api/sales`):\n" +
                    "- Pagination: `_page` (default 1), `_size` (default 10, max 100)\n" +
                    "- Sorting: `_order` using response field names (e.g. `saleDate desc, saleNumber asc`)\n" +
                    "- Filters: `customerName` / `customer`, `branchName` / `branch` (supports `*` wildcard), " +
                    "`cancelled`, `customerId`, `branchId`, `_minTotalAmount`, `_maxTotalAmount`, `_minDate`, `_maxDate`\n\n" +
                    "**Errors:** 4xx/5xx responses use `{ type, error, detail }`."
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }
        });

        return services;
    }
}
