using Microsoft.Data.Entity;
using System.Data.SqlClient;
using Microsoft.Extensions.OptionsModel;
using VitalChoice.Ecommerce.Context;
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

namespace VitalChoice.Infrastructure.Context
{
    public class EcommerceContext : EccomerceContextBase
    {
        public EcommerceContext(IOptions<AppOptionsBase> options): base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<CountModel>(entity =>
            {
                entity.HasKey(f => f.Id);
            });

            builder.Entity<VCustomerFavorite>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.ToTable("VCustomerFavorites");
            });

            builder.Entity<VProductSku>(entity =>
            {
                entity.HasKey(p => p.IdProduct);
                entity.Ignore(x => x.Id);
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

            builder.Entity<HealthwiseOrder>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.ToTable("HealthwiseOrders");
                entity.HasOne(p => p.Order)
                    .WithOne()
                    .HasForeignKey<HealthwiseOrder>(p => p.Id)
                    .HasPrincipalKey<Order>(p => p.Id)
                    .IsRequired();
            });

            builder.Entity<HealthwisePeriod>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.ToTable("HealthwisePeriods");
                entity.HasMany(p => p.HealthwiseOrders)
                    .WithOne(p=>p.HealthwisePeriod)
                    .HasForeignKey(o => o.IdHealthwisePeriod)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired();
                entity.HasOne(p => p.Customer)
                    .WithMany()
                    .HasForeignKey(o => o.IdCustomer)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired();
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
        }
    }
}