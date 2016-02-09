using Microsoft.AspNet.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Authorize.Net.Api.Contracts.V1;
using Authorize.Net.Api.Controllers;
using Authorize.Net.Api.Controllers.Bases;
using Microsoft.Extensions.OptionsModel;
using VitalChoice.Business.Queries.Customer;
using VitalChoice.Business.Queries.Orders;
using VitalChoice.Business.Repositories;
using VitalChoice.Business.Services.Dynamic;
using VitalChoice.Business.Services.Ecommerce;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Customs;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.Services;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Helpers;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.DynamicData.Validation;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Affiliates;
using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Entities.History;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities.Customers;
using VitalChoice.Infrastructure.Domain.Entities.Healthwise;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Affiliates;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Infrastructure.Domain.Transfer.Customers;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Affiliates;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Interfaces.Services.Payments;
using VitalChoice.Workflow.Core;
using Microsoft.Extensions.Logging;
using System.IO;
using CsvHelper;
using System.Text;
using CsvHelper.Configuration;
using VitalChoice.Business.CsvExportMaps;
using VitalChoice.Infrastructure.Domain.Entities.Orders;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using VitalChoice.Interfaces.Services.Settings;
using VitalChoice.Infrastructure.Domain.Transfer.Country;
using FluentValidation.Validators;
using System.Text.RegularExpressions;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Infrastructure.Domain.Mail;
using VitalChoice.Business.Mail;

namespace VitalChoice.Business.Services.Orders
{
    public class OrderSchedulerService : IOrderSchedulerService
    {
        private readonly IOrderService _orderService;
        private readonly ILogger _logger;

        public OrderSchedulerService(IOrderService orderService,
            ILoggerProviderExtended loggerProvider)
        {
            _orderService = orderService;
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

                if (ordersForUpdate.Count != 0)
                {
                    await _orderService.UpdateRangeAsync(ordersForUpdate);
                }
            }
            catch(Exception e)
            {
                _logger.LogCritical("Error till ShipDelayed orders updating",e);
            }
        }
    }
}