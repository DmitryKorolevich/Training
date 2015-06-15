using System;
using System.IO;
using Microsoft.Framework.Runtime;
using Microsoft.Framework.Runtime.Infrastructure;

namespace VitalChoice.Core.Infrastructure
{
	public static class PathResolver
	{
		private static string ApplicationBaseDirectory
		{
			get
			{
#if DNX451 || DNXCORE50
				var locator = CallContextServiceLocator.Locator;

				var appEnv = (IApplicationEnvironment)locator.ServiceProvider.GetService(typeof(IApplicationEnvironment));
				return appEnv.ApplicationBasePath;

#elif NET45
                return AppDomain.CurrentDomain.BaseDirectory;
#endif
			}
		}

		public static string ResolveAppRelativePath(string path)
		{
			return Path.Combine(ApplicationBaseDirectory, path);
		}
	}
}