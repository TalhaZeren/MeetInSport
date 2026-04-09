using System.Net;
using MeetInSport.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MeetInSport.WebApi.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            // Try to process the request normally
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            // If anything fails anywhere in the app, it drops down to here.
            _logger.LogError(ex, "Bir şeyler ters gitti : {Message}", ex.Message);
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        // Default to 500 Internal Server Error
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        var message = "Internal Server Error from the custom middleware.";

        // If it's our custom NotFoundException, change it to a 404
        if (exception is NotFoundException notFoundException)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            message = notFoundException.Message;
        }
        // You can add more 'if' blocks here later for ValidationExceptions, UnauthorizedExceptions, etc.

        var errorDetails = new ErrorDetails
        {
            StatusCode = context.Response.StatusCode,
            Message = message
        };

        await context.Response.WriteAsync(errorDetails.ToString());
    }
}