using Microsoft.Extensions.Logging;
using System;

namespace Kyameru.Core.Extensions
{
    /// <summary>
    /// Extensions for the logger to implement Logger Message.
    /// </summary>
    internal static class LoggerExtensions
    {
        public static void KyameruInfo(this ILogger logger, string routeId, string message)
        {
            logger.LogInformation(PrintMessage(routeId, message));
        }

        public static void KyameruDebug(this ILogger logger, string routeId, string message)
        {
            logger.LogDebug(PrintMessage(routeId, message));
        }

        public static void KyameruError(this ILogger logger, string routeId, string errorMessage)
        {
            logger.LogError(message:PrintMessage(routeId, errorMessage));
        }

        public static void KyameruWarning(this ILogger logger, string routeId, string message)
        {
            logger.LogWarning(PrintMessage(routeId, message));
        }

        public static void KyameruException(this ILogger logger, string routeId, string errorMessage, Exception ex)
        {
            logger.LogError(ex, PrintMessage(routeId, errorMessage));
        }

        public static void KyameruLog(this ILogger logger, string routeId, string message, LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Information:
                    KyameruInfo(logger, routeId, message);
                    break;

                case LogLevel.Warning:
                    KyameruWarning(logger, routeId, message);
                    break;

                case LogLevel.Debug:
                    KyameruDebug(logger, routeId, message);
                    break;

                case LogLevel.Error:
                    KyameruError(logger, routeId, message);
                    break;

                default:
                    KyameruDebug(logger, routeId, message);
                    break;
            }
        }

        private static string PrintMessage(string routeId, string message)
        {
            return $"Kyameru.Route:{routeId} => {message}";
        }
    }
}