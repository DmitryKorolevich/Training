using VitalChoice.ExportService.Context;

namespace VitalChoice.ExportService
{
    public static class ConnectionStringExtension
    {
        public static string GetMasterConnectionString(this ExportDbConnection exportDbConnection)
        {
            return
                $"Server={exportDbConnection.Server};Database=master;User ID={exportDbConnection.AdminUserName};Password={exportDbConnection.AdminPassword};Trusted_Connection=False;Encrypt={exportDbConnection.Encrypt};Connection Timeout=0";
        }
    }
}