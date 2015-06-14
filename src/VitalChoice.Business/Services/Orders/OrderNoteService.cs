using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.Logging;
using VitalChoice.Business.Queries.Product;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.Transaction;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Entities.Products;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Products;
using VitalChoice.DynamicData.Entities;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Interfaces.Services.Order;
using VitalChoice.Interfaces.Services.Product;

namespace VitalChoice.Business.Services.Orders
{
    public class OrderNoteService : IOrderNoteService
    {
        private readonly IEcommerceRepositoryAsync<OrderNote> _orderNoteRepository;
        private readonly ILogger _logger;

        public OrderNoteService(IEcommerceRepositoryAsync<OrderNote> orderNoteRepository)
        {
			_orderNoteRepository = orderNoteRepository;
            _logger = LoggerService.GetDefault();
        }

	    public async Task<OrderNote> AddOrderNote(OrderNote orderNote)
	    {
		    //orderNote.StatusCode = RecordStatusCode.Active;

			//_orderNoteRepository.InsertAsync()

			return null;
	    }
    }
}