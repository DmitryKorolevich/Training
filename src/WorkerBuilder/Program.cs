using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.Dnx.Compilation;
using Microsoft.Dnx.Compilation.CSharp;
using Microsoft.Extensions.PlatformAbstractions;

namespace WorkerBuilder
{
    public class Program
    {
        private static string _outRoot;
        private static readonly ILibraryExporter LibraryManager =
            (ILibraryExporter)CallContextServiceLocator.Locator.ServiceProvider.GetService(typeof(ILibraryExporter));

        private static readonly IApplicationEnvironment Environment =
            (IApplicationEnvironment)
                CallContextServiceLocator.Locator.ServiceProvider.GetService(typeof(IApplicationEnvironment));

        private static string ConvertMetadataReference(IMetadataReference metadataReference)
        {
            var embeddedReference = metadataReference as IMetadataEmbeddedReference;

            if (embeddedReference != null)
            {
                using (var file = File.OpenWrite(Path.Combine(_outRoot, embeddedReference.Name + ".dll")))
                {
                    file.Write(embeddedReference.Contents, 0, embeddedReference.Contents.Length);
                    return null;
                }
            }

            var fileMetadataReference = metadataReference as IMetadataFileReference;

            if (fileMetadataReference != null)
            {
                if (fileMetadataReference.Path.ToLowerInvariant().Contains("\\.dnx"))
                {
                    using (var file = File.OpenWrite(Path.Combine(_outRoot, Path.GetFileName(fileMetadataReference.Path))))
                    {
                        using (var readFile = File.OpenRead(fileMetadataReference.Path))
                        {
                            readFile.CopyTo(file);
                            return null;
                        }
                    }
                }
                return fileMetadataReference.Path;
            }

            var projectReference = metadataReference as IMetadataProjectReference;
            if (projectReference != null)
            {
                if (projectReference.Name != "WorkerBuilder")
                {
                    using (var file = File.OpenWrite(Path.Combine(_outRoot, projectReference.Name + ".dll")))
                    {
                        projectReference.EmitReferenceAssembly(file);
                    }
                }
                return null;
            }

            throw new NotSupportedException();
        }

        private static List<string> GetMetadataReferences()
        {
            var references = new List<string>();
            //var libraryExport = LibraryManager.GetExport(Environment.ApplicationName);
            //if (libraryExport?.MetadataReferences?.Count > 0)
            //{
            //    var roslynReference = libraryExport.MetadataReferences[0] as IRoslynMetadataReference;
            //    var compilationReference = roslynReference?.MetadataReference as CompilationReference;
            //    if (compilationReference != null)
            //    {
            //        references.AddRange(
            //            compilationReference.Compilation.References.Select(reference => reference.Display));
            //        references.Add(roslynReference.Name);
            //        return references;
            //    }
            //}
            var export = LibraryManager.GetAllExports(Environment.ApplicationName);
            references.AddRange(export.MetadataReferences.Select(ConvertMetadataReference));

            return references;
        }

        public static void Main(string[] args)
        {
            _outRoot = Path.GetFullPath(args[0]);
            Console.WriteLine($"Publish Root {_outRoot}");
            bool silent = false;
            if (args.Length > 1)
                silent = args[1] == "-silent";
            var refs = GetMetadataReferences();
            if (!silent)
            {
                Console.WriteLine("Using following references:");
                Console.WriteLine(string.Join("\n", refs.Where(r => r != null)));
            }
            Console.WriteLine(
                $"Writing {Path.Combine(_outRoot, @"ExportWorkerRoleWithSBQueue.xproj.FileListAbsolute.txt")} file.");
            File.WriteAllText(Path.Combine(_outRoot, @"ExportWorkerRoleWithSBQueue.xproj.FileListAbsolute.txt"),
                string.Join("\r\n",
                    Directory.GetFileSystemEntries(_outRoot, "*.*", SearchOption.TopDirectoryOnly)
                        .Where(s => !s.Contains("ExportWorkerRoleWithSBQueue.xproj.FileListAbsolute.txt"))));
        }
    }
}