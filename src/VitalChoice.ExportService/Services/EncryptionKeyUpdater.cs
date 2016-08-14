using System;
using System.Data.SqlClient;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VitalChoice.Data.UOW;
using VitalChoice.ExportService.Context;
using VitalChoice.ExportService.Entities;
using VitalChoice.Infrastructure.Domain.ServiceBus;
using VitalChoice.Infrastructure.Extensions;
using VitalChoice.Infrastructure.ServiceBus.Base.Crypto;

namespace VitalChoice.ExportService.Services
{
    public class EncryptionKeyUpdater: IDisposable
    {
        private readonly IObjectEncryptionHost _encryptionHost;
        private readonly ILogger _logger;
        private readonly IOptions<ExportOptions> _options;
        private readonly DbContextOptions<ExportInfoContext> _contextOptions;
        private readonly IOrderExportService _exportService;
        private bool _terminated;
        private readonly ManualResetEvent _terminateReadyEvent;
        private readonly string _keyFilePath;
        private readonly Thread _updateThread;

        public EncryptionKeyUpdater(IObjectEncryptionHost encryptionHost, ILogger logger, IOptions<ExportOptions> options,
            DbContextOptions<ExportInfoContext> contextOptions, IOrderExportService exportService)
        {
            _encryptionHost = encryptionHost;
            _logger = logger;
            _options = options;
            _contextOptions = contextOptions;
            _exportService = exportService;
            _keyFilePath = options.Value.LocalEncryptionKeyPath;
            _terminateReadyEvent = new ManualResetEvent(true);
            if (!string.IsNullOrWhiteSpace(_keyFilePath))
            {
                _updateThread = new Thread(KeyUpdateProcess);
                _updateThread.Start();
            }
        }

        private void KeyUpdateProcess()
        {
            while (!_terminated)
            {
                try
                {
                    _terminateReadyEvent.Reset();
                    var lastWriteTime = File.GetLastWriteTime(_keyFilePath);
                    if (DateTime.Now - lastWriteTime > TimeSpan.FromDays(30) && _options.Value.ScheduleDayTimeHour == DateTime.Now.Hour)
                    {
                        Aes newAes;
                        var key = CreateNewKey(out newAes);
                        using (
                            SqlConnection conn =
                                new SqlConnection(_options.Value.ExportConnection.GetMasterConnectionString())
                            )
                        {
                            _exportService.SwitchToInMemoryContext();
                            conn.Open();
                            DropCopy(conn);
                            CopyDatabase(conn);
                            ReCryptDatabaseCopy(newAes, 500);
                            _encryptionHost.UpdateLocalKey(key);
                            while (true)
                            {
                                try
                                {
                                    RenameCopyToCurrent(conn);
                                    break;
                                }
                                catch (Exception e)
                                {
                                    _logger.LogCritical(e.ToString());
                                    _logger.LogWarning("Awaiting 1 minute before retry");
                                }
                                Thread.Sleep(TimeSpan.FromMinutes(1));
                            }
                            _exportService.SwitchToRealContext().GetAwaiter().GetResult();
                            conn.Close();
                        }
                    }
                    else if (DateTime.Now - lastWriteTime > TimeSpan.FromDays(30))
                    {
                        _logger.LogWarning($"Skip key update, hour is {DateTime.Now.Hour}, but need {_options.Value.ScheduleDayTimeHour}");
                    }
                }
                catch (Exception e)
                {
                    _logger.LogCritical(e.ToString());
                }
                finally
                {
                    _terminateReadyEvent.Set();
                }
                Thread.Sleep(TimeSpan.FromMinutes(30));
            }
        }

        private static void RenameCopyToCurrent(SqlConnection conn)
        {
            string status;
            using (
                var copyStatus = new SqlCommand("SELECT TOP 1 state_desc FROM sys.databases WHERE name = 'VitalChoice.ExportInfo.Copy';",
                    conn))
            {
                status = copyStatus.ExecuteScalar() as string;
            }
            if (!string.IsNullOrEmpty(status))
            {
                using (
                var currentStatus = new SqlCommand("SELECT TOP 1 state_desc FROM sys.databases WHERE name = 'VitalChoice.ExportInfo';",
                    conn))
                {
                    status = currentStatus.ExecuteScalar() as string;
                }
                if (!string.IsNullOrEmpty(status))
                {
                    using (SqlCommand dropCmd =
                        new SqlCommand("DROP DATABASE [VitalChoice.ExportInfo]", conn))
                    {
                        dropCmd.CommandTimeout = 0;
                        dropCmd.ExecuteNonQuery();
                    }
                }
                using (SqlCommand renameCmd =
                    new SqlCommand("ALTER DATABASE [VitalChoice.ExportInfo.Copy] MODIFY NAME = [VitalChoice.ExportInfo]", conn))
                {
                    renameCmd.CommandTimeout = 0;
                    renameCmd.ExecuteNonQuery();
                }
                using (
                    var statusCmd = new SqlCommand("SELECT TOP 1 state_desc FROM sys.databases WHERE name = 'VitalChoice.ExportInfo';",
                        conn))
                {
                    while (true)
                    {
                        status = statusCmd.ExecuteScalar() as string;
                        if (string.IsNullOrEmpty(status))
                        {
                            Thread.Sleep(TimeSpan.FromSeconds(60));
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }


        private static void DropCopy(SqlConnection conn)
        {
            using (
                var statusCmd = new SqlCommand("SELECT TOP 1 state_desc FROM sys.databases WHERE name = 'VitalChoice.ExportInfo.Copy';",
                    conn))
            {
                var status = statusCmd.ExecuteScalar() as string;
                if (!string.IsNullOrEmpty(status))
                {
                    using (SqlCommand dropCmd =
                        new SqlCommand("DROP DATABASE [VitalChoice.ExportInfo.Copy]", conn))
                    {
                        dropCmd.CommandTimeout = 0;
                        dropCmd.ExecuteNonQuery();
                    }
                    while (true)
                    {
                        status = statusCmd.ExecuteScalar() as string;
                        if (string.IsNullOrEmpty(status))
                        {
                            break;
                        }
                        Thread.Sleep(TimeSpan.FromSeconds(60));
                    }
                }
            }
        }

        private static void CopyDatabase(SqlConnection conn)
        {
            using (
                var statusCmd = new SqlCommand("SELECT TOP 1 state_desc FROM sys.databases WHERE name = 'VitalChoice.ExportInfo.Copy';",
                    conn))
            {
                using (
                    var command = new SqlCommand("CREATE DATABASE [VitalChoice.ExportInfo.Copy] AS COPY OF [VitalChoice.ExportInfo];", conn)
                    )
                {
                    command.CommandTimeout = 0;
                    command.ExecuteNonQuery();
                }
                while (true)
                {
                    var status = statusCmd.ExecuteScalar() as string;
                    if (string.IsNullOrEmpty(status))
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(60));
                    }
                    else if (status == "COPYING")
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(60));
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        private void ReCryptDatabaseCopy(Aes newAes, int batchSize)
        {
            _logger.LogWarning("Starting DB recrypt");
            var copyContext = new ExportInfoCopyContext(_options, _contextOptions);
            var localAes = Aes.Create();
            if (localAes == null)
            {
                throw new CryptographicException("Cannot create AES");
            }
            var localKey = _encryptionHost.GetLocalKey();
            localAes.KeySize = 256;
            localAes.Key = localKey.Key;
            localAes.IV = localKey.IV;

            using (var uow = new UnitOfWork(copyContext, true))
            {
                var customersRep = uow.RepositoryAsync<CustomerPaymentMethodExport>();
                var totalNumber = customersRep.Query().SelectCountAsync().GetAwaiter().GetResult();
                _logger.LogWarning($"Starting customers recrypt, total number of records: {totalNumber}");
                var pages = totalNumber/batchSize + 1;
                for (int i = 0; i < pages; i++)
                {
                    var toUpdate = customersRep.Query().SelectPageAsync(i, batchSize, true).GetAwaiter().GetResult();
                    _logger.LogWarning($"Updating batch {i}, number of elemets: {toUpdate.Items.Count}");
                    foreach (var item in toUpdate.Items)
                    {
                        item.CreditCardNumber = localAes.RewriteBlock(newAes, item.CreditCardNumber);
                    }
                    uow.SaveChanges();
                }


                var ordersRep = uow.RepositoryAsync<OrderPaymentMethodExport>();
                totalNumber = ordersRep.Query().SelectCountAsync().GetAwaiter().GetResult();
                _logger.LogWarning($"Starting orders recrypt, total number of records: {totalNumber}");
                pages = totalNumber/batchSize + 1;
                for (int i = 0; i < pages; i++)
                {
                    var toUpdate = ordersRep.Query().SelectPageAsync(i, batchSize, true).GetAwaiter().GetResult();
                    _logger.LogWarning($"Updating batch {i}, number of elemets: {toUpdate.Items.Count}");
                    foreach (var item in toUpdate.Items)
                    {
                        item.CreditCardNumber = localAes.RewriteBlock(newAes, item.CreditCardNumber);
                    }
                    uow.SaveChanges();
                }
            }
        }

        private static KeyExchange CreateNewKey(out Aes aes)
        {
            aes = Aes.Create();
            if (aes == null)
            {
                throw new CryptographicException("Cannot create AES");
            }
            aes.KeySize = 256;
            aes.GenerateIV();
            aes.GenerateKey();
            var key = new KeyExchange(aes.Key, aes.IV);
            return key;
        }

        public void Dispose()
        {
            _terminated = true;
            _terminateReadyEvent.WaitOne();
            _updateThread.Abort();
        }
    }
}
