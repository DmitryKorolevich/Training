using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Dnx.Compilation;
using Microsoft.Extensions.PlatformAbstractions;

namespace WorkerBuilder
{
    public class RuntimeAssemblyPublishHelper
    {
        public string OutRoot { get; }

        private readonly ILibraryExporter _libraryManager =
            (ILibraryExporter) CallContextServiceLocator.Locator.ServiceProvider.GetService(typeof (ILibraryExporter));

        private readonly IApplicationEnvironment _appEnvironment =
            (IApplicationEnvironment)
                CallContextServiceLocator.Locator.ServiceProvider.GetService(typeof (IApplicationEnvironment));

        public RuntimeAssemblyPublishHelper(string outRoot)
        {
            OutRoot = outRoot;
        }

        private string PublishReference(IMetadataReference metadataReference, bool getPathsOnly)
        {
            var embeddedReference = metadataReference as IMetadataEmbeddedReference;

            if (embeddedReference != null)
            {
                if (getPathsOnly)
                    return Path.Combine(OutRoot, embeddedReference.Name + ".dll");
                using (var file = File.OpenWrite(Path.Combine(OutRoot, embeddedReference.Name + ".dll")))
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
                        return Path.Combine(OutRoot, Path.GetFileName(fileMetadataReference.Path));
                    using (
                        var file = File.OpenWrite(Path.Combine(OutRoot, Path.GetFileName(fileMetadataReference.Path))))
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
                    string folderName = Path.Combine(OutRoot, projectReference.Name);
                    if (getPathsOnly)
                        return Path.Combine(OutRoot, projectReference.Name + ".dll");
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

        public List<string> PublishReferences(bool getPathsOnly = false)
        {
            var references = new List<string>();
            var export = _libraryManager.GetAllExports(_appEnvironment.ApplicationName);
            references.AddRange(
                export.MetadataReferences.Select(r => PublishReference(r, getPathsOnly)).Where(s => s != null));

            return references;
        }

        public void PublishRuntime()
        {
            var activeRuntimePath = GetExecutablePath("dnx.exe");
            Console.WriteLine($"Using runtime: {activeRuntimePath}");
            var runtimeDlls = Directory.GetFiles(activeRuntimePath, "*.dll");
            HashSet<string> exludeDlls = new HashSet<string> {"dnx.clr.dll", "dnx.onecore.dll", "dnx.win32.dll"};
            foreach (var dll in runtimeDlls)
            {
                var fileName = Path.GetFileName(dll);
                if (fileName != null && !exludeDlls.Contains(fileName))
                    File.Copy(dll, Path.Combine(OutRoot, fileName), true);
            }
        }

        private string GetExecutablePath(string name)
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