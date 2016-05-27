﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Avalara.Avatax.Rest.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Infrastructure.Domain.Avatax;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Avatax;
using Address = VitalChoice.Infrastructure.Domain.Avatax.Address;

namespace VitalChoice.Business.Services.Avatax
{
    public class AvalaraTax : IAvalaraTax
    {
        private readonly ITaxService _taxService;
        private readonly IDynamicMapper<AddressDynamic, OrderAddress> _mapper;
        private readonly ICountryNameCodeResolver _countryNameCode;
        private readonly ILogger _logger;
        private readonly string _accountName;
        private readonly string _profileName;
        private readonly string _companyCode;
        private readonly bool _turnOffCommit;
        //TODO move out tax code to global settings
        internal const string ShippingTaxCode = "FR020100";

        public AvalaraTax(ITaxService taxService, IOptions<AppOptions> options, ILoggerProviderExtended loggerProvider,
            IDynamicMapper<AddressDynamic, OrderAddress> mapper, ICountryNameCodeResolver countryNameCode)
        {
            _taxService = taxService;
            _mapper = mapper;
            _countryNameCode = countryNameCode;
            _accountName = options.Value.Avatax.AccountName;
            _profileName = options.Value.Avatax.ProfileName;
            _companyCode = options.Value.Avatax.CompanyCode;
            _turnOffCommit = options.Value.Avatax.TurnOffCommit;
            _logger = loggerProvider.CreateLogger<AvalaraTax>();
        }

        public async Task<bool> CancelTax(string orderCode)
        {
            if (_turnOffCommit)
                return true;
            CancelTaxRequest cancelTaxRequest = new CancelTaxRequest
            {
                CompanyCode = _companyCode,
                DocCode = "TAX" + orderCode,
                DocType = DocType.SalesInvoice,
                CancelCode = CancelCode.DocVoided
            };

            var cancelTaxResult = await _taxService.CancelTax(cancelTaxRequest);
            if (cancelTaxResult.ResultCode != SeverityLevel.Success)
                _logger.LogWarning(string.Join("\n",
                    cancelTaxResult.Messages.Select(m => $"[{m.Source}] {m.Summary} ({m.Details})")));
            return cancelTaxResult.ResultCode == SeverityLevel.Success;
        }

        public async Task<bool> CommitTax(int idOrder, TaxGetType taxGetType = TaxGetType.UseBoth)
        {
            if (_turnOffCommit)
                return true;
            GetTaxRequest getTaxRequest = new GetTaxRequest
            {
                CompanyCode = _companyCode,
                DocCode = "TAX" + idOrder,
                Commit = taxGetType.HasFlag(TaxGetType.Commit),
                DocType = DocType.SalesInvoice,
                DocDate = DateTime.Now
            };

            var postTaxResult = await _taxService.GetTax(getTaxRequest);
            if (postTaxResult.ResultCode != SeverityLevel.Success)
            {
                _logger.LogWarning(string.Join("\n",
                    postTaxResult.Messages.Select(m => $"[{m.Source}] {m.Summary} ({m.Details})")));
            }
            return postTaxResult.ResultCode == SeverityLevel.Success;
        }

        public async Task<decimal> GetTax(OrderDataContext context, TaxGetType taxGetType = TaxGetType.UseBoth)
        {
            if (!_countryNameCode.IsState(context.Order.ShippingAddress, "us", "va") &&
                !_countryNameCode.IsState(context.Order.ShippingAddress, "us", "wa"))
                return 0;

            taxGetType = CommitProtect(taxGetType);

            Address origin;
            Address destination;
            FillAddresses(context.Order.ShippingAddress, out origin, out destination);

            var request = FillGetTaxBaseRequest(context.Order.Customer, context.Order.Id, -(decimal?)context.SafeData.Discount ?? 0, taxGetType, destination, origin);

            var lines = ToTaxLines(context, taxGetType, 1).ToArray();
            if (!lines.Any())
                return 0;
            lines = UnionTaxShipping(lines, context).ToArray();
            request.Lines = lines;

            var result = await _taxService.GetTax(request);

            if (result.ResultCode == SeverityLevel.Success)
            {
                return result.TotalTax;
            }
            _logger.LogWarning(string.Join("\n",
                result.Messages.Select(m => $"[{m.Source}] {m.Summary}\r\n{result.DocCode}")));
            return 0;
        }

        public async Task<decimal> GetTax(OrderRefundDataContext context, TaxGetType taxGetType = TaxGetType.UseBoth)
        {
            if (!_countryNameCode.IsState(context.Order.ShippingAddress, "us", "va") &&
                !_countryNameCode.IsState(context.Order.ShippingAddress, "us", "wa"))
                return 0;

            taxGetType = CommitProtect(taxGetType);

            Address origin;
            Address destination;
            FillAddresses(context.Order.ShippingAddress, out origin, out destination);

            var request = FillGetTaxBaseRequest(context.Order.Customer, context.Order.Id, -(decimal?)context.SafeData.Discount ?? 0, taxGetType,
                destination, origin);

            var lines = ToTaxLines(context, taxGetType, 1).ToArray();
            if (!lines.Any())
                return 0;
            lines = UnionTaxShipping(lines, context).ToArray();
            request.Lines = lines;

            var result = await _taxService.GetTax(request);

            if (result.ResultCode == SeverityLevel.Success)
            {
                return result.TotalTax;
            }
            _logger.LogWarning(string.Join("\n",
                result.Messages.Select(m => $"[{m.Source}] {m.Summary}\r\n{result.DocCode}")));
            return 0;
        }

        private GetTaxRequest FillGetTaxBaseRequest(CustomerDynamic customer, int idOrder, decimal discountTotal, TaxGetType taxGetType,
            Address destinationAddress, Address originAddress)
        {
            int customerId = customer.Id;

            GetTaxRequest getTaxRequest = new GetTaxRequest
            {
                CustomerCode = customerId.ToString(CultureInfo.InvariantCulture),
                DocDate = DateTime.Now,
                CompanyCode = _companyCode,
                CustomerUsageType =
                    customer.IdObjectType == (int) CustomerType.Wholesale &&
                    customer.SafeData.TaxExempt == 1//Yes, Current Certificate
                        ? "G"
                        : null,
                DocCode =
                    "TAX" +
                    (taxGetType.HasFlag(TaxGetType.Perishable) ? $"{idOrder}-P" : $"{idOrder}-NP"),
                DetailLevel = DetailLevel.Tax,
                Commit = taxGetType.HasFlag(TaxGetType.Commit),
                DocType =
                    taxGetType.HasFlag(TaxGetType.SavePermanent) ? DocType.SalesInvoice : DocType.SalesOrder,
                PurchaseOrderNo =
                    (taxGetType.HasFlag(TaxGetType.Perishable) ? $"{idOrder}-P" : $"{idOrder}-NP"),
                CurrencyCode = "USD",
                Discount = discountTotal,
                Addresses = new[] {originAddress, destinationAddress}
            };
            return getTaxRequest;
        }

        private void FillAddresses(AddressDynamic shippingAddress, out Address originAddress,
            out Address destinationAddress)
        {
            originAddress = new Address
            {
                AddressCode = "01",
                Line1 = "Vital Choice Wild Seafood & Organics",
                Line2 = "2460 Salashan Loop Road",
                City = "Ferndale",
                Region = "WA",
                Country = "US",
                PostalCode = "98248"
            };
            destinationAddress = _mapper.ToModelAsync<Address>(shippingAddress).GetAwaiter().GetResult();
            destinationAddress.AddressCode = "02";
            destinationAddress.Country = _countryNameCode.GetCountryCode(shippingAddress);
            destinationAddress.Region = _countryNameCode.GetRegionOrStateCode(shippingAddress);
        }

        private TaxGetType CommitProtect(TaxGetType taxGetType)
        {
            if (_turnOffCommit)
            {
                if (taxGetType.HasFlag(TaxGetType.Commit))
                {
                    taxGetType = taxGetType ^ TaxGetType.Commit;
                }
                if (taxGetType.HasFlag(TaxGetType.SavePermanent))
                {
                    taxGetType = taxGetType ^ TaxGetType.SavePermanent;
                }
            }
            return taxGetType;
        }

        private static IEnumerable<Line> UnionTaxShipping(IEnumerable<Line> lines, OrderDataContext order)
        {
            return lines.Union(new List<Line>
            {
                new Line
                {
                    Amount = order.ShippingTotal,
                    Description = "Shipping Amount",
                    DestinationCode = "02",
                    OriginCode = "01",
                    TaxCode = ShippingTaxCode,
                    Qty = 1,
                    ItemCode = "SHIPPING",
                    LineNo = "01-SHIP",
                    CustomerUsageType =
                        order.Order.Customer.IdObjectType == (int) CustomerType.Wholesale &&
                        order.Order.Customer.SafeData.TaxExempt == 1//Yes, Current Certificate
                            ? "G"
                            : null
                }
            });
        }

        private static IEnumerable<Line> UnionTaxShipping(IEnumerable<Line> lines, OrderRefundDataContext order)
        {
            return lines.Union(new List<Line>
            {
                new Line
                {
                    Amount = order.ShippingTotal,
                    Description = "Shipping Amount",
                    DestinationCode = "02",
                    OriginCode = "01",
                    TaxCode = ShippingTaxCode,
                    Qty = 1,
                    ItemCode = "SHIPPING",
                    LineNo = "01-SHIP",
                    CustomerUsageType =
                        order.Order.Customer.IdObjectType == (int) CustomerType.Wholesale &&
                        order.Order.Customer.SafeData.TaxExempt == 1//Yes, Current Certificate
                            ? "G"
                            : null
                }
            });
        }

        private static IEnumerable<Line> ToTaxLines(OrderDataContext order, TaxGetType taxGetType, int startNumber)
        {
            IEnumerable<SkuOrdered> items = Enumerable.Empty<SkuOrdered>();
            if (taxGetType.HasFlag(TaxGetType.Perishable))
            {
                items =
                    items.Union(order.SkuOrdereds.Union(order.PromoSkus.Where(p => p.Enabled))
                        .Where(s => s.Sku.IdObjectType == (int) ProductType.Perishable));
            }
            if (taxGetType.HasFlag(TaxGetType.NonPerishable))
            {
                items =
                    items.Union(order.SkuOrdereds.Union(order.PromoSkus.Where(p => p.Enabled))
                        .Where(s => s.Sku.IdObjectType == (int) ProductType.NonPerishable));
            }
            return items.Select(
                p => new Line
                {
                    Amount = p.Amount*p.Quantity,
                    Description = p.Sku.Product.Name,
                    DestinationCode = "02",
                    OriginCode = "01",
                    Discounted = (-(decimal?)order.SafeData.Discount ?? 0) > 0,
                    TaxCode = p.Sku.Product.SafeData.TaxCode,
                    Qty = p.Quantity,
                    ItemCode = p.Sku.Code,
                    LineNo = (startNumber++).ToString(CultureInfo.InvariantCulture),
                    Ref1 = p.Sku.Id.ToString(CultureInfo.InvariantCulture),
                    CustomerUsageType =
                        order.Order.Customer.IdObjectType == (int) CustomerType.Wholesale &&
                        order.Order.Customer.SafeData.TaxExempt == 1 //Yes, Current Certificate
                            ? "G"
                            : null
                });
        }

        private static IEnumerable<Line> ToTaxLines(OrderRefundDataContext order, TaxGetType taxGetType, int startNumber)
        {
            IEnumerable<RefundSkuOrdered> items = Enumerable.Empty<RefundSkuOrdered>();
            if (taxGetType.HasFlag(TaxGetType.Perishable))
            {
                items = items.Union(order.RefundSkus.Where(s => s.Sku.IdObjectType == (int) ProductType.Perishable));
            }
            if (taxGetType.HasFlag(TaxGetType.NonPerishable))
            {
                items = items.Union(order.RefundSkus.Where(s => s.Sku.IdObjectType == (int) ProductType.NonPerishable));
            }
            return items.Select(
                p => new Line
                {
                    Amount = p.RefundValue*(decimal) p.RefundPercent/100,
                    Description = p.Sku.Product.Name,
                    DestinationCode = "02",
                    OriginCode = "01",
                    Discounted = (-(decimal?)order.SafeData.Discount ?? 0) > 0,
                    TaxCode = p.Sku.Product.SafeData.TaxCode,
                    Qty = p.Quantity,
                    ItemCode = p.Sku.Code,
                    LineNo = (startNumber++).ToString(CultureInfo.InvariantCulture),
                    Ref1 = p.Sku.Id.ToString(CultureInfo.InvariantCulture),
                    CustomerUsageType =
                        order.Order.Customer.IdObjectType == (int) CustomerType.Wholesale &&
                        order.Order.Customer.SafeData.TaxExempt == 1 //Yes, Current Certificate
                            ? "G"
                            : null
                });
        }
    }
}