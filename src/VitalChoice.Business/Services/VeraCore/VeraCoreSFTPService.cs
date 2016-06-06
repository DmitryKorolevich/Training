using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            ILoggerProviderExtended logger)
        {
            _options = options;
            _logger = logger.CreateLogger<VeraCoreSFTPService>();
        }

        protected void Reconnect()
        {
            if (_sftpClient != null)
            {
                if (_sftpClient.IsConnected)
                {
                    _sftpClient.Disconnect();
                }
                _sftpClient.Dispose();
            }
            var connectionInfo = new PasswordConnectionInfo(_options.Value.VeraCoreSettings.ServerHost,
                _options.Value.VeraCoreSettings.ServerPort,
                _options.Value.VeraCoreSettings.UserName, _options.Value.VeraCoreSettings.Password);
            connectionInfo.Timeout = new TimeSpan(0, 0, 1);

            _sftpClient = new SftpClient(connectionInfo);
            _sftpClient.Connect();
        }

        public void Dispose()
        {
            if (_sftpClient != null)
            {
                if (_sftpClient.IsConnected)
                    _sftpClient.Disconnect();
                _sftpClient.Dispose();
            }
        }

        public string WorkingDirectory
        {
            get
            {
                if (_sftpClient == null)
                {
                    Reconnect();
                }
                return _sftpClient.WorkingDirectory;
            }
        }

        public ICollection<VeraCoreFileInfo> GetFileList(VeraCoreSFTPOptions options)
        {
            if (_sftpClient == null)
            {
                Reconnect();
            }
            IEnumerable<SftpFile> files;
            try
            {
                switch (options)
                {
                    case VeraCoreSFTPOptions.Export:
                        _sftpClient.ChangeDirectory(_options.Value.VeraCoreSettings.ExportFolderName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("options");
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
                        throw new ArgumentOutOfRangeException("options");
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
            if (_sftpClient == null)
            {
                Reconnect();
            }
            _sftpClient.DeleteFile(fileName);
        }

        public MemoryStream DownloadFileData(string fileName)
        {
            if (_sftpClient == null)
            {
                Reconnect();
            }
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
    }
}