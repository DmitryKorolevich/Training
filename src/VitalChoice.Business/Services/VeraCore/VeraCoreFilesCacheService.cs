using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using VitalChoice.Data.Repositories;
using VitalChoice.Infrastructure.Domain.Entities.VeraCore;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.VeraCore;

namespace VitalChoice.Business.Services.VeraCore
{
    public class VeraCoreFilesCacheService : IVeraCoreFilesCacheService
    {
        private readonly IOptions<AppOptions> _options;
        private readonly ILogger _logger;
        private readonly string _directoryPath;

        private const int _bufferSize = 4096;

        public VeraCoreFilesCacheService(
            IOptions<AppOptions> options,
            ILoggerProviderExtended logger)
        {
            _options = options;
            _logger = logger.CreateLogger<VeraCoreFilesCacheService>();
            _directoryPath = _options.Value.VeraCoreSettings.ArchivePath;
        }

        public FileStream GetCached(VeraCoreFileInfo fileInfo, string subPath)
        {
            string path = Path.Combine
                (_directoryPath, fileInfo.FileDate.Year.ToString(CultureInfo.InvariantCulture), fileInfo.FileDate.Month.ToString(CultureInfo.InvariantCulture),
                 subPath);
            string fileName = Path.Combine(path, fileInfo.FileName);
            return File.Open(fileName, FileMode.Open);
        }

        public bool CacheFile(VeraCoreFileInfo file, Stream fileData, string subPath)
        {
            if (file == null)
                throw new ArgumentNullException("file");
            if (fileData == null)
                throw new ArgumentNullException("fileData");
            if (subPath == null)
                throw new ArgumentNullException("subPath");

            try
            {
                string path = Path.Combine
                    (_directoryPath, file.FileDate.Year.ToString(CultureInfo.InvariantCulture),
                     file.FileDate.Month.ToString(CultureInfo.InvariantCulture), subPath);
                string fileName = Path.Combine(path, file.FileName);
                Directory.CreateDirectory(path);
                FileStream fileStream = File.Create(fileName);
                CopyStream(fileStream, fileData);
                fileStream.Close();
                fileData.Seek(0, SeekOrigin.Begin);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return false;
            }
            return true;
        }

        private void GoThroughFolder(List<string> fileList, string path)
        {
            if (fileList == null)
                throw new ArgumentNullException("fileList");
            if (path == null)
                throw new ArgumentNullException("path");

            fileList.AddRange(Directory.GetFiles(path).Select(f => Path.Combine(path, f)));
            foreach (string directory in Directory.GetDirectories(path))
                GoThroughFolder(fileList, Path.Combine(path, directory));
        }

        private List<string> BuildFileTree(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            var result = new List<string>();
            GoThroughFolder(result, path);
            return result;
        }

        private bool MakeArchive(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            try
            {
                List<string> fileList = BuildFileTree(path);
                string workingDirectory = Path.GetDirectoryName(path);
                if (fileList != null && (workingDirectory != null && fileList.Any()))
                {
                    string archiveName = Path.Combine(workingDirectory, Path.GetFileName(path) + ".zip");

                    using (var memoryStream = new MemoryStream())
                    {
                        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                        {
                            foreach (string file in fileList)
                            {
                                var archiveItem = archive.CreateEntry(file.Replace(path + @"\", ""));

                                using (var entryStream = archiveItem.Open())
                                {
                                    using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
                                    {
                                        CopyStream(entryStream, fileStream);
                                    }
                                }
                            }
                        }

                        using (var fileStream = new FileStream(archiveName, FileMode.OpenOrCreate))
                        {
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            memoryStream.CopyTo(fileStream);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return false;
            }
            return true;
        }

        public void MakeBackup()
        {
            string path = _directoryPath;

            //Get year directories
            string[] years = Directory.GetDirectories(path);
            foreach (string year in years)
            {
                string[] months = Directory.GetDirectories(year);
                foreach (string month in months)
                {
                    if (Path.GetFileName(year) != DateTime.Today.Year.ToString(CultureInfo.InvariantCulture)
                        || Path.GetFileName(month) != DateTime.Today.Month.ToString(CultureInfo.InvariantCulture))
                    {
                        if (MakeArchive(month))
                            Directory.Delete(month, true);
                    }
                }
            }
        }

        private void CopyStream(Stream destination, Stream source)
        {
            if (destination == null)
                throw new ArgumentNullException("destination");
            if (source == null)
                throw new ArgumentNullException("source");

            int readed;
            var buffer = new byte[_bufferSize];
            while ((readed = source.Read(buffer, 0, _bufferSize)) > 0)
                destination.Write(buffer, 0, readed);
        }
    }
}