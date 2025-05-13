using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrabajoFinal.Common.Shared.Logging;
using TrabajoFinal.Common.Shared.Results;

namespace CustomerService.Application.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        private readonly ILoggerService _logger;

        public ValidationBehavior(
            IEnumerable<IValidator<TRequest>> validators,
            ILoggerService logger)
        {
            _validators = validators;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // Si no hay validadores para este request, continuamos
            if (!_validators.Any())
            {
                return await next();
            }

            _logger.LogInformation($"Validando request de tipo {typeof(TRequest).Name}");

            // Contexto de validación
            var context = new ValidationContext<TRequest>(request);

            // Ejecutamos todas las validaciones
            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            // Recolectamos los errores
            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            // Si hay errores, los registramos y lanzamos una excepción o devolvemos un Result.Failure
            if (failures.Count > 0)
            {
                var errorMessages = string.Join("; ", failures.Select(f => f.ErrorMessage));
                _logger.LogWarning($"Validación fallida para {typeof(TRequest).Name}: {errorMessages}");

                // Si la respuesta espera un Result<T>, intentamos devolver un Failure
                if (typeof(TResponse).IsGenericType &&
                    typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
                {
                    var resultType = typeof(TResponse);
                    var valueType = resultType.GetGenericArguments()[0];

                    // Crear un Result.Failure con los errores de validación
                    var errorDetails = new ErrorDetails(
                        "Error de validación",
                        System.Net.HttpStatusCode.BadRequest,
                        failures.Select(f => new { Property = f.PropertyName, Message = f.ErrorMessage }).ToList());

                    // Usamos reflexión para crear un Result.Failure del tipo correcto
                    var failureMethod = typeof(Result<>)
                        .MakeGenericType(valueType)
                        .GetMethod("Failure", new[] { typeof(ErrorDetails) });

                    var result = failureMethod.Invoke(null, new object[] { errorDetails });
                    return (TResponse)result;
                }

                // Si no es un Result<T>, lanzamos una excepción tradicional
                throw new ValidationException(failures);
            }

            _logger.LogInformation($"Validación exitosa para {typeof(TRequest).Name}");

            // Continuamos con el siguiente handler
            return await next();
        }
    }
}