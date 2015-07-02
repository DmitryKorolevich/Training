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

        protected override void OnConfiguring(EntityOptionsBuilder builder)
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

            builder.Entity<FieldTypeEntity>().Key(f => f.Id);
            builder.Entity<FieldTypeEntity>().Table("FieldTypes");

		    builder.Entity<BigStringValue>().Key(b => b.IdBigString);
            builder.Entity<BigStringValue>().Ignore(b => b.Id);
		    builder.Entity<BigStringValue>().Table("BigStringValues");

            #endregion


            #region Workflow

            builder.Entity<WorkflowExecutor>().Key(w => w.Id);
            builder.Entity<WorkflowExecutor>().Table("WorkflowExecutors");

            builder.Entity<WorkflowResolverPath>().Key(w => w.Id);
            builder.Entity<WorkflowResolverPath>().Table("WorkflowResolverPaths");
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
            builder.Entity<WorkflowTree>().Table("WorkflowTrees");
		    builder.Entity<WorkflowTree>()
		        .Collection(tree => tree.Actions)
		        .InverseReference()
		        .ForeignKey(action => action.IdTree)
		        .PrincipalKey(tree => tree.Id);

            builder.Entity<WorkflowTreeAction>().Key(w => w.Id);
            builder.Entity<WorkflowTreeAction>().Table("WorkflowTreeActions");
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
            builder.Entity<GiftCertificate>().Table("GiftCertificates");
            builder.Entity<GiftCertificate>().Property(p => p.PublicId).ForSqlServer().UseDefaultValueGeneration();

            #endregion

            #region Discounts

            builder.Entity<DiscountOptionType>().Key(p => p.Id);
            builder.Entity<DiscountOptionType>().Table("DiscountOptionTypes");
            builder.Entity<DiscountOptionType>()
                .Reference(p => p.Lookup)
                .InverseCollection()
                .ForeignKey(p => p.IdLookup)
                .PrincipalKey(p => p.Id)
                .Required(false);

            builder.Entity<DiscountOptionValue>().Key(o => o.Id);
            builder.Entity<DiscountOptionValue>().Table("DiscountOptionValues");
            builder.Entity<DiscountOptionValue>()
                .Reference(v => v.OptionType)
                .InverseCollection()
                .ForeignKey(t => t.IdOptionType)
                .PrincipalKey(v => v.Id);

            builder.Entity<DiscountOptionValue>().Ignore(d => d.BigValue);
            builder.Entity<DiscountOptionValue>().Ignore(d => d.IdBigString);

            builder.Entity<DiscountToCategory>().Key(p => p.Id);
            builder.Entity<DiscountToCategory>().Table("DiscountsToCategories");

            builder.Entity<DiscountToSku>().Key(p => p.Id);
            builder.Entity<DiscountToSku>().Table("DiscountsToSkus");
            builder.Entity<DiscountToSku>().Ignore(p => p.ShortSkuInfo);

            builder.Entity<DiscountToSelectedSku>().Key(p => p.Id);
            builder.Entity<DiscountToSelectedSku>().Table("DiscountsToSelectedSkus");
            builder.Entity<DiscountToSelectedSku>().Ignore(p => p.ShortSkuInfo);

            builder.Entity<DiscountTier>().Key(p => p.Id);
            builder.Entity<DiscountTier>().Table("DiscountTiers");

            builder.Entity<Discount>().Key(p => p.Id);
            builder.Entity<Discount>().Table("Discounts");
            builder.Entity<Discount>()
                .Collection(p => p.OptionValues)
                .InverseReference()
                .ForeignKey(o => o.IdDiscount)
                .PrincipalKey(p => p.Id)
                .Required(false);

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
		    builder.Entity<ProductCategory>().Property(p => p.Id);
            builder.Entity<ProductCategory>().Table("ProductCategories");
		    builder.Entity<ProductCategory>()
		        .Collection(cat => cat.ProductToCategories)
		        .InverseReference()
		        .ForeignKey(c => c.IdCategory)
		        .PrincipalKey(cat => cat.Id);

            builder.Entity<VProductSku>().Key(p => p.IdProduct);
            builder.Entity<VProductSku>().Ignore(x => x.Id);
            builder.Entity<VProductSku>().Table("VProductSkus");

            builder.Entity<VSku>().Key(p => new { p.IdProduct, p.SkuId });
            builder.Entity<VSku>().Ignore(x => x.Id);
            builder.Entity<VSku>().Table("VSkus");

            builder.Entity<ProductOptionType>().Key(p => p.Id);
            builder.Entity<ProductOptionType>().Table("ProductOptionTypes");
		    builder.Entity<ProductOptionType>()
		        .Reference(p => p.Lookup)
		        .InverseCollection()
		        .ForeignKey(p => p.IdLookup)
		        .PrincipalKey(p => p.Id)
                .Required(false);

            builder.Entity<ProductOptionValue>().Key(o => o.Id);
		    builder.Entity<ProductOptionValue>().Property(p => p.Id);
		    builder.Entity<ProductOptionValue>().Table("ProductOptionValues");
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
		    builder.Entity<ProductTypeEntity>().Table("ProductTypes");

            builder.Entity<Sku>().Key(s => s.Id);
		    builder.Entity<Sku>().Property(p => p.Id);
            builder.Entity<Sku>().Table("Skus");
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
            builder.Entity<ProductToCategory>().Table("ProductsToCategories");

            builder.Entity<Product>().Key(p => p.Id);
		    builder.Entity<Product>().Property(p => p.Id);
            builder.Entity<Product>().Table("Products");
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
            builder.Entity<Lookup>().Table("Lookups");
            builder.Entity<LookupVariant>().Key(p => new { p.Id, p.IdLookup });
            builder.Entity<LookupVariant>().Table("LookupVariants");
		    builder.Entity<Lookup>()
		        .Collection(p => p.LookupVariants)
		        .InverseReference()
		        .ForeignKey(p => p.IdLookup)
		        .PrincipalKey(p => p.Id);

            #endregion


            #region Settings

            builder.Entity<Country>().Key(p => p.Id);
            builder.Entity<Country>().Table("Countries");
            builder.Entity<Country>().Ignore(p => p.States);

            builder.Entity<State>().Key(p => p.Id);
            builder.Entity<State>().Table("States");

			#endregion

			#region Users

			builder.Entity<User>().Key(p => p.Id);
			builder.Entity<User>().Table("Users");

            #endregion

            #region Customers

            builder.Entity<Customer>().Key(p => p.Id);
			builder.Entity<Customer>().Table("Customers");
			builder.Entity<Customer>().Reference(p => p.EditedBy).InverseCollection().ForeignKey(p => p.IdEditedBy);
			builder.Entity<Customer>().Reference(p => p.User).InverseReference().ForeignKey<Customer>(p => p.Id).PrincipalKey<User>(p => p.Id).Required();
			builder.Entity<Customer>()
				.Reference(p => p.CustomerType)
				.InverseCollection(p => p.Customers)
				.ForeignKey(p => p.IdCustomerType)
				.PrincipalKey(p => p.Id);
			builder.Entity<Customer>().Collection(p => p.PaymentMethods).InverseReference(p => p.Customer).ForeignKey(p => p.IdPaymentMethod);
			builder.Entity<Customer>().Reference(p => p.DefaultPaymentMethod).InverseReference().ForeignKey<Customer>(p => p.Id).PrincipalKey<PaymentMethod>(p => p.Id).Required();
			builder.Entity<Customer>().Collection(p => p.OrderNotes).InverseReference(p => p.Customer).ForeignKey(p => p.IdCustomer);
			builder.Entity<Customer>().Collection(p => p.Addresses).InverseReference().ForeignKey(p => p.IdCustomer);
			builder.Entity<Customer>().Collection(p => p.CustomerPaymentMethods).InverseReference().ForeignKey(p => p.IdCustomer);
			builder.Entity<Customer>().Collection(p => p.CustomerNotes).InverseReference().ForeignKey(p => p.IdCustomer);
			builder.Entity<CustomerOptionType>().Key(p => p.Id);
			builder.Entity<CustomerOptionType>().Table("CustomerOptionTypes");
			builder.Entity<CustomerOptionType>()
				.Reference(p => p.Lookup)
				.InverseCollection()
				.ForeignKey(p => p.IdLookup)
				.PrincipalKey(p => p.Id)
				.Required(false);
			builder.Entity<CustomerOptionValue>().Key(o => o.Id);
			builder.Entity<CustomerOptionValue>().Table("CustomerOptionValues");
		    builder.Entity<CustomerOptionValue>()
		        .Reference(v => v.OptionType)
		        .InverseCollection()
		        .ForeignKey(t => t.IdOptionType)
		        .PrincipalKey(v => v.Id)
		        .Required();
            builder.Entity<CustomerOptionValue>().Ignore(c => c.BigValue);
            builder.Entity<CustomerOptionValue>().Ignore(c => c.IdBigString);

            builder.Entity<CustomerNote>().Key(p => p.Id);
			builder.Entity<CustomerNote>().Table("CustomerNotes");
			builder.Entity<CustomerNoteOptionType>().Key(p => p.Id);
			builder.Entity<CustomerNoteOptionType>().Table("CustomerNoteOptionTypes");
			builder.Entity<CustomerNoteOptionType>()
				.Reference(p => p.Lookup)
				.InverseCollection()
				.ForeignKey(p => p.IdLookup)
				.PrincipalKey(p => p.Id)
				.Required(false);
			builder.Entity<CustomerNoteOptionValue>().Key(o => o.Id);
			builder.Entity<CustomerNoteOptionValue>().Table("CustomerNoteOptionValues");
			builder.Entity<CustomerNoteOptionValue>()
				.Reference(v => v.OptionType)
				.InverseCollection()
				.ForeignKey(t => t.IdOptionType)
				.PrincipalKey(v => v.Id)
				.Required();

            builder.Entity<CustomerNoteOptionValue>().Ignore(c => c.BigValue);
            builder.Entity<CustomerNoteOptionValue>().Ignore(c => c.IdBigString);

            builder.Entity<CustomerTypeEntity>().Key(p => p.Id);
			builder.Entity<CustomerTypeEntity>().Table("CustomerTypes");
			builder.Entity<CustomerTypeEntity>().Reference(p => p.EditedBy).InverseCollection().ForeignKey(p => p.IdEditedBy);
			builder.Entity<CustomerTypeEntity>().Collection(p => p.PaymentMethods)
				.InverseReference(p => p.CustomerType)
				.ForeignKey(p => p.IdPaymentMethod)
				.PrincipalKey(p => p.Id);
			builder.Entity<CustomerTypeEntity>().Collection(p => p.OrderNotes)
				.InverseReference(p => p.CustomerType)
				.ForeignKey(p => p.IdOrderNote)
				.PrincipalKey(p => p.Id);

			builder.Entity<CustomerToOrderNote>().Key(p => new { p.IdCustomer, p.IdOrderNote });
			builder.Entity<CustomerToOrderNote>().Table("CustomersToOrderNotes");
			builder.Entity<CustomerToOrderNote>().Ignore(p => p.Id);

			builder.Entity<CustomerToPaymentMethod>().Key(p => new { p.IdCustomer, p.IdPaymentMethod });
			builder.Entity<CustomerToPaymentMethod>().Table("CustomersToPaymentMethods");
			builder.Entity<CustomerToPaymentMethod>().Ignore(p => p.Id);

			#endregion

			#region Addresses

			builder.Entity<Address>().Key(p => p.Id);
			builder.Entity<Address>().Table("Addresses");
			builder.Entity<Address>().Reference(p => p.Сountry).InverseCollection().ForeignKey(p => p.IdCountry);
			builder.Entity<Address>().Reference(p => p.State).InverseCollection().ForeignKey(p => p.IdState);
			builder.Entity<AddressOptionType>().Key(p => p.Id);
			builder.Entity<AddressOptionType>().Table("AddressOptionTypes");
			builder.Entity<AddressOptionType>()
				.Reference(p => p.Lookup)
				.InverseCollection()
				.ForeignKey(p => p.IdLookup)
				.PrincipalKey(p => p.Id)
				.Required(false);
			//builder.Entity<AddressOptionType>()
			//	.Reference(p => p.FieldType)
			//	.InverseCollection()
			//	.ForeignKey(p => p.IdFieldType)
			//	.PrincipalKey(p => p.Id)
			//	.Required();
			builder.Entity<AddressOptionValue>().Key(o => o.Id);
			builder.Entity<AddressOptionValue>().Table("AddressOptionValues");
			builder.Entity<AddressOptionValue>()
				.Reference(v => v.OptionType)
				.InverseCollection()
				.ForeignKey(t => t.IdOptionType)
				.PrincipalKey(v => v.Id)
				.Required();

            builder.Entity<AddressOptionValue>().Ignore(c => c.BigValue);
            builder.Entity<AddressOptionValue>().Ignore(c => c.IdBigString);

            builder.Entity<AddressTypeEntity>().Key(p => p.Id);
			builder.Entity<AddressTypeEntity>().Table("AddressTypes");

			#endregion

			#region Orders
			builder.Entity<OrderNoteToCustomerType>().Key(p => new { p.IdOrderNote, p.IdCustomerType });
			builder.Entity<OrderNoteToCustomerType>().Table("OrderNotesToCustomerTypes");
			builder.Entity<OrderNoteToCustomerType>().Ignore(p => p.Id);

			builder.Entity<OrderNote>().Key(p => p.Id);
			builder.Entity<OrderNote>().Table("OrderNotes");
			builder.Entity<OrderNote>().Reference(p => p.EditedBy).InverseCollection().ForeignKey(p => p.IdEditedBy);
			builder.Entity<OrderNote>().Collection(p => p.Customers).InverseReference(p => p.OrderNote).ForeignKey(p => p.IdOrderNote);
			builder.Entity<OrderNote>()
				.Collection(p => p.CustomerTypes)
				.InverseReference(p => p.OrderNote)
				.ForeignKey(p => p.IdOrderNote)
				.PrincipalKey(p => p.Id);

			#endregion

			#region Payment
			builder.Entity<PaymentMethodToCustomerType>().Key(p => new { p.IdPaymentMethod, p.IdCustomerType });
			builder.Entity<PaymentMethodToCustomerType>().Table("PaymentMethodsToCustomerTypes");
			builder.Entity<PaymentMethodToCustomerType>().Ignore(p => p.Id);

			builder.Entity<PaymentMethod>().Key(p => p.Id);
			builder.Entity<PaymentMethod>().Table("PaymentMethods");
			builder.Entity<PaymentMethod>().Reference(p => p.EditedBy).InverseCollection().ForeignKey(p => p.IdEditedBy);
			builder.Entity<PaymentMethod>().Collection(p => p.Customers).InverseReference(p => p.PaymentMethod).ForeignKey(p => p.IdPaymentMethod);
			builder.Entity<PaymentMethod>()
				.Collection(p => p.CustomerTypes)
				.InverseReference(p => p.PaymentMethod)
				.ForeignKey(p => p.IdPaymentMethod)
				.PrincipalKey(p => p.Id);

			builder.Entity<CustomerPaymentMethod>().Key(p => p.Id);
			builder.Entity<CustomerPaymentMethod>().Table("CustomerPaymentMethods");
			builder.Entity<CustomerPaymentMethod>().Reference(p => p.PaymentMethod).InverseCollection().ForeignKey(p => p.IdPaymentMethod);
			builder.Entity<CustomerPaymentMethod>()
				.Reference(p => p.BillingAddress)
				.InverseReference()
				.ForeignKey<CustomerPaymentMethod>(p => p.IdAddress)
				.PrincipalKey<Address>(p => p.Id);
			builder.Entity<CustomerPaymentMethodOptionType>().Key(p => p.Id);
			builder.Entity<CustomerPaymentMethodOptionType>().Table("CustomerPaymentMethodOptionTypes");
			builder.Entity<CustomerPaymentMethodOptionType>()
				.Reference(p => p.Lookup)
				.InverseCollection()
				.ForeignKey(p => p.IdLookup)
				.PrincipalKey(p => p.Id)
				.Required(false);
			//builder.Entity<CustomerPaymentMethodOptionType>()
			//	.Reference(p => p.FieldType)
			//	.InverseCollection()
			//	.ForeignKey(p => p.IdFieldType)
			//	.PrincipalKey(p => p.Id)
			//	.Required();
			builder.Entity<CustomerPaymentMethodOptionValue>().Key(o => o.Id);
			builder.Entity<CustomerPaymentMethodOptionValue>().Table("CustomerPaymentMethodOptionValues");
			builder.Entity<CustomerPaymentMethodOptionValue>()
				.Reference(v => v.OptionType)
				.InverseCollection()
				.ForeignKey(t => t.IdOptionType)
				.PrincipalKey(v => v.Id)
				.Required();

			#endregion

			base.OnModelCreating(builder);
		}
	}
}
