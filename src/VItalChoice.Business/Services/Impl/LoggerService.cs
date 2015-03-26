using System;
using System.Collections.Generic;
using Microsoft.Framework.Logging;

#if DNX451
using Microsoft.Framework.Logging.Console;
using System.Diagnostics;
using NLog.Targets.Wrappers;
using VitalChoice.Business.Services.Contracts;
#endif

namespace VitalChoice.Business.Services.Impl
{
    public static class LoggerService
    {
        private static ILoggerFactory factory = null;

        private static readonly Dictionary<string, ILogger> loggers = new Dictionary<string, ILogger>();

        public static ILogger GetDefault()
        {
#if DNX451
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

                ILogger logger = factory.CreateLogger(name);
                loggers.Add(name, logger);

                return logger;
            }
        }
        //public static void Init(ISettingService settingService)
        //{
        //    LoggerService.settingService = settingService;
        //}

        private static ILogger Get(Type type)
        {
            return Get(type == null ? string.Empty : type.FullName);
        }

        public static ILoggerFactory Build(string rootPath)
        {
#if DNX451
            factory = new LoggerFactory();
            var path = AppDomain.CurrentDomain.BaseDirectory; ;
            var config = new NLog.Config.XmlLoggingConfiguration(rootPath + "\\..\\..\\nlog.config");
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