using System;
using System.Linq;
using System.Collections.Generic;
using VC.Admin.Models.Customer;
using VC.Admin.Models.Order;
using VC.Admin.Models.Product;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.GiftCertificates;
using VitalChoice.Domain.Transfer.Orders;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Entities.Transfer;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Domain.Entities.eCommerce.Payment;
using VitalChoice.Domain.Entities.eCommerce.Addresses;

namespace VC.Admin.ModelConverters
{
    public class OrderModelConverter : IModelToDynamicConverter<OrderManageModel, OrderDynamic>
    {
        private readonly IDynamicToModelMapper<OrderPaymentMethodDynamic> _paymentMethodMapper;
        private readonly IDynamicToModelMapper<OrderAddressDynamic> _addressMapper;

        public OrderModelConverter(IDynamicToModelMapper<OrderAddressDynamic> addressMapper,
                                   IDynamicToModelMapper<OrderPaymentMethodDynamic> paymentMethodMapper)
        {
            _addressMapper = addressMapper;
            _paymentMethodMapper = paymentMethodMapper;
        }

        public void DynamicToModel(OrderManageModel model, OrderDynamic dynamic)
        {
            if(dynamic.Customer!=null)
            {
                model.IdCustomer = dynamic.Customer.Id;
            }

            if(dynamic.Discount!=null)
            {
                model.DiscountCode = dynamic.Discount.Code;
            }

            if(dynamic.GiftCertificates!=null && dynamic.GiftCertificates.Count>0)
            {
                if(model.GCs==null)
                {
                    model.GCs = new List<GCListItemModel>();
                }
                foreach(var item in dynamic.GiftCertificates)
                {
                    model.GCs.Add(new GCListItemModel(item.GiftCertificate));
                }
            }

            if (dynamic.Skus != null)
            {
                model.SkuOrdereds= new List<SkuOrderedManageModel>();
                foreach (var item in dynamic.Skus)
                {
                    model.SkuOrdereds.Add(new SkuOrderedManageModel(item));
                }
            }

            if(!model.ShipDelayType.HasValue)
            {
                model.ShipDelayType = 0;
            }

            model.Shipping = _addressMapper.ToModel<AddressModel>(dynamic.ShippingAddress);
            if(dynamic.PaymentMethod!=null)
            {
                model.IdPaymentMethodType = dynamic.PaymentMethod.IdObjectType;
                if (dynamic.PaymentMethod.IdObjectType== (int)PaymentMethodType.CreditCard)
                {
                    model.CreditCard = _paymentMethodMapper.ToModel<CreditCardModel>(dynamic.PaymentMethod);
                }
                if (dynamic.PaymentMethod.IdObjectType == (int)PaymentMethodType.Oac)
                {
                    model.Oac = _paymentMethodMapper.ToModel<OacPaymentModel>(dynamic.PaymentMethod);
                }
                if (dynamic.PaymentMethod.IdObjectType == (int)PaymentMethodType.Check)
                {
                    model.Check = _paymentMethodMapper.ToModel<CheckPaymentModel>(dynamic.PaymentMethod);
                }
            }
        }

        public void ModelToDynamic(OrderManageModel model, OrderDynamic dynamic)
        {
            if(!String.IsNullOrEmpty(model.DiscountCode))
            {
                dynamic.Discount = new DiscountDynamic();
                dynamic.Discount.Code = model.DiscountCode;
            }

            if(model.GCs!=null)
            {
                dynamic.GiftCertificates = new List<GiftCertificateInOrder>();
                if (!(model.GCs.Count == 1 && String.IsNullOrEmpty(model.GCs[0].Code)))
                {
                    foreach (var gc in model.GCs)
                    {
                        GiftCertificateInOrder item = new GiftCertificateInOrder();
                        item.GiftCertificate = new GiftCertificate();
                        item.GiftCertificate.Code = gc.Code;
                        dynamic.GiftCertificates.Add(item);
                    }
                }
            }

            if (model.SkuOrdereds != null)
            {
                dynamic.Skus = new List<SkuOrdered>();
                foreach (var item in model.SkuOrdereds)
                {
                    dynamic.Skus.Add(item.Convert());
                }
            }

            if(dynamic.DictionaryData.ContainsKey("ShipDelayType") && (int?)dynamic.DictionaryData["ShipDelayType"] == 0)
            {
                dynamic.DictionaryData["ShipDelayType"] = null;
            }

            if (model.Id != 0)
            {
                if (model.Shipping != null)
                {
                    var addressDynamic = _addressMapper.FromModel(model.Shipping);
                    addressDynamic.IdObjectType = (int)AddressType.Shipping;
                    dynamic.ShippingAddress = addressDynamic;
                }

                if (model.IdPaymentMethodType.HasValue)
                {
                    if (model.IdPaymentMethodType.Value == (int)PaymentMethodType.CreditCard)
                    {
                        dynamic.PaymentMethod = _paymentMethodMapper.FromModel(model.CreditCard);
                    }
                    else if (model.IdPaymentMethodType.Value == (int)PaymentMethodType.Oac)
                    {
                        dynamic.PaymentMethod = _paymentMethodMapper.FromModel(model.Oac);
                    }
                    else if (model.IdPaymentMethodType.Value == (int)PaymentMethodType.Check)
                    {
                        dynamic.PaymentMethod = _paymentMethodMapper.FromModel(model.Check);
                    }
                    else
                    {
                        dynamic.PaymentMethod = new OrderPaymentMethodDynamic();
                    }
                    dynamic.PaymentMethod.IdObjectType = model.IdPaymentMethodType.Value;
                }
            }
            else
            {
                var shippingAddress = model.Customer.Shipping.Where(p => p.IsSelected).FirstOrDefault();
                if(shippingAddress!=null)
                {
                    var addressDynamic = _addressMapper.FromModel(shippingAddress);
                    addressDynamic.IdObjectType = (int)AddressType.Shipping;
                    dynamic.ShippingAddress = addressDynamic;
                }

                if (model.IdPaymentMethodType.HasValue)
                {
                    if (model.IdPaymentMethodType.Value == (int)PaymentMethodType.CreditCard)
                    {
                        var card = model.Customer.CreditCards.Where(p => p.IsSelected).FirstOrDefault();
                        if (card != null)
                        {
                            dynamic.PaymentMethod = _paymentMethodMapper.FromModel(card);
                        }
                    }
                    else if (model.IdPaymentMethodType.Value == (int)PaymentMethodType.Oac)
                    {
                        dynamic.PaymentMethod = _paymentMethodMapper.FromModel(model.Customer.Oac);
                    }
                    else if (model.IdPaymentMethodType.Value == (int)PaymentMethodType.Check)
                    {
                        dynamic.PaymentMethod = _paymentMethodMapper.FromModel(model.Customer.Check);
                    }
                    else
                    {
                        dynamic.PaymentMethod = new OrderPaymentMethodDynamic();
                    }
                    dynamic.PaymentMethod.IdObjectType = model.IdPaymentMethodType.Value;
                }
            }
        }
    }
}