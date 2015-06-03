using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Metadata;
using System.Data.SqlClient;
using System.IO;
using Microsoft.Framework.OptionsModel;
using VitalChoice.Data.DataContext;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.eCommerce;
using VitalChoice.Domain.Entities.Localization;
using VitalChoice.Domain.Entities.Options;
using VitalChoice.Domain.Entities.Product;
using VitalChoice.Domain.Entities.Settings;
using VitalChoice.Domain.Entities.Workflow;

namespace VitalChoice.Infrastructure.Context
{
	public class EcommerceContext : DataContext
    {
	    private readonly IOptions<AppOptions> _options;
	    private static bool created;

		public EcommerceContext(IOptions<AppOptions> options)
		{
		    _options = options;
		    // Create the database and schema if it doesn't exist
			// This is a temporary workaround to create database until Entity Framework database migrations 
			// are supported in ASP.NET 5
			if (!created)
			{
				//Database.AsRelational().AsSqlServer();//.EnsureCreated();//ApplyMigration()//.AsMigrationsEnabled()
                created = true;
			}
		}

	    public EcommerceContext(IOptions<AppOptions> options, bool uofScoped = false) : this(options)
	    {
	        
	    }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
		{
            var connectionString = (new SqlConnectionStringBuilder
            {
                DataSource = _options.Options.Connection.Server,
                // TODO: Currently nested queries are run while processing the results of outer queries
                // This either requires MARS or creation of a new connection for each query. Currently using
                // MARS since cloning connections is known to be problematic.
                MultipleActiveResultSets = true,
                InitialCatalog = "VitalChoice.Ecommerce",
                UserID = _options.Options.Connection.UserName,
                Password = _options.Options.Connection.Password,
                ConnectTimeout = 60
            }).ConnectionString;
            builder.UseSqlServer(connectionString);

			base.OnConfiguring(builder);
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
            builder.ForSqlServer().UseIdentity();

            #region Workflow

		    builder.Entity<WorkflowExecutor>().Key(w => w.Id);
            builder.Entity<WorkflowExecutor>().ForSqlServer().Table("WorkflowExecutors");

            builder.Entity<WorkflowResolverPath>().Key(w => w.Id);
            builder.Entity<WorkflowResolverPath>().ForSqlServer().Table("WorkflowResolverPaths");
		    builder.Entity<WorkflowResolverPath>()
		        .Reference(resolverPath => resolverPath.Executor)
		        .InverseCollection()
		        .ForeignKey(resolverPath => resolverPath.IdExecutor)
                .PrincipalKey(executor => executor.Id);
            builder.Entity<WorkflowResolverPath>()
                .Reference(w => w.Resolver)
                .InverseCollection()
                .ForeignKey(w => w.IdResolver);

            builder.Entity<WorkflowTree>().Key(w => w.Id);
            builder.Entity<WorkflowTree>().ForSqlServer().Table("WorkflowTrees");
		    builder.Entity<WorkflowTree>()
		        .Collection(tree => tree.Actions)
		        .InverseReference()
		        .ForeignKey(action => action.IdTree)
		        .PrincipalKey(tree => tree.Id);

            builder.Entity<WorkflowTreeAction>().Key(w => w.Id);
            builder.Entity<WorkflowTreeAction>().ForSqlServer().Table("WorkflowTreeActions");
		    builder.Entity<WorkflowTreeAction>()
		        .Reference(treeAction => treeAction.Executor)
		        .InverseCollection()
		        .ForeignKey(treeAction => treeAction.IdExecutor)
		        .PrincipalKey(executor => executor.Id);
		    builder.Entity<WorkflowTreeAction>()
		        .Reference(action => action.Tree)
		        .InverseCollection()
		        .ForeignKey(action => action.IdTree)
		        .PrincipalKey(tree => tree.Id);

            #endregion

            #region Products

            builder.Entity<ProductCategory>().Key(p => p.Id);
            builder.Entity<ProductCategory>().ForRelational().Table("ProductCategories");

            builder.Entity<GiftCertificate>().Key(p => p.Id);
            builder.Entity<GiftCertificate>().ForRelational().Table("GiftCertificates");
            builder.Entity<GiftCertificate>().Property(p => p.PublicId).ForSqlServer().UseDefaultValueGeneration();

            builder.Entity<VProductSku>().Key(p => p.Id);
		    builder.Entity<VProductSku>().ForSqlServer().Table("VProductSkus");
            
            builder.Entity<ProductOptionType>().Key(p => p.Id);
            builder.Entity<ProductOptionType>().ForSqlServer().Table("ProductOptionTypes");
            builder.Entity<ProductOptionType>().Reference(p => p.Lookup).InverseCollection().ForeignKey(p => p.IdLookup).PrincipalKey(p => p.Id);

            #endregion

            #region Lookups

            builder.Entity<Lookup>().Key(p => p.Id);
            builder.Entity<Lookup>().ForRelational().Table("Lookups");
            builder.Entity<LookupVariant>().Key(p => new { p.Id, p.IdLookup });
            builder.Entity<LookupVariant>().ForRelational().Table("LookupVariants");
            builder.Entity<Lookup>().Collection(p => p.LookupVariants).InverseReference().ForeignKey(p => p.IdLookup).PrincipalKey(p => p.Id);

            #endregion

            #region Settings

            builder.Entity<Country>().Key(p => p.Id);
            builder.Entity<Country>().ForSqlServer().Table("Countries");
            builder.Entity<Country>().Ignore(p => p.States);

            builder.Entity<State>().Key(p => p.Id);
            builder.Entity<State>().ForSqlServer().Table("States");

            #endregion

            base.OnModelCreating(builder);
		}
	}
}
