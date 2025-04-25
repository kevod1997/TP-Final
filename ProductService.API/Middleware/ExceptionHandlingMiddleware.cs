using FluentValidation;
using System.Net;
using System.Text.Json;
using TrabajoFinal.Common.SharedKernel.Models;

namespace ProductService.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error no controlado en la aplicación");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var statusCode = HttpStatusCode.InternalServerError;
            var message = "Se produjo un error interno. Por favor, inténtelo de nuevo más tarde.";
            var errors = new List<string>();

            // Personalizar el mensaje de error según el tipo de excepción
            if (exception is ArgumentException || exception is FormatException)
            {
                statusCode = HttpStatusCode.BadRequest;
                message = "Solicitud inválida";
                errors.Add(exception.Message);
            }
            else if (exception is UnauthorizedAccessException)
            {
                statusCode = HttpStatusCode.Unauthorized;
                message = "No está autorizado para realizar esta acción";
            }
            else if (exception is InvalidOperationException)
            {
                statusCode = HttpStatusCode.BadRequest;
                message = "Operación inválida";
                errors.Add(exception.Message);
            }
            else if (exception is ValidationException validationEx)
            {
                statusCode = HttpStatusCode.BadRequest;
                message = "Error de validación";
                errors = validationEx.Errors.Select(e => e.ErrorMessage).ToList();
            }

            var response = ApiResponse<object>.FailureResponse(message, errors);

            context.Response.StatusCode = (int)statusCode;
            var result = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(result);
        }
    }
}