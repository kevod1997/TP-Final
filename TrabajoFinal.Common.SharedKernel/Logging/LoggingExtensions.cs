using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrabajoFinal.Common.Shared.Logging;

namespace TrabajoFinal.Common.Shared.Logging
{
    public static class LoggingExtensions
    {
        /// <summary>
        /// Configura Serilog con configuración estándar para todos los microservicios
        /// </summary>
        public static IHostBuilder ConfigureStandardLogging(this IHostBuilder builder, string serviceName, string logFilePath = null)
        {
            return builder.UseSerilog((context, services, configuration) =>
            {
                // Configuración base desde appsettings.json
                configuration.ReadFrom.Configuration(context.Configuration);

                // Enriquecer con información útil
                configuration
      .Enrich.WithProperty("ServiceName", serviceName)
      .Enrich.WithProperty("Environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production");

                // Configuración de salida
                configuration.WriteTo.Console();

                // Usar ruta de archivo específica o generar una basada en el nombre del servicio
                if (string.IsNullOrEmpty(logFilePath))
                {
                    logFilePath = $"logs/{serviceName.ToLowerInvariant()}-.txt";
                }

                configuration.WriteTo.File(
                    logFilePath,
                    rollingInterval: RollingInterval.Day,
                    restrictedToMinimumLevel: LogEventLevel.Information);
            });
        }

        /// <summary>
        /// Registra los servicios de logging en el contenedor de DI
        /// </summary>
        public static IServiceCollection AddStandardLogging(this IServiceCollection services)
        {
            // Registrar como singleton en lugar de scoped
            services.AddSingleton<ILoggerService, LoggerService>();

            return services;
        }
    }
}