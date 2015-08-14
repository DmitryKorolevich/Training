using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VitalChoice.Business.Queries.Product;
using VitalChoice.Business.Services.Dynamic;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Customs;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Products;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.DynamicData.Validation;
using VitalChoice.Infrastructure.UnitOfWork;
using VitalChoice.Interfaces.Services.Order;
using VitalChoice.Interfaces.Services.Products;

namespace VitalChoice.Business.Services.Products
{
    public class OrderService : EcommerceDynamicObjectService<OrderDynamic, Order, OrderOptionType, OrderOptionValue>, IOrderService
    {
        private readonly IEcommerceRepositoryAsync<VOrder> _vOrderRepository;
        private readonly IRepositoryAsync<AdminProfile> _adminProfileRepository;

        public OrderService(IEcommerceRepositoryAsync<VOrder> vOrderRepository,
            IEcommerceRepositoryAsync<OrderOptionType> orderOptionTypeRepository,
            IEcommerceRepositoryAsync<Lookup> lookupRepository, IEcommerceRepositoryAsync<Order> orderRepository,
            IEcommerceRepositoryAsync<Sku> skuRepository,
            IEcommerceRepositoryAsync<BigStringValue> bigStringValueRepository,
            OrderMapper mapper,
            IEcommerceRepositoryAsync<OrderOptionValue> orderValueRepositoryAsync,
            IRepositoryAsync<AdminProfile> adminProfileRepository)
            : base(
                mapper, orderRepository, orderOptionTypeRepository, orderValueRepositoryAsync,
                bigStringValueRepository)
        {
            _vOrderRepository = vOrderRepository;
            _adminProfileRepository = adminProfileRepository;
        }

        public Task<ICollection<VOrder>> GetSkusAsync(VOrderFilter filter)
        {
            throw new NotImplementedException();
        }
    }
}