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
        private static string _outRoot;

        private static readonly ILibraryExporter LibraryManager =
            (ILibraryExporter) CallContextServiceLocator.Locator.ServiceProvider.GetService(typeof (ILibraryExporter));

        private static readonly IApplicationEnvironment AppEnvironment =
            (IApplicationEnvironment)
                CallContextServiceLocator.Locator.ServiceProvider.GetService(typeof (IApplicationEnvironment));

        private static string ConvertMetadataReference(IMetadataReference metadataReference, bool getPathsOnly)
        {
            var embeddedReference = metadataReference as IMetadataEmbeddedReference;

            if (embeddedReference != null)
            {
                if (getPathsOnly)
                    return Path.Combine(_outRoot, embeddedReference.Name + ".dll");
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
                    if (getPathsOnly)
                        return Path.Combine(_outRoot, Path.GetFileName(fileMetadataReference.Path));
                    using (
                        var file = File.OpenWrite(Path.Combine(_outRoot, Path.GetFileName(fileMetadataReference.Path))))
                    {
                        using (var readFile = File.OpenRead(fileMetadataReference.Path))
                        {
                            readFile.CopyTo(file);
                            return null;
                        }
                    }
                }
                if (getPathsOnly)
                    return null;
                return fileMetadataReference.Path;
            }

            var projectReference = metadataReference as IMetadataProjectReference;
            if (projectReference != null)
            {
                if (projectReference.Name != "WorkerBuilder")
                {
                    string folderName = Path.Combine(_outRoot, projectReference.Name);
                    if (getPathsOnly)
                        return Path.Combine(_outRoot, projectReference.Name + ".dll");
                    var emitResult = projectReference.EmitAssembly(folderName);
                    if (!emitResult.Success)
                    {
                        throw new InvalidOperationException($"{string.Join("\n", emitResult.Diagnostics.Select(d => d.FormattedMessage))}");
                    }
                    File.Copy(Path.Combine(folderName, projectReference.Name + ".dll"), folderName + ".dll", true);
                    File.Copy(Path.Combine(folderName, projectReference.Name + ".pdb"), folderName + ".pdb", true);
                    File.Copy(Path.Combine(folderName, projectReference.Name + ".xml"), folderName + ".xml", true);
                    Directory.Delete(folderName, true);
                }
                return null;
            }

            throw new NotSupportedException();
        }

        private static List<string> GetMetadataReferences(bool getPathsOnly = false)
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
            var export = LibraryManager.GetAllExports(AppEnvironment.ApplicationName);
            references.AddRange(export.MetadataReferences.Select(r => ConvertMetadataReference(r, getPathsOnly)));

            return references;
        }

        public static void Main(string[] args)
        {
            _outRoot = Path.GetFullPath(args[0]);
            Parallel.ForEach(Directory.GetFiles(_outRoot), File.Delete);
            Console.WriteLine($"Publish Root {_outRoot}");
            var silent = args.Any(a => a == "-silent");
            var createListFile = args.Any(a => a == "-list");
            List<string> refs;
            try
            {
                refs = GetMetadataReferences(createListFile);
                var activeRuntimePath = GetExecutablePath("dnx.exe");
                Console.WriteLine($"Using runtime: {activeRuntimePath}");
                var runtimeDlls = Directory.GetFiles(activeRuntimePath, "*.dll");
                foreach (var dll in runtimeDlls)
                {
                    var fileName = Path.GetFileName(dll);
                    if (fileName != null)
                        File.Copy(dll, Path.Combine(_outRoot, fileName), true);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return;
            }
            if (!silent)
            {
                Console.WriteLine("Using following references:");
                Console.WriteLine(string.Join("\n", refs.Where(r => r != null)));
            }
            if (createListFile)
            {
                Console.WriteLine(
                    $"Writing {Path.Combine(_outRoot, @"ExportWorkerRoleWithSBQueue.xproj.FileListAbsolute.txt")} file.");
                File.WriteAllText(Path.Combine(_outRoot, @"ExportWorkerRoleWithSBQueue.xproj.FileListAbsolute.txt"),
                    string.Join("\r\n",
                        refs.Where(
                            s => s != null && !s.Contains("ExportWorkerRoleWithSBQueue.xproj.FileListAbsolute.txt") && !s.Contains("ExportWorkerRoleWithSBQueue."))
                            .Select(
                                s =>
                                    s.Replace("\\obj\\Debug\\", "\\bin\\").Replace("\\obj\\Release\\", "\\bin\\"))
                            .Union(new[] {Path.Combine(_outRoot, "ExportWorkerRoleWithSBQueue.pdb"), Path.Combine(_outRoot, "ExportWorkerRoleWithSBQueue.dll"), Path.Combine(_outRoot, "ExportWorkerRoleWithSBQueue.xml"), Path.Combine(_outRoot, "ExportWorkerRoleWithSBQueue.dll") })));
            }
        }

        private static string GetExecutablePath(string name)
        {
            var values = Environment.GetEnvironmentVariable("PATH");
            if (values != null)
                foreach (var path in values.Split(';'))
                {
                    var fullPath = Path.Combine(path, name);
                    if (File.Exists(fullPath))
                    {
                        return Path.GetDirectoryName(fullPath);
                    }
                }
            return null;
        }
    }
}