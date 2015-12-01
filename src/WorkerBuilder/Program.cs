using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.Dnx.Compilation;
using Microsoft.Dnx.Compilation.CSharp;
using Microsoft.Extensions.PlatformAbstractions;

namespace WorkerBuilder
{
    public class Program
    {
        public static void Main(string[] args)
        {
            RuntimeAssemblyPublishHelper publishHelper = new RuntimeAssemblyPublishHelper(Path.GetFullPath(args[0]));
            Console.WriteLine($"Publish Root {publishHelper.OutRoot}");
            Parallel.ForEach(Directory.GetFiles(publishHelper.OutRoot), File.Delete);
            var silent = args.Any(a => a == "-silent");
            List<string> frameworkReferences;
            try
            {
                frameworkReferences = publishHelper.PublishReferences();
                publishHelper.PublishRuntime();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return;
            }
            if (!silent)
            {
                Console.WriteLine("Using following framework references:");
                Console.WriteLine(string.Join("\n", frameworkReferences));
            }
        }
    }
}