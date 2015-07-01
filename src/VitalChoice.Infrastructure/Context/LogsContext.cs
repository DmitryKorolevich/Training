using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Metadata;
using Microsoft.Framework.OptionsModel;
using System.Data.SqlClient;
using System.IO;
using VitalChoice.Data.DataContext;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.Localization;
using VitalChoice.Domain.Entities.Logs;
using VitalChoice.Domain.Entities.Options;

namespace VitalChoice.Infrastructure.Context
{
	public class LogsContext : DataContext
    {
		private static bool created;

        private readonly IOptions<AppOptions> appOptionsAccessor;
        
	    public LogsContext(IOptions<AppOptions> appOptionsAccessor)
	    {
            this.appOptionsAccessor = appOptionsAccessor;

            // Create the database and schema if it doesn't exist
            // This is a temporary workaround to create database until Entity Framework database migrations 
            // are supported in ASP.NET 5
            if (!created)
            {
                //Database.AsRelational().AsSqlServer();//.EnsureCreated();//ApplyMigration()//.AsMigrationsEnabled()
                created = true;
            }
        }

        protected override void OnConfiguring(EntityOptionsBuilder builder)
		{
            var connectionString = (new SqlConnectionStringBuilder
            {
                DataSource = @"(LocalDB)\MSSQLLocalDB",
                // TODO: Currently nested queries are run while processing the results of outer queries
                // This either requires MARS or creation of a new connection for each query. Currently using
                // MARS since cloning connections is known to be problematic.
                MultipleActiveResultSets = true,
                AttachDBFilename = appOptionsAccessor.Options.LogPath + "\\Logs.mdf",
                IntegratedSecurity = true,
                ConnectTimeout = 60
            }).ConnectionString;
            builder.UseSqlServer(connectionString);

			base.OnConfiguring(builder);
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
            builder.Entity<CommonLogItem>().Key(p => p.Id);
		    builder.Entity<CommonLogItem>().ForSqlServer().Table("CommonLogItems");

            base.OnModelCreating(builder);
		}
	}
}
