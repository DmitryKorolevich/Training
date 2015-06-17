using System;
using System.Diagnostics;
using Microsoft.Framework.Logging;

namespace VitalChoice.Core.Services
{
    public static class LoggerService
    {
        private static LoggerProviderExtended _loggerProviderExtended;

        public static ILogger GetDefault()
        {
#if !DNXCORE50
            return _loggerProviderExtended?.CreateLogger(new StackFrame(1, false).GetMethod().DeclaringType);
#else
            return _loggerProviderExtended?.CreateLogger(string.Empty);
#endif
        }

        public static ILogger Get(string name = null)
        {
            return _loggerProviderExtended?.CreateLogger(name);
        }

        public static ILogger Get<T>()
        {
            return _loggerProviderExtended?.CreateLogger<T>();
        }

        public static ILogger Get(Type type)
        {
            return _loggerProviderExtended?.CreateLogger(type);
        }

        public static LoggerProviderExtended Build(string rootPath, string logPath)
        {
            return _loggerProviderExtended ?? (_loggerProviderExtended = new LoggerProviderExtended(rootPath, logPath));
        }
    }

}