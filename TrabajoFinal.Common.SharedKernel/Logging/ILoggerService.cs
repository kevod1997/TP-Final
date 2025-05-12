using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrabajoFinal.Common.Shared.Logging
{
    /// <summary>
    /// Interfaz para el servicio de logging compartido entre microservicios
    /// </summary>
    public interface ILoggerService
    {
        void LogInformation(string message, params object[] args);
        void LogWarning(string message, params object[] args);
        void LogError(Exception exception, string message, params object[] args);
        void LogDebug(string message, params object[] args);
        void LogCritical(Exception exception, string message, params object[] args);
    }
}