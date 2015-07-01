using System;
using Microsoft.Framework.Logging;

namespace VitalChoice.Interfaces.Services
{
    public interface ILoggerProviderExtended : ILoggerProvider
    {
        ILoggerFactory Factory { get; }
        ILogger CreateLoggerDefault();
        ILogger CreateLogger(Type type);
        ILogger<T> CreateLogger<T>();
    }
}