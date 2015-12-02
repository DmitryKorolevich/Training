using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Options;

namespace ExportWorkerRoleWithSBQueue.Context
{
    public class ExportDbConnection
    {
        public string Server { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool Encrypt { get; set; }
    }

    public class ExportOptions : AppOptions
    {
        public ExportDbConnection ExportConnection { get; set; }
    }
}
