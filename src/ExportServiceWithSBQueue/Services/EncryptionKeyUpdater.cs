using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using ExportServiceWithSBQueue.Context;
using ExportServiceWithSBQueue.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.ServiceBus;
using VitalChoice.Infrastructure.ServiceBus;
using VitalChoice.Infrastructure.Extensions;
using VitalChoice.Infrastructure.ServiceBus.Base;

namespace ExportServiceWithSBQueue.Services
{
    public class EncryptionKeyUpdater: IDisposable
    {
        private readonly IObjectEncryptionHost _encryptionHost;
        private readonly ILogger _logger;
        private readonly IOptions<ExportOptions> _options;
        private bool _terminated;
        private readonly ManualResetEvent _terminateReadyEvent;
        private readonly string _keyFilePath;

        public EncryptionKeyUpdater(IObjectEncryptionHost encryptionHost, ILogger logger, IOptions<ExportOptions> options)
        {
            _encryptionHost = encryptionHost;
            _logger = logger;
            _options = options;
            _keyFilePath = options.Value.LocalEncryptionKeyPath;
            _terminateReadyEvent = new ManualResetEvent(true);
            if (!string.IsNullOrWhiteSpace(_keyFilePath))
            {
                new Thread(KeyUpdateProcess).Start();
            }
        }

        private void KeyUpdateProcess()
        {
            while (!_terminated)
            {
                try
                {
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
                            conn.Close();
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogCritical(e.Message, e);
                    throw;
                }
                finally
                {
                    _terminateReadyEvent.Set();
                }
            }
        }

        private static void DropCopy(SqlConnection conn)
        {
            var statusCmd = new SqlCommand("SELECT TOP 1 state_desc FROM sys.databases WHERE name = 'VitalChoice.ExportInfo.Copy';", conn);
            var status = statusCmd.ExecuteScalar() as string;
            if (!string.IsNullOrEmpty(status))
            {
                SqlCommand dropCmd =
                    new SqlCommand("DROP DATABASE VitalChoice.ExportInfo.Copy", conn);
                dropCmd.ExecuteNonQuery();
            }
        }

        private static void CopyDatabase(SqlConnection conn)
        {
            var statusCmd = new SqlCommand("SELECT TOP 1 state_desc FROM sys.databases WHERE name = 'VitalChoice.ExportInfo.Copy';", conn);
            var command = new SqlCommand("CREATE DATABASE VitalChoice.ExportInfo.Copy AS COPY OF VitalChoice.ExportInfo;", conn);
            command.ExecuteNonQuery();

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

        private void ReCryptDatabaseCopy(Aes newAes, int batchSize)
        {
            using (ExportInfoCopyContext copyContext = new ExportInfoCopyContext(_options))
            {
                var localAes = Aes.Create();
                var localKey = _encryptionHost.GetLocalKey();
                if (localAes == null)
                {
                    throw new CryptographicException("Cannot create AES");
                }
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
        }
    }
}
