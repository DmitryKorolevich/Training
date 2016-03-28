using System.Linq;
using System.Collections.Generic;
using VC.Admin.Models.Customer;
using VitalChoice.Business.Queries.Product;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.DynamicData.Base;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Products;
using VC.Admin.Models.Orders;
using VC.Admin.Models.Products;
using VitalChoice.Business.Queries.Products;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Business.Helpers;
using VitalChoice.Business.Services.Orders;
using VitalChoice.Data.Extensions;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Interfaces.Services.Orders;

namespace VC.Admin.ModelConverters
{
    public class OrderRefundModelConverter : BaseModelConverter<OrderRefundManageModel, OrderRefundDynamic>
    {
        private readonly OrderService _orderService;
        private readonly OrderRefundService _orderRefundService;
        private readonly IDynamicMapper<AddressDynamic, OrderAddress> _addressMapper;
        private readonly IDynamicMapper<OrderPaymentMethodDynamic, OrderPaymentMethod> _paymentMethodMapper;

        public OrderRefundModelConverter(OrderService orderService,
            OrderRefundService orderRefundService,
            IDynamicMapper<AddressDynamic, OrderAddress> addressMapper,
            IDynamicMapper<OrderPaymentMethodDynamic, OrderPaymentMethod> paymentMethodMapper)
        {
            _orderService = orderService;
            _orderRefundService = orderRefundService;
            _addressMapper = addressMapper;
            _paymentMethodMapper = paymentMethodMapper;
        }

        public override void DynamicToModel(OrderRefundManageModel model, OrderRefundDynamic dynamic)
        {
            if (dynamic.Customer != null)
            {
                model.IdCustomer = dynamic.Customer.Id;
            }
            model.Shipping = _addressMapper.ToModel<AddressModel>(dynamic.ShippingAddress);
            if (dynamic.PaymentMethod != null && dynamic.PaymentMethod.IdObjectType==(int)PaymentMethodType.Oac)
            {
                model.IdPaymentMethodType = dynamic.PaymentMethod.IdObjectType;
                model.Oac = _paymentMethodMapper.ToModel<OacRefundPaymentModel>(dynamic.PaymentMethod);
            }

            if (dynamic.IdOrderSource.HasValue)
            {
                var source = _orderService.SelectAsync(dynamic.IdOrderSource.Value,false).Result;
                if (source != null)
                {
                    model.OrderSourceDateCreated = source.DateCreated;
                    model.OrderSourceTotal = source.Total;
                    model.OrderSourcePaymentMethodType = source.PaymentMethod?.IdObjectType;
                    model.OrderSourceShippingTotal = source.ShippingTotal;
                    model.OrderSourceRefundIds = _orderRefundService.GetRefundIdsForOrder(source.Id).Result;
                    if (dynamic.Id != 0)
                    {
                        model.OrderSourceRefundIds.Remove(dynamic.Id);
                    }
                    var existDifferentRefunds = _orderRefundService.Select(p =>model.OrderSourceRefundIds.Contains(p.Id) && 
                        p.OrderStatus!=OrderStatus.Cancelled, withDefaults:false);

                    if (source.Discount != null)
                    {
                        model.DiscountCode = source.Discount.Code;
                        model.DiscountMessage = source.Discount.GetDiscountMessage((int?)dynamic.SafeData.IdDiscountTier);
                    }

                    if (dynamic.Id ==0)
                    {
                        var refundWithShippingRefunded = existDifferentRefunds.FirstOrDefault(p => p.SafeData.ShippingRefunded == true);
                        model.ShippingRefunded = refundWithShippingRefunded != null;

                        model.RefundSkus=new List<RefundSkuManageModel>();
                        foreach (var skuOrdered in source?.Skus)
                        {
                            var refundSku = new RefundSkuManageModel(skuOrdered);
                            var refundWithSkuExist = existDifferentRefunds.SelectMany(p => p.RefundSkus).FirstOrDefault(p => p?.Sku.Id == refundSku.IdSku);
                            refundSku.Disabled = refundWithSkuExist != null;
                            model.RefundSkus.Add(refundSku);
                        }
                        foreach (var skuOrdered in source?.PromoSkus)
                        {
                            var refundSku = new RefundSkuManageModel(skuOrdered);
                            var refundWithSkuExist = existDifferentRefunds.SelectMany(p => p.RefundSkus).FirstOrDefault(p => p?.Sku.Id == refundSku.IdSku);
                            refundSku.Disabled = refundWithSkuExist != null;
                            model.RefundSkus.Add(refundSku);
                        }

                        model.RefundOrderToGiftCertificates = source.GiftCertificates?.Where(p=>p.GiftCertificate!=null)?.Select
                            (p=>new RefundOrderToGiftCertificateManageModel(p, source.Id)).ToList()
                            ?? new List<RefundOrderToGiftCertificateManageModel>();
                    }
                    else
                    {
                        model.DisableShippingRefunded = true;
                        model.RefundSkus = dynamic.RefundSkus.Select(p => new RefundSkuManageModel(p)).ToList();
                        model.RefundSkus.ForEach(p =>
                        {
                            p.Disabled = true;
                            p.Active = true;
                        });
                        model.RefundOrderToGiftCertificates = dynamic.RefundOrderToGiftCertificates.
                            Select(p => new RefundOrderToGiftCertificateManageModel(p)).ToList();
                        //count refundgcs on the current refund
                        foreach (var refundOrderToGiftCertificateManageModel in model.RefundOrderToGiftCertificates)
                        {
                            refundOrderToGiftCertificateManageModel.AmountRefunded+=refundOrderToGiftCertificateManageModel.Amount;
                        }
                    }

                    //sync AmountRefunded on refundgcs
                    foreach (var refundOrderToGiftCertificateManageModel in model.RefundOrderToGiftCertificates)
                    {
                        var existOrderToGiftCertificateOnDifferentRefunds= existDifferentRefunds.SelectMany(p=>p.RefundOrderToGiftCertificates).
                            Where(p=>p.IdGiftCertificate== refundOrderToGiftCertificateManageModel.IdGiftCertificate).ToList();
                        refundOrderToGiftCertificateManageModel.AmountRefunded +=
                            existOrderToGiftCertificateOnDifferentRefunds.Sum(p => p.Amount);
                    }
                }
            }
        }

        public override void ModelToDynamic(OrderRefundManageModel model, OrderRefundDynamic dynamic)
        {
            if (model.Shipping != null)
            {
                var addressDynamic = _addressMapper.FromModel(model.Shipping, (int)AddressType.Shipping);
                dynamic.ShippingAddress = addressDynamic;
            }
            if (model.IdPaymentMethodType.HasValue)
            {
                dynamic.PaymentMethod = _paymentMethodMapper.FromModel(model.Oac, model.IdPaymentMethodType.Value);
                if (dynamic?.PaymentMethod?.Address != null)
                {
                    dynamic.PaymentMethod.Address.IdObjectType = (int) AddressType.Billing;
                }
            }
            dynamic.Customer=new CustomerDynamic() { Id =model.IdCustomer};

            dynamic.RefundSkus = model.RefundSkus.Where(p => p.Active && p.IdSku.HasValue).Select(p => new RefundSkuOrdered()
            {
                Sku = new SkuDynamic() { Id = p.IdSku.Value},
                Redeem = p.Redeem,
                Quantity = p.Quantity,
                RefundPrice = p.RefundPrice,
                RefundValue = p.RefundValue,
                RefundPercent = p.RefundPercent,
            }).ToList();

            dynamic.RefundOrderToGiftCertificates=model.RefundOrderToGiftCertificates.Select(p=>new RefundOrderToGiftCertificateUsed()
            {
                IdOrder = p.IdOrder,
                IdGiftCertificate = p.IdGiftCertificate,
                AmountUsedOnSourceOrder = p.AmountUsedOnSourceOrder,
                AmountRefunded = p.AmountRefunded,
                Amount = p.Amount,
                Code = p.Code,
            }).ToList();
        }
    }
}