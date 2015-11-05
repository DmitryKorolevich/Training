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
using VitalChoice.Domain.Helpers;
using VitalChoice.Domain.Transfer.Products;
using VitalChoice.DynamicData.Base;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Products;

namespace VC.Admin.ModelConverters
{
    public class OrderModelConverter : BaseModelConverter<OrderManageModel, OrderDynamic>
    {
        private readonly IDynamicMapper<OrderPaymentMethodDynamic> _paymentMethodMapper;
        private readonly IDynamicMapper<OrderAddressDynamic> _addressMapper;
        private readonly ICustomerService _customerService;
        private readonly IDiscountService _discountService;
        private readonly IGcService _gcService;
        private readonly IProductService _productService;

        public OrderModelConverter(IDynamicMapper<OrderAddressDynamic> addressMapper,
            IDynamicMapper<OrderPaymentMethodDynamic> paymentMethodMapper, ICustomerService customerService,
            IDiscountService discountService, IGcService gcService, IProductService productService)
        {
            _addressMapper = addressMapper;
            _paymentMethodMapper = paymentMethodMapper;
            _customerService = customerService;
            _discountService = discountService;
            _gcService = gcService;
            _productService = productService;
        }

        public override void DynamicToModel(OrderManageModel model, OrderDynamic dynamic)
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
            else
            {
                model.GCs = new List<GCListItemModel>() { new GCListItemModel(null) };
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
            if (dynamic.PaymentMethod != null)
            {
                model.IdPaymentMethodType = dynamic.PaymentMethod.IdObjectType;
                if (dynamic.PaymentMethod.IdObjectType == (int) PaymentMethodType.CreditCard)
                {
                    model.CreditCard = _paymentMethodMapper.ToModel<CreditCardModel>(dynamic.PaymentMethod);
                }
                if (dynamic.PaymentMethod.IdObjectType == (int) PaymentMethodType.Oac)
                {
                    model.Oac = _paymentMethodMapper.ToModel<OacPaymentModel>(dynamic.PaymentMethod);
                }
                if (dynamic.PaymentMethod.IdObjectType == (int) PaymentMethodType.Check)
                {
                    model.Check = _paymentMethodMapper.ToModel<CheckPaymentModel>(dynamic.PaymentMethod);
                }
            }
        }

        public override void ModelToDynamic(OrderManageModel model, OrderDynamic dynamic)
        {
            if (!string.IsNullOrEmpty(model.DiscountCode))
            {
                dynamic.Discount =
                    _discountService.Select(queryObject: new DiscountQuery().WithCode(model.DiscountCode).NotDeleted(), withDefaults: true)
                        .FirstOrDefault();
            }

            ModelToGcsDynamic(model, dynamic);

            ModelToSkusDynamic(model, dynamic);

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
            }

            ModelToPaymentDynamic(model, dynamic);

            UpdateCustomer(model, dynamic);
        }

        private void UpdateCustomer(OrderManageModel model, OrderDynamic dynamic)
        {
            if (model.Customer != null)
            {
                //update customer
                var dbCustomer = _customerService.Select(model.Customer.Id, true);
                if (dbCustomer != null)
                {
                    dbCustomer.IdObjectType = (int) model.Customer.CustomerType;
                    dbCustomer.CustomerNotes = dynamic.Customer.CustomerNotes;
                    dbCustomer.Files = dynamic.Customer.Files;
                    if (model.Id == 0)
                    {
                        if (dynamic.Customer.DictionaryData.ContainsKey("Source"))
                        {
                            dbCustomer.Data.Source = dynamic.Customer.Data.Source;
                        }
                        if (dynamic.Customer.DictionaryData.ContainsKey("SourceDetails"))
                        {
                            dbCustomer.Data.SourceDetails = dynamic.Customer.Data.SourceDetails;
                        }
                        dbCustomer.ApprovedPaymentMethods = dynamic.Customer.ApprovedPaymentMethods;
                        dbCustomer.OrderNotes = dynamic.Customer.OrderNotes;

                        var profileAddress =
                            dbCustomer.Addresses.FirstOrDefault(p => p.IdObjectType == (int) AddressType.Profile);
                        if (profileAddress != null)
                        {
                            dbCustomer.Addresses.Remove(profileAddress);
                        }
                        profileAddress =
                            dynamic.Customer.Addresses.FirstOrDefault(p => p.IdObjectType == (int) AddressType.Profile);
                        if (profileAddress != null)
                        {
                            dbCustomer.Addresses.Add(profileAddress);
                        }

                        if (model.UpdateShippingAddressForCustomer)
                        {
                            var shippingAddresses =
                                dbCustomer.Addresses.Where(p => p.IdObjectType == (int) AddressType.Shipping).ToList();
                            foreach (var address in shippingAddresses)
                            {
                                dbCustomer.Addresses.Remove(address);
                            }
                            shippingAddresses =
                                dynamic.Customer.Addresses.Where(p => p.IdObjectType == (int) AddressType.Shipping).ToList();
                            foreach (var address in shippingAddresses)
                            {
                                dbCustomer.Addresses.Add(address);
                            }
                        }

                        if (model.UpdateCardForCustomer && dynamic.PaymentMethod != null)
                        {
                            RemovePaymentMethodsFromDBCustomer(dbCustomer, dynamic.PaymentMethod.IdObjectType,
                                PaymentMethodType.CreditCard);
                            foreach (
                                var method in
                                    dynamic.Customer.CustomerPaymentMethods.Where(
                                        p => p.IdObjectType == (int) PaymentMethodType.CreditCard))
                            {
                                dbCustomer.CustomerPaymentMethods.Add(method);
                            }
                        }
                        if (model.UpdateOACForCustomer && dynamic.PaymentMethod != null)
                        {
                            RemovePaymentMethodsFromDBCustomer(dbCustomer, dynamic.PaymentMethod.IdObjectType,
                                PaymentMethodType.Oac);
                            foreach (
                                var method in
                                    dynamic.Customer.CustomerPaymentMethods.Where(
                                        p => p.IdObjectType == (int) PaymentMethodType.Oac))
                            {
                                dbCustomer.CustomerPaymentMethods.Add(method);
                            }
                        }
                        if (model.UpdateCheckForCustomer && dynamic.PaymentMethod != null)
                        {
                            RemovePaymentMethodsFromDBCustomer(dbCustomer, dynamic.PaymentMethod.IdObjectType,
                                PaymentMethodType.Check);
                            foreach (
                                var method in
                                    dynamic.Customer.CustomerPaymentMethods.Where(
                                        p => p.IdObjectType == (int) PaymentMethodType.Check))
                            {
                                dbCustomer.CustomerPaymentMethods.Add(method);
                            }
                        }
                    }

                    dynamic.Customer = dbCustomer;
                }
            }
        }

        private void ModelToPaymentDynamic(OrderManageModel model, OrderDynamic dynamic)
        {
            if (model.Id != 0)
            {
                if (model.IdPaymentMethodType.HasValue)
                {
                    if (model.IdPaymentMethodType.Value == (int) PaymentMethodType.CreditCard)
                    {
                        dynamic.PaymentMethod = _paymentMethodMapper.FromModel(model.CreditCard);
                    }
                    else if (model.IdPaymentMethodType.Value == (int) PaymentMethodType.Oac)
                    {
                        dynamic.PaymentMethod = _paymentMethodMapper.FromModel(model.Oac);
                    }
                    else if (model.IdPaymentMethodType.Value == (int) PaymentMethodType.Check)
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
                if (model.IdPaymentMethodType.HasValue)
                {
                    if (model.IdPaymentMethodType.Value == (int) PaymentMethodType.CreditCard)
                    {
                        var card = model.Customer?.CreditCards.FirstOrDefault(p => p.IsSelected);
                        if (card != null)
                        {
                            dynamic.PaymentMethod = _paymentMethodMapper.FromModel(card);
                        }
                    }
                    else if (model.IdPaymentMethodType.Value == (int) PaymentMethodType.Oac)
                    {
                        if (model.Customer != null)
                            dynamic.PaymentMethod = _paymentMethodMapper.FromModel(model.Customer.Oac);
                    }
                    else if (model.IdPaymentMethodType.Value == (int) PaymentMethodType.Check)
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

        private void ModelToGcsDynamic(OrderManageModel model, OrderDynamic dynamic)
        {
            if (model.GCs != null)
            {
                if (model.GCs.Any())
                {
                    ICollection<string> codes =
                        model.GCs.Select(g => g.Code).Where(c => !string.IsNullOrWhiteSpace(c)).ToList();
                    var task = _gcService.GetGiftCertificatesAsync(g => codes.Contains(g.Code));
                    dynamic.GiftCertificates = task.Result.Select(g => new GiftCertificateInOrder
                    {
                        GiftCertificate = g,
                        Amount = 0
                    }).ToList();
                }
            }
        }

        private void ModelToSkusDynamic(OrderManageModel model, OrderDynamic dynamic)
        {
            if (model.SkuOrdereds != null)
            {
                model.SkuOrdereds = model.SkuOrdereds.Where(s => !(s.Promo ?? false)).ToList();

                var validList = model.SkuOrdereds.Where(s => s.Id.HasValue).Select(s => s.Id.Value).ToList();
                var notValidList = model.SkuOrdereds.Where(s => !s.Id.HasValue).Select(s => s.Code).ToList();
                var task = _productService.GetSkusOrderedAsync(validList);
                var validSkus = task.Result;
                task = _productService.GetSkusOrderedAsync(notValidList);
                var notValidSkus = task.Result;
                Dictionary<int, SkuOrderedManageModel> valid =
                    // ReSharper disable once PossibleInvalidOperationException
                    model.SkuOrdereds.Where(s => s.Id.HasValue).ToDictionary(s => s.Id.Value, s => s);
                Dictionary<string, SkuOrderedManageModel> notValid =
                    model.SkuOrdereds.Where(s => !s.Id.HasValue).ToDictionary(s => s.Code, s => s);
                foreach (var sku in validSkus)
                {
                    var item = valid[sku.Sku.Id];
                    sku.Amount = item.Price ?? 0;
                    sku.Quantity = item.QTY ?? 0;
                }
                foreach (var sku in notValidSkus)
                {
                    var item = notValid[sku.Sku.Code];
                    sku.Amount = item.Price ?? 0;
                    sku.Quantity = item.QTY ?? 0;
                }
                dynamic.Skus = new List<SkuOrdered>(validSkus.Union(notValidSkus));
            }
        }

        private void RemovePaymentMethodsFromDBCustomer(CustomerDynamic customer, int? orderPaymentMethod, PaymentMethodType method)
        {
            if (orderPaymentMethod == (int)method)
            {
                var customerPaymentMethods = customer.CustomerPaymentMethods.Where(p => p.IdObjectType == (int)method).ToList();
                foreach (var customerPaymentMethod in customerPaymentMethods)
                {
                    customer.CustomerPaymentMethods.Remove(customerPaymentMethod);
                }
            }
        }
    }
}