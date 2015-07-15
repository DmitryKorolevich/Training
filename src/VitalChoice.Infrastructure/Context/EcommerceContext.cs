using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using System.Data.SqlClient;
using System.IO;
using VitalChoice.Data.DataContext;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.eCommerce.Addresses;
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
using Microsoft.Framework.OptionsModel;

namespace VitalChoice.Infrastructure.Context
{
	public class EcommerceContext : DataContext
    {
	    private readonly IOptions<AppOptions> _options;

		public EcommerceContext(IOptions<AppOptions> options)
		{
		    _options = options;
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
            builder.UseSqlServerIdentityColumns();

            #region Base

            builder.Entity<FieldTypeEntity>().Key(f => f.Id);
            builder.Entity<FieldTypeEntity>().ToSqlServerTable("FieldTypes");

		    builder.Entity<BigStringValue>().Key(b => b.IdBigString);
            builder.Entity<BigStringValue>().Ignore(b => b.Id);
		    builder.Entity<BigStringValue>().ToSqlServerTable("BigStringValues");

            #endregion


            #region Workflow

            builder.Entity<WorkflowExecutor>().Key(w => w.Id);
            builder.Entity<WorkflowExecutor>().ToSqlServerTable("WorkflowExecutors");

            builder.Entity<WorkflowResolverPath>().Key(w => w.Id);
            builder.Entity<WorkflowResolverPath>().ToSqlServerTable("WorkflowResolverPaths");
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
            builder.Entity<WorkflowTree>().ToSqlServerTable("WorkflowTrees");
		    builder.Entity<WorkflowTree>()
		        .Collection(tree => tree.Actions)
		        .InverseReference()
		        .ForeignKey(action => action.IdTree)
		        .PrincipalKey(tree => tree.Id);

            builder.Entity<WorkflowTreeAction>().Key(w => w.Id);
            builder.Entity<WorkflowTreeAction>().ToSqlServerTable("WorkflowTreeActions");
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
            builder.Entity<GiftCertificate>().ToSqlServerTable("GiftCertificates");
            builder.Entity<GiftCertificate>().Property(p => p.PublicId).ValueGeneratedOnAddOrUpdate();

            #endregion

            #region Discounts

            builder.Entity<DiscountOptionType>().Key(p => p.Id);
            builder.Entity<DiscountOptionType>().ToSqlServerTable("DiscountOptionTypes");
            builder.Entity<DiscountOptionType>()
                .Reference(p => p.Lookup)
                .InverseCollection()
                .ForeignKey(p => p.IdLookup)
                .PrincipalKey(p => p.Id)
                .Required(false);

            builder.Entity<DiscountOptionValue>().Key(o => o.Id);
            builder.Entity<DiscountOptionValue>().ToSqlServerTable("DiscountOptionValues");
		    builder.Entity<DiscountOptionValue>(entity =>
		    {
		        entity.Reference(v => v.OptionType)
		            .InverseCollection()
		            .ForeignKey(t => t.IdOptionType)
		            .PrincipalKey(v => v.Id);
		        entity.Reference(v => v.Discount).InverseCollection(d => d.OptionValues).ForeignKey(v => v.IdDiscount);
		    });
                

            builder.Entity<DiscountOptionValue>().Ignore(d => d.BigValue);
            builder.Entity<DiscountOptionValue>().Ignore(d => d.IdBigString);

            builder.Entity<DiscountToCategory>().Key(p => p.Id);
            builder.Entity<DiscountToCategory>().ToSqlServerTable("DiscountsToCategories");

            builder.Entity<DiscountToSku>().Key(p => p.Id);
            builder.Entity<DiscountToSku>().ToSqlServerTable("DiscountsToSkus");
            builder.Entity<DiscountToSku>().Ignore(p => p.ShortSkuInfo);

            builder.Entity<DiscountToSelectedSku>().Key(p => p.Id);
            builder.Entity<DiscountToSelectedSku>().ToSqlServerTable("DiscountsToSelectedSkus");
            builder.Entity<DiscountToSelectedSku>().Ignore(p => p.ShortSkuInfo);

            builder.Entity<DiscountTier>().Key(p => p.Id);
            builder.Entity<DiscountTier>().ToSqlServerTable("DiscountTiers");

            builder.Entity<Discount>().Key(p => p.Id);
            builder.Entity<Discount>().ToSqlServerTable("Discounts");

            builder.Entity<Discount>().Ignore(p => p.OptionTypes);

            builder.Entity<Discount>()
                .Collection(p => p.DiscountsToCategories)
                .InverseReference()
                .ForeignKey(t => t.IdDiscount)
                .PrincipalKey(p => p.Id)
                .Required();
            builder.Entity<Discount>()
                .Collection(p => p.DiscountsToSkus)
                .InverseReference()
                .ForeignKey(t => t.IdDiscount)
                .PrincipalKey(p => p.Id)
                .Required();
            builder.Entity<Discount>()
                .Collection(p => p.DiscountsToSelectedSkus)
                .InverseReference()
                .ForeignKey(t => t.IdDiscount)
                .PrincipalKey(p => p.Id)
                .Required();
            builder.Entity<Discount>()
                .Collection(p => p.DiscountTiers)
                .InverseReference()
                .ForeignKey(t => t.IdDiscount)
                .PrincipalKey(p => p.Id)
                .Required();
            builder.Entity<Discount>()
                .Reference(p => p.EditedBy)
                .InverseCollection()
                .ForeignKey(o => o.IdEditedBy)
                .PrincipalKey(p => p.Id)
                .Required(false);

            #endregion

            #region Products

            builder.Entity<ProductCategory>().Key(p => p.Id);
            builder.Entity<ProductCategory>().ToSqlServerTable("ProductCategories");
		    builder.Entity<ProductCategory>()
		        .Collection(cat => cat.ProductToCategories)
		        .InverseReference()
		        .ForeignKey(c => c.IdCategory)
		        .PrincipalKey(cat => cat.Id);

            builder.Entity<VProductSku>().Key(p => p.IdProduct);
            builder.Entity<VProductSku>().Ignore(x => x.Id);
            builder.Entity<VProductSku>().ToSqlServerTable("VProductSkus");

            builder.Entity<VSku>().Key(p => new { p.IdProduct, p.SkuId });
            builder.Entity<VSku>().Ignore(x => x.Id);
            builder.Entity<VSku>().ToSqlServerTable("VSkus");

            builder.Entity<ProductOptionType>().Key(p => p.Id);
            builder.Entity<ProductOptionType>().ToSqlServerTable("ProductOptionTypes");
            builder.Entity<ProductOptionType>()
		        .Reference(p => p.Lookup)
		        .InverseCollection()
		        .ForeignKey(p => p.IdLookup)
		        .PrincipalKey(p => p.Id)
                .Required(false);

            builder.Entity<ProductOptionValue>().Key(o => o.Id);
		    builder.Entity<ProductOptionValue>().ToSqlServerTable("ProductOptionValues");
		    builder.Entity<ProductOptionValue>()
		        .Reference(v => v.OptionType)
		        .InverseCollection()
		        .ForeignKey(v => v.IdOptionType)
		        .PrincipalKey(t => t.Id)
                .Required();
		    builder.Entity<ProductOptionValue>()
		        .Reference(v => v.BigValue)
		        .InverseCollection()
		        .ForeignKey(v => v.IdBigString)
		        .PrincipalKey(b => b.IdBigString)
		        .Required(false);
            builder.Entity<ProductOptionValue>().Property(v => v.IdBigString).Required(false);

            builder.Entity<ProductTypeEntity>().Key(t => t.Id);
		    builder.Entity<ProductTypeEntity>().ToSqlServerTable("ProductTypes");

            builder.Entity<Sku>().Key(s => s.Id);
            builder.Entity<Sku>().Ignore(p => p.IdObjectType);
            builder.Entity<Sku>().ToSqlServerTable("Skus");
		    builder.Entity<Sku>()
		        .Collection(s => s.OptionValues)
		        .InverseReference()
		        .ForeignKey(o => o.IdSku)
		        .PrincipalKey(s => s.Id)
                .Required(false);
            builder.Entity<Sku>().Ignore(p => p.OptionTypes);
            builder.Entity<Sku>().Ignore(p => p.EditedBy);
            builder.Entity<Sku>().Ignore(p => p.IdEditedBy);

            builder.Entity<ProductToCategory>().Key(p => p.Id);
            builder.Entity<ProductToCategory>().ToSqlServerTable("ProductsToCategories");

            builder.Entity<Product>().Key(p => p.Id);
            builder.Entity<Product>().ToSqlServerTable("Products");

            builder.Entity<Product>()
                .Collection(p => p.Skus)
                .InverseReference(p => p.Product)
                .ForeignKey(s => s.IdProduct)
                .PrincipalKey(p => p.Id)
                .Required();
            builder.Entity<Product>()
                .Collection(p => p.OptionValues)
                .InverseReference()
                .ForeignKey(o => o.IdProduct)
                .PrincipalKey(p => p.Id)
                .Required(false);
            builder.Entity<Product>()
                .Reference(p => p.EditedBy)
                .InverseCollection()
                .ForeignKey(o => o.IdEditedBy)
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
            builder.Entity<Lookup>().ToSqlServerTable("Lookups");
            builder.Entity<LookupVariant>().Key(p => new { p.Id, p.IdLookup });
            builder.Entity<LookupVariant>().ToSqlServerTable("LookupVariants");
		    builder.Entity<Lookup>()
		        .Collection(p => p.LookupVariants)
		        .InverseReference(p => p.Lookup)
		        .ForeignKey(p => p.IdLookup)
		        .PrincipalKey(p => p.Id);

            #endregion


            #region Settings

            builder.Entity<Country>().Key(p => p.Id);
            builder.Entity<Country>().ToSqlServerTable("Countries");
            builder.Entity<Country>().Ignore(p => p.States);

            builder.Entity<State>().Key(p => p.Id);
            builder.Entity<State>().ToSqlServerTable("States");

			#endregion

			#region Users

			builder.Entity<User>().Key(p => p.Id);
			builder.Entity<User>().ToSqlServerTable("Users");

			#endregion

			#region Customers

			builder.Entity<VCustomer>().Key(x => x.Id);
			builder.Entity<VCustomer>().ToSqlServerTable("VCustomers");

			builder.Entity<Customer>().Key(p => p.Id);
			builder.Entity<Customer>().ToSqlServerTable("Customers");
		    builder.Entity<Customer>()
		        .Reference(p => p.EditedBy)
		        .InverseCollection()
		        .ForeignKey(p => p.IdEditedBy)
		        .PrincipalKey(p => p.Id)
		        .Required(false);
            builder.Entity<Customer>()
                .Reference(p => p.User)
                .InverseReference(u => u.Customer)
                .ForeignKey<Customer>(p => p.Id)
                .PrincipalKey<User>(p => p.Id)
                .Required();
            builder.Entity<Customer>()
				.Reference(p => p.CustomerType)
				.InverseCollection(p => p.Customers)
				.ForeignKey(p => p.IdObjectType)
				.PrincipalKey(p => p.Id)
                .Required();
		    builder.Entity<Customer>()
		        .Reference(p => p.DefaultPaymentMethod)
		        .InverseReference()
		        .ForeignKey<Customer>(p => p.IdDefaultPaymentMethod)
		        .PrincipalKey<PaymentMethod>(p => p.Id)
		        .Required();

            builder.Entity<Customer>().Ignore(p => p.OptionTypes);

            builder.Entity<CustomerOptionType>().Key(p => p.Id);
			builder.Entity<CustomerOptionType>().ToSqlServerTable("CustomerOptionTypes");
            builder.Entity<CustomerOptionType>()
				.Reference(p => p.Lookup)
				.InverseCollection()
				.ForeignKey(p => p.IdLookup)
				.PrincipalKey(p => p.Id)
				.Required(false);
			builder.Entity<CustomerOptionValue>().Key(o => o.Id);
			builder.Entity<CustomerOptionValue>().ToSqlServerTable("CustomerOptionValues");
		    builder.Entity<CustomerOptionValue>(entity =>
		    {
                entity.Reference(v => v.OptionType)
		            .InverseCollection()
		            .ForeignKey(t => t.IdOptionType)
		            .PrincipalKey(v => v.Id)
		            .Required();
		        entity.Reference(v => v.Customer)
		            .InverseCollection(c => c.OptionValues)
		            .ForeignKey(c => c.IdCustomer)
		            .Required(false);
		    });
		        

            builder.Entity<CustomerNote>().Key(p => p.Id);
			builder.Entity<CustomerNote>().ToSqlServerTable("CustomerNotes");
            builder.Entity<CustomerNote>().Ignore(p => p.IdObjectType);
		    builder.Entity<CustomerNote>()
		        .Reference(p => p.EditedBy)
		        .InverseCollection()
		        .ForeignKey(p => p.IdEditedBy)
		        .PrincipalKey(p => p.Id)
                .Required(false);
		    builder.Entity<CustomerNote>()
		        .Reference(n => n.Customer)
		        .InverseCollection(c => c.CustomerNotes)
		        .ForeignKey(n => n.IdCustomer);

		    builder.Entity<CustomerNote>().Ignore(n => n.OptionTypes);

            builder.Entity<CustomerNoteOptionType>().Key(p => p.Id);
			builder.Entity<CustomerNoteOptionType>().ToSqlServerTable("CustomerNoteOptionTypes");
            builder.Entity<CustomerNoteOptionType>().Ignore(p => p.IdObjectType);
            builder.Entity<CustomerNoteOptionType>()
				.Reference(p => p.Lookup)
				.InverseCollection()
				.ForeignKey(p => p.IdLookup)
				.PrincipalKey(p => p.Id)
				.Required(false);
			builder.Entity<CustomerNoteOptionValue>().Key(o => o.Id);
			builder.Entity<CustomerNoteOptionValue>().ToSqlServerTable("CustomerNoteOptionValues");
		    builder.Entity<CustomerNoteOptionValue>(entity =>
		    {
		        entity.Property(v => v.IdCustomerNote).Required(false);
		        entity.Reference(v => v.OptionType)
		            .InverseCollection()
		            .ForeignKey(t => t.IdOptionType)
		            .PrincipalKey(v => v.Id)
		            .Required();
		        entity.Reference(v => v.CustomerNote)
		            .InverseCollection(n => n.OptionValues)
		            .ForeignKey(v => v.IdCustomerNote);
		    });

            builder.Entity<CustomerNoteOptionValue>().Ignore(c => c.BigValue);
            builder.Entity<CustomerNoteOptionValue>().Ignore(c => c.IdBigString);

            builder.Entity<CustomerTypeEntity>().Key(p => p.Id);
			builder.Entity<CustomerTypeEntity>().ToSqlServerTable("CustomerTypes");
		    builder.Entity<CustomerTypeEntity>()
		        .Reference(p => p.EditedBy)
		        .InverseCollection()
		        .ForeignKey(p => p.IdEditedBy)
		        .PrincipalKey(p => p.Id)
		        .Required(false);
		    builder.Entity<CustomerTypeEntity>().Collection(p => p.PaymentMethods)
		        .InverseReference(p => p.CustomerType)
		        .ForeignKey(p => p.IdCustomerType)
		        .PrincipalKey(p => p.Id);
			builder.Entity<CustomerTypeEntity>().Collection(p => p.OrderNotes)
				.InverseReference(p => p.CustomerType)
				.ForeignKey(p => p.IdCustomerType)
				.PrincipalKey(p => p.Id);

			builder.Entity<CustomerToOrderNote>().Ignore(p => p.Id);
			builder.Entity<CustomerToOrderNote>().Key(p => new { p.IdCustomer, p.IdOrderNote });
			builder.Entity<CustomerToOrderNote>().ToSqlServerTable("CustomersToOrderNotes");
		    builder.Entity<CustomerToOrderNote>()
		        .Reference(n => n.Customer)
		        .InverseCollection(c => c.OrderNotes)
		        .ForeignKey(n => n.IdCustomer);
		    builder.Entity<CustomerToOrderNote>()
		        .Reference(n => n.OrderNote)
		        .InverseCollection(n => n.Customers)
		        .ForeignKey(n => n.IdOrderNote);

            builder.Entity<CustomerToPaymentMethod>().Ignore(p => p.Id);
			builder.Entity<CustomerToPaymentMethod>().Key(p => new { p.IdCustomer, p.IdPaymentMethod });
			builder.Entity<CustomerToPaymentMethod>().ToSqlServerTable("CustomersToPaymentMethods");
		    builder.Entity<CustomerToPaymentMethod>()
		        .Reference(p => p.Customer)
		        .InverseCollection(c => c.PaymentMethods)
		        .ForeignKey(p => p.IdCustomer);
		    builder.Entity<CustomerToPaymentMethod>()
		        .Reference(p => p.PaymentMethod)
		        .InverseCollection(p => p.Customers)
		        .ForeignKey(p => p.IdPaymentMethod);

            #endregion

            #region Addresses

            builder.Entity<Address>().Key(p => p.Id);
			builder.Entity<Address>().ToSqlServerTable("Addresses");
		    builder.Entity<Address>()
		        .Reference(p => p.Сountry)
		        .InverseCollection()
		        .ForeignKey(p => p.IdCountry)
		        .PrincipalKey(c => c.Id)
		        .Required();
		    builder.Entity<Address>()
		        .Reference(p => p.State)
		        .InverseCollection()
		        .ForeignKey(p => p.IdState)
		        .PrincipalKey(s => s.Id)
		        .Required(false);
		    builder.Entity<Address>()
		        .Reference(p => p.EditedBy)
		        .InverseCollection()
		        .ForeignKey(p => p.IdEditedBy)
		        .PrincipalKey(p => p.Id)
		        .Required(false);
		    builder.Entity<Address>()
		        .Reference(a => a.Customer)
		        .InverseCollection(c => c.Addresses)
		        .ForeignKey(a => a.IdCustomer);

		    builder.Entity<Address>().Ignore(a => a.OptionTypes);

            builder.Entity<AddressOptionType>().Key(p => p.Id);
			builder.Entity<AddressOptionType>().ToSqlServerTable("AddressOptionTypes");
            builder.Entity<AddressOptionType>()
				.Reference(p => p.Lookup)
				.InverseCollection()
				.ForeignKey(p => p.IdLookup)
				.PrincipalKey(p => p.Id)
				.Required(false);
			builder.Entity<AddressOptionValue>().Key(o => o.Id);
			builder.Entity<AddressOptionValue>().ToSqlServerTable("AddressOptionValues");
			builder.Entity<AddressOptionValue>()
				.Reference(v => v.OptionType)
				.InverseCollection()
				.ForeignKey(t => t.IdOptionType)
				.PrincipalKey(v => v.Id)
				.Required();
		    builder.Entity<AddressOptionValue>().Property(v => v.IdAddress).Required(false);
            builder.Entity<AddressOptionValue>()
		        .Reference(v => v.Address)
		        .InverseCollection(a => a.OptionValues)
		        .ForeignKey(v => v.IdAddress);

            builder.Entity<AddressOptionValue>().Ignore(c => c.BigValue);
            builder.Entity<AddressOptionValue>().Ignore(c => c.IdBigString);

            builder.Entity<AddressTypeEntity>().Key(p => p.Id);
			builder.Entity<AddressTypeEntity>().ToSqlServerTable("AddressTypes");

			#endregion

			#region Orders
			builder.Entity<OrderNoteToCustomerType>().Key(p => new { p.IdOrderNote, p.IdCustomerType });
			builder.Entity<OrderNoteToCustomerType>().ToSqlServerTable("OrderNotesToCustomerTypes");
			builder.Entity<OrderNoteToCustomerType>().Ignore(p => p.Id);
		    builder.Entity<OrderNoteToCustomerType>()
		        .Reference(n => n.CustomerType)
		        .InverseCollection(t => t.OrderNotes)
		        .ForeignKey(n => n.IdCustomerType);
		    builder.Entity<OrderNoteToCustomerType>()
		        .Reference(n => n.OrderNote)
		        .InverseCollection(n => n.CustomerTypes)
		        .ForeignKey(n => n.IdOrderNote);

            builder.Entity<OrderNote>().Key(p => p.Id);
			builder.Entity<OrderNote>().ToSqlServerTable("OrderNotes");
			builder.Entity<OrderNote>()
				.Reference(p => p.EditedBy)
				.InverseCollection()
				.ForeignKey(p => p.IdEditedBy)
				.PrincipalKey(p => p.Id);

            #endregion

            #region Payment
            builder.Entity<PaymentMethodToCustomerType>().Key(p => new { p.IdPaymentMethod, p.IdCustomerType });
			builder.Entity<PaymentMethodToCustomerType>().ToSqlServerTable("PaymentMethodsToCustomerTypes");
			builder.Entity<PaymentMethodToCustomerType>().Ignore(p => p.Id);
		    builder.Entity<PaymentMethodToCustomerType>()
		        .Reference(p => p.CustomerType)
		        .InverseCollection(t => t.PaymentMethods)
		        .ForeignKey(p => p.IdCustomerType);
		    builder.Entity<PaymentMethodToCustomerType>()
		        .Reference(p => p.PaymentMethod)
		        .InverseCollection(t => t.CustomerTypes)
		        .ForeignKey(p => p.IdPaymentMethod);

            builder.Entity<PaymentMethod>().Key(p => p.Id);
			builder.Entity<PaymentMethod>().ToSqlServerTable("PaymentMethods");
		    builder.Entity<PaymentMethod>()
		        .Reference(p => p.EditedBy)
		        .InverseCollection()
		        .ForeignKey(p => p.IdEditedBy)
		        .PrincipalKey(p => p.Id)
		        .Required(false);

			builder.Entity<CustomerPaymentMethod>().Key(p => p.Id);
			builder.Entity<CustomerPaymentMethod>().ToSqlServerTable("CustomerPaymentMethods");
			builder.Entity<CustomerPaymentMethod>()
				.Reference(p => p.EditedBy)
				.InverseCollection()
				.ForeignKey(p => p.IdEditedBy)
				.PrincipalKey(p => p.Id);
            builder.Entity<CustomerPaymentMethod>()
		        .Reference(p => p.PaymentMethod)
		        .InverseCollection()
		        .ForeignKey(p => p.IdObjectType)
		        .PrincipalKey(p => p.Id)
		        .Required();
		    builder.Entity<CustomerPaymentMethod>()
		        .Reference(p => p.BillingAddress)
		        .InverseReference()
		        .ForeignKey<CustomerPaymentMethod>(p => p.IdAddress)
		        .PrincipalKey<Address>(p => p.Id);
		    builder.Entity<CustomerPaymentMethod>()
		        .Reference(p => p.Customer)
		        .InverseCollection(c => c.CustomerPaymentMethods)
		        .ForeignKey(p => p.IdCustomer);

            builder.Entity<CustomerPaymentMethod>().Ignore(a => a.OptionTypes);

            builder.Entity<CustomerPaymentMethodOptionType>().Key(p => p.Id);
			builder.Entity<CustomerPaymentMethodOptionType>().ToSqlServerTable("CustomerPaymentMethodOptionTypes");
            builder.Entity<CustomerPaymentMethodOptionType>()
				.Reference(p => p.Lookup)
				.InverseCollection()
				.ForeignKey(p => p.IdLookup)
				.PrincipalKey(p => p.Id)
				.Required(false);
			builder.Entity<CustomerPaymentMethodOptionValue>().Key(o => o.Id);
			builder.Entity<CustomerPaymentMethodOptionValue>().ToSqlServerTable("CustomerPaymentMethodOptionValues");
			builder.Entity<CustomerPaymentMethodOptionValue>()
				.Reference(v => v.OptionType)
				.InverseCollection()
				.ForeignKey(t => t.IdOptionType)
				.PrincipalKey(v => v.Id)
				.Required();
		    builder.Entity<CustomerPaymentMethodOptionValue>()
		        .Reference(v => v.CustomerPaymentMethod)
		        .InverseCollection(p => p.OptionValues)
		        .ForeignKey(v => v.IdCustomerPaymentMethod);

            #endregion

            base.OnModelCreating(builder);
		}
	}
}
