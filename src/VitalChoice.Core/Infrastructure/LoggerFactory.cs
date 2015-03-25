using System;
using System.Collections.Generic;
using Microsoft.Framework.Logging;

#if ASPNET50
using Microsoft.Framework.Logging.Console;
using System.Diagnostics;
using NLog.Targets.Wrappers;
#endif


namespace VitalChoice.Core.Infrastructure
{
    public static class AppLoggerFactory
    {
        private static readonly Lazy<ILoggerFactory> sFactory = new Lazy<ILoggerFactory>(BuildFactory);

        private static readonly Dictionary<string, ILogger> loggers = new Dictionary<string, ILogger>();

        public static ILogger GetDefault()
        {
#if ASPNET50
            return Get(new StackFrame(1, false).GetMethod().DeclaringType);
#endif
            return null;
        }
        public static ILogger Get(string name = null)
        {
            name = name ?? string.Empty;
            lock (loggers)
            {
                if (loggers.ContainsKey(name))
                {
                    return loggers[name];
                }

                ILogger logger = sFactory.Value.Create(name);
                loggers.Add(name, logger);

                return logger;
            }
        }

        private static ILogger Get(Type type)
        {
            return Get(type == null ? string.Empty : type.FullName); ;
        }

        private static ILoggerFactory BuildFactory()
        {
            ILoggerFactory factory = null;
#if ASPNET50
            factory = new LoggerFactory();
            var path = AppDomain.CurrentDomain.BaseDirectory;
            var config = new NLog.Config.XmlLoggingConfiguration(path+"\\..\\nlog.config");
            foreach (var target in config.ConfiguredNamedTargets)
            {
                WrapperTargetBase targetBase = target as WrapperTargetBase;
                //TODO: modify for the needed url if needed
            }
            factory.AddNLog(new global::NLog.LogFactory(config));
#endif

            return factory;
        }
    }

}