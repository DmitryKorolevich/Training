using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Metadata;
using System.Data.SqlClient;
using System.IO;
using VitalChoice.Data.DataContext;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.eCommerce;
using VitalChoice.Domain.Entities.Localization;

namespace VitalChoice.Infrastructure.Context
{
	public class EcommerceContext : DataContext
	{
		private static bool created;

		public EcommerceContext()
		{
			// Create the database and schema if it doesn't exist
			// This is a temporary workaround to create database until Entity Framework database migrations 
			// are supported in ASP.NET 5
			if (!created)
			{
				Database.AsRelational().AsSqlServer();//.EnsureCreated();//ApplyMigration()//.AsMigrationsEnabled()
                created = true;
			}
		}

	    public EcommerceContext(bool uofScoped = false) : this()
	    {
	        
	    }       

        protected override void OnConfiguring(DbContextOptions builder)
		{
            var connectionString = (new SqlConnectionStringBuilder
            {
                DataSource = @"localhost",
                // TODO: Currently nested queries are run while processing the results of outer queries
                // This either requires MARS or creation of a new connection for each query. Currently using
                // MARS since cloning connections is known to be problematic.
                MultipleActiveResultSets = true,
                InitialCatalog = "VitalChoice.Ecommerce",
                IntegratedSecurity = true,
                ConnectTimeout = 60
            }).ConnectionString;
            builder.UseSqlServer(connectionString);

			base.OnConfiguring(builder);
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{

			base.OnModelCreating(builder);
		}
	}
}
