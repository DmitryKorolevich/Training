﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Affiliates;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Infrastructure.Domain.Transfer.GiftCertificates;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Reports;

namespace VitalChoice.Interfaces.Services.Orders
{
	public interface IOrderReportService
	{
	    Task<OrdersAgentReport> GetOrdersAgentReportAsync(OrdersAgentReportFilter filter);

	    ICollection<OrdersAgentReportExportItem> ConvertOrdersAgentReportToExportItems(OrdersAgentReport report, bool fullReport);

	    Task<WholesaleDropShipReport> GetWholesaleDropShipReportAsync(WholesaleDropShipReportFilter filter);

        Task<PagedList<WholesaleDropShipReportOrderItem>> GetOrdersForWholesaleDropShipReportAsync(WholesaleDropShipReportFilter filter);
    }
}
