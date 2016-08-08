using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Business.Repositories;
using VitalChoice.Business.Services.Dynamic;
using VitalChoice.Business.Services.Ecommerce;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.Services;
using VitalChoice.DynamicData.Helpers;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Workflow.Core;
using VitalChoice.ObjectMapping.Base;
using System.Linq.Expressions;
using VitalChoice.Data.Transaction;
using VitalChoice.Infrastructure.Context;
using Microsoft.Extensions.Logging;
using VitalChoice.Data.UOW;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Interfaces.Services.Products;

namespace VitalChoice.Business.Services.Orders
{
    public class OrderRefundService : ExtendedEcommerceDynamicService<OrderRefundDynamic, Order, OrderOptionType, OrderOptionValue>,
        IOrderRefundService
    {
        private readonly IWorkflowFactory _treeFactory;
        private readonly IGcService _gcService;

        public OrderRefundService(
            OrderRepository orderRepository,
            IEcommerceRepositoryAsync<BigStringValue> bigStringValueRepository,
            OrderRefundMapper mapper,
            IObjectLogItemExternalService objectLogItemExternalService,
            IEcommerceRepositoryAsync<OrderOptionValue> orderValueRepositoryAsync,
            ILoggerProviderExtended loggerProvider,
            DynamicExtensionsRewriter queryVisitor,
            ITransactionAccessor<EcommerceContext> transactionAccessor,
            IGcService gcService,
            IWorkflowFactory treeFactory, IDynamicEntityOrderingExtension<Order> orderingExtension)
            : base(
                mapper, orderRepository, orderValueRepositoryAsync,
                bigStringValueRepository, objectLogItemExternalService, loggerProvider, queryVisitor, transactionAccessor, orderingExtension
                )
        {
            _treeFactory = treeFactory;
            _gcService = gcService;
        }

        protected override Expression<Func<Order, bool>> AdditionalDefaultConditions => 
            p => p.IdObjectType == (int) OrderType.Refund;

        protected override IQueryLite<Order> BuildIncludes(IQueryLite<Order> query)
		{
		    return
		        query.Include(o => o.RefundSkus)
		            .ThenInclude(s => s.Sku)
                    .ThenInclude(s => s.OptionValues)
                    .Include(o => o.RefundSkus)
                    .ThenInclude(p => p.Sku)
                    .ThenInclude(s => s.Product)
                    .ThenInclude(p => p.OptionValues)
                    .Include(g => g.RefundOrderToGiftCertificates)
		            .ThenInclude(og => og.OrderToGiftCertificate)
                    .ThenInclude(g=>g.GiftCertificate)
                    .Include(o => o.PaymentMethod)
                    .ThenInclude(p => p.BillingAddress)
                    .ThenInclude(a => a.OptionValues)
                    .Include(o => o.PaymentMethod)
                    .ThenInclude(p => p.BillingAddress)
                    .Include(o => o.PaymentMethod)
                    .ThenInclude(p => p.OptionValues)
                    .Include(o => o.PaymentMethod)
                    .ThenInclude(p => p.PaymentMethod)
                    .Include(o => o.ShippingAddress)
                    .ThenInclude(s => s.OptionValues);
		}

        protected override bool LogObjectFullData => true;

        public async Task<OrderRefundDataContext> CalculateRefundOrder(OrderRefundDynamic order)
        {
            var context = new OrderRefundDataContext()
            {
                RefundOrder = order
            };
            var tree = await _treeFactory.CreateTreeAsync<OrderRefundDataContext, decimal>("Refund");
            await tree.ExecuteAsync(context);
            UpdateOrderFromCalculationContext(order, context);
            return context;
        }

        public async Task<ICollection<int>> GetRefundIdsForOrder(int idOrder)
        {
            return await ObjectRepository.Query(p => p.IdOrderSource == idOrder && p.StatusCode != (int)RecordStatusCode.Deleted && p.IdObjectType==(int)OrderType.Refund)
                    .SelectAsync(p => p.Id, false);
        }

        private void UpdateOrderFromCalculationContext(OrderRefundDynamic order, OrderRefundDataContext dataContext)
        {
            order.TaxTotal = dataContext.TaxTotal;
            order.Total = dataContext.Total;
            order.DiscountTotal = dataContext.DiscountTotal;
            order.ShippingTotal = dataContext.ShippingTotal;
            order.ProductsSubtotal = dataContext.ProductsSubtotal;
            order.Data.AutoTotal = dataContext.AutoTotal;
            order.Data.ShippingRefunded = dataContext.ShippingRefunded;
            order.Data.ManualShippingTotal = dataContext.ManualShippingTotal;
            order.Data.RefundGCsUsedOnOrder = dataContext.RefundGCsUsedOnOrder;
            order.Data.ManualRefundOverride = dataContext.ManualRefundOverride;
        }

        public async Task<bool> CancelRefundOrderAsync(int id)
        {
            var order = await SelectAsync(id, false);
            if (order != null)
            {
                order.OrderStatus=OrderStatus.Cancelled;
                foreach (var refundOrderToGiftCertificateUsed in order.RefundOrderToGiftCertificates)
                {
                    refundOrderToGiftCertificateUsed.Amount = 0;
                }
                await UpdateAsync(order);

                //foreach (var refundOrderToGiftCertificateUsed in order.RefundOrderToGiftCertificates)
                //{
                //    if (refundOrderToGiftCertificateUsed.GCBalanceAfterUpdate.HasValue)
                //    {
                //        var gc = await _gcService.GetGiftCertificateAsync(refundOrderToGiftCertificateUsed.IdGiftCertificate);
                //        gc.Balance = refundOrderToGiftCertificateUsed.GCBalanceAfterUpdate.Value;
                //        await _gcService.UpdateGiftCertificateAsync(gc);
                //    }
                //}

                return true;
            }
            return false;
        }

        protected override async Task<Order> InsertAsync(OrderRefundDynamic model, IUnitOfWorkAsync uow)
        {
            Order entity;
            using (var transaction = uow.BeginTransaction())
            {
                try
                {
                    entity = await base.InsertAsync(model, uow);
                    model.IdAddedBy = entity.IdEditedBy;

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }

            return entity;
        }

        protected override async Task<List<Order>> InsertRangeAsync(ICollection<OrderRefundDynamic> models, IUnitOfWorkAsync uow)
        {
            List<Order> entities;
            using (var transaction = uow.BeginTransaction())
            {
                try
                {
                    entities = await base.InsertRangeAsync(models, uow);
                    foreach (var model in models)
                    {
                        var entity = entities.FirstOrDefault(e => e.Id == model.Id);
                        if (entity != null)
                        {
                            model.IdAddedBy = entity.IdAddedBy;
                        }
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }

            return entities;
        }

        protected override async Task<Order> UpdateAsync(OrderRefundDynamic model, IUnitOfWorkAsync uow)
        {
            Order entity;
            using (var transaction = uow.BeginTransaction())
            {
                try
                {
                    entity = await base.UpdateAsync(model, uow);
                    model.IdAddedBy = entity.IdAddedBy;

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            return entity;
        }

        protected override async Task<List<Order>> UpdateRangeAsync(ICollection<OrderRefundDynamic> models, IUnitOfWorkAsync uow)
        {
            List<Order> entities;
            using (var transaction = uow.BeginTransaction())
            {
                try
                {
                    entities = await base.UpdateRangeAsync(models, uow);
                    foreach (var model in models)
                    {
                        var entity = entities.FirstOrDefault(p => p.Id == model.Id);
                        if (entity != null)
                        {
                            model.IdAddedBy = entity.IdAddedBy;
                        }
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }

            return entities;
        }
    }
}