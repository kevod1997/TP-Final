namespace ProductService.API.Middleware
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Capturar y registrar la solicitud entrante
            context.Request.EnableBuffering();
            var requestBody = await ReadRequestBodyAsync(context.Request);
            var requestPath = context.Request.Path;
            var requestMethod = context.Request.Method;
            var requestQueryString = context.Request.QueryString;

            _logger.LogInformation($"Solicitud recibida: {requestMethod} {requestPath}{requestQueryString}");
            if (!string.IsNullOrEmpty(requestBody))
            {
                _logger.LogInformation($"Cuerpo de la solicitud: {requestBody}");
            }

            // Capturar la respuesta
            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error no controlado en la aplicación");
                throw; // Re-lanzar la excepción para que pueda ser manejada por el middleware de excepciones
            }
            finally
            {
                // Registrar la respuesta
                responseBody.Seek(0, SeekOrigin.Begin);
                var responseText = await new StreamReader(responseBody).ReadToEndAsync();

                _logger.LogInformation($"Respuesta enviada: {context.Response.StatusCode}");
                if (!string.IsNullOrEmpty(responseText))
                {
                    _logger.LogInformation($"Cuerpo de la respuesta: {responseText}");
                }

                // Copiar la respuesta de vuelta al stream original
                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }

        private async Task<string> ReadRequestBodyAsync(HttpRequest request)
        {
            request.Body.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(request.Body).ReadToEndAsync();
            request.Body.Seek(0, SeekOrigin.Begin);
            return text;
        }
    }
}