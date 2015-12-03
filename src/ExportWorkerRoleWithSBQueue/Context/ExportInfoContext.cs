using System.Data.SqlClient;
using ExportWorkerRoleWithSBQueue.Entities;
using Microsoft.Data.Entity;
using Microsoft.Extensions.OptionsModel;
using VitalChoice.Data.Context;

namespace ExportWorkerRoleWithSBQueue.Context
{
    public class ExportInfoContext : DataContext
    {
        private readonly IOptions<ExportOptions> _options;

        public ExportInfoContext(IOptions<ExportOptions> options)
        {
            _options = options;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            var connectionString = (new SqlConnectionStringBuilder
            {
                DataSource = _options.Value.ExportConnection.Server,
                MultipleActiveResultSets = true,
                InitialCatalog = "VitalChoice.ExportInfo",
                UserID = _options.Value.ExportConnection.UserName,
                Password = _options.Value.ExportConnection.Password,
                ConnectTimeout = 60,
                Encrypt = _options.Value.ExportConnection.Encrypt,
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