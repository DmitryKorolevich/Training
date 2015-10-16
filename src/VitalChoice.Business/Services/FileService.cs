using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Framework.Logging;
using Microsoft.Framework.OptionsModel;
using VitalChoice.Domain.Entities.Files;
using VitalChoice.Domain.Entities.Options;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Business.Services
{
    public class FileService : IFileService
    {
        private const string ALLOWED_EXTENSIONS = ".jpg,.jpeg,.gif,.png,.pdf";
        private const int MAX_FILE_SIZE = 10485760;
        private const int NUMBER_OF_TRIES = 20;
        private const int MILISECONDS_INTERVAL_BETWEEN_TRIES = 50;

        private static string _rootDir;
        private readonly ILogger logger;
        private readonly IOptions<AppOptions> appOptions;
        private static string error = "";

        public static void Init(string rootDir)
        {
            if (!String.IsNullOrEmpty(rootDir))
            {
                try
                {
                    _rootDir = rootDir.ToLower();
                    DirectoryInfo dirInfo = new DirectoryInfo(rootDir);
                    if (!dirInfo.Exists)
                    {
                        dirInfo.Create();
                    }
                }
                catch(Exception e)
                {
                    error += e.ToString();
                }
            }
        }

        public FileService(IOptions<AppOptions> appOptions, ILoggerProviderExtended loggerProvider)
        {
            this.logger = loggerProvider.CreateLoggerDefault();
            this.appOptions = appOptions;
        }

        #region Dirs

        public DirectoryInfoObject GetDirectories()
        {
            DirectoryInfoObject toReturn = new DirectoryInfoObject("/", "/");
            DirectoryInfo dirInfo = new DirectoryInfo(_rootDir);
            var dirs = dirInfo.GetDirectories("*", SearchOption.AllDirectories).Select(p => new DirectoryInfoObject()
            {
                Name = p.Name,
                FullRelativeName = ConvertPathToUrl(p.FullName),
                Directories = new List<DirectoryInfoObject>(),
                Updated = p.LastWriteTime,
            }).ToList();

            AssigntDirectories(toReturn, dirs);

            return toReturn;
        }

        public DirectoryInfoObject AddDirectory(string fullRelativeName, string name)
        {
            DirectoryInfoObject toReturn = null;
            name = name.Trim().Replace("/", "");
            if (fullRelativeName == "/")
            {
                fullRelativeName = String.Empty;
            }
            var path = ConvertUrlToPath(fullRelativeName);
            path = path + @"\" + name;
            DirectoryInfo dirInfo = new DirectoryInfo(path.ToLower());
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
                toReturn = new DirectoryInfoObject(name, fullRelativeName + "/" + name);
            }
            else
            {
                throw new AppValidationException("Directory", "The directory with this name already exists.");
            }
            return toReturn;
        }

        public bool DeleteDirectory(string fullRelativeName)
        {
            bool toReturn = false;
            string tempUrl = fullRelativeName;
            if (tempUrl == "/")
            {
                return false;
            }
            var path = ConvertUrlToPath(fullRelativeName);
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            if (dirInfo.Exists)
            {
                if (dirInfo.GetFiles().Length > 0)
                {
                    throw new AppValidationException("The folder with files can't be deleted.");
                }
                if (dirInfo.GetDirectories().Length > 0)
                {
                    throw new AppValidationException("The folder with directories can't be deleted.");
                }

                dirInfo.Delete();
                toReturn = true;
            }

            return toReturn;
        }

        #endregion

        #region Files

        public IEnumerable<FileInfoObject> GetFiles(string fullRelativeName)
        {
            IEnumerable<FileInfoObject> toReturn = new List<FileInfoObject>();
            DirectoryInfo dirInfo = new DirectoryInfo(ConvertUrlToPath(fullRelativeName));
            if (dirInfo.Exists)
            {
                toReturn = dirInfo.GetFiles().Select(p => new FileInfoObject(p.Name, ConvertPathToUrl(p.FullName),
                    ConvertPathToUrl(dirInfo.FullName), p.Length, p.LastWriteTime, appOptions.Value.FilesRelativePath)).ToList();
            }

            return toReturn;
        }

        public FileInfoObject AddFile(string fullRelativeName, string name, byte[] content)
        {
            FileInfoObject toReturn = null;
            name = name.Trim().Replace("/", "");
            var extension = Path.GetExtension(name);

            List<MessageInfo> messages = new List<MessageInfo>();
            if (!ALLOWED_EXTENSIONS.Split(',').Contains(extension.ToLower()))
            {
                messages.Add(new MessageInfo()
                {
                    Field = name,
                    Message = "The uploaded file must be .jpg, .gif, .png or .pdf."
                });
            }
            if (content.Length > MAX_FILE_SIZE)
            {
                messages.Add(new MessageInfo()
                {
                    Field = name,
                    Message = "The uploaded file must be less than 10 mb."
                });
            }
            if (messages.Count > 0)
            {
                throw new AppValidationException(messages);
            }

            string tempUrl = fullRelativeName;
            if (tempUrl == "/")
            {
                tempUrl = String.Empty;
            }
            var path = ConvertUrlToPath(tempUrl);
            var directory = path;
            path = path + @"\" + name;
            path = path.ToLower();
            FileInfo fileInfo = new FileInfo(path);
            if(fileInfo.Exists)
            {
                path=GetAvaliableFilePath(path, 1);
                fileInfo = new FileInfo(path);
            }

            SaveToFileSystem(fileInfo.FullName, content);
            fileInfo = new FileInfo(path);
            var resDir = ConvertPathToUrl(directory);
            if(resDir==String.Empty)
            {
                resDir = "/";
            }
            toReturn = new FileInfoObject(Path.GetFileName(path).ToLower(), ConvertPathToUrl(path), resDir, content.Length, fileInfo.LastWriteTime,
                appOptions.Value.FilesRelativePath);

            return toReturn;
        }

        private string GetAvaliableFilePath(string path, int index)
        {
            string toReturn = path;
            var ext = Path.GetExtension(toReturn);
            toReturn = toReturn.Substring(0, toReturn.LastIndexOf(ext));
            toReturn = String.Format("{0} ({1}){2}",toReturn, index, ext);
            FileInfo fileInfo = new FileInfo(toReturn);
            if (fileInfo.Exists)
            {
                index++;
                toReturn = GetAvaliableFilePath(path, index);
            }
            return toReturn;
        }

        public bool DeleteFile(string fullRelativeName)
        {
            bool toReturn = false;
            if(fullRelativeName.StartsWith(appOptions.Value.FilesRelativePath))
            {
                fullRelativeName = fullRelativeName.Substring(appOptions.Value.FilesRelativePath.Length, fullRelativeName.Length - appOptions.Value.FilesRelativePath.Length);
            }
            var path = ConvertUrlToPath(fullRelativeName);
            FileInfo fileInfo = new FileInfo(path);
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
                toReturn = true;
            }

            return toReturn;
        }

        #endregion

        #region Private

        private static string ConvertUrlToPath(string url)
        {
            return _rootDir + url.ToLower().Replace("/", @"\").Replace(@"\..","");
        }

        private static string ConvertPathToUrl(string path)
        {
            return path.ToLower().Replace(_rootDir, "").Replace(@"\", "/");
        }

        private static FileStream CreateWriteStream(string currentFileUrl, int numberOfTries, int timeIntervalBetweenTries)
        {
            int tries = 0;
            while (true)
            {
                try
                {
                    return File.Open(currentFileUrl, FileMode.Create, FileAccess.Write, FileShare.None);
                }
                catch (IOException e)
                {
                    if (!IsFileLocked(e))
                        throw;
                    if (++tries > numberOfTries)
                        throw new Exception("Something went wrong. Please try resave it again.");
                    Thread.Sleep(timeIntervalBetweenTries);
                }
            }
        }
        private static bool IsFileLocked(IOException exception)
        {
            int errorCode = Marshal.GetHRForException(exception) & ((1 << 16) - 1);
            return errorCode == 32 || errorCode == 33;
        }

        public static void SaveToFileSystem(string fullPath, byte[] content)
        {
            using (FileStream fileStream = CreateWriteStream(fullPath, NUMBER_OF_TRIES,
                MILISECONDS_INTERVAL_BETWEEN_TRIES))
            {
                using (BinaryWriter writer = new BinaryWriter(fileStream))
                {
                    writer.Write(content, 0, content.Length);
                }
            }
        }

        private void AssigntDirectories(DirectoryInfoObject root, ICollection<DirectoryInfoObject> dirs)
        {
            foreach (var dir in dirs)
            {
                var forSearch = dir.FullRelativeName.Substring(0, dir.FullRelativeName.LastIndexOf("/"));
                if(String.IsNullOrEmpty(forSearch))
                {
                    forSearch = "/";
                }
                if (forSearch == root.FullRelativeName)
                {
                    root.Directories.Add(dir);
                }
                root.Directories = root.Directories.OrderBy(p => p.FullRelativeName).ToList();
            }

            foreach (var dir in root.Directories)
            {
                AssigntDirectories(dir, dirs);
            }
        }

        #endregion
    }
}
