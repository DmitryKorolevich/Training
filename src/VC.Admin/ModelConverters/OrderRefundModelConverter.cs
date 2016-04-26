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
        private readonly IDiscountService _discountService;
        private readonly IProductService _productService;

        public OrderRefundModelConverter(OrderService orderService,
            OrderRefundService orderRefundService,
            IDynamicMapper<AddressDynamic, OrderAddress> addressMapper,
            IDynamicMapper<OrderPaymentMethodDynamic, OrderPaymentMethod> paymentMethodMapper,
            IDiscountService discountService,
            IProductService productService)
        {
            _orderService = orderService;
            _orderRefundService = orderRefundService;
            _addressMapper = addressMapper;
            _paymentMethodMapper = paymentMethodMapper;
            _discountService = discountService;
            _productService = productService;
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
                    model.OrderSourceRefundIds = _orderRefundService.GetRefundIdsForOrder(source.Id).Result;

                    var existAllRefunds = _orderRefundService.Select(p =>model.OrderSourceRefundIds.Contains(p.Id) && 
                        p.OrderStatus!=OrderStatus.Cancelled, withDefaults:false);
                    var existDifferentRefunds = existAllRefunds.Where(p => p.Id != dynamic.Id).ToList();

                    model.DiscountedSubtotal = model.ProductsSubtotal;
                    if (source.Discount != null)
                    {
                        model.DiscountCode = source.Discount.Code;
                        model.DiscountMessage = source.Discount.GetDiscountMessage((int?)dynamic.SafeData.IdDiscountTier);
                        model.DiscountedSubtotal -= model.DiscountTotal;
                    }

                    if (dynamic.Id ==0)
                    {
                        var refundWithShippingRefunded = existAllRefunds.FirstOrDefault(p => p.SafeData.ShippingRefunded == true);
                        if (refundWithShippingRefunded != null)
                        {
                            model.DisableShippingRefunded = true;
                            model.ShippingRefunded = true;
                            model.ManualShippingTotal = 0;
                        }

                        model.RefundSkus=new List<RefundSkuManageModel>();
                        foreach (var skuOrdered in source?.Skus)
                        {
                            var refundSku = new RefundSkuManageModel(skuOrdered);
                            var refundWithSkuExist = existAllRefunds.SelectMany(p => p.RefundSkus).FirstOrDefault(p => p?.Sku.Id == refundSku.IdSku);
                            if (refundWithSkuExist != null)
                            {
                                refundSku.Disabled = true;
                                refundSku.Redeem = refundWithSkuExist.Redeem;
                            }
                            refundSku.Active = refundSku.Disabled;
                            model.RefundSkus.Add(refundSku);
                        }
                        foreach (var skuOrdered in source?.PromoSkus.Where(p=>p.Enabled))
                        {
                            var refundSku = new RefundSkuManageModel(skuOrdered);
                            var refundWithSkuExist = existAllRefunds.SelectMany(p => p.RefundSkus).FirstOrDefault(p => p?.Sku.Id == refundSku.IdSku);
                            if (refundWithSkuExist != null)
                            {
                                refundSku.Disabled = true;
                                refundSku.Redeem = refundWithSkuExist.Redeem;
                            }
                            refundSku.Active = refundSku.Disabled;
                            model.RefundSkus.Add(refundSku);
                        }

                        model.RefundOrderToGiftCertificates = source.GiftCertificates?.Where(p=>p.GiftCertificate!=null)?.Select
                            (p=>new RefundOrderToGiftCertificateManageModel(p, source.Id)).ToList()
                            ?? new List<RefundOrderToGiftCertificateManageModel>();
                    }
                    else
                    {
                        model.DisableShippingRefunded = true;
                        model.ManualShippingTotal = model.ShippingTotal;
                        model.RefundSkus = dynamic.RefundSkus.Select(p => new RefundSkuManageModel(p)).ToList();
                        model.RefundSkus.ForEach(p =>
                        {
                            p.Disabled = true;
                            p.Active = true;
                        });
                        model.RefundOrderToGiftCertificates = dynamic.RefundOrderToGiftCertificates.
                            Select(p => new RefundOrderToGiftCertificateManageModel(p)).ToList();

                        var giftCertificateInOrders = source.GiftCertificates?.Where(p => p.GiftCertificate != null);
                        if (giftCertificateInOrders != null)
                        {
                            foreach (var sourceGc in giftCertificateInOrders)
                            {
                                var modelGc = model.RefundOrderToGiftCertificates.FirstOrDefault(p => p.IdGiftCertificate == sourceGc.GiftCertificate.Id);
                                if (modelGc == null)
                                {
                                    model.RefundOrderToGiftCertificates.Add(new RefundOrderToGiftCertificateManageModel(sourceGc, source.Id));
                                }
                            }
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
                    model.GiftCertificatesUsedAmountOnSourceOrder =
                        model.RefundOrderToGiftCertificates.Sum(p =>p.AmountUsedOnSourceOrder);
                }
            }
        }

        public override void ModelToDynamic(OrderRefundManageModel model, OrderRefundDynamic dynamic)
        {
            if (!string.IsNullOrEmpty(model.DiscountCode))
            {
                dynamic.Discount = _discountService.GetByCode(model.DiscountCode).Result;
            }

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

            dynamic.RefundSkus = model.RefundSkus.Where(p => p.Active && !p.Disabled && p.IdSku.HasValue).Select(p => new RefundSkuOrdered()
            {
                Sku = new SkuDynamic() { Id = p.IdSku.Value},
                Redeem = p.Redeem,
                Quantity = p.Quantity,
                RefundPrice = p.RefundPrice,
                RefundValue = p.RefundValue,
                RefundPercent = p.RefundPercent,
            }).ToList();

            var task = _productService.GetRefundSkusOrderedAsync(dynamic.RefundSkus.Select(p=>p.Sku.Id).ToArray());
            var fullSkus = task.Result;

            foreach (var refundSkuOrdered in dynamic.RefundSkus)
            {
                var fullSku = fullSkus.FirstOrDefault(p => p.Sku.Id == refundSkuOrdered.Sku.Id);
                if (fullSku != null)
                {
                    refundSkuOrdered.Sku = fullSku.Sku;
                }
            }

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