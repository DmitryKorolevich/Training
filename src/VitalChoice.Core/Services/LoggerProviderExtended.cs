using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Xml;
using Antlr4.Runtime.Misc;
using Microsoft.Extensions.Logging;
#if !DOTNET5_4
using NLog;
#endif
using VitalChoice.Interfaces.Services;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace VitalChoice.Core.Services
{
#if !DOTNET5_4
    public static class NLogLoggerFactoryExtensions
    {
        public static ILoggerFactory AddNLog(
            this ILoggerFactory factory,
            global::NLog.LogFactory logFactory)
        {
            factory.AddProvider(new NLogLoggerProvider(logFactory));
            return factory;
        }
    }

    public class NLogLoggerProvider : ILoggerProvider
    {
        private readonly LogFactory _logFactory;

        public NLogLoggerProvider(LogFactory logFactory)
        {
            _logFactory = logFactory;
        }

        public ILogger CreateLogger(string name)
        {
            return new Logger(_logFactory.GetLogger(name));
        }

        private class Logger : ILogger
        {
            private readonly global::NLog.Logger _logger;

            public Logger(global::NLog.Logger logger)
            {
                _logger = logger;
            }

            public void Log(
                LogLevel logLevel,
                int eventId,
                object state,
                Exception exception,
                Func<object, Exception, string> formatter)
            {
                var nLogLogLevel = GetLogLevel(logLevel);
                var message = formatter != null ? formatter(state, exception) : LogFormatter.Formatter(state, exception);
                if (!string.IsNullOrEmpty(message))
                {
                    var eventInfo = LogEventInfo.Create(nLogLogLevel, _logger.Name, exception,
                        CultureInfo.InvariantCulture, message);
                    eventInfo.Properties["EventId"] = eventId;
                    _logger.Log(eventInfo);
                }
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return _logger.IsEnabled(GetLogLevel(logLevel));
            }

            private static global::NLog.LogLevel GetLogLevel(LogLevel logLevel)
            {
                switch (logLevel)
                {
                    case LogLevel.Verbose:
                        return global::NLog.LogLevel.Trace;
                    case LogLevel.Debug:
                        return global::NLog.LogLevel.Debug;
                    case LogLevel.Information:
                        return global::NLog.LogLevel.Info;
                    case LogLevel.Warning:
                        return global::NLog.LogLevel.Warn;
                    case LogLevel.Error:
                        return global::NLog.LogLevel.Error;
                    case LogLevel.Critical:
                        return global::NLog.LogLevel.Fatal;
                }
                return global::NLog.LogLevel.Debug;
            }

            public IDisposable BeginScopeImpl([NotNull] object state)
            {
                return NestedDiagnosticsContext.Push(state.ToString());
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
#endif

    public class LoggerProviderExtended : ILoggerProviderExtended
    {
        private readonly ILoggerFactory _factory;

        private readonly Dictionary<string, ILogger> _loggers = new Dictionary<string, ILogger>();

        public ILoggerFactory Factory => _factory;

        internal LoggerProviderExtended(string basePath, string logPath)
        {
            _factory = new LoggerFactory();
#if !DOTNET5_4
            int backLevelCount = 0;
            var searchPath = basePath;
            while (!File.Exists(searchPath + @"\nlog.config") && backLevelCount <= 5)
            {
                searchPath = Path.Combine(searchPath, "..");
                backLevelCount++;
            }
            var fileName = searchPath + @"\nlog.config";
            if (File.Exists(fileName))
            {
                var xml = File.ReadAllText(fileName);
                xml = xml.Replace("{logdir}", logPath);
                using (var stringReader = new StringReader(xml))
                {
                    XmlReader reader = XmlReader.Create(stringReader);
                    var config = new NLog.Config.XmlLoggingConfiguration(reader, fileName);
                    _factory.AddNLog(new NLog.LogFactory(config));
                }
            }
            else
            {
                _factory.AddNLog(new NLog.LogFactory(new NLog.Config.LoggingConfiguration()));
            }
#endif
        }

        public ILogger CreateLogger(string name)
        {
            name = name ?? string.Empty;
            lock (_loggers)
            {
                ILogger result;
                if (_loggers.TryGetValue(name, out result))
                {
                    return result;
                }
                ILogger logger = _factory.CreateLogger(name);
                _loggers.Add(name, logger);
                return logger;
            }
        }

        public ILogger CreateLoggerDefault()
        {
#if !DOTNET5_4
            return CreateLogger(new StackFrame(1, false).GetMethod().DeclaringType);
#else
            return CreateLogger(string.Empty);
#endif
        }

        public ILogger CreateLogger(Type type)
        {
            return CreateLogger(type?.FullName ?? string.Empty);
        }

        public ILogger<T> CreateLogger<T>()
        {
            return (ILogger<T>)CreateLogger(typeof(T).FullName);
        }

        public void Dispose()
        {
            _factory.Dispose();
        }
    }
}