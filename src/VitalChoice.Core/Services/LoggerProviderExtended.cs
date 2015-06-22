using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using Microsoft.Framework.Logging;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Core.Services
{
    public class LoggerProviderExtended : ILoggerProviderExtended
    {
        private readonly ILoggerFactory _factory;

        private readonly Dictionary<string, ILogger> _loggers = new Dictionary<string, ILogger>();

        internal LoggerProviderExtended(string basePath, string logPath)
        {
            _factory = new LoggerFactory();
#if !DNXCORE50
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
#if !DNXCORE50
            return CreateLogger(new StackFrame(1, false).GetMethod().DeclaringType);
#else
            return CreateLogger(string.Empty);
#endif
        }

        public ILogger CreateLogger(Type type)
        {
            return CreateLogger(type?.FullName ?? string.Empty);
        }

        public ILogger CreateLogger<T>()
        {
            return CreateLogger(typeof(T).FullName);
        }
    }
}