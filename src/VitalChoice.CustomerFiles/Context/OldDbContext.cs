using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using VitalChoice.CustomerFiles.Entities;
using VitalChoice.Data.Context;
using VitalChoice.Infrastructure.Domain.Options;

namespace VitalChoice.CustomerFiles.Context
{
    public class OldDbContext : DataContext
    {
        protected readonly IOptions<AppOptions> Options;

        public OldDbContext(IOptions<AppOptions> options, DbContextOptions<OldDbContext> contextOptions) : base(contextOptions)
        {
            Options = options;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            var connectionString = new SqlConnectionStringBuilder
            {
                DataSource = "(local)",
                MultipleActiveResultSets = true,
                InitialCatalog = "vitalchoice2.0",
                IntegratedSecurity = true,
                ConnectTimeout = 60
            }.ConnectionString;
            builder.UseSqlServer(connectionString);

            base.OnConfiguring(builder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<VCustomerOldFile>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("CustomerFiles");
            });
        }
    }
}