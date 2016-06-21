using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using VitalChoice.Data.Context;
using VitalChoice.ExportService.Entities;

namespace VitalChoice.ExportService.Context
{
    public class ExportInfoContext : DataContext
    {
        protected readonly IOptions<ExportOptions> Options;

        public ExportInfoContext(IOptions<ExportOptions> options, DbContextOptions<ExportInfoContext> contextOptions): base(contextOptions)
        {
            Options = options;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            var connectionString = (new SqlConnectionStringBuilder
            {
                DataSource = Options.Value.ExportConnection.Server,
                MultipleActiveResultSets = true,
                InitialCatalog = "VitalChoice.ExportInfo",
                UserID = Options.Value.ExportConnection.UserName,
                Password = Options.Value.ExportConnection.Password,
                ConnectTimeout = 60,
                Encrypt = Options.Value.ExportConnection.Encrypt,
                TrustServerCertificate = false
            }).ConnectionString;
            builder.UseSqlServer(connectionString);

            base.OnConfiguring(builder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ForSqlServerUseIdentityColumns();
            builder.Entity<CustomerPaymentMethodExport>(entity =>
            {
                entity.Ignore(c => c.Id);
                entity.ToTable("CustomerPaymentMethods");
                entity.HasKey(c => new {c.IdCustomer, c.IdPaymentMethod});
            });
            builder.Entity<OrderPaymentMethodExport>(entity =>
            {
                entity.Ignore(c => c.Id);
                entity.ToTable("OrderPaymentMethods");
                entity.HasKey(c => new {c.IdOrder});
            });
        }
    }
}