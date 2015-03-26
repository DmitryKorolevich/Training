﻿#if dnx451 || dnxcore50 || NET45
using System;
using System.IO;

#if dnx451 || dnxcore50
using Microsoft.Framework.Runtime;
using Microsoft.Framework.Runtime.Infrastructure;
#endif

namespace VitalChoice.Core.Infrastructure
{
	internal static class PathResolver
	{
		private static string ApplicationBaseDirectory
		{
			get
			{
#if dnx451 || dnxcore50
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
#endif