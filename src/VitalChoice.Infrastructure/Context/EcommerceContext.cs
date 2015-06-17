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
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Entities.eCommerce.Payment;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Entities.eCommerce.Users;
using VitalChoice.Domain.Entities.Localization;
using VitalChoice.Domain.Entities.Options;
using VitalChoice.Domain.Entities.Settings;
using VitalChoice.Domain.Entities.Workflow;
using VitalChoice.Domain.Entities.eCommerce.GiftCertificates;
using VitalChoice.Domain.Entities.eCommerce.Discounts;

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

            #region Base

            builder.Entity<FieldType>().Key(f => f.Id);
            builder.Entity<FieldType>().ForSqlServer().Table("FieldTypes");

            #endregion


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

            #region GiftCertificates

            builder.Entity<GiftCertificate>().Key(p => p.Id);
            builder.Entity<GiftCertificate>().ForRelational().Table("GiftCertificates");
            builder.Entity<GiftCertificate>().Property(p => p.PublicId).ForSqlServer().UseDefaultValueGeneration();

            #endregion

            #region Discounts

            builder.Entity<DiscountOptionType>().Key(p => p.Id);
            builder.Entity<DiscountOptionType>().ForSqlServer().Table("DiscountOptionTypes");
            builder.Entity<DiscountOptionType>()
                .Reference(p => p.Lookup)
                .InverseCollection()
                .ForeignKey(p => p.IdLookup)
                .PrincipalKey(p => p.Id);
            builder.Entity<DiscountOptionType>()
                .Reference(p => p.FieldType)
                .InverseCollection()
                .ForeignKey(p => p.IdFieldType)
                .PrincipalKey(p => p.Id);

            builder.Entity<DiscountOptionValue>().Key(o => o.Id);
            builder.Entity<DiscountOptionValue>().ForSqlServer().Table("DiscountOptionValues");
            builder.Entity<DiscountOptionValue>()
                .Reference(v => v.OptionType)
                .InverseCollection()
                .ForeignKey(t => t.IdOptionType)
                .PrincipalKey(v => v.Id);

            builder.Entity<DiscountToCategory>().Key(p => p.Id);
            builder.Entity<DiscountToCategory>().ForRelational().Table("DiscountsToCategories");

            builder.Entity<DiscountToSku>().Key(p => p.Id);
            builder.Entity<DiscountToSku>().ForRelational().Table("DiscountsToSkus");
            builder.Entity<DiscountToSku>().Ignore(p => p.ShortSkuInfo);

            builder.Entity<DiscountToSelectedSku>().Key(p => p.Id);
            builder.Entity<DiscountToSelectedSku>().ForRelational().Table("DiscountsToSelectedSkus");
            builder.Entity<DiscountToSelectedSku>().Ignore(p => p.ShortSkuInfo);

            builder.Entity<DiscountTier>().Key(p => p.Id);
            builder.Entity<DiscountTier>().ForRelational().Table("DiscountTiers");

            builder.Entity<Discount>().Key(p => p.Id);
            builder.Entity<Discount>().ForSqlServer().Table("Discounts");
            builder.Entity<Discount>()
                .Collection(p => p.OptionValues)
                .InverseReference()
                .ForeignKey(o => o.IdDiscount)
                .PrincipalKey(p => p.Id);

            builder.Entity<Discount>().Ignore(p => p.OptionTypes);

            builder.Entity<Discount>()
                .Collection(p => p.DiscountsToCategories)
                .InverseReference()
                .ForeignKey(t => t.IdDiscount)
                .PrincipalKey(p => p.Id);
            builder.Entity<Discount>()
                .Collection(p => p.DiscountsToSkus)
                .InverseReference()
                .ForeignKey(t => t.IdDiscount)
                .PrincipalKey(p => p.Id);
            builder.Entity<Discount>()
                .Collection(p => p.DiscountsToSelectedSkus)
                .InverseReference()
                .ForeignKey(t => t.IdDiscount)
                .PrincipalKey(p => p.Id);
            builder.Entity<Discount>()
                .Collection(p => p.DiscountTiers)
                .InverseReference()
                .ForeignKey(t => t.IdDiscount)
                .PrincipalKey(p => p.Id);

            #endregion

            #region Products

            builder.Entity<ProductCategory>().Key(p => p.Id);
            builder.Entity<ProductCategory>().Property(p => p.Id).GenerateValueOnAdd();
            builder.Entity<ProductCategory>().ForRelational().Table("ProductCategories");
		    builder.Entity<ProductCategory>()
		        .Collection(cat => cat.ProductToCategories)
		        .InverseReference()
		        .ForeignKey(c => c.IdCategory)
		        .PrincipalKey(cat => cat.Id);

            builder.Entity<VProductSku>().Key(p => p.IdProduct);
            builder.Entity<VProductSku>().Ignore(x => x.Id);
            builder.Entity<VProductSku>().ForSqlServer().Table("VProductSkus");

            builder.Entity<ProductOptionType>().Key(p => p.Id);
            builder.Entity<ProductOptionType>().ForSqlServer().Table("ProductOptionTypes");
		    builder.Entity<ProductOptionType>()
		        .Reference(p => p.Lookup)
		        .InverseCollection()
		        .ForeignKey(p => p.IdLookup)
		        .PrincipalKey(p => p.Id)
                .Required(false);
            builder.Entity<ProductOptionType>()
                .Reference(p => p.FieldType)
                .InverseCollection()
                .ForeignKey(p => p.IdFieldType)
                .PrincipalKey(p => p.Id)
                .Required();

            builder.Entity<ProductOptionValue>().Key(o => o.Id);
		    builder.Entity<ProductOptionValue>().Property(p => p.Id).GenerateValueOnAdd();
		    builder.Entity<ProductOptionValue>().ForSqlServer().Table("ProductOptionValues");
		    builder.Entity<ProductOptionValue>()
		        .Reference(v => v.OptionType)
		        .InverseCollection()
		        .ForeignKey(t => t.IdOptionType)
		        .PrincipalKey(v => v.Id)
                .Required();

            builder.Entity<ProductTypeEntity>().Key(t => t.Id);
		    builder.Entity<ProductTypeEntity>().ForSqlServer().Table("ProductTypes");

            builder.Entity<Sku>().Key(s => s.Id);
            builder.Entity<Sku>().Property(p => p.Id).GenerateValueOnAdd();
            builder.Entity<Sku>().ForSqlServer().Table("Skus");
		    builder.Entity<Sku>()
		        .Collection(s => s.OptionValues)
		        .InverseReference()
		        .ForeignKey(o => o.IdSku)
		        .PrincipalKey(s => s.Id)
                .Required(false);
            builder.Entity<Sku>().Ignore(p => p.OptionTypes);

            builder.Entity<ProductToCategory>().Key(p => p.Id);
            builder.Entity<ProductToCategory>().ForRelational().Table("ProductsToCategories");

            builder.Entity<Product>().Key(p => p.Id);
            builder.Entity<Product>().Property(p => p.Id).GenerateValueOnAdd();
            builder.Entity<Product>().ForSqlServer().Table("Products");
            builder.Entity<Product>()
                .Collection(p => p.Skus)
                .InverseReference(p=>p.Product)
                .ForeignKey(s => s.IdProduct)
                .PrincipalKey(p => p.Id)
                .Required();
            builder.Entity<Product>()
                .Collection(p => p.OptionValues)
                .InverseReference()
                .ForeignKey(o => o.IdProduct)
                .PrincipalKey(p => p.Id)
                .Required(false);

            builder.Entity<Product>().Ignore(p => p.OptionTypes);

		    builder.Entity<Product>()
		        .Collection(p => p.ProductsToCategories)
		        .InverseReference()
		        .ForeignKey(t => t.IdProduct)
		        .PrincipalKey(p => p.Id)
		        .Required();

            #endregion


            #region Lookups

            builder.Entity<Lookup>().Key(p => p.Id);
            builder.Entity<Lookup>().ForRelational().Table("Lookups");
            builder.Entity<LookupVariant>().Key(p => new { p.Id, p.IdLookup });
            builder.Entity<LookupVariant>().ForRelational().Table("LookupVariants");
		    builder.Entity<Lookup>()
		        .Collection(p => p.LookupVariants)
		        .InverseReference()
		        .ForeignKey(p => p.IdLookup)
		        .PrincipalKey(p => p.Id);

            #endregion


            #region Settings

            builder.Entity<Country>().Key(p => p.Id);
            builder.Entity<Country>().ForSqlServer().Table("Countries");
            builder.Entity<Country>().Ignore(p => p.States);

            builder.Entity<State>().Key(p => p.Id);
            builder.Entity<State>().ForSqlServer().Table("States");

			#endregion

			#region Users

			builder.Entity<User>().Key(p => p.Id);
			builder.Entity<User>().ForSqlServer().Table("Users");

			#endregion

			#region Customers

			builder.Entity<Customer>().Key(p => p.Id);
			builder.Entity<Customer>().ForSqlServer().Table("Customers");
			builder.Entity<Customer>().Reference(p => p.EditedBy).InverseCollection().ForeignKey(p => p.IdEditedBy);
			builder.Entity<Customer>().Reference(p => p.User).InverseReference().ForeignKey<Customer>(p => p.Id).PrincipalKey<User>(p => p.Id).Required();
			builder.Entity<Customer>()
				.Reference(p => p.CustomerType)
				.InverseCollection(p => p.Customers)
				.ForeignKey(p => p.IdCustomerType)
				.PrincipalKey(p => p.Id);

			builder.Entity<CustomerType>().Key(p => p.Id);
			builder.Entity<CustomerType>().ForSqlServer().Table("CustomerTypes");
			builder.Entity<CustomerType>().Reference(p => p.EditedBy).InverseCollection().ForeignKey(p => p.IdEditedBy);
			builder.Entity<CustomerType>().Collection(p => p.PaymentMethods)
				.InverseReference(p => p.CustomerType)
				.ForeignKey(p => p.IdPaymentMethod)
				.PrincipalKey(p => p.Id);
			builder.Entity<CustomerType>().Collection(p => p.OrderNotes)
				.InverseReference(p => p.CustomerType)
				.ForeignKey(p => p.IdOrderNote)
				.PrincipalKey(p => p.Id);
			//builder.Entity<CustomerType>().Reference(p => p.RecordStatusCode).InverseCollection().ForeignKey(p => p.StatusCode);

			#endregion

			#region Orders
			builder.Entity<OrderNoteToCustomerType>().Key(p => new { p.IdOrderNote, p.IdCustomerType });
			builder.Entity<OrderNoteToCustomerType>().ForSqlServer().Table("OrderNotesToCustomerTypes");
			builder.Entity<OrderNoteToCustomerType>().Ignore(p => p.Id);

			builder.Entity<OrderNote>().Key(p => p.Id);
			builder.Entity<OrderNote>().ForSqlServer().Table("OrderNotes");
			builder.Entity<OrderNote>().Reference(p => p.EditedBy).InverseCollection().ForeignKey(p => p.IdEditedBy);
			//builder.Entity<OrderNote>().Reference(p => p.RecordStatusCode).InverseCollection().ForeignKey(p => p.StatusCode);
			builder.Entity<OrderNote>()
				.Collection(p => p.CustomerTypes)
				.InverseReference(p => p.OrderNote)
				.ForeignKey(p => p.IdOrderNote)
				.PrincipalKey(p => p.Id);

			#endregion

			#region Payment
			builder.Entity<PaymentMethodToCustomerType>().Key(p => new { p.IdPaymentMethod, p.IdCustomerType });
			builder.Entity<PaymentMethodToCustomerType>().ForSqlServer().Table("PaymentMethodsToCustomerTypes");
			builder.Entity<PaymentMethodToCustomerType>().Ignore(p => p.Id);

			builder.Entity<PaymentMethod>().Key(p => p.Id);
			builder.Entity<PaymentMethod>().ForSqlServer().Table("PaymentMethods");
			builder.Entity<PaymentMethod>().Reference(p => p.EditedBy).InverseCollection().ForeignKey(p => p.IdEditedBy);
			//builder.Entity<PaymentMethod>().Reference<RecordStatusCode>().InverseCollection().ForeignKey(p => p.StatusCode);
			builder.Entity<PaymentMethod>()
				.Collection(p => p.CustomerTypes)
				.InverseReference(p => p.PaymentMethod)
				.ForeignKey(p => p.IdPaymentMethod)
				.PrincipalKey(p => p.Id);

			#endregion

			base.OnModelCreating(builder);
		}
	}
}
