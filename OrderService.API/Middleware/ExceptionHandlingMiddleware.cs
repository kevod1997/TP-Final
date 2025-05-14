using OrderService.Domain.Exceptions;
using System.Net;
using System.Text.Json;
using TrabajoFinal.Common.Shared.Logging;

namespace OrderService.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggerService _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILoggerService logger)
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
            // Asegúrate de que el cuerpo de la respuesta no se haya enviado aún
            if (!context.Response.HasStarted)
            {
                context.Response.Clear();
                context.Response.ContentType = "application/json";

                var response = new ErrorResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = "Se produjo un error interno. Por favor, inténtelo de nuevo más tarde."
                };

                // Personalizar el mensaje de error según el tipo de excepción
                if (exception is ArgumentException argEx)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Message = argEx.Message;
                    response.Details = argEx.ParamName != null ? $"Parámetro: {argEx.ParamName}" : null;
                }
                else if (exception is FluentValidation.ValidationException valEx)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Message = "Error de validación";
                    response.Details = string.Join("; ", valEx.Errors.Select(e => e.ErrorMessage));
                }
                else if (exception is OrderDomainException orderEx)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Message = orderEx.Message;
                }
                else if (exception is FormatException)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Message = exception.Message;
                }
                else if (exception is UnauthorizedAccessException)
                {
                    response.StatusCode = HttpStatusCode.Unauthorized;
                    response.Message = "No está autorizado para realizar esta acción.";
                }
                else if (exception is InvalidOperationException)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Message = exception.Message;
                }

                context.Response.StatusCode = (int)response.StatusCode;
                var result = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(result);
            }
        }
    }

    public class ErrorResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
    }
}