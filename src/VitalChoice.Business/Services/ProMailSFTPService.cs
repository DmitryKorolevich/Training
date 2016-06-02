using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VitalChoice.Business.Queries.Content;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Interfaces.Services.Content;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Interfaces.Services;
using VitalChoice.Infrastructure.UnitOfWork;
using System.Threading;
using VitalChoice.ContentProcessing.Cache;
using VitalChoice.Ecommerce.Cache;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Content.Articles;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.ContentManagement;
using VitalChoice.Infrastructure.Domain.Content.Recipes;
using VitalChoice.Ecommerce.Domain.Mail;
using System.Reflection;
using VitalChoice.Infrastructure.Domain.Mail;
using Templates;
using System.Dynamic;
using System.IO;
using Microsoft.Extensions.Options;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using Templates.Exceptions;
using VitalChoice.Ecommerce.Domain.Helpers;
using Templates.Runtime;
using VitalChoice.Infrastructure.Domain.Content.Emails;
using VitalChoice.Infrastructure.Domain.Entities.ProMail;
using VitalChoice.Infrastructure.Domain.Options;

namespace VitalChoice.Business.Services
{
    public class ProMailSFTPService : IProMailSFTPService, IDisposable
    {
        private readonly IOptions<AppOptions> _options;
        private readonly ILogger _logger;

        private SftpClient _sftpClient;

        public ProMailSFTPService(
            IOptions<AppOptions> options,
            ILoggerProviderExtended logger)
        {
            _options = options;
            _logger = logger.CreateLogger<ProMailSFTPService>();
            var connectionInfo = new PasswordConnectionInfo(options.Value.ProMailSettings.ServerHost,
                options.Value.ProMailSettings.ServerPort,
                options.Value.ProMailSettings.UserName, options.Value.ProMailSettings.Password);
            connectionInfo.Timeout = new TimeSpan(0,0,1);

            Reconnect();
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
            var connectionInfo = new PasswordConnectionInfo(_options.Value.ProMailSettings.ServerHost,
                _options.Value.ProMailSettings.ServerPort,
                _options.Value.ProMailSettings.UserName, _options.Value.ProMailSettings.Password);
            connectionInfo.Timeout = new TimeSpan(0, 0, 1);

            _sftpClient = new SftpClient(connectionInfo);
        }

        public void Dispose()
        {
            if (_sftpClient.IsConnected)
                _sftpClient.Disconnect();
            _sftpClient.Dispose();
        }

        public ICollection<ProMailFileInfo> GetFileList(ProMailSFTPOptions options)
        {
            IEnumerable<SftpFile> files;
            try
            {
                switch (options)
                {
                    case ProMailSFTPOptions.Export:
                        _sftpClient.ChangeDirectory("/" + _options.Value.ProMailSettings.ExportFolderName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("options");
                }
                files = _sftpClient.ListDirectory("/");
            }
            catch (Exception e)
            {
                Reconnect();
                switch (options)
                {
                    case ProMailSFTPOptions.Export:
                        _sftpClient.ChangeDirectory("/" + _options.Value.ProMailSettings.ExportFolderName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("options");
                }
                files  = _sftpClient.ListDirectory("/");

                _logger.LogError(e.ToString());
            }
            var toReturn = files.Where(p=>!p.IsDirectory).Select(p => new ProMailFileInfo()
            {
                FileDate = p.LastWriteTime,
                FileName = p.Name,
                FileSize = p.Length
            }).ToList();

            return toReturn;
        }

        public void RemoveFile(string fileName)
        {
            _sftpClient.DeleteFile(fileName);
        }

        public MemoryStream DownloadFileData(string fileName)
        {
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