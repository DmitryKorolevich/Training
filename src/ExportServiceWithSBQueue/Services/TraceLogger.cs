﻿using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace ExportServiceWithSBQueue.Services
{
    public class TraceLogger<T> : TraceLogger, ILogger<T>
    {
        protected override string FormatError<TState>(TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            return $"[{typeof(T).FullName}] {base.FormatError(state, exception, formatter)}";
        }
    }

    public class TraceLogger: ILogger, IDisposable
    {
        private readonly object _scope;
        private bool _disposed;

        private TraceLogger(object scope)
        {
            _scope = scope;
        }

        public TraceLogger()
        {
            
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var logMessage = FormatError(state, exception, formatter);
            switch (logLevel)
            {
                case LogLevel.Debug:
                    Trace.TraceInformation(logMessage);
                    break;
                case LogLevel.Trace:
                    Trace.TraceInformation(logMessage);
                    break;
                case LogLevel.Information:
                    Trace.TraceInformation(logMessage);
                    break;
                case LogLevel.Warning:
                    Trace.TraceWarning(logMessage);
                    break;
                case LogLevel.Error:
                    Trace.TraceError(logMessage);
                    break;
                case LogLevel.Critical:
                    Trace.TraceError(logMessage);
                    break;
                case LogLevel.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
            }
            Trace.Flush();
        }

        protected virtual string FormatError<TState>(TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            return formatter != null ? formatter(state, exception) : exception.Message;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new TraceLogger(state);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                var disposable = _scope as IDisposable;
                disposable?.Dispose();
                _disposed = true;
            }
        }
    }
}
