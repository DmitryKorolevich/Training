using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Transaction;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.VeraCore;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.VeraCore;
using System.Text;
using System.Text.RegularExpressions;

namespace VitalChoice.Business.Services.VeraCore
{
    public class VeraCoreNotificationService : IVeraCoreNotificationService
    {
        private readonly IOptions<AppOptions> _options;
        private readonly IRepositoryAsync<VeraCoreProcessItem> _veraCoreProcessItemRepository;
        private readonly IRepositoryAsync<VeraCoreProcessLogItem> _veraCoreProcessLogItemRepository;
        private readonly IVeraCoreSFTPService _veraCoreSFTPService;
        private readonly IVeraCoreFilesCacheService _veraCoreFilesCacheService;
        private readonly ITransactionAccessor<VitalChoiceContext> _transactionAccessor;
        private readonly Regex _shipFileNamePattern;
        private readonly Regex _cancelFileNamePattern;
        private readonly ILogger _logger;

        public VeraCoreNotificationService(
            IOptions<AppOptions> options,
            IRepositoryAsync<VeraCoreProcessItem> veraCoreProcessItemRepository,
            IRepositoryAsync<VeraCoreProcessLogItem> veraCoreProcessLogItemRepository,
            IVeraCoreSFTPService veraCoreSFTPService,
            IVeraCoreFilesCacheService veraCoreFilesCacheService,
            ITransactionAccessor<VitalChoiceContext> transactionAccessor,
            ILoggerProviderExtended logger)
        {
            _options = options;
            _veraCoreProcessItemRepository = veraCoreProcessItemRepository;
            _veraCoreProcessLogItemRepository = veraCoreProcessLogItemRepository;
            _veraCoreSFTPService = veraCoreSFTPService;
            _veraCoreFilesCacheService = veraCoreFilesCacheService;
            _transactionAccessor = transactionAccessor;
            _shipFileNamePattern = new Regex(VeraCoreConstants.ShipPattern, RegexOptions.Compiled);
            _cancelFileNamePattern = new Regex(VeraCoreConstants.CancelPattern, RegexOptions.Compiled);
            _logger = logger.CreateLogger<VeraCoreSFTPService>();
        }

        public async Task ProcessFiles()
        {
            _logger.LogInformation("Initiating FTP Scan");
            try
            {
                var fileList = _veraCoreSFTPService.GetFileList(VeraCoreSFTPOptions.Export);
                _logger.LogInformation("List Done");

                using (var uow = _transactionAccessor.CreateUnitOfWork())
                {
                    var localVeraCoreProcessItemRepository = uow.RepositoryAsync<VeraCoreProcessItem>();
                    var localVeraCoreProcessLogItemRepository = uow.RepositoryAsync<VeraCoreProcessLogItem>();

                    //Do not process again the same files
                    var fileNames = fileList.Select(p => p.FileName).ToList();
                    fileNames = fileNames.Where(p => _shipFileNamePattern.IsMatch(p) || _cancelFileNamePattern.IsMatch(p)).ToList();
                    var logItems = await localVeraCoreProcessItemRepository.Query(p => fileNames.Contains(p.FileName)).SelectAsync(false);
                    foreach (var logItem in logItems)
                    {
                        var file = fileList.FirstOrDefault(p => p.FileName == logItem.FileName);
                        if (file != null && file.FileDate == logItem.FileDate && file.FileSize == logItem.FileSize)
                        {
                            fileList.Remove(file);
                            //And remove from source
                            _veraCoreSFTPService.RemoveFile(file.FileName);
                        }
                    }

                    _logger.LogInformation("Downloading files");
                    var files = new List<VeraCoreFile>();
                    foreach (var fileInfo in fileList)
                    {
                        var file = CacheFile(fileInfo);
                        if (file != null)
                        {
                            files.Add(file);
                        }
                    }

                    var activeItems = new List<VeraCoreProcessItem>();
                    var newLogItems = new List<VeraCoreProcessLogItem>();
                    files.ForEach(p =>
                    {
                        var activeItem = new VeraCoreProcessItem()
                        {
                            Attempt = 0,
                            Data = p.Data,
                            FileName = p.FileName,
                            FileSize = p.FileSize,
                            FileDate = p.FileDate,
                            DateCreated = DateTime.Now,
                        };
                        if (_shipFileNamePattern.IsMatch(p.FileName))
                        {
                            activeItem.IdType=VeraCoreProcessItemType.Ship;
                        }
                        if (_cancelFileNamePattern.IsMatch(p.FileName))
                        {
                            activeItem.IdType = VeraCoreProcessItemType.Cancel;
                        }
                        activeItems.Add(activeItem);

                        var logItem = new VeraCoreProcessLogItem()
                        {
                            FileName = p.FileName,
                            FileSize = p.FileSize,
                            FileDate = p.FileDate,
                            DateCreated = DateTime.Now,
                        };
                        newLogItems.Add(logItem);
                    });

                    await localVeraCoreProcessItemRepository.InsertRangeAsync(activeItems);
                    await localVeraCoreProcessLogItemRepository.InsertRangeAsync(newLogItems);
                    await uow.SaveChangesAsync();

                    foreach (var file in files)
                    {
                        _veraCoreSFTPService.RemoveFile(file.FileName);
                    }
                }

                _logger.LogInformation("SFTP files are processed succefully");
            }
            catch (Exception e)
            {
                _logger.LogError("SFTP files error");
                _logger.LogError(e.ToString());
            }
        }

        private VeraCoreFile CacheFile(VeraCoreFileInfo fileInfo)
        {
            VeraCoreFile toReturn = null;
            using (MemoryStream stream = _veraCoreSFTPService.DownloadFileData(fileInfo.FileName))
            {
                var result = _veraCoreFilesCacheService.CacheFile(fileInfo, stream,
                    _options.Value.VeraCoreSettings.ExportFolderName);
                if (result)
                {
                    toReturn = new VeraCoreFile(fileInfo, Encoding.ASCII.GetString(stream.ToArray()));
                }
                stream.Close();
            }
            return toReturn;
        }

        public async Task ProcessQueue()
        {
            throw new NotImplementedException();
        }
    }
}