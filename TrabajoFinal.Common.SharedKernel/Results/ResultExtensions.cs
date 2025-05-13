using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TrabajoFinal.Common.Shared.Results
{
    /// <summary>
    /// Extensiones para convertir objetos Result en ActionResult
    /// </summary>
    public static class ResultExtensions
    {
        /// <summary>
        /// Convierte un Result<T> en un ActionResult<T>
        /// </summary>
        public static ActionResult<T> ToActionResult<T>(this Result<T> result, ControllerBase controller)
        {
            if (result.IsSuccess)
            {
                // Caso especial para bool y otros tipos que podrían necesitar un manejo específico
                if (typeof(T) == typeof(bool))
                {
                    bool value = (bool)(object)result.Value;
                    if (value)
                    {
                        // No usar NoContent directamente, ya que esto causaría un problema de conversión
                        return controller.Ok(result.Value) as ActionResult<T>;
                    }
                }

                return controller.Ok(result.Value);
            }

            // Convertir ErrorDetails a ActionResult
            var statusCode = (int)result.Error.StatusCode;
            var response = new
            {
                message = result.Error.Message,
                details = result.Error.Data
            };

            return CreateActionResult<T>(controller, statusCode, response);
        }

        /// <summary>
        /// Convierte un Result<bool> específicamente para IActionResult (útil para Delete)
        /// </summary>
        public static IActionResult ToActionResultWithNoContent(this Result<bool> result, ControllerBase controller)
        {
            if (result.IsSuccess && result.Value)
            {
                return controller.NoContent();
            }
            else if (result.IsSuccess)
            {
                return controller.Ok(result.Value);
            }

            // Convertir ErrorDetails a ActionResult
            var statusCode = (int)result.Error.StatusCode;
            var response = new
            {
                message = result.Error.Message,
                details = result.Error.Data
            };

            return CreateActionResult(controller, statusCode, response);
        }

        /// <summary>
        /// Convierte un Result (sin valor) en un IActionResult
        /// </summary>
        public static IActionResult ToActionResult(this Result result, ControllerBase controller)
        {
            if (result.IsSuccess)
            {
                return controller.Ok();
            }

            return ToActionResult(result.Error, controller);
        }

        /// <summary>
        /// Convierte detalles de error en un IActionResult
        /// </summary>
        private static IActionResult ToActionResult(ErrorDetails error, ControllerBase controller)
        {
            var statusCode = (int)error.StatusCode;
            var response = new
            {
                message = error.Message,
                details = error.Data
            };

            return CreateActionResult(controller, statusCode, response);
        }

        /// <summary>
        /// Crea un ActionResult<T> basado en el código de estado
        /// </summary>
        private static ActionResult<T> CreateActionResult<T>(ControllerBase controller, int statusCode, object response)
        {
            return statusCode switch
            {
                StatusCodes.Status200OK => controller.Ok(response),
                StatusCodes.Status201Created => controller.Created(string.Empty, response),
                StatusCodes.Status204NoContent => controller.Ok(response) as ActionResult<T>, // Cambiado para evitar problemas de conversión
                StatusCodes.Status400BadRequest => controller.BadRequest(response),
                StatusCodes.Status401Unauthorized => controller.Unauthorized(response) as ActionResult<T>,
                StatusCodes.Status403Forbidden => controller.Forbid() as ActionResult<T>,
                StatusCodes.Status404NotFound => controller.NotFound(response),
                StatusCodes.Status409Conflict => controller.Conflict(response),
                StatusCodes.Status422UnprocessableEntity => controller.UnprocessableEntity(response),
                _ => controller.StatusCode(statusCode, response)
            };
        }

        /// <summary>
        /// Crea un IActionResult basado en el código de estado
        /// </summary>
        private static IActionResult CreateActionResult(ControllerBase controller, int statusCode, object response)
        {
            return statusCode switch
            {
                StatusCodes.Status200OK => controller.Ok(response),
                StatusCodes.Status201Created => controller.Created(string.Empty, response),
                StatusCodes.Status204NoContent => controller.NoContent(),
                StatusCodes.Status400BadRequest => controller.BadRequest(response),
                StatusCodes.Status401Unauthorized => controller.Unauthorized(response),
                StatusCodes.Status403Forbidden => controller.Forbid(),
                StatusCodes.Status404NotFound => controller.NotFound(response),
                StatusCodes.Status409Conflict => controller.Conflict(response),
                StatusCodes.Status422UnprocessableEntity => controller.UnprocessableEntity(response),
                _ => controller.StatusCode(statusCode, response)
            };
        }

        /// <summary>
        /// Extensión para crear un Result.Success desde un controller
        /// </summary>
        public static Result<T> Success<T>(this ControllerBase controller, T value)
        {
            return Result<T>.Success(value);
        }

        /// <summary>
        /// Extensión para crear un Result.Failure desde un controller
        /// </summary>
        public static Result<T> Failure<T>(this ControllerBase controller, string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest, object data = null)
        {
            return Result<T>.Failure(new ErrorDetails(message, statusCode, data));
        }

        /// <summary>
        /// Extensión para crear un Result.NotFound desde un controller
        /// </summary>
        public static Result<T> NotFound<T>(this ControllerBase controller, string message = "Recurso no encontrado")
        {
            return Result<T>.NotFound(message);
        }
    }
}