using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using VC.Admin.Models.Customer;
using VC.Admin.Models.Customers;
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
using VitalChoice.SharedWeb.Helpers;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;

namespace VC.Admin.ModelConverters
{
    public class OrderModelConverter : BaseModelConverter<OrderManageModel, OrderDynamic>
    {
        private readonly IDynamicMapper<OrderPaymentMethodDynamic, OrderPaymentMethod> _paymentMethodMapper;
        private readonly IDynamicMapper<AddressDynamic, OrderAddress> _addressMapper;
        private readonly ICustomerService _customerService;
        private readonly IDiscountService _discountService;
        private readonly IPromotionService _promotionService;
        private readonly IGcService _gcService;
        private readonly IProductService _productService;
        private readonly IEcommerceRepositoryAsync<OrderToGiftCertificate> _gcInOrderRep;

        public OrderModelConverter(IDynamicMapper<AddressDynamic, OrderAddress> addressMapper,
            IDynamicMapper<OrderPaymentMethodDynamic, OrderPaymentMethod> paymentMethodMapper, ICustomerService customerService,
            IDiscountService discountService, IGcService gcService, IProductService productService, IPromotionService promotionService,
            IEcommerceRepositoryAsync<OrderToGiftCertificate> gcInOrderRep)
        {
            _addressMapper = addressMapper;
            _paymentMethodMapper = paymentMethodMapper;
            _customerService = customerService;
            _discountService = discountService;
            _gcService = gcService;
            _productService = productService;
            _promotionService = promotionService;
            _gcInOrderRep = gcInOrderRep;
        }

        public override async Task DynamicToModelAsync(OrderManageModel model, OrderDynamic dynamic)
        {
            if(dynamic.Customer!=null)
            {
                model.IdCustomer = dynamic.Customer.Id;
            }

            if(dynamic.Discount!=null)
            {
                model.DiscountCode = dynamic.Discount.Code;
                model.DiscountMessage = dynamic.Discount.GetDiscountMessage((int?)dynamic.SafeData.IdDiscountTier);
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
                    model.GiftCertificatesSubtotal += item.Amount;
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
            
            if (dynamic.PromoSkus != null)
            {
                model.PromoSkus = new List<PromoSkuOrderedManageModel>();
                foreach (var item in dynamic.PromoSkus)
                {
                    model.PromoSkus.Add(new PromoSkuOrderedManageModel(item));
                }
            }

            if (!model.ShipDelayType.HasValue || model.IdObjectType == (int)OrderType.AutoShip)
            {
                model.ShipDelayType = ShipDelayType.None;
            }

            if(model.OrderStatus.HasValue)
            {
                model.CombinedEditOrderStatus = model.OrderStatus.Value;
            }
            else
            {
                model.ShouldSplit = true;
                if(model.POrderStatus==OrderStatus.Cancelled || model.NPOrderStatus==OrderStatus.Cancelled)
                {
                    model.CombinedEditOrderStatus = OrderStatus.Cancelled;
                } else if(model.POrderStatus == OrderStatus.Shipped || model.NPOrderStatus == OrderStatus.Shipped)
                {
                    model.CombinedEditOrderStatus = OrderStatus.Shipped;
                }
                else if (model.POrderStatus == OrderStatus.Exported || model.NPOrderStatus == OrderStatus.Exported)
                {
                    model.CombinedEditOrderStatus = OrderStatus.Exported;
                }
                else if (model.POrderStatus == OrderStatus.OnHold || model.NPOrderStatus == OrderStatus.OnHold)
                {
                    model.CombinedEditOrderStatus = OrderStatus.OnHold;
                }
                else if (model.POrderStatus == OrderStatus.Processed || model.NPOrderStatus == OrderStatus.Processed ||
                    model.POrderStatus == OrderStatus.ShipDelayed || model.NPOrderStatus == OrderStatus.ShipDelayed)
                {
                    model.CombinedEditOrderStatus = OrderStatus.Processed;
                } else if (model.POrderStatus == OrderStatus.Incomplete || model.NPOrderStatus == OrderStatus.Incomplete)
                {
                    model.CombinedEditOrderStatus = OrderStatus.Incomplete;
                }
            }

            model.Shipping = await _addressMapper.ToModelAsync<AddressModel>(dynamic.ShippingAddress);
            if (dynamic.PaymentMethod != null)
            {
                model.IdPaymentMethodType = dynamic.PaymentMethod.IdObjectType;
                switch (dynamic.PaymentMethod.IdObjectType)
                {
                    case (int) PaymentMethodType.CreditCard:
                        model.CreditCard = await _paymentMethodMapper.ToModelAsync<CreditCardModel>(dynamic.PaymentMethod);
                        break;
                    case (int) PaymentMethodType.Oac:
                        model.Oac = await _paymentMethodMapper.ToModelAsync<OacPaymentModel>(dynamic.PaymentMethod);
                        break;
                    case (int) PaymentMethodType.Check:
                        model.Check = await _paymentMethodMapper.ToModelAsync<CheckPaymentModel>(dynamic.PaymentMethod);
                        break;
                    case (int) PaymentMethodType.WireTransfer:
                        model.WireTransfer = await _paymentMethodMapper.ToModelAsync<WireTransferPaymentModel>(dynamic.PaymentMethod);
                        break;
                    case (int) PaymentMethodType.Marketing:
                        model.Marketing = await _paymentMethodMapper.ToModelAsync<MarketingPaymentModel>(dynamic.PaymentMethod);
                        break;
                    case (int) PaymentMethodType.VCWellnessEmployeeProgram:
                        model.VCWellness = await _paymentMethodMapper.ToModelAsync<VCWellnessEmployeeProgramPaymentModel>(dynamic.PaymentMethod);
                        break;
                }
            }
        }

        public override async Task ModelToDynamicAsync(OrderManageModel model, OrderDynamic dynamic)
        {
            if (!string.IsNullOrEmpty(model.DiscountCode))
            {
                dynamic.Discount = await _discountService.GetByCode(model.DiscountCode);
            }

            await ModelToGcsDynamic(model, dynamic);
            await ModelToSkusDynamic(model, dynamic);

            if (dynamic.SafeData.ShipDelayType!=null)
            {
                if (dynamic.SafeData.ShipDelayType == ShipDelayType.None)
                {
                    dynamic.Data.ShipDelayType = null;
                    dynamic.Data.ShipDelayDate = null;
                    dynamic.Data.ShipDelayDateP = null;
                    dynamic.Data.ShipDelayDateNP = null;
                }
                if (dynamic.SafeData.ShipDelayType == ShipDelayType.EntireOrder)
                {
                    dynamic.Data.ShipDelayDateP = null;
                    dynamic.Data.ShipDelayDateNP = null;
                }
                if (dynamic.SafeData.ShipDelayType == ShipDelayType.PerishableAndNonPerishable)
                {
                    dynamic.Data.ShipDelayDate = null;
                    if (!model.ShouldSplit)
                    {
                        dynamic.Data.ShipDelayType = null;
                        dynamic.Data.ShipDelayDateP = null;
                        dynamic.Data.ShipDelayDateNP = null;
                    }
                }
            }

            if (!model.UseShippingAndBillingFromCustomer)
            {
                if (model.Shipping != null)
                {
                    var addressDynamic = await _addressMapper.FromModelAsync(model.Shipping, (int)AddressType.Shipping);
                    dynamic.ShippingAddress = addressDynamic;
                }
            }
            else
            {
                var shippingAddress = model.Customer?.Shipping.FirstOrDefault(p => p.IsSelected);
                if(shippingAddress!=null)
                {
                    var addressDynamic = await _addressMapper.FromModelAsync(shippingAddress, (int)AddressType.Shipping);
                    dynamic.ShippingAddress = addressDynamic;
                }
            }

            await ModelToPaymentDynamic(model, dynamic);

            await UpdateCustomer(model, dynamic);
        }

        private async Task UpdateCustomer(OrderManageModel model, OrderDynamic dynamic)
        {
            if (model.Customer != null)
            {
                //update customer
                var dbCustomer = await _customerService.SelectAsync(model.Customer.Id, true);
                if (dbCustomer != null)
                {
                    dbCustomer.IdObjectType = (int) model.Customer.CustomerType;
                    dbCustomer.CustomerNotes = dynamic.Customer.CustomerNotes;
                    dbCustomer.Files = dynamic.Customer.Files;
                    if (dynamic.Customer.DictionaryData.ContainsKey("Source"))
                    {
                        dbCustomer.Data.Source = dynamic.Customer.Data.Source;
                    }
                    if (dynamic.Customer.DictionaryData.ContainsKey("SourceDetails"))
                    {
                        dbCustomer.Data.SourceDetails = dynamic.Customer.Data.SourceDetails;
                    }
                    if (model.Id == 0)
                    {
                        dbCustomer.ApprovedPaymentMethods = dynamic.Customer.ApprovedPaymentMethods;
                        dbCustomer.OrderNotes = dynamic.Customer.OrderNotes;
                        dbCustomer.Email = dynamic.Customer.Email;
                        dbCustomer.ProfileAddress = dynamic.Customer.ProfileAddress;

                        if (model.UpdateShippingAddressForCustomer)
                        {
                            dbCustomer.ShippingAddresses = dynamic.Customer.ShippingAddresses;
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
                                if(method.DictionaryData.ContainsKey("CheckNumber"))
                                {
                                    method.DictionaryData.Remove("CheckNumber");
                                }
                                if (method.DictionaryData.ContainsKey("PaidInFull"))
                                {
                                    method.DictionaryData.Remove("PaidInFull");
                                }
                                dbCustomer.CustomerPaymentMethods.Add(method);
                            }
                        }
                        if (model.UpdateCheckForCustomer && dynamic.PaymentMethod != null)
                        {
                            RemovePaymentMethodsFromDBCustomer(dbCustomer, dynamic.PaymentMethod.IdObjectType,
                                PaymentMethodType.WireTransfer);
                            foreach (
                                var method in
                                    dynamic.Customer.CustomerPaymentMethods.Where(
                                        p => p.IdObjectType == (int)PaymentMethodType.WireTransfer))
                            {
                                if (method.DictionaryData.ContainsKey("PaymentComment"))
                                {
                                    method.DictionaryData.Remove("PaymentComment");
                                }
                                dbCustomer.CustomerPaymentMethods.Add(method);
                            }
                        }
                        if (model.UpdateCheckForCustomer && dynamic.PaymentMethod != null)
                        {
                            RemovePaymentMethodsFromDBCustomer(dbCustomer, dynamic.PaymentMethod.IdObjectType,
                                PaymentMethodType.Marketing);
                            foreach (
                                var method in
                                    dynamic.Customer.CustomerPaymentMethods.Where(
                                        p => p.IdObjectType == (int)PaymentMethodType.Marketing))
                            {
                                if (method.DictionaryData.ContainsKey("PaymentComment"))
                                {
                                    method.DictionaryData.Remove("PaymentComment");
                                }
                                if (method.DictionaryData.ContainsKey("MarketingPromotionType"))
                                {
                                    method.DictionaryData.Remove("MarketingPromotionType");
                                }
                                dbCustomer.CustomerPaymentMethods.Add(method);
                            }
                        }
                        if (model.UpdateCheckForCustomer && dynamic.PaymentMethod != null)
                        {
                            RemovePaymentMethodsFromDBCustomer(dbCustomer, dynamic.PaymentMethod.IdObjectType,
                                PaymentMethodType.VCWellnessEmployeeProgram);
                            foreach (
                                var method in
                                    dynamic.Customer.CustomerPaymentMethods.Where(
                                        p => p.IdObjectType == (int)PaymentMethodType.VCWellnessEmployeeProgram))
                            {
                                if (method.DictionaryData.ContainsKey("PaymentComment"))
                                {
                                    method.DictionaryData.Remove("PaymentComment");
                                }
                                dbCustomer.CustomerPaymentMethods.Add(method);
                            }
                        }
                    }

                    dynamic.Customer = dbCustomer;
                }
            }
        }

        private async Task ModelToPaymentDynamic(OrderManageModel model, OrderDynamic dynamic)
        {
            if (!model.UseShippingAndBillingFromCustomer)
            {
                if (model.IdPaymentMethodType.HasValue)
                {
                    switch (model.IdPaymentMethodType.Value)
                    {
                        case (int) PaymentMethodType.CreditCard:
                            dynamic.PaymentMethod = await _paymentMethodMapper.FromModelAsync(model.CreditCard, model.IdPaymentMethodType.Value);
                            break;
                        case (int) PaymentMethodType.Oac:
                            dynamic.PaymentMethod = await _paymentMethodMapper.FromModelAsync(model.Oac, model.IdPaymentMethodType.Value);
                            break;
                        case (int) PaymentMethodType.Check:
                            dynamic.PaymentMethod = await _paymentMethodMapper.FromModelAsync(model.Check, model.IdPaymentMethodType.Value);
                            break;
                        case (int)PaymentMethodType.WireTransfer:
                            dynamic.PaymentMethod = await _paymentMethodMapper.FromModelAsync(model.WireTransfer, model.IdPaymentMethodType.Value);
                            break;
                        case (int)PaymentMethodType.Marketing:
                            dynamic.PaymentMethod = await _paymentMethodMapper.FromModelAsync(model.Marketing, model.IdPaymentMethodType.Value);
                            break;
                        case (int)PaymentMethodType.VCWellnessEmployeeProgram:
                            dynamic.PaymentMethod = await _paymentMethodMapper.FromModelAsync(model.VCWellness, model.IdPaymentMethodType.Value);
                            break;
                        default:
                            dynamic.PaymentMethod = new OrderPaymentMethodDynamic() {IdObjectType = model.IdPaymentMethodType.Value };
                            break;
                    }
                }
            }
            else
            {
                if (model.IdPaymentMethodType.HasValue)
                {
                    switch (model.IdPaymentMethodType.Value)
                    {
                        case (int) PaymentMethodType.CreditCard:
                            var card = model.Customer?.CreditCards.FirstOrDefault(p => p.IsSelected);
                            if (card != null)
                            {
                                dynamic.PaymentMethod = await _paymentMethodMapper.FromModelAsync(card, model.IdPaymentMethodType.Value);
                            }
                            break;
                        case (int) PaymentMethodType.Oac:
                            if (model.Customer != null)
                                dynamic.PaymentMethod = await _paymentMethodMapper.FromModelAsync(model.Customer.Oac, model.IdPaymentMethodType.Value);
                            break;
                        case (int) PaymentMethodType.Check:
                            if (model.Customer != null)
                                dynamic.PaymentMethod = await _paymentMethodMapper.FromModelAsync(model.Customer.Check, model.IdPaymentMethodType.Value);
                            break;
                        case (int)PaymentMethodType.WireTransfer:
                            if (model.Customer != null)
                                dynamic.PaymentMethod = await _paymentMethodMapper.FromModelAsync(model.Customer.WireTransfer, model.IdPaymentMethodType.Value);
                            break;
                        case (int)PaymentMethodType.Marketing:
                            if (model.Customer != null)
                                dynamic.PaymentMethod = await _paymentMethodMapper.FromModelAsync(model.Customer.Marketing, model.IdPaymentMethodType.Value);
                            break;
                        case (int)PaymentMethodType.VCWellnessEmployeeProgram:
                            if (model.Customer != null)
                                dynamic.PaymentMethod = await _paymentMethodMapper.FromModelAsync(model.Customer.VCWellness, model.IdPaymentMethodType.Value);
                            break;
                        default:
                            dynamic.PaymentMethod = new OrderPaymentMethodDynamic() { IdObjectType = model.IdPaymentMethodType.Value };
                            break;
                    }
                }
            }
            if (dynamic.PaymentMethod?.Address != null)
            {
                dynamic.PaymentMethod.Address.IdObjectType = (int)AddressType.Billing;
            }
        }

        private async Task ModelToGcsDynamic(OrderManageModel model, OrderDynamic dynamic)
        {
            if (model.GCs != null)
            {
                if (model.GCs.Any())
                {
                    ICollection<string> codes =
                        model.GCs.Select(g => g.Code).Where(c => !string.IsNullOrWhiteSpace(c)).ToList();
                    var gcs = await _gcService.GetGiftCertificatesAsync(g => codes.Contains(g.Code));
                    var gcIds = gcs.Select(g => g.Id).Distinct().ToList();
                    var gcsOrdered = await _gcInOrderRep.Query(g => gcIds.Contains(g.IdGiftCertificate) && g.IdOrder == dynamic.Id).SelectAsync(false);
                    dynamic.GiftCertificates = gcs.Select(g => new GiftCertificateInOrder
                    {
                        GiftCertificate = g,
                        Amount = gcsOrdered.Where(gco => gco.IdGiftCertificate == g.Id).Select(gco => gco.Amount).FirstOrDefault()
                    }).ToList();
                }
            }
        }

        private async Task ModelToSkusDynamic(OrderManageModel model, OrderDynamic dynamic)
        {
            if (model.SkuOrdereds != null)
            {
                var promotionIds = model.PromoSkus.Where(p => p.Id.HasValue && p.IsAllowDisable).Select(p => p.Id.Value).ToList();
                var promotions = await _promotionService.SelectAsync(promotionIds, true);
                dynamic.PromoSkus = promotions.Select(p => new PromoOrdered
                {
                    Promotion = p,
                    Enabled = model.PromoSkus?.FirstOrDefault(m => m.Id.HasValue && m.Id.Value == p.Id)?.IsEnabled ?? true
                }).ToList();
                model.SkuOrdereds = model.SkuOrdereds.Where(s => !(s.Promo ?? false)).ToList();

                var validList = model.SkuOrdereds.Where(s => s.Id.HasValue).Select(s => s.Id.Value).ToList();
                var notValidList = model.SkuOrdereds.Where(s => !s.Id.HasValue).Select(s => s.Code).ToList();
                var validSkus = await _productService.GetSkusOrderedAsync(validList);
                var notValidSkus = await _productService.GetSkusOrderedAsync(notValidList);
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