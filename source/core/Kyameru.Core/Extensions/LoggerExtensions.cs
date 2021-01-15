using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kyameru.Core.Extensions
{
    /// <summary>
    /// Extensions for the logger to implement Logger Message.
    /// </summary>
    internal static class LoggerExtensions
    {
        /// <summary>
        /// Information log for from.
        /// </summary>
        private static readonly Action<ILogger, string, string, Exception> info;

        /// <summary>
        /// Debug log for from
        /// </summary>
        private static readonly Action<ILogger, string, string, Exception> debug;

        /// <summary>
        /// Error log
        /// </summary>
        private static readonly Action<ILogger, string, string, Exception> error;

        private static readonly Action<ILogger, string, string, Exception> warning;

        static LoggerExtensions()
        {
            info = LoggerMessage.Define<string, string>(
                LogLevel.Information,
                new EventId(1, "Kyameru.Route"),
                "{Route} => {Message}");

            debug = LoggerMessage.Define<string, string>(
                LogLevel.Debug,
                new EventId(2, "Kyameru.Route"),
                "{Route} => {Message}");

            error = LoggerMessage.Define<string, string>(
                LogLevel.Error,
                new EventId(3, "Kyameru.Route"),
                "{Route} => {Exception}");

            warning = LoggerMessage.Define<string, string>(
                LogLevel.Warning,
                new EventId(4, "Kyameru.Route"),
                "{Route} => {Message}");
        }

        public static void KyameruInfo(this ILogger logger, string routeId, string message)
        {
            info(logger, routeId, message, null);
        }

        public static void KyameruDebug(this ILogger logger, string routeId, string message)
        {
            debug(logger, routeId, message, null);
        }

        public static void KyameruError(this ILogger logger, string routeId, string errorMessage)
        {
            error(logger, routeId, errorMessage, null);
        }

        public static void KyameruWarning(this ILogger logger, string routeId, string message)
        {
            warning(logger, routeId, message, null);
        }

        public static void KyameruException(this ILogger logger, string routeId, string errorMessage, Exception ex)
        {
            error(logger, routeId, errorMessage, ex);
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
    }
}