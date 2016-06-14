using System;
using System.Data.SqlClient;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.ExportService.Context;
using VitalChoice.ExportService.Entities;
using VitalChoice.Infrastructure.Domain.ServiceBus;
using VitalChoice.Infrastructure.Extensions;
using VitalChoice.Infrastructure.ServiceBus.Base;

namespace VitalChoice.ExportService.Services
{
    public class EncryptionKeyUpdater: IDisposable
    {
        private readonly IObjectEncryptionHost _encryptionHost;
        private readonly ILogger _logger;
        private readonly IOptions<ExportOptions> _options;
        private readonly DbContextOptions<ExportInfoContext> _contextOptions;
        private bool _terminated;
        private readonly ManualResetEvent _terminateReadyEvent;
        private readonly string _keyFilePath;
        private readonly Thread _updateThread;

        public EncryptionKeyUpdater(IObjectEncryptionHost encryptionHost, ILogger logger, IOptions<ExportOptions> options, DbContextOptions<ExportInfoContext> contextOptions)
        {
            _encryptionHost = encryptionHost;
            _logger = logger;
            _options = options;
            _contextOptions = contextOptions;
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
                    var lasAccessed = File.GetLastWriteTime(_keyFilePath);
                    if (DateTime.Now - lasAccessed > TimeSpan.FromDays(30))
                    {
                        Aes newAes;
                        var key = CreateNewKey(out newAes);
                        using (
                            SqlConnection conn =
                                new SqlConnection(_options.Value.ExportConnection.GetMasterConnectionString())
                            )
                        {
                            conn.Open();
                            DropCopy(conn);
                            CopyDatabase(conn);
                            ReCryptDatabaseCopy(newAes, 500);
                            _encryptionHost.UpdateLocalKey(key);
                            RenameCopyToCurrent(conn);
                            conn.Close();
                        }
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
                Thread.Sleep(TimeSpan.FromDays(1));
            }
        }

        private static void RenameCopyToCurrent(SqlConnection conn)
        {
            string status;
            using (
                var statusCmd = new SqlCommand("SELECT TOP 1 state_desc FROM sys.databases WHERE name = 'VitalChoice.ExportInfo.Copy';",
                    conn))
            {
                status = statusCmd.ExecuteScalar() as string;
            }
            if (!string.IsNullOrEmpty(status))
            {
                using (SqlCommand dropCmd =
                    new SqlCommand("DROP DATABASE [VitalChoice.ExportInfo]", conn))
                {
                    dropCmd.ExecuteNonQuery();
                }
                using (SqlCommand renameCmd =
                    new SqlCommand("ALTER DATABASE [VitalChoice.ExportInfo.Copy] MODIFY NAME = [VitalChoice.ExportInfo]", conn))
                {
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
            using (ExportInfoCopyContext copyContext = new ExportInfoCopyContext(_options, _contextOptions))
            {
                var localAes = Aes.Create();
                if (localAes == null)
                {
                    throw new CryptographicException("Cannot create AES");
                }
                var localKey = _encryptionHost.GetLocalKey();
                localAes.KeySize = 256;
                localAes.Key = localKey.Key;
                localAes.IV = localKey.IV;

                var uow = new UnitOfWorkBase(copyContext);

                var customersRep = uow.RepositoryAsync<CustomerPaymentMethodExport>();
                var totalNumber = customersRep.Query().SelectCountAsync().GetAwaiter().GetResult();
                var pages = totalNumber/batchSize + 1;
                for (int i = 0; i < pages; i++)
                {
                    var toUpdate = customersRep.Query().SelectPageAsync(i, batchSize, true).GetAwaiter().GetResult();
                    foreach (var item in toUpdate.Items)
                    {
                        item.CreditCardNumber = localAes.RewriteBlock(newAes, item.CreditCardNumber);
                    }
                    uow.SaveChanges();
                }


                var ordersRep = uow.RepositoryAsync<OrderPaymentMethodExport>();
                totalNumber = ordersRep.Query().SelectCountAsync().GetAwaiter().GetResult();
                pages = totalNumber/batchSize + 1;
                for (int i = 0; i < pages; i++)
                {
                    var toUpdate = ordersRep.Query().SelectPageAsync(i, batchSize, true).GetAwaiter().GetResult();
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
