using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Authorize.Net.Util;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Orders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VitalChoice.Business.Mailings;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Ecommerce.Domain.Mail;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Newsletters;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.VeraCore;

namespace VitalChoice.Business.Services.Orders
{
    public class OrderSchedulerService : IOrderSchedulerService
    {
        private readonly OrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly INotificationService _notificationService;
        private readonly IOptions<AppOptions> _options;
        private readonly IEcommerceRepositoryAsync<OrderShippingPackage> _orderShippingPackageRepository;
        private readonly IEcommerceRepositoryAsync<NewsletterBlockedEmail> _newsletterBlockedEmailRepository;
        private readonly ILogger _logger;

        public OrderSchedulerService(
            OrderService orderService,
            ICustomerService customerService,
            INotificationService notificationService,
            IOptions<AppOptions> options,
            IEcommerceRepositoryAsync<OrderShippingPackage> orderShippingPackageRepository,
            IEcommerceRepositoryAsync<NewsletterBlockedEmail> newsletterBlockedEmailRepository,
            ILoggerProviderExtended loggerProvider)
        {
            _orderService = orderService;
            _customerService = customerService;
            _notificationService = notificationService;
            _options = options;
            _orderShippingPackageRepository = orderShippingPackageRepository;
            _newsletterBlockedEmailRepository = newsletterBlockedEmailRepository;
            _logger = loggerProvider.CreateLogger<OrderSchedulerService>();
        }

        public async Task UpdateShipDelayedOrders()
        {
            DateTime now = DateTime.Now;

            try
            {
                var shipDelayedOrders = await _orderService.SelectAsync(p => p.StatusCode != (int)RecordStatusCode.Deleted &&
                  p.OrderStatus == OrderStatus.ShipDelayed || p.POrderStatus == OrderStatus.ShipDelayed || p.NPOrderStatus == OrderStatus.ShipDelayed);

                List<OrderDynamic> ordersForUpdate = new List<OrderDynamic>();

                bool pPartNeedUpdate = false;
                bool npPartNeedUpdate = false;
                foreach (var shipDelayedOrder in shipDelayedOrders)
                {
                    pPartNeedUpdate = false;
                    npPartNeedUpdate = false;
                    if (shipDelayedOrder.OrderStatus == OrderStatus.ShipDelayed && shipDelayedOrder.SafeData.ShipDelayType == (int)ShipDelayType.EntireOrder
                        && shipDelayedOrder.SafeData.ShipDelayDate != null && shipDelayedOrder.SafeData.ShipDelayDate < now)
                    {
                        shipDelayedOrder.OrderStatus = OrderStatus.Processed;
                        shipDelayedOrder.Data.ShipDelayType = null;
                        shipDelayedOrder.Data.ShipDelayDate = null;
                        ordersForUpdate.Add(shipDelayedOrder);
                    }


                    if (shipDelayedOrder.POrderStatus == OrderStatus.ShipDelayed &&
                        ((shipDelayedOrder.SafeData.ShipDelayType == (int)ShipDelayType.EntireOrder && shipDelayedOrder.SafeData.ShipDelayDate != null && shipDelayedOrder.SafeData.ShipDelayDate < now)
                        || (shipDelayedOrder.SafeData.ShipDelayType == (int)ShipDelayType.PerishableAndNonPerishable && shipDelayedOrder.SafeData.ShipDelayDateP != null && shipDelayedOrder.SafeData.ShipDelayDateP < now)))
                    {
                        shipDelayedOrder.POrderStatus = OrderStatus.Processed;
                        shipDelayedOrder.Data.ShipDelayDateP = null;
                        pPartNeedUpdate = true;
                    }

                    if (shipDelayedOrder.NPOrderStatus == OrderStatus.ShipDelayed &&
                        ((shipDelayedOrder.SafeData.ShipDelayType == (int)ShipDelayType.EntireOrder && shipDelayedOrder.SafeData.ShipDelayDate != null && shipDelayedOrder.SafeData.ShipDelayDate < now)
                        || (shipDelayedOrder.SafeData.ShipDelayType == (int)ShipDelayType.PerishableAndNonPerishable && shipDelayedOrder.SafeData.ShipDelayDateNP != null && shipDelayedOrder.SafeData.ShipDelayDateNP < now)))
                    {
                        shipDelayedOrder.NPOrderStatus = OrderStatus.Processed;
                        shipDelayedOrder.Data.ShipDelayDateNP = null;
                        npPartNeedUpdate = true;
                    }

                    if (pPartNeedUpdate || npPartNeedUpdate)
                    {
                        if (ordersForUpdate.FirstOrDefault(p => p.Id == shipDelayedOrder.Id) == null)
                        {
                            ordersForUpdate.Add(shipDelayedOrder);
                        }

                        //update common part if needed
                        if (shipDelayedOrder.SafeData.ShipDelayType == (int)ShipDelayType.EntireOrder && shipDelayedOrder.POrderStatus != OrderStatus.ShipDelayed
                            && shipDelayedOrder.NPOrderStatus != OrderStatus.ShipDelayed)
                        {
                            shipDelayedOrder.Data.ShipDelayDate = null;
                            shipDelayedOrder.Data.ShipDelayType = null;
                        }

                        if (shipDelayedOrder.SafeData.ShipDelayType == (int)ShipDelayType.PerishableAndNonPerishable && shipDelayedOrder.POrderStatus != OrderStatus.ShipDelayed
                            && shipDelayedOrder.NPOrderStatus != OrderStatus.ShipDelayed)
                        {
                            shipDelayedOrder.Data.ShipDelayType = null;
                        }
                    }
                }

                List<OrderDynamic> ordersForUpdateAfterCalculating = new List<OrderDynamic>();

                if (ordersForUpdate.Count != 0)
                {
                    foreach (var order in ordersForUpdate)
                    {
                        ordersForUpdateAfterCalculating.Add(order);
                        //try
                        //{
                        //    order.Customer = await _customerService.SelectAsync(order.Customer.Id);
                        //    var context = await _orderService.CalculateOrder(order, OrderStatus.Processed);

                        //    List<MessageInfo> messages =new List<MessageInfo>(context.Messages);
                        //    messages.AddRange(context.SkuOrdereds.Where(p => p.Messages != null).SelectMany(p => p.Messages));
                        //    messages.AddRange(context.PromoSkus.Where(p => p.Enabled && p.Messages != null).SelectMany(p => p.Messages));
                        //    if(messages.Count>0)
                        //    {
                        //        //TODO - add additioanl logic for this case
                        //    }
                        //    else
                        //    {
                        //        ordersForUpdateAfterCalculating.Add(context.Order);
                        //    }
                        //}
                        //catch (Exception e)
                        //{
                        //    _logger.LogCritical($"Error till ShipDelayed orders updating - calculating order({order.Id})", e);
                        //}
                    }
                }

                if (ordersForUpdateAfterCalculating.Count != 0)
                {
                    await _orderService.UpdateRangeAsync(ordersForUpdateAfterCalculating);
                }
            }
            catch (Exception e)
            {
                _logger.LogCritical("Error till ShipDelayed orders updating", e);
            }
        }

        public async Task SendOrderProductReviewEmails()
        {
            var now =new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            var packages = await _orderShippingPackageRepository.Query(p=>p.ShippedDate>=now.AddDays(-EmailConstants.OrderProductReviewEmailDaysCount)
                && p.ShippedDate< now.AddDays(-EmailConstants.OrderProductReviewEmailDaysCount+1)).Include(p=>p.Order).ThenInclude(p=>p.Customer)
                .SelectAsync(false);

            var emails = packages.Select(p => p.Order.Customer.Email).Where(p => !string.IsNullOrEmpty(p)).Distinct();
            var blockedEmails = (await _newsletterBlockedEmailRepository.Query(p => emails.Contains(p.Email) && p.IdNewsletter== EmailConstants.ProductReviewIdNewsletter)
                .SelectAsync(false)).Select(p=>p.Email).ToList();
            var orderNotifications = new List<ShipmentNotificationItem>();
            foreach (var orderShippingPackage in packages)
            {
                if (!blockedEmails.Contains(orderShippingPackage.Order.Customer.Email))
                {
                    if (!orderNotifications.Any(p=>p.IdOrder == orderShippingPackage.IdOrder && p.POrderType == (POrderType?)orderShippingPackage.POrderType)
                        && ((orderShippingPackage.Order.OrderStatus == OrderStatus.Shipped && !orderShippingPackage.POrderType.HasValue) ||
                        (orderShippingPackage.Order.POrderStatus == OrderStatus.Shipped && orderShippingPackage.POrderType==(int)POrderType.P) ||
                        (orderShippingPackage.Order.NPOrderStatus == OrderStatus.Shipped && orderShippingPackage.POrderType == (int)POrderType.NP)))
                    {
                        orderNotifications.Add(new ShipmentNotificationItem()
                        {
                            IdOrder  = orderShippingPackage.IdOrder,
                            POrderType = (POrderType?)orderShippingPackage.POrderType,
                        }); 
                    }
                }
            }

            if (orderNotifications.Count > 0)
            {
                var orders = await _orderService.SelectAsync(orderNotifications.Select(p=>p.IdOrder).Distinct().ToList());
                var customers = await _customerService.SelectAsync(orders.Select(p=>p.Customer.Id).Distinct().ToList());

                var models =new List<OrderProductReviewEmail>();
                foreach (var orderNotification in orderNotifications)
                {
                    var order = orders.FirstOrDefault(p => p.Id == orderNotification.IdOrder);
                    if (order != null)
                    {
                        var customer = customers.FirstOrDefault(p => p.Id == order.Customer.Id);
                        if (customer != null)
                        {
                            OrderProductReviewEmail model = new OrderProductReviewEmail();
                            model.PublicHost = _options.Value.PublicHost;
                            model.CustomerName = $"{customer.ProfileAddress.SafeData.FirstName} {customer.ProfileAddress.SafeData.LastName}";
                            model.Email = customer.Email;
                            model.UrlEncodedEmail = WebUtility.UrlEncode(customer.Email);
                            var skus = order.Skus.Where(p=>!p.Sku.Hidden && p.Sku.Product.IdVisibility.HasValue);
                            if (orderNotification.POrderType==POrderType.P)
                            {
                                skus = skus.Where(p => p.Sku.Product.IdObjectType == (int)ProductType.Perishable);
                            }
                            if (orderNotification.POrderType == POrderType.NP)
                            {
                                skus = skus.Where(p => p.Sku.Product.IdObjectType == (int)ProductType.NonPerishable);
                            }
                            foreach (var sku in skus)
                            {
                                OrderProductReviewEmailProductItem item = new OrderProductReviewEmailProductItem();
                                item.Thumbnail = sku.Sku.Product.SafeData.Thumbnail;
                                item.ProductUrl = "/product/" + sku.Sku.Product.Url;
                                item.DisplayName = sku.Sku.Product.Name ?? String.Empty;
                                if (!string.IsNullOrEmpty(sku.Sku.Product.SafeData.SubTitle))
                                {
                                    item.DisplayName += " " + sku.Sku.Product.SafeData.SubTitle;
                                }
                                item.DisplayName += $" ({sku.Sku.SafeData.QTY})";
                                model.Products.Add(item);
                            }
                            models.Add(model);
                        }
                    }
                }

                await _notificationService.SendOrderProductReviewEmailsAsync(models);
            }
        }

        public async Task SendOrderProductReviewEmailTest(int id)
        {
            var order = await _orderService.SelectAsync(id);
            var customer = await _customerService.SelectAsync(order.Customer.Id);

            OrderProductReviewEmail model=new OrderProductReviewEmail();
            model.PublicHost = _options.Value.PublicHost;
            model.CustomerName =$"{customer.ProfileAddress.SafeData.FirstName} {customer.ProfileAddress.SafeData.LastName}";
            model.Email = customer.Email;
            model.UrlEncodedEmail = WebUtility.UrlEncode(customer.Email);
            foreach (var sku in order.Skus)
            {
                OrderProductReviewEmailProductItem item = new OrderProductReviewEmailProductItem();
                item.Thumbnail = sku.Sku.Product.SafeData.Thumbnail;
                item.ProductUrl = "/product/" + sku.Sku.Product.Url;
                item.DisplayName = sku.Sku.Product.Name ?? String.Empty;
                if (!string.IsNullOrEmpty(sku.Sku.Product.SafeData.SubTitle))
                {
                    item.DisplayName += " " + sku.Sku.Product.SafeData.SubTitle;
                }
                item.DisplayName += $" ({sku.Sku.SafeData.QTY})";
                model.Products.Add(item);
            }

            await _notificationService.SendOrderProductReviewEmailsAsync(new[] {model});
        }
    }
}