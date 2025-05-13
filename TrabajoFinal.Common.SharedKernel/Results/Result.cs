using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TrabajoFinal.Common.Shared.Results
{
    /// <summary>
    /// Representa el resultado de una operación que puede tener éxito o fallar
    /// </summary>
    /// <typeparam name="T">Tipo del valor de éxito</typeparam>
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T Value { get; }
        public ErrorDetails Error { get; }

        // Constructor privado para resultado exitoso
        private Result(T value)
        {
            IsSuccess = true;
            Value = value;
            Error = null;
        }

        // Constructor privado para resultado fallido
        private Result(ErrorDetails error)
        {
            IsSuccess = false;
            Value = default;
            Error = error;
        }

        // Método estático para crear un resultado exitoso
        public static Result<T> Success(T value)
        {
            return new Result<T>(value);
        }

        // Método estático para crear un resultado fallido
        public static Result<T> Failure(ErrorDetails error)
        {
            return new Result<T>(error);
        }

        // Método estático para crear un resultado fallido con mensaje
        public static Result<T> Failure(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            return new Result<T>(new ErrorDetails(message, statusCode));
        }

        // Método estático para crear un resultado fallido de NotFound
        public static Result<T> NotFound(string message = "Recurso no encontrado")
        {
            return new Result<T>(new ErrorDetails(message, HttpStatusCode.NotFound));
        }
    }

    /// <summary>
    /// Versión no genérica para operaciones que no retornan valor
    /// </summary>
    public class Result
    {
        public bool IsSuccess { get; }
        public ErrorDetails Error { get; }

        private Result(bool isSuccess, ErrorDetails error = null)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success()
        {
            return new Result(true);
        }

        public static Result Failure(ErrorDetails error)
        {
            return new Result(false, error);
        }

        public static Result Failure(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            return new Result(false, new ErrorDetails(message, statusCode));
        }

        public static Result<T> Success<T>(T value)
        {
            return Result<T>.Success(value);
        }

        public static Result<T> Failure<T>(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            return Result<T>.Failure(message, statusCode);
        }
    }

    public class ErrorDetails
    {
        public string Message { get; }
        public HttpStatusCode StatusCode { get; }
        public object Data { get; }

        public ErrorDetails(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest, object data = null)
        {
            Message = message;
            StatusCode = statusCode;
            Data = data;
        }
    }
}
