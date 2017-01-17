using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VitalChoice.Business.Queries.Orders;
using VitalChoice.Business.Queries.Users;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.Transaction;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Interfaces.Services.Settings;
using System.Collections.Generic;
using VitalChoice.Business.Mailings;
using VitalChoice.Business.Repositories;
using VitalChoice.Business.Services.Dynamic;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Services;
using VitalChoice.DynamicData.Helpers;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities.Settings;
using VitalChoice.Infrastructure.Domain.Transfer.Customers;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Payments;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Services.Orders
{
    public class MultipleShipmentsOrderService : OrderService
    {
        public MultipleShipmentsOrderService(
            IEcommerceRepositoryAsync<VOrderWithRegionInfoItem> vOrderWithRegionInfoItemRepository,
            OrderRepository orderRepository,
            IEcommerceRepositoryAsync<BigStringValue> bigStringValueRepository,
            OrderMapper mapper,
            IObjectLogItemExternalService objectLogItemExternalService,
            IEcommerceRepositoryAsync<OrderOptionValue> orderValueRepositoryAsync,
            IRepositoryAsync<AdminProfile> adminProfileRepository,
            ProductMapper productMapper,
            CustomerMapper customerMapper,
            ICustomerService customerService, IWorkflowFactory treeFactory,
            ILoggerFactory loggerProvider,
            IEcommerceRepositoryAsync<VCustomer> vCustomerRepositoryAsync,
            DynamicExtensionsRewriter queryVisitor,
            IEncryptedOrderExportService encryptedOrderExportService,
            SpEcommerceRepository sPEcommerceRepository,
            IPaymentMethodService paymentMethodService,
            IEcommerceRepositoryAsync<OrderToGiftCertificate> orderToGiftCertificateRepositoryAsync,
            IExtendedDynamicServiceAsync
                <OrderPaymentMethodDynamic, OrderPaymentMethod, CustomerPaymentMethodOptionType, OrderPaymentMethodOptionValue>
                paymentGenericService,
            IDynamicMapper<AddressDynamic, OrderAddress> addressMapper,
            IProductService productService,
            INotificationService notificationService,
            ICountryNameCodeResolver countryService, ITransactionAccessor<EcommerceContext> transactionAccessor,
            SkuMapper skuMapper,
            IEcommerceRepositoryAsync<OrderToSku> orderToSkusRepository, 
            IDiscountService discountService,
            IEcommerceRepositoryAsync<VAutoShip> vAutoShipRepository,
            IEcommerceRepositoryAsync<VAutoShipOrder> vAutoShipOrderRepository,
            AffiliateOrderPaymentRepository affiliateOrderPaymentRepository,
            ICountryNameCodeResolver codeResolver,
            IDynamicEntityOrderingExtension<Order> orderingExtension, 
            ReferenceData referenceData, AppSettings appSettings,
            IAdminEditLockService lockService)
            : base(
                vOrderWithRegionInfoItemRepository,
                orderRepository,
                bigStringValueRepository,
                mapper,
                objectLogItemExternalService,
                orderValueRepositoryAsync,
                adminProfileRepository,
                productMapper,
                customerMapper,
                customerService, 
                treeFactory,
                loggerProvider,
                vCustomerRepositoryAsync,
                queryVisitor,
                encryptedOrderExportService,
                sPEcommerceRepository,
                paymentMethodService,
                orderToGiftCertificateRepositoryAsync,
                paymentGenericService,
                addressMapper,
                productService,
                notificationService,
                countryService,
                transactionAccessor,
                skuMapper,
                orderToSkusRepository,
                discountService,
                vAutoShipRepository,
                vAutoShipOrderRepository,
                affiliateOrderPaymentRepository,
                codeResolver,
                orderingExtension,
                referenceData,
                appSettings,
                lockService)
        {
        }

        protected override IQueryLite<Order> BuildIncludes(IQueryLite<Order> query)
        {
            return
                query.Include(o => o.Discount)
                    .ThenInclude(d => d.OptionValues)
                    .Include(o => o.Discount)
                    .ThenInclude(d => d.DiscountTiers)
                    .Include(o => o.Discount)
                    .ThenInclude(p => p.DiscountsToSelectedSkus)
                    .Include(o => o.Discount)
                    .ThenInclude(p => p.DiscountsToSkus)
                    .Include(o => o.Discount)
                    .ThenInclude(p => p.DiscountsToCategories)
                    .Include(o => o.Discount)
                    .ThenInclude(p => p.DiscountsToSelectedCategories)
                    .Include(o => o.GiftCertificates)
                    .ThenInclude(g => g.GiftCertificate)
                    .Include(o => o.PromoSkus)
                    .ThenInclude(p => p.Sku)
                    .ThenInclude(s => s.OptionValues)
                    .Include(o => o.PromoSkus)
                    .ThenInclude(p => p.Sku)
                    .ThenInclude(s => s.Product)
                    .ThenInclude(p => p.OptionValues)
                    .Include(o => o.PromoSkus)
                    .ThenInclude(p => p.Promo)
                    .ThenInclude(s => s.OptionValues)
                    .Include(o => o.PromoSkus)
                    .ThenInclude(s => s.InventorySkus)
                    .Include(o => o.PaymentMethod)
                    .ThenInclude(p => p.BillingAddress)
                    .ThenInclude(a => a.OptionValues)
                    .Include(o => o.PaymentMethod)
                    .ThenInclude(p => p.BillingAddress)
                    .Include(o => o.PaymentMethod)
                    .ThenInclude(p => p.BillingAddress)
                    .Include(o => o.PaymentMethod)
                    .ThenInclude(p => p.OptionValues)
                    .Include(o => o.PaymentMethod)
                    .ThenInclude(p => p.PaymentMethod)
                    .Include(o => o.ShippingAddress)
                    .ThenInclude(s => s.OptionValues)
                    .Include(o => o.Skus)
                    .ThenInclude(s => s.Sku)
                    .ThenInclude(s => s.OptionValues)
                    .Include(o => o.Skus)
                    .ThenInclude(s => s.Sku)
                    .ThenInclude(s => s.Product)
                    .ThenInclude(s => s.OptionValues)
                    .Include(o => o.Skus)
                    .ThenInclude(s => s.Sku)
                    .ThenInclude(s => s.Product)
                    .ThenInclude(s => s.ProductsToCategories)
                    .Include(o => o.Skus)
                    .ThenInclude(s => s.InventorySkus)
                    .Include(o => o.Skus)
                    .ThenInclude(s => s.GeneratedGiftCertificates)
                    .Include(o => o.HealthwiseOrder)
                    .Include(o => o.ReshipProblemSkus)
                    .ThenInclude(g => g.Sku)
                    .Include(o => o.OrderShippingPackages)
                    .Include(p => p.CartAdditionalShipments)
                    .ThenInclude(p => p.ShippingAddress)
                    .ThenInclude(s => s.OptionValues)
                    .Include(p => p.CartAdditionalShipments)
                    .ThenInclude(o => o.Skus)
                    .ThenInclude(s => s.Sku)
                    .ThenInclude(s => s.OptionValues)
                    .Include(p => p.CartAdditionalShipments)
                    .ThenInclude(o => o.Skus)
                    .ThenInclude(s => s.Sku)
                    .ThenInclude(s => s.Product)
                    .ThenInclude(s => s.OptionValues);
        }
    }
}