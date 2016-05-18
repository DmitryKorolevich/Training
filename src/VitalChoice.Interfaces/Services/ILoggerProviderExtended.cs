using System;
using Microsoft.Extensions.Logging;

namespace VitalChoice.Interfaces.Services
{
    public interface ILoggerProviderExtended : ILoggerProvider
    {
        ILoggerFactory Factory { get; }
        ILogger CreateLogger(Type type);
        ILogger<T> CreateLogger<T>();
    }
}