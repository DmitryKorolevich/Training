using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
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
using VitalChoice.Domain.Entities.eCommerce.Affiliates;
using VitalChoice.Domain.Entities.eCommerce.Help;
using VitalChoice.Domain.Entities.eCommerce.Promotions;
using VitalChoice.Domain.Entities.eCommerce.History;
using VitalChoice.Domain.Entities.eCommerce.Promotion;

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
                DataSource = _options.Value.Connection.Server,
                // TODO: Currently nested queries are run while processing the results of outer queries
                // This either requires MARS or creation of a new connection for each query. Currently using
                // MARS since cloning connections is known to be problematic.
                MultipleActiveResultSets = true,
                InitialCatalog = "VitalChoice.Ecommerce",
                UserID = _options.Value.Connection.UserName,
                Password = _options.Value.Connection.Password,
                ConnectTimeout = 60
            }).ConnectionString;
            builder.UseSqlServer(connectionString);

            base.OnConfiguring(builder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.UseSqlServerIdentityColumns();

            #region Base

            builder.Entity<FieldTypeEntity>().HasKey(f => f.Id);
            builder.Entity<FieldTypeEntity>().ToTable("FieldTypes");

            builder.Entity<BigStringValue>().HasKey(b => b.IdBigString);
            builder.Entity<BigStringValue>().Ignore(b => b.Id);
            builder.Entity<BigStringValue>().ToTable("BigStringValues");

            #endregion


            #region Workflow

            builder.Entity<WorkflowExecutor>().HasKey(w => w.Id);
            builder.Entity<WorkflowExecutor>().ToTable("WorkflowExecutors");
            builder.Entity<WorkflowExecutor>()
                .HasMany(e => e.ResolverPaths)
                .WithOne(r => r.Resolver)
                .ForeignKey(r => r.IdResolver)
                .PrincipalKey(e => e.Id);
            builder.Entity<WorkflowExecutor>()
                .HasMany(e => e.ResolverPaths)
                .WithOne(r => r.Resolver)
                .ForeignKey(r => r.IdResolver)
                .PrincipalKey(e => e.Id);
            builder.Entity<WorkflowExecutor>()
                .HasMany(e => e.Dependencies)
                .WithOne(d => d.Parent)
                .ForeignKey(d => d.IdParent)
                .PrincipalKey(e => e.Id);

            builder.Entity<WorkflowResolverPath>().HasKey(w => w.Id);
            builder.Entity<WorkflowResolverPath>().ToTable("WorkflowResolverPaths");
            builder.Entity<WorkflowResolverPath>()
                .HasOne(resolverPath => resolverPath.Executor)
                .WithMany()
                .ForeignKey(resolverPath => resolverPath.IdExecutor)
                .PrincipalKey(executor => executor.Id);
            builder.Entity<WorkflowResolverPath>()
                .HasOne(w => w.Resolver)
                .WithMany(r => r.ResolverPaths)
                .ForeignKey(w => w.IdResolver)
                .PrincipalKey(w => w.Id);

            builder.Entity<WorkflowTree>().HasKey(w => w.Id);
            builder.Entity<WorkflowTree>().ToTable("WorkflowTrees");
            builder.Entity<WorkflowTree>()
                .HasMany(tree => tree.Actions)
                .WithOne(action => action.Tree)
                .ForeignKey(action => action.IdTree)
                .PrincipalKey(tree => tree.Id);

            builder.Entity<WorkflowTreeAction>().HasKey(a => new { a.IdExecutor, a.IdTree });
            builder.Entity<WorkflowTreeAction>().Ignore(a => a.Id);
            builder.Entity<WorkflowTreeAction>().ToTable("WorkflowTreeActions");
            builder.Entity<WorkflowTreeAction>()
                .HasOne(treeAction => treeAction.Executor)
                .WithOne()
                .ForeignKey<WorkflowTreeAction>(treeAction => treeAction.IdExecutor)
                .PrincipalKey<WorkflowExecutor>(executor => executor.Id);
            builder.Entity<WorkflowTreeAction>()
                .HasOne(action => action.Tree)
                .WithMany(tree => tree.Actions)
                .ForeignKey(action => action.IdTree)
                .PrincipalKey(tree => tree.Id);

            builder.Entity<WorkflowActionDependency>(entity =>
            {
                entity.ToTable("WorkflowActionDependencies");
                entity.HasKey(d => new { d.IdParent, d.IdDependent });
                entity.Ignore(d => d.Id);
                entity.HasOne(d => d.Parent)
                    .WithMany(e => e.Dependencies)
                    .ForeignKey(d => d.IdParent)
                    .PrincipalKey(e => e.Id);
                entity.HasOne(d => d.Dependent)
                    .WithMany()
                    .ForeignKey(d => d.IdDependent)
                    .PrincipalKey(e => e.Id);
            });

            builder.Entity<WorkflowActionAggregation>(entity =>
            {
                entity.ToTable("WorkflowActionAggregations");
                entity.HasKey(d => new { d.IdParent, d.IdAggregate });
                entity.Ignore(d => d.Id);
                entity.HasOne(d => d.Parent)
                    .WithMany(e => e.Aggreagations)
                    .ForeignKey(d => d.IdParent)
                    .PrincipalKey(e => e.Id);
                entity.HasOne(d => d.ToAggregate)
                    .WithMany()
                    .ForeignKey(d => d.IdAggregate)
                    .PrincipalKey(e => e.Id);
            });

            #endregion

            #region GiftCertificates

            builder.Entity<GiftCertificate>().HasKey(p => p.Id);
            builder.Entity<GiftCertificate>().ToTable("GiftCertificates");
            builder.Entity<GiftCertificate>().Property(p => p.PublicId).ValueGeneratedOnAdd();

            #endregion

            #region Discounts

            builder.Entity<DiscountOptionType>().HasKey(p => p.Id);
            builder.Entity<DiscountOptionType>().ToTable("DiscountOptionTypes");
            builder.Entity<DiscountOptionType>()
                .HasOne(p => p.Lookup)
                .WithMany()
                .ForeignKey(p => p.IdLookup)
                .PrincipalKey(p => p.Id)
                .Required(false);

            builder.Entity<DiscountOptionValue>().HasKey(o => o.Id);
            builder.Entity<DiscountOptionValue>().ToTable("DiscountOptionValues");
            builder.Entity<DiscountOptionValue>()
                .HasOne(v => v.OptionType)
                .WithMany()
                .ForeignKey(t => t.IdOptionType)
                .PrincipalKey(v => v.Id);

            builder.Entity<DiscountOptionValue>().Ignore(d => d.BigValue);
            builder.Entity<DiscountOptionValue>().Ignore(d => d.IdBigString);

            builder.Entity<DiscountToCategory>().HasKey(p => p.Id);
            builder.Entity<DiscountToCategory>().ToTable("DiscountsToCategories");

            builder.Entity<DiscountToSku>().HasKey(p => p.Id);
            builder.Entity<DiscountToSku>().ToTable("DiscountsToSkus");
            builder.Entity<DiscountToSku>().Ignore(p => p.ShortSkuInfo);

            builder.Entity<DiscountToSelectedSku>().HasKey(p => p.Id);
            builder.Entity<DiscountToSelectedSku>().ToTable("DiscountsToSelectedSkus");
            builder.Entity<DiscountToSelectedSku>().Ignore(p => p.ShortSkuInfo);

            builder.Entity<DiscountToSelectedCategory>().HasKey(p => p.Id);
            builder.Entity<DiscountToSelectedCategory>().ToTable("DiscountToSelectedCategories");

            builder.Entity<DiscountTier>().HasKey(p => p.Id);
            builder.Entity<DiscountTier>().ToTable("DiscountTiers");

            builder.Entity<Discount>().HasKey(p => p.Id);
            builder.Entity<Discount>().ToTable("Discounts");
            builder.Entity<Discount>()
                .HasMany(p => p.OptionValues)
                .WithOne()
                .ForeignKey(o => o.IdDiscount)
                .PrincipalKey(p => p.Id)
                .Required(false);

            builder.Entity<Discount>().Ignore(p => p.OptionTypes);

            builder.Entity<Discount>()
                .HasMany(p => p.DiscountsToCategories)
                .WithOne()
                .ForeignKey(t => t.IdDiscount)
                .PrincipalKey(p => p.Id)
                .Required();
            builder.Entity<Discount>()
                .HasMany(p => p.DiscountsToSelectedCategories)
                .WithOne()
                .ForeignKey(t => t.IdDiscount)
                .PrincipalKey(p => p.Id)
                .Required();
            builder.Entity<Discount>()
                .HasMany(p => p.DiscountsToSkus)
                .WithOne()
                .ForeignKey(t => t.IdDiscount)
                .PrincipalKey(p => p.Id)
                .Required();
            builder.Entity<Discount>()
                .HasMany(p => p.DiscountsToSelectedSkus)
                .WithOne()
                .ForeignKey(t => t.IdDiscount)
                .PrincipalKey(p => p.Id)
                .Required();
            builder.Entity<Discount>()
                .HasMany(p => p.DiscountTiers)
                .WithOne()
                .ForeignKey(t => t.IdDiscount)
                .PrincipalKey(p => p.Id)
                .Required();
            builder.Entity<Discount>()
                .HasOne(p => p.EditedBy)
                .WithMany()
                .ForeignKey(o => o.IdEditedBy)
                .PrincipalKey(p => p.Id)
                .Required(false);

            #endregion


            #region Promotions

            builder.Entity<PromotionTypeEntity>().HasKey(p => p.Id);
            builder.Entity<PromotionTypeEntity>().ToTable("PromotionTypes");

            builder.Entity<PromotionOptionType>().HasKey(p => p.Id);
            builder.Entity<PromotionOptionType>().ToTable("PromotionOptionTypes");
            builder.Entity<PromotionOptionType>()
                .HasOne(p => p.Lookup)
                .WithMany()
                .ForeignKey(p => p.IdLookup)
                .PrincipalKey(p => p.Id)
                .Required(false);

            builder.Entity<PromotionOptionValue>().HasKey(o => o.Id);
            builder.Entity<PromotionOptionValue>().ToTable("PromotionOptionValues");
            builder.Entity<PromotionOptionValue>()
                .HasOne(v => v.OptionType)
                .WithMany()
                .ForeignKey(t => t.IdOptionType)
                .PrincipalKey(v => v.Id);

            builder.Entity<PromotionOptionValue>().Ignore(d => d.BigValue);
            builder.Entity<PromotionOptionValue>().Ignore(d => d.IdBigString);

            builder.Entity<PromotionToBuySku>().HasKey(p => p.Id);
            builder.Entity<PromotionToBuySku>().ToTable("PromotionsToBuySkus");
            builder.Entity<PromotionToBuySku>().Ignore(p => p.ShortSkuInfo);

            builder.Entity<PromotionToGetSku>().HasKey(p => p.Id);
            builder.Entity<PromotionToGetSku>().ToTable("PromotionsToGetSkus");
            builder.Entity<PromotionToGetSku>().Ignore(p => p.ShortSkuInfo);

            builder.Entity<PromotionToSelectedCategory>().HasKey(p => p.Id);
            builder.Entity<PromotionToSelectedCategory>().ToTable("PromotionsToSelectedCategories");

            builder.Entity<Promotion>().HasKey(p => p.Id);
            builder.Entity<Promotion>().ToTable("Promotions");
            builder.Entity<Promotion>()
                .HasMany(p => p.OptionValues)
                .WithOne()
                .ForeignKey(o => o.IdPromotion)
                .PrincipalKey(p => p.Id)
                .Required(false);

            builder.Entity<Promotion>().Ignore(p => p.OptionTypes);

            builder.Entity<Promotion>()
                .HasMany(p => p.PromotionsToBuySkus)
                .WithOne()
                .ForeignKey(t => t.IdPromotion)
                .PrincipalKey(p => p.Id)
                .Required();
            builder.Entity<Promotion>()
                .HasMany(p => p.PromotionsToGetSkus)
                .WithOne()
                .ForeignKey(t => t.IdPromotion)
                .PrincipalKey(p => p.Id)
                .Required();
            builder.Entity<Promotion>()
                .HasMany(p => p.PromotionsToSelectedCategories)
                .WithOne()
                .ForeignKey(t => t.IdPromotion)
                .PrincipalKey(p => p.Id)
                .Required();
            builder.Entity<Promotion>()
                .HasOne(p => p.EditedBy)
                .WithMany()
                .ForeignKey(o => o.IdEditedBy)
                .PrincipalKey(p => p.Id)
                .Required(false);

			#endregion

			#region Products

			builder.Entity<VCustomerFavorite>().HasKey(x => x.Id);
			builder.Entity<VCustomerFavorite>().ToTable("VCustomerFavorites");

			builder.Entity<ProductCategory>().HasKey(p => p.Id);
            builder.Entity<ProductCategory>().ToTable("ProductCategories");
            builder.Entity<ProductCategory>()
                .HasMany(cat => cat.ProductToCategories)
                .WithOne()
                .ForeignKey(c => c.IdCategory)
                .PrincipalKey(cat => cat.Id);

            builder.Entity<InventoryCategory>().HasKey(p => p.Id);
            builder.Entity<InventoryCategory>().ToTable("InventoryCategories");

            builder.Entity<VProductSku>().HasKey(p => p.IdProduct);
            builder.Entity<VProductSku>().Ignore(x => x.Id);
            builder.Entity<VProductSku>().Ignore(x => x.EditedByAgentId);
            builder.Entity<VProductSku>().ToTable("VProductSkus");

            builder.Entity<VSku>().HasKey(p => new { p.IdProduct, p.SkuId });
            builder.Entity<VSku>().Ignore(x => x.Id);
            builder.Entity<VSku>().ToTable("VSkus");

            builder.Entity<VProductsWithReview>().HasKey(p => p.IdProduct);
            builder.Entity<VProductsWithReview>().Ignore(x => x.Id);
            builder.Entity<VProductsWithReview>().ToTable("VProductsWithReviews");

            builder.Entity<ProductOptionType>().HasKey(p => p.Id);
            builder.Entity<ProductOptionType>().ToTable("ProductOptionTypes");
            builder.Entity<ProductOptionType>()
                .HasOne(p => p.Lookup)
                .WithMany()
                .ForeignKey(p => p.IdLookup)
                .PrincipalKey(p => p.Id)
                .Required(false);

            builder.Entity<ProductOptionValue>().HasKey(o => o.Id);
            builder.Entity<ProductOptionValue>().ToTable("ProductOptionValues");
            builder.Entity<ProductOptionValue>().Property(v => v.IdProduct).IsRequired(false);
            builder.Entity<ProductOptionValue>().Property(v => v.IdSku).IsRequired(false);
            builder.Entity<ProductOptionValue>()
                .HasOne(v => v.OptionType)
                .WithMany()
                .ForeignKey(v => v.IdOptionType)
                .PrincipalKey(t => t.Id)
                .Required();
            builder.Entity<ProductOptionValue>()
                .HasOne(v => v.BigValue)
                .WithMany()
                .ForeignKey(v => v.IdBigString)
                .PrincipalKey(b => b.IdBigString)
                .Required(false);
            builder.Entity<ProductOptionValue>().Property(v => v.IdBigString).IsRequired(false);

            builder.Entity<ProductTypeEntity>().HasKey(t => t.Id);
            builder.Entity<ProductTypeEntity>().ToTable("ProductTypes");

            builder.Entity<Sku>().HasKey(s => s.Id);
            builder.Entity<Sku>().Ignore(p => p.IdObjectType);
            builder.Entity<Sku>().ToTable("Skus");
            builder.Entity<Sku>()
                .HasMany(s => s.OptionValues)
                .WithOne()
                .ForeignKey(o => o.IdSku).Required(false)
                .PrincipalKey(s => s.Id);
            builder.Entity<Sku>().Ignore(p => p.OptionTypes);
            builder.Entity<Sku>().Ignore(p => p.EditedBy);
            builder.Entity<Sku>().Ignore(p => p.IdEditedBy);

            builder.Entity<ProductToCategory>().HasKey(p => p.Id);
            builder.Entity<ProductToCategory>().ToTable("ProductsToCategories");

            builder.Entity<Product>().HasKey(p => p.Id);
            builder.Entity<Product>().ToTable("Products");

            builder.Entity<Product>()
                .HasMany(p => p.Skus)
                .WithOne(p => p.Product)
                .ForeignKey(s => s.IdProduct)
                .PrincipalKey(p => p.Id)
                .Required();
            builder.Entity<Product>()
                .HasMany(p => p.OptionValues)
                .WithOne()
                .ForeignKey(o => o.IdProduct).Required(false)
                .PrincipalKey(p => p.Id);
            builder.Entity<Product>()
                .HasOne(p => p.EditedBy)
                .WithMany()
                .ForeignKey(o => o.IdEditedBy)
                .PrincipalKey(p => p.Id)
                .Required(false);

            builder.Entity<Product>().Ignore(p => p.OptionTypes);

            builder.Entity<Product>()
                .HasMany(p => p.ProductsToCategories)
                .WithOne()
                .ForeignKey(t => t.IdProduct)
                .PrincipalKey(p => p.Id)
                .Required();

            builder.Entity<ProductReview>().HasKey(p => p.Id);
            builder.Entity<ProductReview>().ToTable("ProductReviews");

            builder.Entity<ProductReview>()
                .HasOne(p => p.Product)
                .WithMany()
                .ForeignKey(s => s.IdProduct)
                .PrincipalKey(p => p.Id)
                .Required();

            builder.Entity<ProductOutOfStockRequest>().HasKey(p => p.Id);
            builder.Entity<ProductOutOfStockRequest>().ToTable("ProductOutOfStockRequests");

            #endregion


            #region Lookups

            builder.Entity<Lookup>().HasKey(p => p.Id);
            builder.Entity<Lookup>().ToTable("Lookups");
            builder.Entity<LookupVariant>().HasKey(p => new { p.Id, p.IdLookup });
            builder.Entity<LookupVariant>().ToTable("LookupVariants");
            builder.Entity<Lookup>()
                .HasMany(p => p.LookupVariants)
                .WithOne(p => p.Lookup)
                .ForeignKey(p => p.IdLookup)
                .PrincipalKey(p => p.Id);

            #endregion


            #region Settings

            builder.Entity<Country>().HasKey(p => p.Id);
            builder.Entity<Country>().ToTable("Countries");
            builder.Entity<Country>().Ignore(p => p.States);

            builder.Entity<State>().HasKey(p => p.Id);
            builder.Entity<State>().ToTable("States");

            #endregion

            #region Users

            builder.Entity<User>().HasKey(p => p.Id);
            builder.Entity<User>().ToTable("Users");

			#endregion

			#region Customers

			builder.Entity<VCustomer>().HasKey(x => x.Id);
            builder.Entity<VCustomer>().ToTable("VCustomers");

            builder.Entity<Customer>().HasKey(p => p.Id);
            builder.Entity<Customer>().ToTable("Customers");
            builder.Entity<Customer>()
                .HasOne(p => p.EditedBy)
                .WithMany()
                .ForeignKey(p => p.IdEditedBy)
                .PrincipalKey(p => p.Id)
                .Required(false);
            builder.Entity<Customer>()
                .HasOne(p => p.User)
                .WithOne(u => u.Customer)
                .ForeignKey<Customer>(p => p.Id)
                .PrincipalKey<User>(p => p.Id)
                .Required();
            builder.Entity<Customer>()
                .HasMany(p => p.OptionValues)
                .WithOne()
                .ForeignKey(o => o.IdCustomer)
                .PrincipalKey(p => p.Id)
                .Required();
            builder.Entity<Customer>()
                .HasOne(p => p.CustomerType)
                .WithMany(p => p.Customers)
                .ForeignKey(p => p.IdObjectType)
                .PrincipalKey(p => p.Id)
                .Required();
            builder.Entity<Customer>()
                .HasMany(p => p.PaymentMethods)
                .WithOne(p => p.Customer)
                .ForeignKey(p => p.IdCustomer)
                .PrincipalKey(c => c.Id)
                .Required();
            builder.Entity<Customer>()
                .HasOne(p => p.DefaultPaymentMethod)
                .WithOne()
                .ForeignKey<Customer>(p => p.IdDefaultPaymentMethod)
                .PrincipalKey<PaymentMethod>(p => p.Id)
                .Required();
            builder.Entity<Customer>()
                .HasMany(p => p.OrderNotes)
                .WithOne(p => p.Customer)
                .ForeignKey(p => p.IdCustomer)
                .PrincipalKey(c => c.Id)
                .Required();
            builder.Entity<Customer>()
                .HasMany(p => p.Addresses)
                .WithOne()
                .ForeignKey(p => p.IdCustomer)
                .PrincipalKey(c => c.Id)
                .Required();
            builder.Entity<Customer>()
                .HasMany(p => p.CustomerPaymentMethods)
                .WithOne()
                .ForeignKey(p => p.IdCustomer)
                .PrincipalKey(c => c.Id)
                .Required();
            builder.Entity<Customer>()
                .HasMany(p => p.CustomerNotes)
                .WithOne()
                .ForeignKey(p => p.IdCustomer)
                .PrincipalKey(c => c.Id)
                .Required();
            builder.Entity<Customer>()
                .HasMany(p => p.Files)
                .WithOne()
                .ForeignKey(p => p.IdCustomer)
                .PrincipalKey(c => c.Id)
                .Required();

            builder.Entity<Customer>().Ignore(p => p.OptionTypes);

            builder.Entity<CustomerOptionType>().HasKey(p => p.Id);
            builder.Entity<CustomerOptionType>().ToTable("CustomerOptionTypes");
            builder.Entity<CustomerOptionType>()
                .HasOne(p => p.Lookup)
                .WithMany()
                .ForeignKey(p => p.IdLookup)
                .PrincipalKey(p => p.Id)
                .Required(false);
            builder.Entity<CustomerOptionValue>().HasKey(o => o.Id);
            builder.Entity<CustomerOptionValue>().ToTable("CustomerOptionValues");
            builder.Entity<CustomerOptionValue>()
                .HasOne(v => v.OptionType)
                .WithMany()
                .ForeignKey(t => t.IdOptionType)
                .PrincipalKey(v => v.Id)
                .Required();

            builder.Entity<CustomerFile>().HasKey(p => p.Id);
            builder.Entity<CustomerFile>().ToTable("CustomerFiles");

            builder.Entity<CustomerNote>().HasKey(p => p.Id);
            builder.Entity<CustomerNote>().ToTable("CustomerNotes");
            builder.Entity<CustomerNote>().Ignore(p => p.IdObjectType);
            builder.Entity<CustomerNote>()
                .HasOne(p => p.EditedBy)
                .WithMany()
                .ForeignKey(p => p.IdEditedBy)
                .PrincipalKey(p => p.Id)
                .Required(false);
            builder.Entity<CustomerNote>()
                .HasMany(n => n.OptionValues)
                .WithOne()
                .ForeignKey(o => o.IdCustomerNote)
                .PrincipalKey(n => n.Id)
                .Required();

            builder.Entity<CustomerNote>().Ignore(n => n.OptionTypes);

            builder.Entity<CustomerNoteOptionType>().HasKey(p => p.Id);
            builder.Entity<CustomerNoteOptionType>().ToTable("CustomerNoteOptionTypes");
            builder.Entity<CustomerNoteOptionType>().Ignore(p => p.IdObjectType);
            builder.Entity<CustomerNoteOptionType>()
                .HasOne(p => p.Lookup)
                .WithMany()
                .ForeignKey(p => p.IdLookup)
                .PrincipalKey(p => p.Id)
                .Required(false);
            builder.Entity<CustomerNoteOptionValue>().HasKey(o => o.Id);
            builder.Entity<CustomerNoteOptionValue>().ToTable("CustomerNoteOptionValues");
            builder.Entity<CustomerNoteOptionValue>()
                .HasOne(v => v.OptionType)
                .WithMany()
                .ForeignKey(t => t.IdOptionType)
                .PrincipalKey(v => v.Id)
                .Required();

            builder.Entity<CustomerNoteOptionValue>().Ignore(c => c.BigValue);
            builder.Entity<CustomerNoteOptionValue>().Ignore(c => c.IdBigString);

            builder.Entity<CustomerTypeEntity>().HasKey(p => p.Id);
            builder.Entity<CustomerTypeEntity>().ToTable("CustomerTypes");
            builder.Entity<CustomerTypeEntity>()
                .HasOne(p => p.EditedBy)
                .WithMany()
                .ForeignKey(p => p.IdEditedBy)
                .PrincipalKey(p => p.Id)
                .Required(false);
            builder.Entity<CustomerTypeEntity>().HasMany(p => p.PaymentMethods)
                .WithOne(p => p.CustomerType)
                .ForeignKey(p => p.IdCustomerType)
                .PrincipalKey(p => p.Id);
            builder.Entity<CustomerTypeEntity>().HasMany(p => p.OrderNotes)
                .WithOne(p => p.CustomerType)
                .ForeignKey(p => p.IdCustomerType)
                .PrincipalKey(p => p.Id);

            builder.Entity<CustomerToOrderNote>().Ignore(p => p.Id);
            builder.Entity<CustomerToOrderNote>().HasKey(p => new { p.IdCustomer, p.IdOrderNote });
            builder.Entity<CustomerToOrderNote>().ToTable("CustomersToOrderNotes");

            builder.Entity<CustomerToPaymentMethod>().Ignore(p => p.Id);
            builder.Entity<CustomerToPaymentMethod>().HasKey(p => new { p.IdCustomer, p.IdPaymentMethod });
            builder.Entity<CustomerToPaymentMethod>().ToTable("CustomersToPaymentMethods");

            #endregion

            #region Addresses

            builder.Entity<Address>().HasKey(p => p.Id);
            builder.Entity<Address>().ToTable("Addresses");
            builder.Entity<Address>()
                .HasOne(p => p.Сountry)
                .WithMany()
                .ForeignKey(p => p.IdCountry)
                .PrincipalKey(c => c.Id)
                .Required();
            builder.Entity<Address>()
                .HasOne(p => p.State)
                .WithMany()
                .ForeignKey(p => p.IdState)
                .PrincipalKey(s => s.Id)
                .Required(false);
            builder.Entity<Address>()
                .HasOne(p => p.EditedBy)
                .WithMany()
                .ForeignKey(p => p.IdEditedBy)
                .PrincipalKey(p => p.Id)
                .Required(false);
            builder.Entity<Address>()
                .HasMany(a => a.OptionValues)
                .WithOne()
                .ForeignKey(o => o.IdAddress)
                .PrincipalKey(a => a.Id)
                .Required();

            builder.Entity<Address>().Ignore(a => a.OptionTypes);

            builder.Entity<AddressOptionType>().HasKey(p => p.Id);
            builder.Entity<AddressOptionType>().ToTable("AddressOptionTypes");
            builder.Entity<AddressOptionType>()
                .HasOne(p => p.Lookup)
                .WithMany()
                .ForeignKey(p => p.IdLookup)
                .PrincipalKey(p => p.Id)
                .Required(false);
            builder.Entity<AddressOptionValue>().HasKey(o => o.Id);
            builder.Entity<AddressOptionValue>().ToTable("AddressOptionValues");
            builder.Entity<AddressOptionValue>()
                .HasOne(v => v.OptionType)
                .WithMany()
                .ForeignKey(t => t.IdOptionType)
                .PrincipalKey(v => v.Id)
                .Required();

            builder.Entity<AddressOptionValue>().Ignore(c => c.BigValue);
            builder.Entity<AddressOptionValue>().Ignore(c => c.IdBigString);

            builder.Entity<AddressTypeEntity>().HasKey(p => p.Id);
            builder.Entity<AddressTypeEntity>().ToTable("AddressTypes");

            #endregion

            #region Orders Notes
            builder.Entity<OrderNoteToCustomerType>().HasKey(p => new { p.IdOrderNote, p.IdCustomerType });
            builder.Entity<OrderNoteToCustomerType>().ToTable("OrderNotesToCustomerTypes");
            builder.Entity<OrderNoteToCustomerType>().Ignore(p => p.Id);

            //builder.Entity<OrderNoteToCustomerType>()
            //    .HasOne(n => n.CustomerType)
            //    .WithMany()
            //    .ForeignKey(n => n.IdCustomerType)
            //    .PrincipalKey(t => t.Id)
            //    .Required();

            builder.Entity<OrderNote>().HasKey(p => p.Id);
            builder.Entity<OrderNote>().ToTable("OrderNotes");
            builder.Entity<OrderNote>()
                .HasOne(p => p.EditedBy)
                .WithMany()
                .ForeignKey(p => p.IdEditedBy)
                .PrincipalKey(p => p.Id);
            builder.Entity<OrderNote>()
                .HasMany(p => p.Customers)
                .WithOne(p => p.OrderNote)
                .ForeignKey(p => p.IdOrderNote)
                .PrincipalKey(p => p.Id);
            builder.Entity<OrderNote>()
                .HasMany(p => p.CustomerTypes)
                .WithOne(p => p.OrderNote)
                .ForeignKey(p => p.IdOrderNote)
                .PrincipalKey(p => p.Id);

            #endregion

            #region Payment
            builder.Entity<PaymentMethodToCustomerType>().HasKey(p => new { p.IdPaymentMethod, p.IdCustomerType });
            builder.Entity<PaymentMethodToCustomerType>().ToTable("PaymentMethodsToCustomerTypes");
            builder.Entity<PaymentMethodToCustomerType>().Ignore(p => p.Id);

            builder.Entity<PaymentMethod>().HasKey(p => p.Id);
            builder.Entity<PaymentMethod>().ToTable("PaymentMethods");
            builder.Entity<PaymentMethod>().HasOne(p => p.EditedBy).WithMany().ForeignKey(p => p.IdEditedBy).PrincipalKey(p => p.Id)
                .Required(false);
            builder.Entity<PaymentMethod>()
                .HasMany(p => p.Customers)
                .WithOne(p => p.PaymentMethod)
                .ForeignKey(p => p.IdPaymentMethod)
                .PrincipalKey(p => p.Id);
            builder.Entity<PaymentMethod>()
                .HasMany(p => p.CustomerTypes)
                .WithOne(p => p.PaymentMethod)
                .ForeignKey(p => p.IdPaymentMethod)
                .PrincipalKey(p => p.Id);

            builder.Entity<CustomerPaymentMethod>().HasKey(p => p.Id);
            builder.Entity<CustomerPaymentMethod>().ToTable("CustomerPaymentMethods");
            builder.Entity<CustomerPaymentMethod>()
                .HasOne(p => p.EditedBy)
                .WithMany()
                .ForeignKey(p => p.IdEditedBy)
                .PrincipalKey(p => p.Id)
                .Required(false);
            builder.Entity<CustomerPaymentMethod>()
                .HasOne(p => p.PaymentMethod)
                .WithMany()
                .ForeignKey(p => p.IdObjectType)
                .PrincipalKey(p => p.Id)
                .Required();
            builder.Entity<CustomerPaymentMethod>()
                .HasOne(p => p.BillingAddress)
                .WithOne()
                .ForeignKey<CustomerPaymentMethod>(p => p.IdAddress)
                .PrincipalKey<Address>(p => p.Id)
                .Required(false);
            builder.Entity<CustomerPaymentMethod>()
                .HasMany(p => p.OptionValues)
                .WithOne()
                .ForeignKey(v => v.IdCustomerPaymentMethod)
                .PrincipalKey(p => p.Id)
                .Required();

            builder.Entity<CustomerPaymentMethod>().Ignore(a => a.OptionTypes);

            builder.Entity<CustomerPaymentMethodOptionType>().HasKey(p => p.Id);
            builder.Entity<CustomerPaymentMethodOptionType>().ToTable("CustomerPaymentMethodOptionTypes");
            builder.Entity<CustomerPaymentMethodOptionType>()
                .HasOne(p => p.Lookup)
                .WithMany()
                .ForeignKey(p => p.IdLookup)
                .PrincipalKey(p => p.Id)
                .Required(false);
            builder.Entity<CustomerPaymentMethodOptionValue>().HasKey(o => o.Id);
            builder.Entity<CustomerPaymentMethodOptionValue>().ToTable("CustomerPaymentMethodValues");
            builder.Entity<CustomerPaymentMethodOptionValue>()
                .HasOne(v => v.OptionType)
                .WithMany()
                .ForeignKey(t => t.IdOptionType)
                .PrincipalKey(v => v.Id)
                .Required();
            builder.Entity<CustomerPaymentMethodOptionValue>().Ignore(v => v.BigValue);
            builder.Entity<CustomerPaymentMethodOptionValue>().Ignore(v => v.IdBigString);

            #endregion

            #region Orders

            builder.Entity<VOrder>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.ToTable("VOrders");
                entity.Ignore(t => t.EditedByAgentId);
            });

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
                    .ForeignKey(o => o.IdCustomer)
                    .PrincipalKey(c => c.Id)
                    .Required();
                entity.HasMany(o => o.Skus)
                    .WithOne(a => a.Order)
                    .ForeignKey(s => s.IdOrder)
                    .PrincipalKey(o => o.Id)
                    .Required();
                entity.HasMany(o => o.GiftCertificates)
                    .WithOne(a => a.Order)
                    .ForeignKey(g => g.IdOrder)
                    .PrincipalKey(o => o.Id)
                    .Required();
                entity.HasOne(o => o.Discount)
                    .WithMany()
                    .ForeignKey(o => o.IdDiscount)
                    .PrincipalKey(d => d.Id)
                    .Required(false);
                entity.HasOne(o => o.PaymentMethod)
                    .WithOne()
                    .ForeignKey<Order>(o => o.IdPaymentMethod)
                    .PrincipalKey<OrderPaymentMethod>(p => p.Id)
                    .Required(false);
                entity.HasMany(o => o.OptionValues)
                    .WithOne()
                    .ForeignKey(v => v.IdOrder)
                    .PrincipalKey(o => o.Id)
                    .Required();
                entity.HasOne(o => o.ShippingAddress)
                    .WithOne()
                    .ForeignKey<Order>(o => o.IdShippingAddress)
                    .PrincipalKey<OrderAddress>(a => a.Id)
                    .Required(false);
                entity.HasOne(p => p.EditedBy)
                    .WithMany()
                    .ForeignKey(p => p.IdEditedBy)
                    .PrincipalKey(p => p.Id)
                    .Required(false);
                entity.Ignore(o => o.OptionTypes);
            });

            builder.Entity<OrderOptionValue>(entity =>
            {
                entity.HasKey(o => o.Id);
                entity.ToTable("OrderOptionValues");
                entity.HasOne(v => v.OptionType)
                    .WithOne()
                    .ForeignKey<OrderOptionValue>(v => v.IdOptionType)
                    .PrincipalKey<OrderOptionType>(t => t.Id)
                    .Required();
                entity.Ignore(v => v.BigValue);
                entity.Ignore(v => v.IdBigString);
            });

            builder.Entity<OrderOptionType>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.ToTable("OrderOptionTypes");
                entity.HasOne(t => t.Lookup)
                    .WithOne()
                    .ForeignKey<OrderOptionType>(t => t.IdLookup)
                    .PrincipalKey<Lookup>(l => l.Id)
                    .Required(false);
            });

            builder.Entity<OrderToGiftCertificate>(entity =>
            {
                entity.Ignore(s => s.Id);
                entity.HasKey(s => new { s.IdOrder, s.IdGiftCertificate });
                entity.ToTable("OrderToGiftCertificates");
                entity.HasOne(g => g.GiftCertificate)
                    .WithOne()
                    .ForeignKey<OrderToGiftCertificate>(g => g.IdGiftCertificate)
                    .PrincipalKey<GiftCertificate>(g => g.Id);
                entity.HasOne(g => g.Order)
                    .WithMany(o => o.GiftCertificates)
                    .ForeignKey(g => g.IdOrder)
                    .PrincipalKey(o => o.Id);
            });

            builder.Entity<OrderToSku>(entity =>
            {
                entity.Ignore(s => s.Id);
                entity.HasKey(s => new { s.IdOrder, s.IdSku });
                entity.ToTable("OrderToSkus");
                entity.HasOne(s => s.Order)
                    .WithMany(o => o.Skus)
                    .ForeignKey(s => s.IdOrder)
                    .PrincipalKey(o => o.Id);
                entity.HasOne(s => s.Sku)
                    .WithOne()
                    .ForeignKey<OrderToSku>(s => s.IdSku)
                    .PrincipalKey<Sku>(s => s.Id);
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
                    .ForeignKey(o => o.IdAddress)
                    .PrincipalKey(c => c.Id)
                    .Required(false);
                entity.HasOne(o => o.PaymentMethod)
                    .WithOne()
                    .ForeignKey<OrderPaymentMethod>(s => s.IdObjectType)
                    .PrincipalKey<PaymentMethod>(o => o.Id)
                    .Required();
                entity.HasMany(o => o.OptionValues)
                    .WithOne()
                    .ForeignKey(g => g.IdOrderPaymentMethod)
                    .PrincipalKey(o => o.Id)
                    .Required();
                entity.HasOne(p => p.EditedBy)
                    .WithMany()
                    .ForeignKey(p => p.IdEditedBy)
                    .PrincipalKey(p => p.Id)
                    .Required(false);
                entity.Ignore(o => o.OptionTypes);
            });

            builder.Entity<OrderPaymentMethodOptionValue>(entity =>
            {
                entity.HasKey(o => o.Id);
                entity.ToTable("OrderPaymentMethodOptionValues");
                entity.HasOne(v => v.OptionType)
                    .WithOne()
                    .ForeignKey<OrderPaymentMethodOptionValue>(v => v.IdOptionType)
                    .PrincipalKey<CustomerPaymentMethodOptionType>(t => t.Id)
                    .Required();
                entity.Ignore(v => v.BigValue);
                entity.Ignore(v => v.IdBigString);
            });

            builder.Entity<OrderAddress>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("OrderAddresses");
                entity.HasOne(p => p.Сountry)
                    .WithMany()
                    .ForeignKey(p => p.IdCountry)
                    .PrincipalKey(c => c.Id)
                    .Required();
                entity.HasOne(p => p.State)
                    .WithMany()
                    .ForeignKey(p => p.IdState)
                    .PrincipalKey(s => s.Id)
                    .Required(false);
                entity.HasOne(p => p.EditedBy)
                    .WithMany()
                    .ForeignKey(p => p.IdEditedBy)
                    .PrincipalKey(p => p.Id)
                    .Required(false);
                entity.HasMany(a => a.OptionValues)
                    .WithOne()
                    .ForeignKey(o => o.IdOrderAddress)
                    .PrincipalKey(a => a.Id)
                    .Required();
                entity.Ignore(a => a.OptionTypes);
            });

            builder.Entity<OrderAddressOptionValue>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.ToTable("OrderAddressOptionValues");
                entity.HasOne(v => v.OptionType)
                    .WithMany()
                    .ForeignKey(t => t.IdOptionType)
                    .PrincipalKey(v => v.Id)
                    .Required();
                entity.Ignore(v => v.BigValue);
                entity.Ignore(v => v.IdBigString);
            });

            builder.Entity<RefundSku>(entity =>
            {
                entity.HasKey(r => new { r.IdOrder, r.IdSku });
                entity.ToTable("RefundSkus");
                entity.HasOne(r => r.Order)
                    .WithMany()
                    .ForeignKey(r => r.IdOrder)
                    .PrincipalKey(o => o.Id)
                    .Required();
                entity.HasOne(r => r.Sku)
                    .WithMany()
                    .ForeignKey(r => r.IdSku)
                    .PrincipalKey(s => s.Id)
                    .Required();
            });

            builder.Entity<ReshipProblemSku>(entity =>
            {
                entity.HasKey(r => new { r.IdOrder, r.IdSku });
                entity.ToTable("ReshipProblemSkus");
                entity.HasOne(r => r.Order)
                    .WithMany()
                    .ForeignKey(r => r.IdOrder)
                    .PrincipalKey(o => o.Id)
                    .Required();
                entity.HasOne(r => r.Sku)
                    .WithMany()
                    .ForeignKey(r => r.IdSku)
                    .PrincipalKey(s => s.Id)
                    .Required();
            });

            #endregion

            #region Affiliates

            builder.Entity<VAffiliate>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.ToTable("VAffiliates");
                entity.Ignore(x => x.EditedByAgentId);
            });

            builder.Entity<AffiliateOptionValue>(entity =>
            {
                entity.HasKey(o => o.Id);
                entity.ToTable("AffiliateOptionValues");
                entity.HasOne(v => v.OptionType)
                    .WithMany()
                    .ForeignKey(t => t.IdOptionType)
                    .PrincipalKey(v => v.Id)
                    .Required();
                entity.HasOne(v => v.BigValue)
                    .WithMany()
                    .ForeignKey(v => v.IdBigString)
                    .PrincipalKey(b => b.IdBigString)
                    .Required(false);
                entity.Property(v => v.IdBigString).IsRequired(false);
            });

            builder.Entity<AffiliateOptionType>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.ToTable("AffiliateOptionTypes");
                entity.HasOne(p => p.Lookup)
                    .WithMany()
                    .ForeignKey(p => p.IdLookup)
                    .PrincipalKey(p => p.Id)
                    .Required(false);
            });

            builder.Entity<Affiliate>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.ToTable("Affiliates");
                entity.HasOne(p => p.User)
                    .WithMany()
                    .ForeignKey(p => p.Id)
                    .PrincipalKey(p => p.Id)
                    .Required();
                entity.HasMany(p => p.OptionValues)
                    .WithOne()
                    .ForeignKey(o => o.IdAffiliate)
                    .PrincipalKey(p => p.Id)
                    .Required();
                entity.Ignore(p => p.OptionTypes);
                entity.HasOne(p => p.EditedBy)
                    .WithMany()
                    .ForeignKey(o => o.IdEditedBy)
                    .PrincipalKey(p => p.Id)
                    .Required(false);
                entity.Ignore(p => p.IdObjectType);
            });

            builder.Entity<AffiliateOrderPayment>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.ToTable("AffiliateOrderPayments");
                entity.HasOne(p => p.Order)
                    .WithOne(p => p.AffiliateOrderPayment)
                    .ForeignKey<AffiliateOrderPayment>(p=>p.Id)
                    .PrincipalKey<Order>(p => p.Id)
                    .Required();
            });

            builder.Entity<AffiliatePayment>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.ToTable("AffiliatePayments");
                entity.HasMany(p => p.OrderPayments)
                    .WithOne()
                    .ForeignKey(o => o.IdAffiliatePayment)
                    .PrincipalKey(p => p.Id)
                    .Required(false);
            });

            builder.Entity<VCustomerInAffiliate>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.ToTable("VCustomersInAffiliates");
            });

            #endregion

            #region Help

            builder.Entity<HelpTicket>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.ToTable("HelpTickets");
                entity.HasOne(p => p.Order)
                    .WithMany()
                    .ForeignKey(p => p.IdOrder)
                    .PrincipalKey(p => p.Id)
                    .Required();
                entity.HasMany(p => p.Comments)
                    .WithOne(p => p.HelpTicket)
                    .ForeignKey(p => p.IdHelpTicket)
                    .PrincipalKey(p => p.Id)
                    .Required();
                entity.Ignore(p => p.IdCustomer);
                entity.Ignore(p => p.Customer);
                entity.Ignore(p => p.CustomerEmail);
            });

            builder.Entity<HelpTicketComment>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.ToTable("HelpTicketComments");
                entity.Ignore(p => p.EditedBy);
            });

            builder.Entity<VHelpTicket>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.ToTable("VHelpTickets");
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
                    .ForeignKey(p => p.IdObjectHistoryLogDataItem)
                    .PrincipalKey(p => p.IdObjectHistoryLogDataItem)
                    .Required(false);
            });

            #endregion

            base.OnModelCreating(builder);
        }
    }
}
