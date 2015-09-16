using System;
using System.Linq;
using System.Collections.Generic;
using VC.Admin.Models.Customer;
using VC.Admin.Models.Order;
using VC.Admin.Models.Product;
using VitalChoice.Business.Queries.Product;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.GiftCertificates;
using VitalChoice.Domain.Transfer.Orders;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Entities.Transfer;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Domain.Entities.eCommerce.Payment;
using VitalChoice.Domain.Entities.eCommerce.Addresses;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.eCommerce.Discounts;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Transfer.Products;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Products;

namespace VC.Admin.ModelConverters
{
    public class OrderModelConverter : IModelToDynamicConverter<OrderManageModel, OrderDynamic>
    {
        private readonly IDynamicToModelMapper<OrderPaymentMethodDynamic> _paymentMethodMapper;
        private readonly IDynamicToModelMapper<OrderAddressDynamic> _addressMapper;
        private readonly ICustomerService _customerService;
        private readonly IDiscountService _discountService;
        private readonly IGcService _gcService;
        private readonly IProductService _skuService;
        private readonly IEcommerceDynamicObjectService<ProductDynamic, Product, ProductOptionType, ProductOptionValue> _productService;

        public OrderModelConverter(IDynamicToModelMapper<OrderAddressDynamic> addressMapper,
            IDynamicToModelMapper<OrderPaymentMethodDynamic> paymentMethodMapper, ICustomerService customerService,
            IDiscountService discountService, IGcService gcService,
            IEcommerceDynamicObjectService<ProductDynamic, Product, ProductOptionType, ProductOptionValue>
                productService, IProductService skuService)
        {
            _addressMapper = addressMapper;
            _paymentMethodMapper = paymentMethodMapper;
            _customerService = customerService;
            _discountService = discountService;
            _gcService = gcService;
            _productService = productService;
            _skuService = skuService;
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
            if(model.Customer!=null)
            {
                dynamic.Customer = _customerService.Select(model.Customer.Id, true);
            }

            if (!string.IsNullOrEmpty(model.DiscountCode))
            {
                dynamic.Discount =
                    _discountService.Select(new DiscountQuery().WithCode(model.DiscountCode).NotDeleted(), true)
                        .FirstOrDefault();
            }

            if(model.GCs!=null)
            {
                if (model.GCs?.Any() ?? false)
                {
                    ICollection<string> codes =
                        model.GCs.Select(g => g.Code).Where(c => !string.IsNullOrWhiteSpace(c)).ToList();
                    var task = _gcService.GetGiftCertificatesAsync(g => codes.Contains(g.Code));
                    task.Wait();
                    dynamic.GiftCertificates = task.Result.Select(g => new GiftCertificateInOrder
                    {
                        GiftCertificate = g,
                        Amount = 0
                    }).ToList();
                }
            }

            if (model.SkuOrdereds != null)
            {
                var validList = model.SkuOrdereds.Where(s => s.Id.HasValue && s.IdProductType.HasValue).ToList();
                var notValidList = model.SkuOrdereds.Where(s => !s.Id.HasValue || !s.IdProductType.HasValue).ToList();
                Dictionary<int, SkuOrderedManageModel> keyedCollection =
                    // ReSharper disable once PossibleInvalidOperationException
                    validList.ToDictionary(s => s.Id.Value, s => s);
                dynamic.Skus =
                    _skuService.GetSkus(validList.Select(s => new SkuInfo
                    {
                        // ReSharper disable once PossibleInvalidOperationException
                        Id = s.Id.Value,
                        // ReSharper disable once PossibleInvalidOperationException
                        IdProductType = (ProductType) s.IdProductType
                    }).ToList(), true).Select(s =>
                    {
                        var orderedItem = keyedCollection[s.Id];
                        return new SkuOrdered
                        {
                            Sku = s,
                            Amount = orderedItem.Price ?? 0,
                            Quantity = orderedItem.QTY ?? 0,
                            ProductWithoutSkus = _productService.Select(s.IdProduct)
                        };
                    }).Union(_skuService.GetSkus(notValidList.Select(s => s.Code).ToList(), true).Select(s =>
                    {
                        var orderedItem = keyedCollection[s.Id];
                        return new SkuOrdered
                        {
                            Sku = s,
                            Amount = orderedItem.Price ?? 0,
                            Quantity = orderedItem.QTY ?? 0,
                            ProductWithoutSkus = _productService.Select(s.IdProduct)
                        };
                    })).ToList();
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
                    if (dynamic.PaymentMethod != null)
                    {
                        dynamic.PaymentMethod.IdObjectType = model.IdPaymentMethodType.Value;
                    }
                }
            }
            else
            {
                var shippingAddress = model.Customer?.Shipping.FirstOrDefault(p => p.IsSelected);
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
                        var card = model.Customer?.CreditCards.FirstOrDefault(p => p.IsSelected);
                        if (card != null)
                        {
                            dynamic.PaymentMethod = _paymentMethodMapper.FromModel(card);
                        }
                    }
                    else if (model.IdPaymentMethodType.Value == (int)PaymentMethodType.Oac)
                    {
                        if (model.Customer != null)
                            dynamic.PaymentMethod = _paymentMethodMapper.FromModel(model.Customer.Oac);
                    }
                    else if (model.IdPaymentMethodType.Value == (int)PaymentMethodType.Check)
                    {
                        if (model.Customer != null)
                            dynamic.PaymentMethod = _paymentMethodMapper.FromModel(model.Customer.Check);
                    }
                    else
                    {
                        dynamic.PaymentMethod = new OrderPaymentMethodDynamic();
                    }
                    if (dynamic.PaymentMethod != null)
                    {
                        dynamic.PaymentMethod.IdObjectType = model.IdPaymentMethodType.Value;
                    }
                }
            }
        }
    }
}