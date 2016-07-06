using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace VitalChoice.Ecommerce.Domain.Helpers
{
    public static class LoggerExtensions
    {
        public static void LogInfo<T>(this ILogger logger, Func<T, string> messageFunc, T value)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation(messageFunc(value));
            }
        }

        public static void LogInfo<T1, T2>(this ILogger logger, Func<T1, T2, string> messageFunc, T1 value1, T2 value2)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation(messageFunc(value1, value2));
            }
        }

        public static void LogWarn<T>(this ILogger logger, Func<T, string> messageFunc, T value)
        {
            if (logger.IsEnabled(LogLevel.Warning))
            {
                logger.LogWarning(messageFunc(value));
            }
        }

        public static void LogTrace<T>(this ILogger logger, Func<T, string> messageFunc, T value)
        {
            if (logger.IsEnabled(LogLevel.Trace))
            {
                logger.LogTrace(messageFunc(value));
            }
        }

        public static void LogInfo(this ILogger logger, Func<string> messageFunc)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation(messageFunc());
            }
        }

        public static void LogWarn(this ILogger logger, Func<string> messageFunc)
        {
            if (logger.IsEnabled(LogLevel.Warning))
            {
                logger.LogWarning(messageFunc());
            }
        }

        public static void LogTrace(this ILogger logger, Func<string> messageFunc)
        {
            if (logger.IsEnabled(LogLevel.Trace))
            {
                logger.LogTrace(messageFunc());
            }
        }
    }
}