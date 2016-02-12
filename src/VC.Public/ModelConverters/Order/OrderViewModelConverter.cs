using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VC.Public.Models.Cart;
using VC.Public.Models.Profile;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VC.Public.ModelConverters.Checkout
{
    public class OrderViewModelConverter : BaseModelConverter<OrderViewModel, OrderDynamic>
    {
        private readonly IDynamicMapper<AddressDynamic, OrderAddress> _addressMapper;

        public OrderViewModelConverter(IDynamicMapper<AddressDynamic, OrderAddress> addressMapper)
        {
            _addressMapper = addressMapper;
        }

        public override void DynamicToModel(OrderViewModel model, OrderDynamic dynamic)
        {
            if (dynamic?.PaymentMethod.IdObjectType == (int)PaymentMethodType.NoCharge && dynamic.Customer.ProfileAddress != null)
            {
                model.BillingAddress = _addressMapper.ToModel<AddressModel>(dynamic.Customer.ProfileAddress);
            }
            else if (dynamic?.PaymentMethod?.Address != null)
            {
                model.BillingAddress = _addressMapper.ToModel<AddressModel>(dynamic.PaymentMethod.Address);
            }
        }

        public override void ModelToDynamic(OrderViewModel model, OrderDynamic dynamic)
        {
        }
    }
}
