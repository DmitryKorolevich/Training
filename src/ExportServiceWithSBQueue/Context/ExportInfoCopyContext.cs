using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace VitalChoice.ExportService.Context
{
    public class ExportInfoCopyContext : ExportInfoContext
    {
        public ExportInfoCopyContext(IOptions<ExportOptions> options, DbContextOptions<ExportInfoContext> contextOptions) : base(options, contextOptions)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            var connectionString = (new SqlConnectionStringBuilder
            {
                DataSource = Options.Value.ExportConnection.Server,
                MultipleActiveResultSets = true,
                InitialCatalog = "VitalChoice.ExportInfo.Copy",
                UserID = Options.Value.ExportConnection.UserName,
                Password = Options.Value.ExportConnection.Password,
                ConnectTimeout = 60,
                Encrypt = Options.Value.ExportConnection.Encrypt,
                TrustServerCertificate = false
            }).ConnectionString;
            builder.UseSqlServer(connectionString);

            base.OnConfiguring(builder);
        }
    }
}
