using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Infrastructure.Domain.Options
{
    public class ExportService
    {
        public string ConnectionString { get; set; }
        public string SendQueueName { get; set; }
        public string ReceiveQueueName { get; set; }
    }
}