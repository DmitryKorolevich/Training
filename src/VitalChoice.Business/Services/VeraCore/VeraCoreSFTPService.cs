using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using VitalChoice.Ecommerce.Domain.Entities.VeraCore;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.VeraCore;

namespace VitalChoice.Business.Services.VeraCore
{
    public class VeraCoreSFTPService : IVeraCoreSFTPService, IDisposable
    {
        private readonly IOptions<AppOptions> _options;
        private readonly ILogger _logger;

        private SftpClient _sftpClient;

        public VeraCoreSFTPService(
            IOptions<AppOptions> options,
            ILoggerFactory logger)
        {
            _options = options;
            _logger = logger.CreateLogger<VeraCoreSFTPService>();
        }

        public void Dispose()
        {
            if (_sftpClient?.IsConnected ?? false)
                _sftpClient.Disconnect();
            _sftpClient?.Dispose();
        }

        public string GetWorkingDirectory()
        {
            EnsureClient();
            return _sftpClient.WorkingDirectory;
        }

        public void UploadFile(VeraCoreSFTPOptions options, Stream file, string name)
        {
            EnsureClient();
            try
            {
                switch (options)
                {
                    case VeraCoreSFTPOptions.GiftList:
                        CreateAndChangeDirectory(_options.Value.VeraCoreSettings.GiftListFolderName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(options));
                }
                _sftpClient.UploadFile(file, name, false);
            }
            catch (Exception e)
            {
                Reconnect();
                switch (options)
                {
                    case VeraCoreSFTPOptions.GiftList:
                        CreateAndChangeDirectory(_options.Value.VeraCoreSettings.GiftListFolderName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(options));
                }
                _sftpClient.UploadFile(file, name, false);

                _logger.LogError(e.ToString());
            }
        }

        public ICollection<VeraCoreFileInfo> GetFileList(VeraCoreSFTPOptions options)
        {
            EnsureClient();
            IEnumerable<SftpFile> files;
            try
            {
                switch (options)
                {
                    case VeraCoreSFTPOptions.Export:
                        _sftpClient.ChangeDirectory(_options.Value.VeraCoreSettings.ExportFolderName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(options));
                }
                files = _sftpClient.ListDirectory(".");
            }
            catch (Exception e)
            {
                Reconnect();
                switch (options)
                {
                    case VeraCoreSFTPOptions.Export:
                        _sftpClient.ChangeDirectory(_options.Value.VeraCoreSettings.ExportFolderName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(options));
                }
                files  = _sftpClient.ListDirectory(".");

                _logger.LogError(e.ToString());
            }
            var toReturn = files.Where(p=>!p.IsDirectory).Select(p => new VeraCoreFileInfo()
            {
                FileDate = p.LastWriteTime,
                FileName = p.Name,
                FileSize = p.Length
            }).ToList();

            return toReturn;
        }

        public void RemoveFile(string fileName)
        {
            EnsureClient();
            _sftpClient.DeleteFile(fileName);
        }

        public MemoryStream DownloadFileData(string fileName)
        {
            EnsureClient();
            var result = new MemoryStream();
            try
            {
                _sftpClient.DownloadFile(fileName, result);
            }
            catch (Exception e)
            {
                Reconnect();
                result.Close();
                result = new MemoryStream();
                _sftpClient.DownloadFile(fileName, result);

                _logger.LogError(e.ToString());
            }
            result.Seek(0, SeekOrigin.Begin);
            return result;
        }

        protected virtual void EnsureClient()
        {
            if (_sftpClient == null)
            {
                _sftpClient = CreateAndConnect();
            }
        }

        protected virtual void Reconnect()
        {
            if (_sftpClient?.IsConnected ?? false)
            {
                _sftpClient.Disconnect();
            }
            _sftpClient?.Dispose();

            _sftpClient = CreateAndConnect();
        }

        protected virtual SftpClient CreateAndConnect()
        {
            var connectionInfo = new PasswordConnectionInfo(_options.Value.VeraCoreSettings.ServerHost,
                _options.Value.VeraCoreSettings.ServerPort,
                _options.Value.VeraCoreSettings.UserName, _options.Value.VeraCoreSettings.Password)
            {
                Timeout = new TimeSpan(0, 0, 30)
            };
            var result = new SftpClient(connectionInfo);
            result.Connect();
            return result;
        }

        private void CreateAndChangeDirectory(string dirName)
        {
            if (!_sftpClient.Exists(dirName))
            {
                _sftpClient.CreateDirectory(dirName);
            }
            _sftpClient.ChangeDirectory(dirName);
        }
    }
}