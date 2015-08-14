using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using VitalChoice.Business.Queries.Product;
using VitalChoice.Data.Extensions;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Exceptions;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Domain.Entities.eCommerce.Orders;

namespace VitalChoice.Business.Services.Dynamic
{
    public class OrderMapper : DynamicObjectMapper<OrderDynamic, Order, OrderOptionType, OrderOptionValue>
    {
        public OrderMapper(IIndex<Type, IDynamicToModelMapper> mappers, IIndex<Type, IModelToDynamicConverter> container, IEcommerceRepositoryAsync<OrderOptionType> orderRepositoryAsync)
            : base(mappers, container, orderRepositoryAsync)
        {

        }

        public override Expression<Func<OrderOptionValue, int?>> ObjectIdSelector
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        protected override Task FromEntityRangeInternalAsync(ICollection<DynamicEntityPair<OrderDynamic, Order>> items, bool withDefaults = false)
        {
            throw new NotImplementedException();
        }

        protected override Task ToEntityRangeInternalAsync(ICollection<DynamicEntityPair<OrderDynamic, Order>> items)
        {
            throw new NotImplementedException();
        }

        protected override Task UpdateEntityRangeInternalAsync(ICollection<DynamicEntityPair<OrderDynamic, Order>> items)
        {
            throw new NotImplementedException();
        }
    }
}
