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
                    "(sale and individual items), paginated/sorted/filtered listing, and " +
                    "tiered quantity discounts. See the general API conventions (§3.7) for " +
                    "pagination, sorting, and filtering query parameters."
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
