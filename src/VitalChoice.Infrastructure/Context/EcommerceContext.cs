using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using VitalChoice.Caching.Extensions;
using VitalChoice.Ecommerce.Context;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Options;
using VitalChoice.Infrastructure.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Entities.Help;
using VitalChoice.Infrastructure.Domain.Transfer.Affiliates;
using VitalChoice.Infrastructure.Domain.Transfer.Customers;
using VitalChoice.Infrastructure.Domain.Transfer.Help;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Infrastructure.Domain.Entities.Healthwise;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Entities.CatalogRequests;
using VitalChoice.Infrastructure.Domain.Entities.Checkout;
using VitalChoice.Infrastructure.Domain.Entities.Customers;
using VitalChoice.Infrastructure.Domain.Entities.InventorySkus;
using VitalChoice.Infrastructure.Domain.Entities.Newsletters;
using VitalChoice.Infrastructure.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Entities.Products;
using VitalChoice.Infrastructure.Domain.Entities.Reports;

namespace VitalChoice.Infrastructure.Context
{
    public class EcommerceContext : EcommerceContextBase
    {
        public EcommerceContext(IOptions<AppOptionsBase> options, DbContextOptions<EcommerceContext> dbContextOptions): base(options, dbContextOptions)
        {
        }

        protected override void Carts(ModelBuilder builder)
        {
            builder.Entity<CartExtended>(entity =>
            {
                entity.ToTable("Carts");
                entity.HasKey(c => c.Id);
                entity.CacheUniqueIndex(c => c.CartUid);
                entity.HasMany(c => c.GiftCertificates).WithOne().HasForeignKey(g => g.IdCart).HasPrincipalKey(c => c.Id);
                entity.HasMany(c => c.Skus).WithOne().HasForeignKey(s => s.IdCart).HasPrincipalKey(c => c.Id);
            });
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            #region SPs

            builder.Entity<CountModel>(entity =>
            {
                entity.HasKey(f => f.Id);
            });

            builder.Entity<IdModel>(entity =>
            {
                entity.HasKey(f => f.Id);
            });

            builder.Entity<WholesaleDropShipReportSkuRawItem>(entity =>
            {
                entity.HasKey(f => f.Id);
            });

            builder.Entity<TransactionAndRefundRawItem>(entity =>
            {
                entity.HasKey(f => f.RowNumber);
                entity.Ignore(f => f.Id);
            });

            builder.Entity<OrdersSummarySalesOrderTypeStatisticItem>(entity =>
            {
                entity.HasKey(f => f.Id);
                entity.Ignore(f => f.Name);
                entity.Ignore(f => f.Average);
            });

            builder.Entity<OrdersSummarySalesOrderItem>(entity =>
            {
                entity.HasKey(f => f.Id);
                entity.Ignore(f => f.SourceName);
            });

            builder.Entity<CustomerOrdersTotal>(entity =>
            {
                entity.HasKey(f => f.Id);
            });

            builder.Entity<CustomerLastOrder>(entity =>
            {
                entity.HasKey(f => f.Id);
            });

            builder.Entity<SkuBreakDownReportRawItem>(entity =>
            {
                entity.HasKey(f => new {f.IdSku, f.CustomerIdObjectType});
                entity.Ignore(f => f.Id);
            });

            builder.Entity<SkuPOrderTypeBreakDownReportRawItem>(entity =>
            {
                entity.HasKey(f => f.RowNumber);
                entity.Ignore(f => f.Id);
            });

            builder.Entity<SkuPOrderTypeFutureBreakDownReportRawItem>(entity =>
            {
                entity.HasKey(f => f.RowNumber);
                entity.Ignore(f => f.Id);
            });

            builder.Entity<MailingReportItem>(entity =>
            {
                entity.HasKey(f => f.Id);
                entity.Ignore(f => f.CustomerOrderSource);
                entity.Ignore(f => f.CountryCode);
                entity.Ignore(f => f.StateCode);
            });

            builder.Entity<ShippedViaSummaryReportRawItem>(entity =>
            {
                entity.HasKey(f => new {f.IdWarehouse,f.IdShipMethodFreightService, f.ShipMethodFreightCarrier });
                entity.Ignore(f => f.Id);
            });

            builder.Entity<ShippedViaReportRawOrderItem>(entity =>
            {
                entity.HasKey(f => f.Id);
                entity.Ignore(f => f.ServiceCodeName);
                entity.Ignore(f => f.WarehouseName);
                entity.Ignore(f => f.ShipMethodFreightServiceName);
                entity.Ignore(f => f.StateCode);
            });

            builder.Entity<ProductQualitySalesReportItem>(entity =>
            {
                entity.HasKey(f => f.Id);
                entity.Ignore(f => f.SalesPerIssue);
            });

            builder.Entity<ProductQualitySkusReportItem>(entity =>
            {
                entity.HasKey(f => f.Id);
            });

            builder.Entity<KPIReportDBSaleRawItem>(entity =>
            {
                entity.HasKey(f => f.Id);
                entity.NonCached();
            });

            builder.Entity<ShortOrderItemModel>(entity =>
            {
                entity.HasKey(f => f.Id);
            });

            builder.Entity<AAFESReportItem>(entity =>
            {
                entity.Ignore(f => f.Id);
                entity.Ignore(f => f.ServiceUrl);
                entity.HasKey(f => f.RowNumber);
            });

            builder.Entity<CustomerSkuUsageReportRawItem>(entity =>
            {
                entity.Ignore(f => f.Id);
                entity.Ignore(f => f.CustomerType);
                entity.Ignore(f => f.CategoryNames);
                entity.Ignore(f => f.ShippingCountryCode);
                entity.Ignore(f => f.ShippingStateCode);
                entity.HasKey(f => new { f.IdCustomer, f.IdSku});
            });

            #endregion

            builder.Entity<VCustomerFavorite>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.ToTable("VCustomerFavorites");
            });

            builder.Entity<VProductSku>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Ignore(x => x.EditedByAgentId);
                entity.ToTable("VProductSkus");
            });

            builder.Entity<VSku>(entity =>
            {
                entity.HasKey(p => new { p.IdProduct, p.SkuId });
                entity.Ignore(x => x.Id);
                entity.ToTable("VSkus");
            });

            builder.Entity<VProductsWithReview>(entity =>
            {
                entity.HasKey(p => p.IdProduct);
                entity.Ignore(x => x.Id);
                entity.ToTable("VProductsWithReviews");
            });

            builder.Entity<ProductCategoryStatisticItem>(entity =>
            {
                entity.HasKey(p => p.Id);
            });

            builder.Entity<SkusInProductCategoryStatisticItem>(entity =>
            {
                entity.HasKey(p => p.Code);
                entity.Ignore(p => p.Id);
            });

            builder.Entity<VCustomer>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.ToTable("VCustomers");
            });

            builder.Entity<VOrder>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.ToTable("VOrders");
                entity.Ignore(t => t.EditedByAgentId);
            });

			builder.Entity<VAutoShip>(entity =>
			{
				entity.HasKey(t => t.Id);
				entity.ToTable("VAutoShips");
			});

			builder.Entity<VAutoShipOrder>(entity =>
			{
				entity.HasKey(t => t.Id);
				entity.ToTable("VAutoShipOrders");
			});

			builder.Entity<VOrderWithRegionInfoItem>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.ToTable("VOrderWithRegionInfoItems");
                entity.Ignore(p => p.CustomerFirstName);
                entity.Ignore(p => p.CustomerLastName);
                entity.Ignore(p => p.CustomerOrdersCount);
            });

            builder.Entity<OrdersRegionStatisticItem>(entity =>
            {
                entity.HasKey(p => p.Region);
                entity.Ignore(p => p.Id);
            });

            builder.Entity<InventorySkuUsageRawReportItem>(entity =>
            {
                entity.HasKey(p => p.Id);
            });

            builder.Entity<InventoriesSummaryUsageRawReportItem>(entity =>
            {
                entity.HasKey(p => new {p.IdInventorySku, p.Date });
                entity.Ignore(p => p.Id);
            });

            builder.Entity<OrdersZipStatisticItem>(entity =>
            {
                entity.HasKey(p => p.Zip);
                entity.Ignore(p => p.Id);
            });

            builder.Entity<VAffiliate>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.ToTable("VAffiliates");
                entity.Ignore(x => x.EditedByAgentId);
                entity.HasOne(v => v.NotPaidCommission)
                    .WithOne(p => p.Affiliate)
                    .HasForeignKey<VAffiliate>(p => p.Id)
                    .HasPrincipalKey<VAffiliateNotPaidCommission>(p => p.Id)
                    .IsRequired();
            });

            builder.Entity<VAffiliateNotPaidCommission>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.NonCached();
                entity.ToTable("VAffiliateNotPaidCommissions");
            });

            builder.Entity<VCustomerInAffiliate>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.ToTable("VCustomersInAffiliates");
            });

            builder.Entity<AffiliateSummaryReportModel>(entity =>
            {
                entity.HasKey(p => new { p.From, p.IdType });
            });

            builder.Entity<HelpTicket>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.ToTable("HelpTickets");
                entity.HasOne(p => p.Order)
                    .WithMany()
                    .HasForeignKey(p => p.IdOrder)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired();
                entity.HasMany(p => p.Comments)
                    .WithOne(p => p.HelpTicket)
                    .HasForeignKey(p => p.IdHelpTicket)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired();
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

            builder.Entity<VHealthwisePeriod>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.ToTable("VHealthwisePeriods");
            });

            builder.Entity<CatalogRequestAddress>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("CatalogRequestAddresses");
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
                entity.HasMany(a => a.OptionValues)
                    .WithOne()
                    .HasForeignKey(o => o.IdCatalogRequestAddress)
                    .HasPrincipalKey(a => a.Id)
                    .IsRequired();
                entity.Ignore(a => a.OptionTypes);
                entity.Ignore(a => a.IdEditedBy);
                entity.Ignore(a => a.EditedBy);
            });

            builder.Entity<CatalogRequestAddressOptionValue>(entity =>
            {
                entity.HasKey(o => new { o.IdCatalogRequestAddress, o.IdOptionType });
                entity.Ignore(o => o.Id);
                entity.ToTable("CatalogRequestAddressOptionValues");
                entity.HasOne(v => v.OptionType)
                    .WithMany()
                    .HasForeignKey(t => t.IdOptionType)
                    .HasPrincipalKey(v => v.Id)
                    .IsRequired();
                entity.Ignore(v => v.BigValue);
                entity.Ignore(v => v.IdBigString);
            });

            builder.Entity<VTopProducts>(entity =>
            {
                entity.Ignore(e => e.Id);

                entity.HasKey(e => new {e.IdSku, e.IdCustomer});
                entity.HasOne(e => e.Sku).WithMany().HasForeignKey(e => e.IdSku).HasPrincipalKey(s => s.Id);
            });

            builder.Entity<OneTimeDiscountToCustomerUsage>(entity =>
            {
                entity.Ignore(e => e.Id);

                entity.ToTable("OneTimeDiscountToCustomerUsages");

                entity.HasKey(e => new {e.IdCustomer, e.IdDiscount});
            });

            builder.Entity<Newsletter>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("Newsletters");
                entity.HasMany(p => p.BlockedEmails)
                    .WithOne()
                    .HasForeignKey(e => e.IdNewsletter)
                    .HasPrincipalKey(s => s.Id);
            });

            builder.Entity<NewsletterBlockedEmail>(entity =>
            {
                entity.HasKey(p => new { p.IdNewsletter, p.Email });
                entity.ToTable("NewsletterBlockedEmails");
                entity.Ignore(p => p.Id);
            });

            builder.Entity<VWholesaleSummaryInfo>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("VWholesaleSummaryInfo");
            });

            builder.Entity<KPICacheItem>(entity =>
            {
                entity.HasKey(f => f.Id);
                entity.ToTable("KPICacheItems");
            });

            builder.Entity<VOrderCountOnCustomer>(entity =>
            {
                entity.HasKey(f => f.IdCustomer);
                entity.Ignore(f => f.Id);
                entity.ToTable("VOrderCountOnCustomers");
            });

            builder.Entity<VCustomerWithDublicateEmail>(entity =>
            {
                entity.HasKey(f => f.Email);
                entity.Ignore(f => f.Id);
                entity.ToTable("VCustomersWithDublicateEmails");
                entity.NonCached();
            });
        }
    }
}