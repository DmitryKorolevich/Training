using System.IO;
using Microsoft.Extensions.PlatformAbstractions;

namespace VitalChoice.Core.Infrastructure
{
	public static class PathResolver
	{
		private static string ApplicationBaseDirectory
		{
			get
			{
				var locator = CallContextServiceLocator.Locator;

				var appEnv = (IApplicationEnvironment)locator.ServiceProvider.GetService(typeof(IApplicationEnvironment));
				return appEnv.ApplicationBasePath;
			}
		}

		public static string ResolveAppRelativePath(string path)
		{
			return Path.Combine(ApplicationBaseDirectory, path);
		}
	}
}