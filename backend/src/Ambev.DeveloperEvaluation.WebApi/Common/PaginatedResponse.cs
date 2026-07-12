namespace Ambev.DeveloperEvaluation.WebApi.Common;

/// <summary>
/// Envelope for paginated list responses. The paginated payload fields
/// (<c>totalItems</c>, <c>currentPage</c>, <c>totalPages</c>) match the exact wire format
/// required by the project's general API conventions (§3.7); the inherited
/// <c>Success</c>/<c>Message</c>/<c>Errors</c> fields are kept on top for consistency with
/// every other response in this API.
/// </summary>
public class PaginatedResponse<T> : ApiResponseWithData<IEnumerable<T>>
{
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int TotalItems { get; set; }
}