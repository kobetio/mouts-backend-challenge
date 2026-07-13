using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ambev.DeveloperEvaluation.WebApi.Common;

[Route("api/[controller]")]
[ApiController]
public class BaseController : ControllerBase
{
    protected int GetCurrentUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new NullReferenceException());

    protected string GetCurrentUserEmail() =>
        User.FindFirst(ClaimTypes.Email)?.Value ?? throw new NullReferenceException();

    protected IActionResult Ok<T>(T data) =>
            base.Ok(new ApiResponseWithData<T> { Data = data, Success = true });

    /// <summary>
    /// Returns a pre-built response envelope as-is (no second wrap). Use when the action
    /// already constructs <see cref="ApiResponse"/>, <see cref="ApiResponseWithData{T}"/>,
    /// or <see cref="PaginatedResponse{T}"/>.
    /// </summary>
    protected IActionResult OkEnvelope(object response) => new OkObjectResult(response);

    protected IActionResult Created<T>(string routeName, object routeValues, T data) =>
        base.CreatedAtRoute(routeName, routeValues, new ApiResponseWithData<T> { Data = data, Success = true });

    protected IActionResult BadRequest(string message) =>
        base.BadRequest(new ApiResponse { Message = message, Success = false });

    protected IActionResult BadRequestError(string type, string error, string detail) =>
        base.BadRequest(new ErrorResponse { Type = type, Error = error, Detail = detail });

    protected IActionResult BadRequestValidationErrors(IEnumerable<ValidationFailure> errors) =>
        BadRequestError(
            "ValidationError",
            "Validation failed",
            string.Join("; ", errors.Select(e => e.ErrorMessage)));

    protected IActionResult NotFound(string message = "Resource not found") =>
        base.NotFound(new ApiResponse { Message = message, Success = false });

    protected IActionResult NotFoundError(string detail) =>
        base.NotFound(new ErrorResponse
        {
            Type = "ResourceNotFound",
            Error = "Resource not found",
            Detail = detail
        });

    // Calls the framework's ControllerBase.Ok(object) directly (not the Ok<T> helper above) —
    // PaginatedResponse<T> is already a full response envelope, so wrapping it again in
    // ApiResponseWithData<T> would double-nest the payload.
    protected IActionResult OkPaginated<T>(PaginatedList<T> pagedList) =>
            OkEnvelope(new PaginatedResponse<T>
            {
                Data = pagedList,
                CurrentPage = pagedList.CurrentPage,
                TotalPages = pagedList.TotalPages,
                TotalItems = pagedList.TotalCount,
                Success = true
            });
}
