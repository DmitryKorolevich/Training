using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using VitalChoice.Data.Context;
using VitalChoice.Ecommerce.Domain.Entities.Logs;
using VitalChoice.Ecommerce.Domain.Options;

namespace VitalChoice.Ecommerce.Context
{
    public class LogsContext : DataContext
    {
        protected readonly IOptions<AppOptionsBase> Options;

        public LogsContext(IOptions<AppOptionsBase> options, DbContextOptions<LogsContext> contextOptions) : base(contextOptions)
        {
            Options = options;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            //var connectionString = (new SqlConnectionStringBuilder
            //{
            //    DataSource = @"(LocalDB)\MSSQLLocalDB",
            //    // TODO: Currently nested queries are run while processing the results of outer queries
            //    // This either requires MARS or creation of a new connection for each query. Currently using
            //    // MARS since cloning connections is known to be problematic.
            //    MultipleActiveResultSets = true,
            //    AttachDBFilename = Options.Value.LogPath + "\\Logs.mdf",
            //    IntegratedSecurity = true,
            //    ConnectTimeout = 60
            //}).ConnectionString;
            builder.UseSqlServer("Data Source=.\\;Integrated Security=true;initial catalog=Logs;");

            base.OnConfiguring(builder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<CommonLogItem>().HasKey(p => p.Id);
            builder.Entity<CommonLogItem>().ToTable("CommonLogItems");

            base.OnModelCreating(builder);
        }
    }
}