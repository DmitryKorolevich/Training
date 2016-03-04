using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExportServiceWithSBQueue.Context;
using VitalChoice.Ecommerce.Domain.Options;

namespace ExportServiceWithSBQueue
{
    public static class ConnectionStringExtension
    {
        public static string GetMasterConnectionString(this ExportDbConnection exportDbConnection)
        {
            return
                $"Server={exportDbConnection.Server};Database=master;User ID={exportDbConnection.AdminUserName};Password={exportDbConnection.AdminPassword};Trusted_Connection=False;Encrypt={exportDbConnection.Encrypt};";
        }
    }
}