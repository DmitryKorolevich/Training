using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Xml;
using Antlr4.Runtime.Misc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VitalChoice.Infrastructure.Domain.Options;
using NLog;
using NLog.Config;
using NLog.Targets;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Azure;
using VitalChoice.Interfaces.Services;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace VitalChoice.Core.Services
{
    public class CustomFactory : ILoggerFactory
    {
        private readonly ILoggerFactory _initial;
        private readonly IHostingEnvironment _env;

        public CustomFactory(ILoggerFactory initial, IHostingEnvironment env)
        {
            _initial = initial;
            _env = env;
        }

        public void Dispose()
        {
            _initial.Dispose();
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _initial.CreateLogger($"{_env.ApplicationName}::{categoryName ?? string.Empty}");
        }

        public void AddProvider(ILoggerProvider provider)
        {
            _initial.AddProvider(provider);
        }
    }

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

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
                Func<TState, Exception, string> formatter)
            {
                var nLogLogLevel = GetLogLevel(logLevel);
                var message = formatter != null
                    ? formatter(state, exception)
                    : (exception != null ? $"[{exception.Source}]{exception.Message}\n{exception.StackTrace}" : state?.ToString());
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
                    case LogLevel.Trace:
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

            public IDisposable BeginScope<TState>([NotNull] TState state)
            {
                return NestedDiagnosticsContext.Push(state.ToString());
            }
        }

        public void Dispose()
        {
            _logFactory.Dispose();
        }
    }

    public class LoggerProviderExtended : ILoggerProvider
    {
        private readonly ILoggerFactory _factory;

        public ILoggerFactory Factory => _factory;

        public LoggerProviderExtended(IHostingEnvironment env)
        {
            string basePath = env.ContentRootPath;
            string logPath = env.ContentRootPath;
            _factory = new LoggerFactory();
            //_factory = new CustomFactory(new LoggerFactory(), env);
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
                throw new ApiException($"Cannot locate nlog.config in {basePath + @"\nlog.config"}");
            }
        }

        public ILogger CreateLogger(string name)
        {
            name = name ?? string.Empty;
            ILogger logger = _factory.CreateLogger(name);
            return logger;
        }

        public void Dispose()
        {
            _factory.Dispose();
        }
    }
}