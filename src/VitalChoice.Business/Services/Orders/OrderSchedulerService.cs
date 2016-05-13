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
using Microsoft.Extensions.OptionsModel;
using VitalChoice.Business.Mail;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Ecommerce.Domain.Mail;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace VitalChoice.Business.Services.Orders
{
    public class OrderSchedulerService : IOrderSchedulerService
    {
        private readonly OrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly INotificationService _notificationService;
        private readonly IOptions<AppOptions> _options;
        private readonly ILogger _logger;

        public OrderSchedulerService(
            OrderService orderService,
            ICustomerService customerService,
            INotificationService notificationService,
            IOptions<AppOptions> options,
            ILoggerProviderExtended loggerProvider)
        {
            _orderService = orderService;
            _customerService = customerService;
            _notificationService = notificationService;
            _options = options;
            _logger = loggerProvider.CreateLoggerDefault();
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
            //get orders by part ship date in range from EmailConstants.OrderProductReviewEmailDaysCount to EmailConstants.OrderProductReviewEmailDaysCount+1
            //and check that customers of these orders have emails and these emails aren't in the black list and then send emails for them
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

            await _notificationService.SendOrderProductReviewEmailAsync(new[] {model});
        }
    }
}