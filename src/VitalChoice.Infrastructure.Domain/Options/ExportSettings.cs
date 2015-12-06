using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Infrastructure.Domain.Options
{
    public class ExportService
    {
        public string ConnectionString { get; set; }
        public string PlainQueueName { get; set; }
        public string EncryptedQueueName { get; set; }
        public string CertThumbprint { get; set; }
        public string RootThumbprint { get; set; }
        public bool EncryptionHostSessionExpire { get; set; }
        public string ServerHostName { get; set; }
    }
}