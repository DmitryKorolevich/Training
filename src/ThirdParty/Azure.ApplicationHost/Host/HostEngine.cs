using System.Reflection;
using System.Runtime.Versioning;
using Azure.ApplicationHost.Base;

namespace Azure.ApplicationHost.Host
{
    public class HostEngine
    {
        public static int Execute(string[] argv, FrameworkName targetFramework, DnxHostedApplication.ApplicationMainInfo info)
        {
            var bootstrapperContext = GetBootstrapperContext(targetFramework, info);
            return RuntimeBootstrapper.Execute(argv, bootstrapperContext);
        }

        private static BootstrapperContext GetBootstrapperContext(FrameworkName targetFramework, DnxHostedApplication.ApplicationMainInfo info)
        {
            var bootstrapperContext = new BootstrapperContext
            {
                OperatingSystem = info.OperatingSystem,
                OsVersion = info.OsVersion,
                Architecture = info.Architecture,
                RuntimeDirectory = info.RuntimeDirectory,
                ApplicationBase = info.ApplicationBase,
                TargetFramework = targetFramework,
                RuntimeType = "Clr"
            };
            return bootstrapperContext;
        }
    }
}
