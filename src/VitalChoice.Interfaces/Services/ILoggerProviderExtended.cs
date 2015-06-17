using System;
using Microsoft.Framework.Logging;

namespace VitalChoice.Interfaces.Services
{
    public interface ILoggerProviderExtended : ILoggerProvider
    {
        ILogger CreateLoggerDefault();
        ILogger CreateLogger(Type type);
        ILogger CreateLogger<T>();
    }
}