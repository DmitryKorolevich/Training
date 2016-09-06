using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using VitalChoice.Business.Services.VeraCore;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Dynamic;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.ExportService.Context;
using VitalChoice.Infrastructure.Domain.Avatax;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Shipping;
using VitalChoice.Interfaces.Services.Avatax;
using VitalChoice.Interfaces.Services.Orders;

namespace VitalChoice.ExportService.Services
{
    public class VeraCoreExportService : IVeraCoreExportService
    {
        private readonly IOrderService _orderService;
        private readonly IAvalaraTax _avalaraTax;
        private readonly IDynamicMapper<AddressDynamic, OrderAddress> _addressMapper;
        private readonly VeraCoreOrderSoapClient _client;
        private const string OacDescription = "On Approved Credit";
        private const string MarketingDonationDescription = "Marketing-Donation";
        private const string MarketingPromoDescription = "Marketing-Promo";
        private const string VcWellnessDescription = "VC Wellness";
        //private const string ManualCcDescription = "Manual Credit Card Process";
        private const string CheckDescription = "Check Payment";
        private const string NoChargeDescription = "No Charge";
        private const string PrepaidDescription = "Prepaid";
        private const string GcReDeem = "GCREDEEM";
        private const string AmountDiscountSku = "PROCRED";


        public VeraCoreExportService(IOptions<ExportOptions> options, IOrderService orderService, IAvalaraTax avalaraTax,
            IDynamicMapper<AddressDynamic, OrderAddress> addressMapper)
        {
            _orderService = orderService;
            _avalaraTax = avalaraTax;
            _addressMapper = addressMapper;
            _client = new VeraCoreOrderSoapClient
            {
                DebugHeaderValue = new DebugHeader(),
                AuthenticationHeaderValue = new AuthenticationHeader
                {
                    Username = options.Value.VeraCore.UserName,
                    Password = options.Value.VeraCore.Password
                }
            };
        }

        public async Task ExportOrder(OrderDynamic order, ExportSide exportSide)
        {
            var status = order.OrderStatus;
            var npStatus = order.NPOrderStatus;
            var pStatus = order.POrderStatus;

            var context = await _orderService.CalculateOrder(order, OrderStatus.Processed);

            //restore order statuses
            order.OrderStatus = status;
            order.NPOrderStatus = npStatus;
            order.POrderStatus = pStatus;

            if (context.SplitInfo.ShouldSplit)
            {
                VeraCoreExportOrder nonPerishablePart;
                VeraCoreExportOrder perishablePart;
                switch (exportSide)
                {
                    case ExportSide.All:
                        if (order.POrderStatus != OrderStatus.Processed)
                        {
                            throw new ApiException($"Cannot export order {order.Id}. Invalid P status: {order.POrderStatus}");
                        }
                        if (order.NPOrderStatus != OrderStatus.Processed)
                        {
                            throw new ApiException($"Cannot export order {order.Id}. Invalid NP status: {order.POrderStatus}");
                        }

                        perishablePart = await CreateExportFromOrder(order, context, ExportSide.Perishable);
                        if (_client.AddOrder(perishablePart) == null)
                        {
                            throw new ApiException("Export failed.");
                        }
                        await _avalaraTax.GetTax(context, TaxGetType.SavePermanent | TaxGetType.Perishable);
                        UpdateOrderStatus(order, context, ExportSide.Perishable);
                        nonPerishablePart = await CreateExportFromOrder(order, context, ExportSide.NonPerishable);
                        if (_client.AddOrder(nonPerishablePart) == null)
                        {
                            throw new ApiException("Export failed.");
                        }
                        await _avalaraTax.GetTax(context, TaxGetType.SavePermanent | TaxGetType.NonPerishable);
                        UpdateOrderStatus(order, context, ExportSide.NonPerishable);
                        break;
                    case ExportSide.Perishable:
                        if (order.POrderStatus != OrderStatus.Processed)
                        {
                            throw new ApiException($"Cannot export order {order.Id}. Invalid P status: {order.POrderStatus}");
                        }

                        perishablePart = await CreateExportFromOrder(order, context, ExportSide.Perishable);
                        if (_client.AddOrder(perishablePart) == null)
                        {
                            throw new ApiException("Export failed.");
                        }
                        await _avalaraTax.GetTax(context, TaxGetType.SavePermanent | TaxGetType.Perishable);
                        UpdateOrderStatus(order, context, ExportSide.Perishable);
                        break;
                    case ExportSide.NonPerishable:
                        if (order.NPOrderStatus != OrderStatus.Processed)
                        {
                            throw new ApiException($"Cannot export order {order.Id}. Invalid NP status: {order.POrderStatus}");
                        }

                        nonPerishablePart = await CreateExportFromOrder(order, context, ExportSide.NonPerishable);
                        if (_client.AddOrder(nonPerishablePart) == null)
                        {
                            throw new ApiException("Export failed.");
                        }
                        await _avalaraTax.GetTax(context, TaxGetType.SavePermanent | TaxGetType.NonPerishable);
                        UpdateOrderStatus(order, context, ExportSide.NonPerishable);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(exportSide), exportSide, null);
                }
            }
            else
            {
                if (order.OrderStatus != OrderStatus.Processed)
                {
                    throw new ApiException($"Cannot export order {order.Id}. Invalid status: {order.OrderStatus}");
                }

                var veracoreOrder = await CreateExportFromOrder(order, context, ExportSide.All);
                if (_client.AddOrder(veracoreOrder) == null)
                {
                    throw new ApiException("Export failed.");
                }
                await _avalaraTax.GetTax(context, TaxGetType.SavePermanent | TaxGetType.UseBoth);
                UpdateOrderStatus(order, context, ExportSide.All);
            }
            if (order.PaymentMethod.IdObjectType == (int) PaymentMethodType.CreditCard)
            {
                _paymentMapper.SecureObject(order.PaymentMethod);
            }
        }

        public async Task ExportRefund(OrderRefundDynamic order)
        {
            if (order.OrderStatus != OrderStatus.Processed)
            {
                throw new ApiException($"Cannot export order {order.Id}. Invalid status: {order.OrderStatus}");
            }
            var veracoreOrder = await CreateExportFromRefund(order);
            if (_client.AddOrder(veracoreOrder) == null)
            {
                throw new ApiException("Export failed.");
            }
            order.OrderStatus = OrderStatus.Exported;
        }

        private static string FormatCcType(CreditCardType internalCcType)
        {
            switch (internalCcType)
            {
                case CreditCardType.MasterCard:
                    return "Master Card";
                case CreditCardType.Visa:
                    return "VISA";
                case CreditCardType.AmericanExpress:
                    return "American Express";
                case CreditCardType.Discover:
                    return "Discover";
                default:
                    return "";
            }
        }

        private void UpdateOrderStatus(OrderDynamic order, OrderDataContext context, ExportSide exportSide)
        {
            if (context.SplitInfo.ShouldSplit)
            {
                switch (exportSide)
                {
                    case ExportSide.All:
                        order.POrderStatus = OrderStatus.Exported;
                        order.NPOrderStatus = OrderStatus.Exported;
                        break;
                    case ExportSide.Perishable:
                        order.POrderStatus = OrderStatus.Exported;
                        break;
                    case ExportSide.NonPerishable:
                        order.NPOrderStatus = OrderStatus.Exported;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(exportSide), exportSide, null);
                }
            }
            else
            {
                order.OrderStatus = OrderStatus.Exported;
            }
        }

        private void ParseGeneralInfo(MappedObject order, VeraCoreExportOrder promailOrder, ExportSide exportSide)
        {
            promailOrder.Header.EntryDate = order.DateCreated;
            promailOrder.Header.PONumber = order.SafeData.PoNumber;
            switch (exportSide)
            {
                case ExportSide.All:
                    promailOrder.Header.ID = order.Id.ToString(CultureInfo.InvariantCulture);
                    promailOrder.Header.Comments = order.SafeData.OrderNotes;
                    break;
                case ExportSide.Perishable:
                    promailOrder.Header.ID = order.Id.ToString(CultureInfo.InvariantCulture) + "-P";
                    promailOrder.Header.Comments = "Perishable Items\n" + (string) order.SafeData.OrderNotes;
                    break;
                case ExportSide.NonPerishable:
                    promailOrder.Header.ID = order.Id.ToString(CultureInfo.InvariantCulture) + "-NP";
                    promailOrder.Header.Comments = "Non-Perishable Items\n" + (string) order.SafeData.OrderNotes;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(exportSide), exportSide, null);
            }

            promailOrder.Header.InsertDate = DateTime.Now;
        }

        private async Task ParseBillingInfo(OrderPaymentMethodDynamic paymentMethod, CustomerDynamic customer, VeraCoreExportOrder veracoreOrder)
        {
            var address = paymentMethod.Address;
            if (address != null)
            {
                var orderedBy = await _addressMapper.ToModelAsync<OrderedBy>(address);
                orderedBy.UID = customer.Id.ToString(CultureInfo.InvariantCulture);
                orderedBy.Email = customer.Email;
                orderedBy.FullName = ((string) address.SafeData.FirstName ?? string.Empty) + " " +
                                     ((string) address.SafeData.LastName ?? string.Empty);

                veracoreOrder.OrderedBy = orderedBy;

                var billTo = await _addressMapper.ToModelAsync<OrderBillTo>(address);
                billTo.UID = customer.Id.ToString(CultureInfo.InvariantCulture);
                billTo.Email = customer.Email;
                billTo.FullName = ((string) address.SafeData.FirstName ?? string.Empty) + " " +
                                  ((string) address.SafeData.LastName ?? string.Empty);

                veracoreOrder.BillTo = billTo;
            }
        }

        private async Task ParseShippingInfo(OrderRefundDynamic order, VeraCoreExportOrder exportOrder)
        {
            var shipTo = await _addressMapper.ToModelAsync<OrderShipTo>(order.ShippingAddress);
            shipTo.Key = "1";
            shipTo.FullName = ((string) order.ShippingAddress.SafeData.FirstName ?? string.Empty) + " " +
                              ((string) order.ShippingAddress.SafeData.LastName ?? string.Empty);
            exportOrder.ShipTo = new[] {shipTo};
        }

        private async Task ParseShippingInfo(OrderDynamic order, OrderDataContext context, VeraCoreExportOrder exportOrder,
            ExportSide exportSide)
        {
            var shipTo = await _addressMapper.ToModelAsync<OrderShipTo>(order.ShippingAddress);
            shipTo.Key = "1";
            shipTo.FullName = ((string) order.ShippingAddress.SafeData.FirstName ?? string.Empty) + " " +
                              ((string) order.ShippingAddress.SafeData.LastName ?? string.Empty);
            exportOrder.ShipTo = new[] {shipTo};

            var upgradeP = (ShippingUpgradeOption?)(int?) context.Order.SafeData.ShippingUpgradeP;
            var upgradeNp = (ShippingUpgradeOption?)(int?)context.Order.SafeData.ShippingUpgradeNP;

            var prefferedShipMethod = (PreferredShipMethod?) order.ShippingAddress.SafeData.PreferredShipMethod ?? PreferredShipMethod.Best;

            var orderVariables = new List<OrderVariable>();
            int varCounter = 1;
            if (!string.IsNullOrEmpty((string) order.SafeData.GiftMessage))
            {
                orderVariables.Add(new OrderVariable
                {
                    VariableField = new VariableField
                    {
                        FieldName = "Gift Message"
                    },
                    Value = EnsureGiftMessageLimit(order),
                    SeqID = varCounter++
                });
            }
            if (order.Discount != null || order.IdObjectType == (int) OrderType.AutoShipOrder)
            {
                orderVariables.Add(new OrderVariable
                {
                    VariableField = new VariableField
                    {
                        FieldName = "Discount"
                    },
                    Value = "Yes",
                    SeqID = varCounter++
                });
            }
            var deliveryInstructions = (string) order.ShippingAddress.SafeData.DeliveryInstructions;
            if (!string.IsNullOrEmpty(deliveryInstructions))
            {
                orderVariables.Add(new OrderVariable
                {
                    SeqID = varCounter++,
                    VariableField = new VariableField
                    {
                        FieldName = "Carrier Instructions"
                    },
                    Value = deliveryInstructions
                });
            }

            switch (exportSide)
            {
                case ExportSide.All:
                    exportOrder.Money.ShippingHandlingCharge = context.StandardShippingOverriden;
                    exportOrder.Money.SpecialHandlingCharge = context.SurchargeShippingOverriden;
                    exportOrder.Shipping.FreightCode = context.SplitInfo.GetSwsCode(context.ShippingCostGroup, upgradeP, prefferedShipMethod);
                    exportOrder.Shipping.FreightCodeDescription = context.SplitInfo.GetCarrierDescription(upgradeP, prefferedShipMethod);

                    if (upgradeP != null)
                    {
                        orderVariables.Add(new OrderVariable
                        {
                            VariableField = new VariableField
                            {
                                FieldName = "Upgrade"
                            },
                            Value = "Yes",
                            SeqID = varCounter
                        });
                    }
                    break;
                case ExportSide.Perishable:
                    exportOrder.Money.ShippingHandlingCharge = context.SplitInfo.PerishableShippingOveridden;
                    exportOrder.Money.SpecialHandlingCharge = context.SplitInfo.PerishableSurchargeOverriden;
                    exportOrder.Shipping.FreightCode = context.SplitInfo.GetSwsCode(context.SplitInfo.PerishableCostGroup, upgradeP,
                        prefferedShipMethod);
                    exportOrder.Shipping.FreightCodeDescription = context.SplitInfo.GetCarrierDescription(upgradeP, prefferedShipMethod);

                    if (upgradeP != null)
                    {
                        orderVariables.Add(new OrderVariable
                        {
                            VariableField = new VariableField
                            {
                                FieldName = "Upgrade"
                            },
                            Value = "Yes",
                            SeqID = varCounter
                        });
                    }
                    break;
                case ExportSide.NonPerishable:
                    exportOrder.Money.ShippingHandlingCharge = context.SplitInfo.NonPerishableShippingOverriden;
                    exportOrder.Money.SpecialHandlingCharge = context.SplitInfo.NonPerishableSurchargeOverriden;
                    exportOrder.Shipping.FreightCode = context.SplitInfo.GetSwsCode(context.SplitInfo.NonPerishableCostGroup, upgradeNp,
                        prefferedShipMethod);
                    exportOrder.Shipping.FreightCodeDescription = context.SplitInfo.GetCarrierDescription(upgradeNp, prefferedShipMethod);

                    if (upgradeNp != null)
                    {
                        orderVariables.Add(new OrderVariable
                        {
                            VariableField = new VariableField
                            {
                                FieldName = "Upgrade"
                            },
                            Value = "Yes",
                            SeqID = varCounter
                        });
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(exportSide), exportSide, null);
            }
            if (orderVariables.Any())
                exportOrder.OrderVariables = orderVariables.ToArray();
        }

        private static readonly Regex WhiteSpace = new Regex("\\s+", RegexOptions.Compiled);

        private static string EnsureGiftMessageLimit(OrderDynamic order)
        {
            var giftMessage = WhiteSpace.Replace((string) order.SafeData.GiftMessage, " ");
            giftMessage = giftMessage.Length > 255 ? giftMessage.Substring(0, 255) : giftMessage;
            return giftMessage;
        }

        private void ParsePaymentInfo(OrderDynamic order, OrderDataContext context, VeraCoreExportOrder exportOrder,
            ExportSide exportSide)
        {
            decimal chargeAmount;
            switch (exportSide)
            {
                case ExportSide.All:
                    chargeAmount = context.Total;

                    if (order.PaymentMethod.IdObjectType != (int) PaymentMethodType.Check ||
                        (bool?) order.PaymentMethod.SafeData.PaidInFull == true)
                    {
                        exportOrder.Money.TaxAmount = context.TaxTotal;
                    }
                    break;
                case ExportSide.Perishable:
                    chargeAmount = context.SplitInfo.PerishableTotal;

                    if (order.PaymentMethod.IdObjectType != (int) PaymentMethodType.Check ||
                        (bool?) order.PaymentMethod.SafeData.PaidInFull == true)
                    {
                        exportOrder.Money.TaxAmount = context.SplitInfo.PerishableTax;
                    }
                    break;
                case ExportSide.NonPerishable:
                    chargeAmount = context.SplitInfo.NonPerishableTotal;

                    if (order.PaymentMethod.IdObjectType != (int) PaymentMethodType.Check ||
                        (bool?) order.PaymentMethod.SafeData.PaidInFull == true)
                    {
                        exportOrder.Money.TaxAmount = context.SplitInfo.NonPerishableTax;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(exportSide), exportSide, null);
            }
            switch ((PaymentMethodType) order.PaymentMethod.IdObjectType)
            {
                case PaymentMethodType.CreditCard:
                    var expDate = (DateTime) order.PaymentMethod.Data.ExpDate;
                    exportOrder.Payment.PaymentType.Sequence = 1;
                    //NOTBUG: revisit manual CC payment, we not using manual payment anymore
                    exportOrder.Payment.PaymentType.Description =
                        FormatCcType((CreditCardType) order.PaymentMethod.Data.CardType);
                    exportOrder.Payment.CCExpirationDate = $"{expDate.Month:D2}{expDate.Year%100:D2}";
                    exportOrder.Payment.CCNumber = order.PaymentMethod.Data.CardNumber;
                    exportOrder.Payment.PaymentAmount = chargeAmount;
                    break;
                case PaymentMethodType.Oac:
                    exportOrder.Payment.PaymentType.Sequence = 1;
                    exportOrder.Payment.PaymentType.Description = OacDescription;
                    break;
                case PaymentMethodType.Check:
                    var today = DateTime.Today;
                    exportOrder.Payment.PaymentType.Sequence = 1;
                    exportOrder.Payment.PaymentType.Description = CheckDescription;
                    exportOrder.Payment.CCExpirationDate = $"{today.Month:D2}/{today.Day:D2}/{today.Year}";
                    exportOrder.Payment.PaymentAmount = chargeAmount;
                    exportOrder.Payment.CCNumber = order.PaymentMethod.Data.CheckNumber;
                    break;
                case PaymentMethodType.NoCharge:
                    exportOrder.Payment.PaymentType.Sequence = 1;
                    exportOrder.Payment.PaymentType.Description = NoChargeDescription;
                    break;
                case PaymentMethodType.WireTransfer:
                    exportOrder.Payment.PaymentType.Sequence = 1;
                    exportOrder.Payment.PaymentType.Description = PrepaidDescription;
                    break;
                case PaymentMethodType.Marketing:
                    exportOrder.Payment.PaymentType.Sequence = 1;
                    exportOrder.Payment.PaymentType.Description =
                        (int) order.PaymentMethod.Data.MarketingPromotionType == 1
                            ? MarketingPromoDescription
                            : MarketingDonationDescription;
                    break;
                case PaymentMethodType.VCWellnessEmployeeProgram:
                    exportOrder.Payment.PaymentType.Sequence = 1;
                    exportOrder.Payment.PaymentType.Description = VcWellnessDescription;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ParsePaymentInfo(OrderRefundDynamic order, VeraCoreExportOrder exportOrder)
        {
            switch ((PaymentMethodType) order.PaymentMethod.IdObjectType)
            {
                case PaymentMethodType.Oac:
                    exportOrder.Payment.PaymentType.Sequence = 1;
                    exportOrder.Payment.PaymentType.Description = OacDescription;
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        private static void ParseProductInfo(OrderRefundDynamic refund, VeraCoreExportOrder exportOrder)
        {
            exportOrder.Offers = new[]
            {
                new OfferOrdered
                {
                    OrderShipToKey = new OrderShipToKey
                    {
                        Key = "1"
                    },
                    LineNumber = 0,
                    Quantity = 1,
                    UnitPrice = -refund.Total,
                    ProductDetails = new[]
                    {
                        new OrderProductDetail
                        {
                            PartNumber = "0"
                        }
                    },
                    Offer = new OfferID
                    {
                        Header = new OfferIDHeader
                        {
                            ID = "REFUND"
                        }
                    }
                }
            };
        }

        private static void ParseProductInfo(OrderDataContext context, VeraCoreExportOrder exportOrder,
            ExportSide exportSide)
        {
            int lineIndex = 0;
            List<OfferOrdered> offers = new List<OfferOrdered>();
            switch (exportSide)
            {
                case ExportSide.All:
                    offers.AddRange(context.ItemsOrdered.Select(product => CreateProductOffer(product, ref lineIndex)));
                    if (context.GiftCertificatesSubtotal < 0)
                    {
                        offers.Add(CreateGcOffer(context.GiftCertificatesSubtotal, ref lineIndex));
                    }
                    if (context.DiscountTotal > 0)
                    {
                        offers.Add(new OfferOrdered
                        {
                            OrderShipToKey = new OrderShipToKey
                            {
                                Key = "1"
                            },
                            LineNumber = lineIndex,
                            Quantity = 1,
                            UnitPrice = -context.DiscountTotal,
                            ProductDetails = new[]
                            {
                                new OrderProductDetail
                                {
                                    PartNumber = "0"
                                }
                            },
                            Offer = new OfferID
                            {
                                Header = new OfferIDHeader
                                {
                                    ID = AmountDiscountSku
                                }
                            }
                        });
                    }
                    break;
                case ExportSide.Perishable:
                    offers.AddRange(
                        context.SplitInfo.GetPerishablePartProducts().Select(product => CreateProductOffer(product, ref lineIndex)));
                    if (context.SplitInfo.PerishableGiftCertificateAmount < 0)
                    {
                        offers.Add(CreateGcOffer(context.SplitInfo.PerishableGiftCertificateAmount, ref lineIndex));
                    }
                    if (context.DiscountTotal > 0)
                    {
                        offers.Add(new OfferOrdered
                        {
                            OrderShipToKey = new OrderShipToKey
                            {
                                Key = "1"
                            },
                            LineNumber = lineIndex,
                            Quantity = 1,
                            UnitPrice = -context.SplitInfo.PerishableDiscount,
                            ProductDetails = new[]
                            {
                                new OrderProductDetail
                                {
                                    PartNumber = "0"
                                }
                            },
                            Offer = new OfferID
                            {
                                Header = new OfferIDHeader
                                {
                                    ID = AmountDiscountSku
                                }
                            }
                        });
                    }
                    break;
                case ExportSide.NonPerishable:
                    offers.AddRange(
                        context.SplitInfo.GetNonPerishablePartProducts().Select(product => CreateProductOffer(product, ref lineIndex)));
                    if (context.SplitInfo.NonPerishableGiftCertificateAmount < 0)
                    {
                        offers.Add(CreateGcOffer(context.SplitInfo.NonPerishableGiftCertificateAmount, ref lineIndex));
                    }
                    if (context.DiscountTotal > 0)
                    {
                        offers.Add(new OfferOrdered
                        {
                            OrderShipToKey = new OrderShipToKey
                            {
                                Key = "1"
                            },
                            LineNumber = lineIndex,
                            Quantity = 1,
                            UnitPrice = -context.SplitInfo.NonPerishableDiscount,
                            ProductDetails = new[]
                            {
                                new OrderProductDetail
                                {
                                    PartNumber = "0"
                                }
                            },
                            Offer = new OfferID
                            {
                                Header = new OfferIDHeader
                                {
                                    ID = AmountDiscountSku
                                }
                            }
                        });
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(exportSide), exportSide, null);
            }
            exportOrder.Offers = offers.ToArray();
        }

        private static OfferOrdered CreateProductOffer(ItemOrdered product, ref int lineIndex)
        {
            return new OfferOrdered
            {
                OrderShipToKey = new OrderShipToKey
                {
                    Key = "1"
                },
                LineNumber = lineIndex++,
                Quantity = product.Quantity,
                UnitPrice = product.Amount,
                ProductDetails = new[]
                {
                    new OrderProductDetail
                    {
                        PartNumber = product.Sku.Id.ToString(CultureInfo.InvariantCulture)
                    }
                },
                Offer = new OfferID
                {
                    Header = new OfferIDHeader
                    {
                        ID = product.Sku.Code
                    }
                }
            };
        }

        private static OfferOrdered CreateGcOffer(decimal amount, ref int lineIndex)
        {
            return new OfferOrdered
            {
                OrderShipToKey = new OrderShipToKey
                {
                    Key = "1"
                },
                LineNumber = lineIndex++,
                Quantity = 1,
                UnitPrice = amount,
                ProductDetails = new[]
                {
                    new OrderProductDetail
                    {
                        PartNumber = "0"
                    }
                },
                Offer = new OfferID
                {
                    Header = new OfferIDHeader
                    {
                        ID = GcReDeem
                    }
                }
            };
        }

        private async Task<VeraCoreExportOrder> CreateExportFromRefund(OrderRefundDynamic refund)
        {
            var result = new VeraCoreExportOrder
            {
                Money = new OrderMoney(),
                Shipping = new OrderShipping(),
                Header = new OrderHeader(),
                Classification = new OrderClassification(),
                Payment = new OrderPayment
                {
                    PaymentType = new PaymentType()
                }
            };
            ParseGeneralInfo(refund, result, ExportSide.All);
            ParsePaymentInfo(refund, result);
            await ParseBillingInfo(refund.PaymentMethod, refund.Customer, result);
            await ParseShippingInfo(refund, result);
            ParseProductInfo(refund, result);

            return result;
        }

        private async Task<VeraCoreExportOrder> CreateExportFromOrder(OrderDynamic order, OrderDataContext context, ExportSide exportSide)
        {
            var result = new VeraCoreExportOrder
            {
                Money = new OrderMoney(),
                Shipping = new OrderShipping(),
                Header = new OrderHeader(),
                Classification = new OrderClassification(),
                Payment = new OrderPayment
                {
                    PaymentType = new PaymentType()
                }
            };
            ParseGeneralInfo(order, result, exportSide);
            ParsePaymentInfo(order, context, result, exportSide);
            await ParseBillingInfo(order.PaymentMethod, order.Customer, result);
            await ParseShippingInfo(order, context, result, exportSide);
            ParseProductInfo(context, result, exportSide);

            return result;
        }
    }
}