using System;
using System.Data.SqlClient;
using VitalChoice.Caching.Extensions;
using VitalChoice.Data.Context;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Affiliates;
using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.Ecommerce.Domain.Entities.Checkout;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Discounts;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Entities.History;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Entities.Promotions;
using VitalChoice.Ecommerce.Domain.Entities.Users;
using VitalChoice.Ecommerce.Domain.Entities.Workflow;
using VitalChoice.Ecommerce.Domain.Options;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using VitalChoice.Ecommerce.Domain.Entities.Healthwise;
using VitalChoice.Ecommerce.Domain.Entities.InventorySkus;

namespace VitalChoice.Ecommerce.Context
{
    public class EcommerceContextBase : DataContext
    {
        public EcommerceContextBase(IOptions<AppOptionsBase> options, DbContextOptions contextOptions) : base(contextOptions)
        {
            Options = options;
        }

        protected readonly IOptions<AppOptionsBase> Options;

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            //((IDbContextOptionsBuilderInfrastructure)builder).AddOrUpdateExtension(new DbContextCacheExtension());
            
            var connectionString = (new SqlConnectionStringBuilder
            {
                DataSource = Options.Value.Connection.Server,
                // TODO: Currently nested queries are run while processing the results of outer queries
                // This either requires MARS or creation of a new connection for each query. Currently using
                // MARS since cloning connections is known to be problematic.
                MultipleActiveResultSets = true,
                InitialCatalog = "VitalChoice.Ecommerce",
                UserID = Options.Value.Connection.UserName,
                Password = Options.Value.Connection.Password,
                ConnectTimeout = 60
            }).ConnectionString;
            builder.UseSqlServer(connectionString);

            base.OnConfiguring(builder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ForSqlServerUseIdentityColumns();

            #region Base

            AppOptions(builder);
            FieldTypes(builder);
            BigStringValues(builder);

            #endregion

            #region Workflow

            WorkflowExecutors(builder);
            WorkflowResolverPaths(builder);
            WorkflowTrees(builder);
            WorkflowTreeActions(builder);
            WorkflowActionDependencies(builder);
            WorkflowActionAggregations(builder);

            #endregion

            #region GiftCertificates

            GiftCertificates(builder);

            #endregion

            #region Discounts

            DiscountOptionTypes(builder);
            DiscountOptionValues(builder);
            DiscountsToCategories(builder);
            DiscountsToSkus(builder);
            DiscountsToSelectedSkus(builder);
            DiscountToSelectedCategories(builder);
            DiscountTiers(builder);
            Discounts(builder);

            #endregion

            #region Promotions

            builder.Entity<PromotionTypeEntity>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("PromotionTypes");
            });

            builder.Entity<PromotionOptionType>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("PromotionOptionTypes");
                entity
                    .HasOne(p => p.Lookup)
                    .WithMany()
                    .HasForeignKey(p => p.IdLookup)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired(false);
                entity.HasIndex(e => new { e.Name, e.IdObjectType }).IsUnique(true);
            });

            builder.Entity<PromotionOptionValue>(entity =>
            {
                entity.Ignore(o => o.Id);
                entity.HasKey(d => new { d.IdPromotion, d.IdOptionType });
                entity.ToTable("PromotionOptionValues");
                entity
                    .HasOne(v => v.OptionType)
                    .WithMany()
                    .HasForeignKey(t => t.IdOptionType)
                    .HasPrincipalKey(v => v.Id);

                entity.Ignore(d => d.BigValue);
                entity.Ignore(d => d.IdBigString);
            });

            builder.Entity<PromotionToBuySku>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("PromotionsToBuySkus");
                entity.Ignore(p => p.ShortSkuInfo);
            });

            builder.Entity<PromotionToGetSku>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("PromotionsToGetSkus");
                entity.Ignore(p => p.ShortSkuInfo);
            });

            builder.Entity<PromotionToSelectedCategory>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("PromotionsToSelectedCategories");
            });

            builder.Entity<Promotion>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("Promotions");
                entity
                    .HasMany(p => p.OptionValues)
                    .WithOne()
                    .HasForeignKey(o => o.IdPromotion)
                    .HasPrincipalKey(p => p.Id);

                entity.Ignore(p => p.OptionTypes);

                entity
                    .HasMany(p => p.PromotionsToBuySkus)
                    .WithOne()
                    .HasForeignKey(t => t.IdPromotion)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired();
                entity
                    .HasMany(p => p.PromotionsToGetSkus)
                    .WithOne()
                    .HasForeignKey(t => t.IdPromotion)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired();
                entity
                    .HasMany(p => p.PromotionsToSelectedCategories)
                    .WithOne()
                    .HasForeignKey(t => t.IdPromotion)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired();
                entity
                    .HasOne(p => p.EditedBy)
                    .WithMany()
                    .HasForeignKey(o => o.IdEditedBy)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired(false);
            });

            #endregion

            #region Products

            builder.Entity<ProductCategory>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("ProductCategories");
                entity
                    .HasMany(cat => cat.ProductToCategories)
                    .WithOne()
                    .HasForeignKey(c => c.IdCategory)
                    .HasPrincipalKey(cat => cat.Id);
            });

            builder.Entity<InventoryCategory>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("InventoryCategories");
            });

            builder.Entity<ProductOptionType>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("ProductOptionTypes");
                entity
                    .HasOne(p => p.Lookup)
                    .WithMany()
                    .HasForeignKey(p => p.IdLookup)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired(false);
                entity.HasIndex(e => new { e.Name, e.IdObjectType }).IsUnique(true);
            });


            builder.Entity<ProductOptionValue>(entity =>
            {
                entity.Ignore(o => o.Id);
                entity.HasKey(d => new { d.IdProduct, d.IdOptionType });
                entity.ToTable("ProductOptionValues");
                entity
                    .HasOne(v => v.OptionType)
                    .WithMany()
                    .HasForeignKey(v => v.IdOptionType)
                    .HasPrincipalKey(t => t.Id)
                    .IsRequired();
                entity
                    .HasOne(v => v.BigValue)
                    .WithMany()
                    .HasForeignKey(v => v.IdBigString)
                    .HasPrincipalKey(b => b.IdBigString)
                    .IsRequired(false);
            });


            builder.Entity<ProductTypeEntity>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.ToTable("ProductTypes");
            });

            SkuToInventorySku(builder);

            builder.Entity<Sku>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Ignore(p => p.IdObjectType);
                entity.ToTable("Skus");
                entity
                    .HasMany(s => s.OptionValues)
                    .WithOne()
                    .HasForeignKey(o => o.IdSku)
                    .HasPrincipalKey(s => s.Id);
                entity
                    .HasMany(s => s.SkusToInventorySkus)
                    .WithOne()
                    .HasForeignKey(o => o.IdSku)
                    .HasPrincipalKey(s => s.Id);
                entity.Ignore(p => p.OptionTypes);
                entity.Ignore(p => p.EditedBy);
                entity.Ignore(p => p.IdEditedBy);
            });


            builder.Entity<SkuOptionValue>(entity =>
            {
                entity.Ignore(o => o.Id);
                entity.HasKey(d => new { d.IdSku, d.IdOptionType });
                entity.ToTable("SkuOptionValues");
                entity
                    .HasOne(v => v.OptionType)
                    .WithMany()
                    .HasForeignKey(v => v.IdOptionType)
                    .HasPrincipalKey(t => t.Id)
                    .IsRequired();
                entity.Ignore(d => d.BigValue);
                entity.Ignore(d => d.IdBigString);
            });

            builder.Entity<ProductToCategory>(entity =>
            {
                entity.Ignore(p => p.Id);
                entity.HasKey(p => new { p.IdProduct, p.IdCategory });
                entity.ToTable("ProductsToCategories");
            });

            builder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("Products");
                entity
                    .HasMany(p => p.Skus)
                    .WithOne(p => p.Product)
                    .HasForeignKey(s => s.IdProduct)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired();
                entity
                    .HasMany(p => p.OptionValues)
                    .WithOne()
                    .HasForeignKey(o => o.IdProduct)
                    .HasPrincipalKey(p => p.Id);
                entity
                    .HasOne(p => p.EditedBy)
                    .WithMany()
                    .HasForeignKey(o => o.IdEditedBy)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired(false);
                entity.Ignore(p => p.OptionTypes);
                entity
                    .HasMany(p => p.ProductsToCategories)
                    .WithOne(p=>p.Product)
                    .HasForeignKey(t => t.IdProduct)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired();
            });


            builder.Entity<ProductReview>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("ProductReviews");
                entity
                    .HasOne(p => p.Product)
                    .WithMany()
                    .HasForeignKey(s => s.IdProduct)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired();
            });


            builder.Entity<ProductOutOfStockRequest>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("ProductOutOfStockRequests");
            });


            #endregion

            #region InventorySkus
            
            InventorySkuCategories(builder);
            InventorySkuOptionTypes(builder);
            InventorySkuOptionValues(builder);
            InventorySkus(builder);

            #endregion

            #region Lookups

            builder.Entity<Lookup>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("Lookups");
            });

            builder.Entity<LookupVariant>(entity =>
            {
                entity.HasKey(p => new { p.Id, p.IdLookup });
                entity.ToTable("LookupVariants");
            });

            builder.Entity<Lookup>(entity =>
            {
                entity
                    .HasMany(p => p.LookupVariants)
                    .WithOne(p => p.Lookup)
                    .HasForeignKey(p => p.IdLookup)
                    .HasPrincipalKey(p => p.Id);
            });


            #endregion

            #region Settings

            builder.Entity<Country>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("Countries");
                entity.Ignore(p => p.States);
                entity.CacheWhen(c => c.StatusCode != RecordStatusCode.Deleted);
                entity.HasIndex(c => c.CountryCode).IsUnique();
            });


            builder.Entity<State>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("States");
                entity.CacheWhen(c => c.StatusCode != RecordStatusCode.Deleted);
                entity.HasIndex(c => new {c.StateCode, c.CountryCode}).IsUnique();
            });


            #endregion

            #region Users

            builder.Entity<User>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("Users");
            });


            #endregion

            #region Customers

            builder.Entity<Customer>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("Customers");
                entity
                    .HasOne(p => p.EditedBy)
                    .WithMany()
                    .HasForeignKey(p => p.IdEditedBy)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired(false);
                entity
                    .HasOne(p => p.User)
                    .WithOne()
                    .HasForeignKey<Customer>(p => p.Id)
                    .HasPrincipalKey<User>(p => p.Id)
                    .IsRequired();
                entity
                    .HasMany(p => p.OptionValues)
                    .WithOne()
                    .HasForeignKey(o => o.IdCustomer)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired();
                entity
                    .HasOne(p => p.CustomerType)
                    .WithMany(p => p.Customers)
                    .HasForeignKey(p => p.IdObjectType)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired();
                entity
                    .HasMany(p => p.PaymentMethods)
                    .WithOne(p => p.Customer)
                    .HasForeignKey(p => p.IdCustomer)
                    .HasPrincipalKey(c => c.Id)
                    .IsRequired();
                entity
                    .HasOne(p => p.DefaultPaymentMethod)
                    .WithOne()
                    .HasForeignKey<Customer>(p => p.IdDefaultPaymentMethod)
                    .HasPrincipalKey<PaymentMethod>(p => p.Id)
                    .IsRequired();
                entity
                    .HasMany(p => p.OrderNotes)
                    .WithOne(p => p.Customer)
                    .HasForeignKey(p => p.IdCustomer)
                    .HasPrincipalKey(c => c.Id)
                    .IsRequired();
                entity
                    .HasOne(p => p.ProfileAddress)
                    .WithOne()
                    .HasForeignKey<Customer>(p => p.IdProfileAddress)
                    .HasPrincipalKey<Address>(c => c.Id)
                    .IsRequired();
                entity
                    .HasMany(p => p.ShippingAddresses)
                    .WithOne()
                    .HasForeignKey(p => p.IdCustomer)
                    .HasPrincipalKey(c => c.Id)
                    .IsRequired();
                entity
                    .HasMany(p => p.CustomerPaymentMethods)
                    .WithOne()
                    .HasForeignKey(p => p.IdCustomer)
                    .HasPrincipalKey(c => c.Id)
                    .IsRequired();
                entity
                    .HasMany(p => p.CustomerNotes)
                    .WithOne()
                    .HasForeignKey(p => p.IdCustomer)
                    .HasPrincipalKey(c => c.Id)
                    .IsRequired();
                entity
                    .HasMany(p => p.Files)
                    .WithOne()
                    .HasForeignKey(p => p.IdCustomer)
                    .HasPrincipalKey(c => c.Id)
                    .IsRequired();
                entity.Ignore(p => p.OptionTypes);
            });


            builder.Entity<CustomerToShippingAddress>(entity =>
            {
                entity.ToTable("CustomerToShippingAddresses");
                entity.Ignore(c => c.Id);
                entity.HasKey(c => new { c.IdCustomer, c.IdAddress });
                entity.HasOne(c => c.Customer)
                    .WithMany(c => c.ShippingAddresses)
                    .HasForeignKey(c => c.IdCustomer)
                    .HasPrincipalKey(c => c.Id);
                entity.HasOne(c => c.ShippingAddress)
                    .WithOne()
                    .HasForeignKey<CustomerToShippingAddress>(c => c.IdAddress)
                    .HasPrincipalKey<Address>(c => c.Id);
            });

            builder.Entity<CustomerOptionType>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("CustomerOptionTypes");
                entity
                    .HasOne(p => p.Lookup)
                    .WithMany()
                    .HasForeignKey(p => p.IdLookup)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired(false);
                entity.HasIndex(e => new { e.Name, e.IdObjectType }).IsUnique(true);
            });


            builder.Entity<CustomerOptionValue>(entity =>
            {
                entity.HasKey(o => new { o.IdCustomer, o.IdOptionType });
                entity.Ignore(c => c.Id);
                entity.ToTable("CustomerOptionValues");
                entity
                    .HasOne(v => v.OptionType)
                    .WithMany()
                    .HasForeignKey(t => t.IdOptionType)
                    .HasPrincipalKey(v => v.Id)
                    .IsRequired();
                entity
                    .HasOne(c => c.BigValue)
                    .WithOne()
                    .HasForeignKey<CustomerOptionValue>(c => c.IdBigString)
                    .HasPrincipalKey<BigStringValue>(v => v.IdBigString);
            });


            builder.Entity<CustomerFile>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("CustomerFiles");
            });


            builder.Entity<CustomerNote>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("CustomerNotes");
                entity.Ignore(p => p.IdObjectType);
                entity
                    .HasOne(p => p.EditedBy)
                    .WithMany()
                    .HasForeignKey(p => p.IdEditedBy)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired(false);
                entity
                    .HasOne(p => p.AddedBy)
                    .WithMany()
                    .HasForeignKey(p => p.IdAddedBy)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired(false);
                entity
                    .HasMany(n => n.OptionValues)
                    .WithOne()
                    .HasForeignKey(o => o.IdCustomerNote)
                    .HasPrincipalKey(n => n.Id)
                    .IsRequired();
                entity.Ignore(n => n.OptionTypes);
            });


            builder.Entity<CustomerNoteOptionType>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("CustomerNoteOptionTypes");
                entity.Ignore(p => p.IdObjectType);
                entity
                    .HasOne(p => p.Lookup)
                    .WithMany()
                    .HasForeignKey(p => p.IdLookup)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired(false);
                entity.HasIndex(e => e.Name).IsUnique(true);
            });

            builder.Entity<CustomerNoteOptionValue>(entity =>
            {
                entity.HasKey(o => new { o.IdCustomerNote, o.IdOptionType });
                entity.Ignore(o => o.Id);
                entity.ToTable("CustomerNoteOptionValues");
                entity
                    .HasOne(v => v.OptionType)
                    .WithMany()
                    .HasForeignKey(t => t.IdOptionType)
                    .HasPrincipalKey(v => v.Id)
                    .IsRequired();
                entity.Ignore(c => c.BigValue);
                entity.Ignore(c => c.IdBigString);
            });


            builder.Entity<CustomerTypeEntity>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("CustomerTypes");
                entity
                    .HasOne(p => p.EditedBy)
                    .WithMany()
                    .HasForeignKey(p => p.IdEditedBy)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired(false);
                entity.HasMany(p => p.PaymentMethods)
                    .WithOne(p => p.CustomerType)
                    .HasForeignKey(p => p.IdCustomerType)
                    .HasPrincipalKey(p => p.Id);
                entity.HasMany(p => p.OrderNotes)
                    .WithOne(p => p.CustomerType)
                    .HasForeignKey(p => p.IdCustomerType)
                    .HasPrincipalKey(p => p.Id);
            });


            builder.Entity<CustomerToOrderNote>(entity =>
            {
                entity.Ignore(p => p.Id);
                entity.HasKey(p => new { p.IdCustomer, p.IdOrderNote });
                entity.ToTable("CustomersToOrderNotes");
            });


            builder.Entity<CustomerToPaymentMethod>(entity =>
            {
                entity.Ignore(p => p.Id);
                entity.HasKey(p => new { p.IdCustomer, p.IdPaymentMethod });
                entity.ToTable("CustomersToPaymentMethods");
            });


            #endregion

            #region Addresses

            builder.Entity<Address>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("Addresses");
                entity
                    .HasOne(p => p.Country)
                    .WithMany()
                    .HasForeignKey(p => p.IdCountry)
                    .HasPrincipalKey(c => c.Id)
                    .IsRequired();
                entity
                    .HasOne(p => p.State)
                    .WithMany()
                    .HasForeignKey(p => p.IdState)
                    .HasPrincipalKey(s => s.Id)
                    .IsRequired(false);
                entity
                    .HasOne(p => p.EditedBy)
                    .WithMany()
                    .HasForeignKey(p => p.IdEditedBy)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired(false);
                entity
                    .HasMany(a => a.OptionValues)
                    .WithOne(v => v.Address)
                    .HasForeignKey(o => o.IdAddress)
                    .HasPrincipalKey(a => a.Id)
                    .IsRequired();
                entity.Ignore(a => a.OptionTypes);
            });


            builder.Entity<AddressOptionType>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("AddressOptionTypes");
                entity
                    .HasOne(p => p.Lookup)
                    .WithMany()
                    .HasForeignKey(p => p.IdLookup)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired(false);
                entity.HasIndex(e => new { e.Name, e.IdObjectType }).IsUnique(true);
            });


            builder.Entity<AddressOptionValue>(entity =>
            {
                entity.HasKey(o => new { o.IdAddress, o.IdOptionType });
                entity.Ignore(o => o.Id);
                entity.ToTable("AddressOptionValues");
                entity
                    .HasOne(v => v.OptionType)
                    .WithMany()
                    .HasForeignKey(t => t.IdOptionType)
                    .HasPrincipalKey(v => v.Id)
                    .IsRequired();
                entity.Ignore(c => c.BigValue);
                entity.Ignore(c => c.IdBigString);
            });


            builder.Entity<AddressTypeEntity>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("AddressTypes");
            });


            #endregion

            #region Orders Notes

            builder.Entity<OrderNoteToCustomerType>(entity =>
            {
                entity.HasKey(p => new { p.IdOrderNote, p.IdCustomerType });
                entity.ToTable("OrderNotesToCustomerTypes");
                entity.Ignore(p => p.Id);
            });


            //builder.Entity<OrderNoteToCustomerType>()
            //    .HasOne(n => n.CustomerType)
            //    .WithMany()
            //    .HasForeignKey(n => n.IdCustomerType)
            //    .HasPrincipalKey(t => t.Id)
            //    .IsRequired();

            builder.Entity<OrderNote>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("OrderNotes");
                entity
                    .HasOne(p => p.EditedBy)
                    .WithMany()
                    .HasForeignKey(p => p.IdEditedBy)
                    .HasPrincipalKey(p => p.Id);
                entity
                    .HasMany(p => p.Customers)
                    .WithOne(p => p.OrderNote)
                    .HasForeignKey(p => p.IdOrderNote)
                    .HasPrincipalKey(p => p.Id);
                entity
                    .HasMany(p => p.CustomerTypes)
                    .WithOne(p => p.OrderNote)
                    .HasForeignKey(p => p.IdOrderNote)
                    .HasPrincipalKey(p => p.Id);
            });


            #endregion

            #region Payment

            builder.Entity<PaymentMethodToCustomerType>(entity =>
            {
                entity.HasKey(p => new { p.IdPaymentMethod, p.IdCustomerType });
                entity.ToTable("PaymentMethodsToCustomerTypes");
                entity.Ignore(p => p.Id);
            });


            builder.Entity<PaymentMethod>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("PaymentMethods");
                entity.HasOne(p => p.EditedBy).WithMany().HasForeignKey(p => p.IdEditedBy).HasPrincipalKey(p => p.Id)
                    .IsRequired(false);
                entity
                    .HasMany(p => p.Customers)
                    .WithOne(p => p.PaymentMethod)
                    .HasForeignKey(p => p.IdPaymentMethod)
                    .HasPrincipalKey(p => p.Id);
                entity
                    .HasMany(p => p.CustomerTypes)
                    .WithOne(p => p.PaymentMethod)
                    .HasForeignKey(p => p.IdPaymentMethod)
                    .HasPrincipalKey(p => p.Id);
            });


            builder.Entity<CustomerPaymentMethod>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("CustomerPaymentMethods");
                entity
                    .HasOne(p => p.EditedBy)
                    .WithMany()
                    .HasForeignKey(p => p.IdEditedBy)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired(false);
                entity
                    .HasOne(p => p.PaymentMethod)
                    .WithMany()
                    .HasForeignKey(p => p.IdObjectType)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired();
                entity
                    .HasOne(p => p.BillingAddress)
                    .WithOne()
                    .HasForeignKey<CustomerPaymentMethod>(p => p.IdAddress)
                    .HasPrincipalKey<Address>(p => p.Id)
                    .IsRequired(false);
                entity
                    .HasMany(p => p.OptionValues)
                    .WithOne()
                    .HasForeignKey(v => v.IdCustomerPaymentMethod)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired();
                entity.Ignore(a => a.OptionTypes);
            });


            builder.Entity<CustomerPaymentMethodOptionType>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("CustomerPaymentMethodOptionTypes");
                entity
                    .HasOne(p => p.Lookup)
                    .WithMany()
                    .HasForeignKey(p => p.IdLookup)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired(false);
                entity.HasIndex(e => new { e.Name, e.IdObjectType }).IsUnique(true);
            });

            builder.Entity<CustomerPaymentMethodOptionValue>(entity =>
            {
                entity.Ignore(o => o.Id);
                entity.HasKey(o => new { o.IdCustomerPaymentMethod, o.IdOptionType });
                entity.ToTable("CustomerPaymentMethodValues");
                entity
                    .HasOne(v => v.OptionType)
                    .WithMany()
                    .HasForeignKey(t => t.IdOptionType)
                    .HasPrincipalKey(v => v.Id)
                    .IsRequired();
                entity.Ignore(v => v.BigValue);
                entity.Ignore(v => v.IdBigString);
            });


            #endregion

            #region Orders

            builder.Entity<OrderTypeEntity>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.ToTable("OrderTypes");
            });

            builder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.Id);
                entity.ToTable("Orders");
                entity.HasOne(o => o.Customer)
                    .WithMany()
                    .HasForeignKey(o => o.IdCustomer)
                    .HasPrincipalKey(c => c.Id)
                    .IsRequired();
                entity.HasOne(o => o.OrderSource)
                    .WithMany()
                    .HasForeignKey(o => o.IdOrderSource)
                    .HasPrincipalKey(c => c.Id)
                    .IsRequired(false);
                entity.HasMany(o => o.Skus)
                    .WithOne(a => a.Order)
                    .HasForeignKey(s => s.IdOrder)
                    .HasPrincipalKey(o => o.Id)
                    .IsRequired();
                entity.HasMany(o => o.GiftCertificates)
                    .WithOne(a => a.Order)
                    .HasForeignKey(g => g.IdOrder)
                    .HasPrincipalKey(o => o.Id)
                    .IsRequired();
                entity.HasOne(o => o.Discount)
                    .WithMany()
                    .HasForeignKey(o => o.IdDiscount)
                    .HasPrincipalKey(d => d.Id)
                    .IsRequired(false);
                entity.HasOne(o => o.PaymentMethod)
                    .WithOne()
                    .HasForeignKey<Order>(o => o.IdPaymentMethod)
                    .HasPrincipalKey<OrderPaymentMethod>(p => p.Id)
                    .IsRequired(false);
                entity.HasMany(o => o.OptionValues)
                    .WithOne()
                    .HasForeignKey(v => v.IdOrder)
                    .HasPrincipalKey(o => o.Id)
                    .IsRequired();
                entity.HasOne(o => o.ShippingAddress)
                    .WithOne()
                    .HasForeignKey<Order>(o => o.IdShippingAddress)
                    .HasPrincipalKey<OrderAddress>(a => a.Id)
                    .IsRequired(false);
                entity.HasOne(p => p.EditedBy)
                    .WithMany()
                    .HasForeignKey(p => p.IdEditedBy)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired(false);
                entity.Ignore(o => o.OptionTypes);
            });

            builder.Entity<OrderOptionValue>(entity =>
            {
                entity.HasKey(o => new { o.IdOrder, o.IdOptionType });
                entity.Ignore(o => o.Id);
                entity.ToTable("OrderOptionValues");
                entity.HasOne(v => v.OptionType)
                    .WithOne()
                    .HasForeignKey<OrderOptionValue>(v => v.IdOptionType)
                    .HasPrincipalKey<OrderOptionType>(t => t.Id)
                    .IsRequired();
                entity.Ignore(v => v.BigValue);
                entity.Ignore(v => v.IdBigString);
            });

            builder.Entity<OrderOptionType>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.ToTable("OrderOptionTypes");
                entity.HasOne(t => t.Lookup)
                    .WithOne()
                    .HasForeignKey<OrderOptionType>(t => t.IdLookup)
                    .HasPrincipalKey<Lookup>(l => l.Id)
                    .IsRequired(false);
                entity.HasIndex(e => new { e.Name, e.IdObjectType }).IsUnique(true);
            });

            builder.Entity<OrderToPromoToInventorySku>(entity =>
            {
                entity.Ignore(s => s.Id);
                entity.HasKey(s => new { s.IdOrder, s.IdSku, s.IdInventorySku });
                entity.ToTable("OrderToPromosToInventorySkus");
            });

            builder.Entity<OrderToPromo>(entity =>
            {
                entity.Ignore(p => p.Id);
                entity.ToTable("OrderToPromos");
                entity.HasKey(p => new {p.IdOrder, p.IdSku});
                entity.HasOne(s => s.Order)
                    .WithMany(o => o.PromoSkus)
                    .HasForeignKey(s => s.IdOrder)
                    .HasPrincipalKey(o => o.Id);
                entity.HasOne(s => s.Sku)
                    .WithOne()
                    .HasForeignKey<OrderToPromo>(s => s.IdSku)
                    .HasPrincipalKey<Sku>(s => s.Id);
                entity.HasOne(p => p.Promo)
                    .WithMany()
                    .HasForeignKey(p => p.IdPromo)
                    .HasPrincipalKey(p => p.Id);
                entity.HasMany(s => s.InventorySkus)
                    .WithOne()
                    .HasForeignKey(s => new { s.IdOrder, s.IdSku })
                    .HasPrincipalKey(s => new { s.IdOrder, s.IdSku });
            });

            builder.Entity<OrderToGiftCertificate>(entity =>
            {
                entity.Ignore(s => s.Id);
                entity.HasKey(s => new { s.IdOrder, s.IdGiftCertificate });
                entity.ToTable("OrderToGiftCertificates");
                entity.HasOne(g => g.GiftCertificate)
                    .WithOne()
                    .HasForeignKey<OrderToGiftCertificate>(g => g.IdGiftCertificate)
                    .HasPrincipalKey<GiftCertificate>(g => g.Id);
                entity.HasOne(g => g.Order)
                    .WithMany(o => o.GiftCertificates)
                    .HasForeignKey(g => g.IdOrder)
                    .HasPrincipalKey(o => o.Id);
            });


            builder.Entity<RefundOrderToGiftCertificate>(entity =>
            {
                entity.Ignore(s => s.Id);
                entity.HasKey(s => new { s.IdRefundOrder, s.IdOrder, s.IdGiftCertificate });
                entity.ToTable("RefundOrderToGiftCertificates");
                entity.HasOne(g => g.RefundOrder)
                    .WithMany(p => p.RefundOrderToGiftCertificates)
                    .HasForeignKey(g => g.IdRefundOrder)
                    .HasPrincipalKey(g => g.Id)
                    .IsRequired();
                entity.HasOne(g => g.OrderToGiftCertificate)
                    .WithMany()
                    .HasForeignKey(g => new {g.IdOrder, g.IdGiftCertificate})
                    .HasPrincipalKey(o => new {o.IdOrder, o.IdGiftCertificate})
                    .IsRequired();
            });
            builder.Entity<OrderToSkuToInventorySku>(entity =>
            {
                entity.Ignore(s => s.Id);
                entity.HasKey(s => new {s.IdOrder, s.IdSku, s.IdInventorySku});
                entity.ToTable("OrderToSkusToInventorySkus");
            });

            builder.Entity<OrderToSku>(entity =>
            {
                entity.Ignore(s => s.Id);
                entity.HasKey(s => new { s.IdOrder, s.IdSku });
                entity.ToTable("OrderToSkus");
                entity.HasOne(s => s.Order)
                    .WithMany(o => o.Skus)
                    .HasForeignKey(s => s.IdOrder)
                    .HasPrincipalKey(o => o.Id);
                entity.HasOne(s => s.Sku)
                    .WithOne()
                    .HasForeignKey<OrderToSku>(s => s.IdSku)
                    .HasPrincipalKey<Sku>(s => s.Id);
                entity.HasMany(s => s.InventorySkus)
                    .WithOne()
                    .HasForeignKey(s => new { s.IdOrder, s.IdSku})
                    .HasPrincipalKey(s => new { s.IdOrder, s.IdSku });
                entity.HasMany(s => s.GeneratedGiftCertificates)
                    .WithOne(g => g.Sku)
                    .HasForeignKey(g => new {g.IdOrder, g.IdSku})
                    .HasPrincipalKey(s => new {s.IdOrder, s.IdSku});
            });

            builder.Entity<OrderStatusEntity>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.ToTable("OrderStatuses");
            });

            builder.Entity<OrderPaymentMethod>(entity =>
            {
                entity.HasKey(o => o.Id);
                entity.ToTable("OrderPaymentMethods");
                entity.HasOne(o => o.BillingAddress)
                    .WithMany()
                    .HasForeignKey(o => o.IdAddress)
                    .HasPrincipalKey(c => c.Id)
                    .IsRequired(false);
                entity.HasOne(o => o.PaymentMethod)
                    .WithOne()
                    .HasForeignKey<OrderPaymentMethod>(s => s.IdObjectType)
                    .HasPrincipalKey<PaymentMethod>(o => o.Id)
                    .IsRequired();
                entity.HasMany(o => o.OptionValues)
                    .WithOne()
                    .HasForeignKey(g => g.IdOrderPaymentMethod)
                    .HasPrincipalKey(o => o.Id)
                    .IsRequired();
                entity.HasOne(p => p.EditedBy)
                    .WithMany()
                    .HasForeignKey(p => p.IdEditedBy)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired(false);
                entity.Ignore(o => o.OptionTypes);
            });

            builder.Entity<OrderPaymentMethodOptionValue>(entity =>
            {
                entity.HasKey(o => new { o.IdOrderPaymentMethod, o.IdOptionType });
                entity.Ignore(o => o.Id);
                entity.ToTable("OrderPaymentMethodOptionValues");
                entity.HasOne(v => v.OptionType)
                    .WithOne()
                    .HasForeignKey<OrderPaymentMethodOptionValue>(v => v.IdOptionType)
                    .HasPrincipalKey<CustomerPaymentMethodOptionType>(t => t.Id)
                    .IsRequired();
                entity.Ignore(v => v.BigValue);
                entity.Ignore(v => v.IdBigString);
            });

            builder.Entity<OrderAddress>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("OrderAddresses");
                entity.HasOne(p => p.Сountry)
                    .WithMany()
                    .HasForeignKey(p => p.IdCountry)
                    .HasPrincipalKey(c => c.Id)
                    .IsRequired();
                entity.HasOne(p => p.State)
                    .WithMany()
                    .HasForeignKey(p => p.IdState)
                    .HasPrincipalKey(s => s.Id)
                    .IsRequired(false);
                entity.HasOne(p => p.EditedBy)
                    .WithMany()
                    .HasForeignKey(p => p.IdEditedBy)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired(false);
                entity.HasMany(a => a.OptionValues)
                    .WithOne()
                    .HasForeignKey(o => o.IdOrderAddress)
                    .HasPrincipalKey(a => a.Id)
                    .IsRequired();
                entity.Ignore(a => a.OptionTypes);
            });

            builder.Entity<OrderAddressOptionValue>(entity =>
            {
                entity.HasKey(o => new { o.IdOrderAddress, o.IdOptionType });
                entity.Ignore(o => o.Id);
                entity.ToTable("OrderAddressOptionValues");
                entity.HasOne(v => v.OptionType)
                    .WithMany()
                    .HasForeignKey(t => t.IdOptionType)
                    .HasPrincipalKey(v => v.Id)
                    .IsRequired();
                entity.Ignore(v => v.BigValue);
                entity.Ignore(v => v.IdBigString);
            });

            builder.Entity<RefundSku>(entity =>
            {
                entity.Ignore(s => s.Id);
                entity.HasKey(r => new { r.IdOrder, r.IdSku });
                entity.ToTable("RefundSkus");
                entity.HasOne(r => r.Order)
                    .WithMany(p=>p.RefundSkus)
                    .HasForeignKey(r => r.IdOrder)
                    .HasPrincipalKey(o => o.Id)
                    .IsRequired();
                entity.HasOne(r => r.Sku)
                    .WithMany()
                    .HasForeignKey(r => r.IdSku)
                    .HasPrincipalKey(s => s.Id)
                    .IsRequired();
            });

            builder.Entity<ReshipProblemSku>(entity =>
            {
                entity.HasKey(r => new { r.IdOrder, r.IdSku });
                entity.ToTable("ReshipProblemSkus");
                entity.HasOne(r => r.Order)
                    .WithMany(p=>p.ReshipProblemSkus)
                    .HasForeignKey(r => r.IdOrder)
                    .HasPrincipalKey(o => o.Id)
                    .IsRequired();
                entity.HasOne(r => r.Sku)
                    .WithMany()
                    .HasForeignKey(r => r.IdSku)
                    .HasPrincipalKey(s => s.Id)
                    .IsRequired();
                entity.Ignore(v => v.Id);
            });

            #endregion

            #region Affiliates

            builder.Entity<AffiliateOptionValue>(entity =>
            {
                entity.HasKey(o => new { o.IdAffiliate, o.IdOptionType });
                entity.Ignore(o => o.Id);
                entity.ToTable("AffiliateOptionValues");
                entity.HasOne(v => v.OptionType)
                    .WithMany()
                    .HasForeignKey(t => t.IdOptionType)
                    .HasPrincipalKey(v => v.Id)
                    .IsRequired();
                entity.HasOne(v => v.BigValue)
                    .WithMany()
                    .HasForeignKey(v => v.IdBigString)
                    .HasPrincipalKey(b => b.IdBigString)
                    .IsRequired(false);
                entity.Property(v => v.IdBigString).IsRequired(false);
            });

            builder.Entity<AffiliateOptionType>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.ToTable("AffiliateOptionTypes");
                entity.HasOne(p => p.Lookup)
                    .WithMany()
                    .HasForeignKey(p => p.IdLookup)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired(false);
                entity.HasIndex(e => new { e.Name, e.IdObjectType }).IsUnique(true);
            });

            builder.Entity<Affiliate>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.ToTable("Affiliates");
                entity.HasOne(p => p.User)
                    .WithMany()
                    .HasForeignKey(p => p.Id)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired();
                entity.HasMany(p => p.OptionValues)
                    .WithOne()
                    .HasForeignKey(o => o.IdAffiliate)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired();
                entity.Ignore(p => p.OptionTypes);
                entity.HasOne(p => p.EditedBy)
                    .WithMany()
                    .HasForeignKey(o => o.IdEditedBy)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired(false);
                entity.Ignore(p => p.IdObjectType);
            });

            builder.Entity<AffiliateOrderPayment>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.ToTable("AffiliateOrderPayments");
                entity.HasOne(p => p.Order)
                    .WithOne(p => p.AffiliateOrderPayment)
                    .HasForeignKey<AffiliateOrderPayment>(p => p.Id)
                    .HasPrincipalKey<Order>(p => p.Id)
                    .IsRequired();
                entity.HasOne(p => p.Affiliate)
                    .WithMany()
                    .HasForeignKey(p => p.IdAffiliate)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired();
            });

            builder.Entity<AffiliatePayment>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.ToTable("AffiliatePayments");
                entity.HasMany(p => p.OrderPayments)
                    .WithOne()
                    .HasForeignKey(o => o.IdAffiliatePayment)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired(false);
            });

            #endregion

            #region ObjectHistory

            builder.Entity<ObjectHistoryLogDataItem>(entity =>
            {
                entity.HasKey(t => t.IdObjectHistoryLogDataItem);
                entity.ToTable("ObjectHistoryLogDataItems");
                entity.Ignore(p => p.Id);
            });

            builder.Entity<ObjectHistoryLogItem>(entity =>
            {
                entity.HasKey(t => t.IdObjectHistoryLogItem);
                entity.ToTable("ObjectHistoryLogItems");
                entity.Ignore(p => p.Id);
                entity.Ignore(p => p.EditedBy);
                entity.HasOne(p => p.DataItem)
                    .WithMany()
                    .HasForeignKey(p => p.IdObjectHistoryLogDataItem)
                    .HasPrincipalKey(p => p.IdObjectHistoryLogDataItem)
                    .IsRequired(false);
            });

            #endregion

            #region Checkout

            Carts(builder);
            CartToSkus(builder);
            CartToGiftCertificates(builder);

            #endregion

            #region HealthWise

            builder.Entity<HealthwiseOrder>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.ToTable("HealthwiseOrders");
                entity.HasOne(p => p.Order)
                    .WithOne(p=>p.HealthwiseOrder)
                    .HasForeignKey<HealthwiseOrder>(p => p.Id)
                    .HasPrincipalKey<Order>(p => p.Id)
                    .IsRequired();
            });

            builder.Entity<HealthwisePeriod>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.ToTable("HealthwisePeriods");
                entity.HasMany(p => p.HealthwiseOrders)
                    .WithOne(p => p.HealthwisePeriod)
                    .HasForeignKey(o => o.IdHealthwisePeriod)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired();
                entity.HasOne(p => p.Customer)
                    .WithMany()
                    .HasForeignKey(o => o.IdCustomer)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired();
            });

            #endregion
        }

        protected virtual void SkuToInventorySku(ModelBuilder builder)
        {
            builder.Entity<SkuToInventorySku>(entity =>
            {
                entity.HasKey(o => new
                {
                    o.IdSku,
                    o.IdInventorySku
                });
                entity.ToTable("SkusToInventorySkus");
                entity.Ignore(p=>p.Id);
            });
        }

        protected virtual void InventorySkuOptionTypes(ModelBuilder builder)
        {
            builder.Entity<InventorySkuOptionType>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.ToTable("InventorySkuOptionTypes");
                entity.HasOne(p => p.Lookup)
                    .WithMany()
                    .HasForeignKey(p => p.IdLookup)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired(false);
                entity.HasIndex(e => new { e.Name, e.IdObjectType }).IsUnique(true);
            });
        }

        protected virtual void InventorySkuOptionValues(ModelBuilder builder)
        {
            builder.Entity<InventorySkuOptionValue>(entity =>
            {
                entity.HasKey(o => new {
                    o.IdInventorySku,
                    o.IdOptionType
                });
                entity.Ignore(o => o.Id);
                entity.ToTable("InventorySkuOptionValues");
                entity.HasOne(v => v.OptionType)
                    .WithMany()
                    .HasForeignKey(t => t.IdOptionType)
                    .HasPrincipalKey(v => v.Id)
                    .IsRequired();
                entity.Ignore(v => v.BigValue);
                entity.Ignore(v => v.IdBigString);
            });
        }

        protected virtual void InventorySkus(ModelBuilder builder)
        {
            builder.Entity<InventorySku>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.ToTable("InventorySkus");
                entity.HasMany(p => p.OptionValues)
                    .WithOne()
                    .HasForeignKey(o => o.IdInventorySku)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired();
                entity.HasOne(p => p.EditedBy)
                    .WithMany()
                    .HasForeignKey(o => o.IdEditedBy)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired(false);
                entity.HasOne(p => p.InventorySkuCategory)
                    .WithMany()
                    .HasForeignKey(o => o.IdInventorySkuCategory)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired(false);
                entity.Ignore(p => p.OptionTypes);
                entity.Ignore(p => p.IdObjectType);
            });
        }

        protected virtual void InventorySkuCategories(ModelBuilder builder)
        {
            builder.Entity<InventorySkuCategory>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("InventorySkuCategories");
            });
        }

        protected virtual void CartToGiftCertificates(ModelBuilder builder)
        {
            builder.Entity<CartToGiftCertificate>(entity =>
            {
                entity.ToTable("CartToGiftCertificates");
                entity.Ignore(c => c.Id);
                entity.HasKey(c => new {c.IdCart, c.IdGiftCertificate});
                entity.HasOne(c => c.GiftCertificate).WithMany().HasForeignKey(c => c.IdGiftCertificate).HasPrincipalKey(s => s.Id);
            });
        }

        protected virtual void CartToSkus(ModelBuilder builder)
        {
            builder.Entity<CartToSku>(entity =>
            {
                entity.ToTable("CartToSkus");
                entity.Ignore(c => c.Id);
                entity.HasKey(c => new {c.IdCart, c.IdSku});
                entity.HasOne(c => c.Sku).WithMany().HasForeignKey(c => c.IdSku).HasPrincipalKey(s => s.Id);
            });
        }

        protected virtual void Carts(ModelBuilder builder)
        {
            builder.Entity<Cart>(entity =>
            {
                entity.ToTable("Carts");
                entity.HasKey(c => c.Id);
                entity.HasIndex(c => c.CartUid).IsUnique();
                entity.HasMany(c => c.GiftCertificates).WithOne().HasForeignKey(g => g.IdCart).HasPrincipalKey(c => c.Id);
                entity.HasMany(c => c.Skus).WithOne().HasForeignKey(s => s.IdCart).HasPrincipalKey(c => c.Id);
            });
        }

        protected virtual void Discounts(ModelBuilder builder)
        {
            builder.Entity<Discount>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("Discounts");
                entity
                    .HasMany(p => p.OptionValues)
                    .WithOne()
                    .HasForeignKey(o => o.IdDiscount)
                    .HasPrincipalKey(p => p.Id);

                entity.Ignore(p => p.OptionTypes);

                entity
                    .HasMany(p => p.DiscountsToCategories)
                    .WithOne()
                    .HasForeignKey(t => t.IdDiscount)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired();
                entity
                    .HasMany(p => p.DiscountsToSelectedCategories)
                    .WithOne()
                    .HasForeignKey(t => t.IdDiscount)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired();
                entity
                    .HasMany(p => p.DiscountsToSkus)
                    .WithOne()
                    .HasForeignKey(t => t.IdDiscount)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired();
                entity
                    .HasMany(p => p.DiscountsToSelectedSkus)
                    .WithOne()
                    .HasForeignKey(t => t.IdDiscount)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired();
                entity
                    .HasMany(p => p.DiscountTiers)
                    .WithOne()
                    .HasForeignKey(t => t.IdDiscount)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired();
                entity
                    .HasOne(p => p.EditedBy)
                    .WithMany()
                    .HasForeignKey(o => o.IdEditedBy)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired(false);
            });
        }

        protected virtual void DiscountTiers(ModelBuilder builder)
        {
            builder.Entity<DiscountTier>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("DiscountTiers");
            });
        }

        protected virtual void DiscountToSelectedCategories(ModelBuilder builder)
        {
            builder.Entity<DiscountToSelectedCategory>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("DiscountToSelectedCategories");
            });
        }

        protected virtual void DiscountsToSelectedSkus(ModelBuilder builder)
        {
            builder.Entity<DiscountToSelectedSku>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("DiscountsToSelectedSkus");
                entity.Ignore(p => p.ShortSkuInfo);
            });
        }

        protected virtual void DiscountsToSkus(ModelBuilder builder)
        {
            builder.Entity<DiscountToSku>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("DiscountsToSkus");
                entity.Ignore(p => p.ShortSkuInfo);
            });
        }

        protected virtual void DiscountsToCategories(ModelBuilder builder)
        {
            builder.Entity<DiscountToCategory>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("DiscountsToCategories");
            });
        }

        protected virtual void DiscountOptionValues(ModelBuilder builder)
        {
            builder.Entity<DiscountOptionValue>(entity =>
            {
                entity.Ignore(o => o.Id);
                entity.HasKey(d => new {d.IdDiscount, d.IdOptionType});
                entity.ToTable("DiscountOptionValues");
                entity
                    .HasOne(v => v.OptionType)
                    .WithMany()
                    .HasForeignKey(t => t.IdOptionType)
                    .HasPrincipalKey(v => v.Id);

                entity.Ignore(d => d.BigValue);
                entity.Ignore(d => d.IdBigString);
            });
        }

        protected virtual void DiscountOptionTypes(ModelBuilder builder)
        {
            builder.Entity<DiscountOptionType>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("DiscountOptionTypes");
                entity
                    .HasOne(p => p.Lookup)
                    .WithMany()
                    .HasForeignKey(p => p.IdLookup)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired(false);
                entity.HasIndex(e => new {e.Name, e.IdObjectType}).IsUnique(true);
            });
        }

        protected virtual void GiftCertificates(ModelBuilder builder)
        {
            builder.Entity<GiftCertificate>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("GiftCertificates");
                entity.Property(p => p.PublicId).ValueGeneratedOnAdd();
                entity.HasOne(s => s.Order)
                     .WithMany()
                     .HasForeignKey(g =>g.IdOrder)
                     .HasPrincipalKey(s => s.Id)
                     .IsRequired(false);
            });
        }

        protected virtual void WorkflowActionAggregations(ModelBuilder builder)
        {
            builder.Entity<WorkflowActionAggregation>(entity =>
            {
                entity.ToTable("WorkflowActionAggregations");
                entity.HasKey(d => new {d.IdParent, d.IdAggregate});
                entity.Ignore(d => d.Id);
                entity.HasOne(d => d.Parent)
                    .WithMany(e => e.Aggreagations)
                    .HasForeignKey(d => d.IdParent)
                    .HasPrincipalKey(e => e.Id);
                entity.HasOne(d => d.ToAggregate)
                    .WithMany()
                    .HasForeignKey(d => d.IdAggregate)
                    .HasPrincipalKey(e => e.Id);
            });
        }

        protected virtual void WorkflowActionDependencies(ModelBuilder builder)
        {
            builder.Entity<WorkflowActionDependency>(entity =>
            {
                entity.ToTable("WorkflowActionDependencies");
                entity.HasKey(d => new {d.IdParent, d.IdDependent});
                entity.Ignore(d => d.Id);
                entity.HasOne(d => d.Parent)
                    .WithMany(e => e.Dependencies)
                    .HasForeignKey(d => d.IdParent)
                    .HasPrincipalKey(e => e.Id);
                entity.HasOne(d => d.Dependent)
                    .WithMany()
                    .HasForeignKey(d => d.IdDependent)
                    .HasPrincipalKey(e => e.Id);
            });
        }

        protected virtual void WorkflowTreeActions(ModelBuilder builder)
        {
            builder.Entity<WorkflowTreeAction>(entity =>
            {
                entity.HasKey(a => new {a.IdExecutor, a.IdTree});
                entity.Ignore(a => a.Id);
                entity.ToTable("WorkflowTreeActions");
                entity
                    .HasOne(treeAction => treeAction.Executor)
                    .WithOne()
                    .HasForeignKey<WorkflowTreeAction>(treeAction => treeAction.IdExecutor)
                    .HasPrincipalKey<WorkflowExecutor>(executor => executor.Id);
                entity
                    .HasOne(action => action.Tree)
                    .WithMany(tree => tree.Actions)
                    .HasForeignKey(action => action.IdTree)
                    .HasPrincipalKey(tree => tree.Id);
            });
        }

        protected virtual void WorkflowTrees(ModelBuilder builder)
        {
            builder.Entity<WorkflowTree>(entity =>
            {
                entity.HasKey(w => w.Id);
                entity.ToTable("WorkflowTrees");
                entity
                    .HasMany(tree => tree.Actions)
                    .WithOne(action => action.Tree)
                    .HasForeignKey(action => action.IdTree)
                    .HasPrincipalKey(tree => tree.Id);
            });
        }

        protected virtual void WorkflowResolverPaths(ModelBuilder builder)
        {
            builder.Entity<WorkflowResolverPath>(entity =>
            {
                entity.HasKey(w => w.Id);
                entity.ToTable("WorkflowResolverPaths");
                entity
                    .HasOne(resolverPath => resolverPath.Executor)
                    .WithMany()
                    .HasForeignKey(resolverPath => resolverPath.IdExecutor)
                    .HasPrincipalKey(executor => executor.Id);
                entity
                    .HasOne(w => w.Resolver)
                    .WithMany(r => r.ResolverPaths)
                    .HasForeignKey(w => w.IdResolver)
                    .HasPrincipalKey(w => w.Id);
            });
        }

        protected virtual void WorkflowExecutors(ModelBuilder builder)
        {
            builder.Entity<WorkflowExecutor>(entity =>
            {
                entity.HasKey(w => w.Id);
                entity.ToTable("WorkflowExecutors");
                entity
                    .HasMany(e => e.ResolverPaths)
                    .WithOne(r => r.Resolver)
                    .HasForeignKey(r => r.IdResolver)
                    .HasPrincipalKey(e => e.Id);
                entity
                    .HasMany(e => e.ResolverPaths)
                    .WithOne(r => r.Resolver)
                    .HasForeignKey(r => r.IdResolver)
                    .HasPrincipalKey(e => e.Id);
                entity
                    .HasMany(e => e.Dependencies)
                    .WithOne(d => d.Parent)
                    .HasForeignKey(d => d.IdParent)
                    .HasPrincipalKey(e => e.Id);
            });
        }

        protected virtual void BigStringValues(ModelBuilder builder)
        {
            builder.Entity<BigStringValue>(entity =>
            {
                entity.HasKey(b => b.IdBigString);
                entity.Ignore(b => b.Id);
                entity.ToTable("BigStringValues");
            });
        }

        protected virtual void FieldTypes(ModelBuilder builder)
        {
            builder.Entity<FieldTypeEntity>(entity =>
            {
                entity.HasKey(f => f.Id);
                entity.ToTable("FieldTypes");
            });
        }

        protected virtual void AppOptions(ModelBuilder builder)
        {
            builder.Entity<AppOption>().HasKey(f => f.OptionName);
            builder.Entity<AppOption>().Ignore(f => f.Id);
            builder.Entity<AppOption>().ToTable("AppOptions");
        }
    }
}
