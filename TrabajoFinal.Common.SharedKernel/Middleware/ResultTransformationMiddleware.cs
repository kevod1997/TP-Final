using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using TrabajoFinal.Common.Shared.Logging;

namespace TrabajoFinal.Common.Shared.Middleware
{
    /// <summary>
    /// Middleware para transformar objetos Result en respuestas HTTP
    /// </summary>
    public class ResultTransformationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggerService _logger;

        public ResultTransformationMiddleware(RequestDelegate next, ILoggerService logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Guardar la referencia al método original
            var originalBodyStream = context.Response.Body;

            // Interceptar la respuesta
            await using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            // Continuar con el pipeline
            await _next(context);

            // Si la respuesta ya se ha enviado, no hacer nada
            if (context.Response.HasStarted)
                return;

            // Leer la respuesta
            responseBody.Seek(0, SeekOrigin.Begin);
            var responseBodyText = await new StreamReader(responseBody).ReadToEndAsync();
            responseBody.Seek(0, SeekOrigin.Begin);

            // Si la respuesta está vacía, no hacer nada
            if (string.IsNullOrWhiteSpace(responseBodyText))
            {
                await responseBody.CopyToAsync(originalBodyStream);
                return;
            }

            // Intentar deserializar como un objeto Result
            try
            {
                // Verificamos primero si es un array JSON
                if (responseBodyText.Trim().StartsWith("["))
                {
                    // Es un array, no un Result, así que pasamos la respuesta original
                    await responseBody.CopyToAsync(originalBodyStream);
                    return;
                }

                // Intentamos parsear como objeto JSON
                JsonNode jsonNode;
                try
                {
                    jsonNode = JsonNode.Parse(responseBodyText);

                    // Si es null o no es un objeto JSON, simplemente devolvemos la respuesta original
                    if (jsonNode == null || jsonNode is not JsonObject)
                    {
                        await responseBody.CopyToAsync(originalBodyStream);
                        return;
                    }
                }
                catch
                {
                    // Si hay un error al parsear, simplemente devolvemos la respuesta original
                    await responseBody.CopyToAsync(originalBodyStream);
                    return;
                }

                // Verificar si es un objeto Result
                if (jsonNode["isSuccess"] != null)
                {
                    bool isSuccess = jsonNode["isSuccess"].GetValue<bool>();

                    if (!isSuccess && jsonNode["error"] != null)
                    {
                        // Es un resultado fallido, transformarlo en una respuesta HTTP adecuada
                        var errorNode = jsonNode["error"];
                        var statusCode = errorNode["statusCode"] != null ? errorNode["statusCode"].GetValue<int>() : 400;
                        var message = errorNode["message"] != null ? errorNode["message"].GetValue<string>() : "Error desconocido";

                        context.Response.StatusCode = statusCode;

                        var errorResponse = new
                        {
                            message = message,
                            details = errorNode["data"]
                        };

                        // Limpiar el stream original antes de escribir
                        context.Response.Body = originalBodyStream;

                        await JsonSerializer.SerializeAsync(context.Response.Body, errorResponse);
                        _logger.LogInformation("Transformada respuesta de error: StatusCode={0}, Mensaje={1}", statusCode, message);
                        return;
                    }
                    else if (isSuccess && jsonNode["value"] != null)
                    {
                        // Es un resultado exitoso, extraer solo el valor
                        context.Response.StatusCode = 200;

                        // Limpiar el stream original antes de escribir
                        context.Response.Body = originalBodyStream;

                        // Serializar directamente el nodo "value"
                        await context.Response.WriteAsync(jsonNode["value"].ToJsonString());
                        return;
                    }
                }

                // Si llegamos aquí, no es un objeto Result o no pudimos procesarlo correctamente
                await responseBody.CopyToAsync(originalBodyStream);
            }
            catch (Exception ex)
            {
                // Si hay algún error, simplemente pasar la respuesta original
                _logger.LogWarning("Error al procesar el objeto Result, devolviendo respuesta original: {0}", ex.Message);
                await responseBody.CopyToAsync(originalBodyStream);
            }
            finally
            {
                // Asegurarnos de que el stream original esté restaurado
                if (context.Response.Body != originalBodyStream)
                {
                    context.Response.Body = originalBodyStream;
                }
            }
        }
    }

    // Extensión para registrar el middleware
    public static class ResultTransformationMiddlewareExtensions
    {
        public static IApplicationBuilder UseResultTransformation(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ResultTransformationMiddleware>();
        }
    }
}