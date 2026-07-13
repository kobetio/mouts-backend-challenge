namespace Ambev.DeveloperEvaluation.WebApi.Common;

/// <summary>
/// Standard error envelope for every 4xx/5xx response: the body has exactly these three fields.
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Machine-readable error type identifier (e.g. "ValidationError", "ResourceNotFound").
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Short, human-readable summary of the problem.
    /// </summary>
    public string Error { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable explanation specific to this occurrence.
    /// </summary>
    public string Detail { get; set; } = string.Empty;
}
