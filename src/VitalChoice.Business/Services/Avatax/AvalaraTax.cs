using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Avalara.Avatax.Rest.Services;
using Microsoft.Framework.Logging;
using Microsoft.Framework.OptionsModel;
using VitalChoice.Domain.Avatax;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Entities.Options;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Helpers;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Entities.Transfer;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Avatax;
using VitalChoice.Workflow.Contexts;

namespace VitalChoice.Business.Services.Avatax
{
    public class AvalaraTax : IAvalaraTax
    {
        private readonly ITaxService _taxService;
        private readonly IDynamicToModelMapper<OrderAddressDynamic> _mapper;
        private readonly ILogger _logger;
        private readonly string _accountName;
        private readonly string _profileName;
        private readonly string _companyCode;
        private readonly bool _turnOffCommit;
        //TODO move out tax code to global settings
        internal const string ShippingTaxCode = "FR020100";

        public AvalaraTax(ITaxService taxService, IOptions<AppOptions> options, ILoggerProviderExtended loggerProvider,
            IDynamicToModelMapper<OrderAddressDynamic> mapper)
        {
            _taxService = taxService;
            _mapper = mapper;
            _accountName = options.Value.Avatax.AccountName;
            _profileName = options.Value.Avatax.ProfileName;
            _companyCode = options.Value.Avatax.CompanyCode;
            _turnOffCommit = options.Value.Avatax.TurnOffCommit;
            _logger = loggerProvider.CreateLoggerDefault();
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

        public async Task<decimal> GetTax(OrderContext order, TaxGetType taxGetType = TaxGetType.UseBoth)
        {
            if (!order.IsState(order.Order.ShippingAddress, "us", "va") &&
                !order.IsState(order.Order.ShippingAddress, "us", "wa"))
                return 0;

            taxGetType = CommitProtect(taxGetType);

            Address origin;
            Address destination;
            FillAddresses(order, out origin, out destination);

            var request = FillGetTaxBaseRequest(order, taxGetType, destination, origin);

            var lines = ToTaxLines(order, taxGetType, 1).ToArray();
            if (!lines.Any())
                return 0;
            lines = UnionTaxShipping(lines, order).ToArray();
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

        private GetTaxRequest FillGetTaxBaseRequest(OrderContext order, TaxGetType taxGetType,
            Address destinationAddress, Address originAddress)
        {
            int customerId = order.Order.Customer.Id;

            GetTaxRequest getTaxRequest = new GetTaxRequest
            {
                CustomerCode = customerId.ToString(CultureInfo.InvariantCulture),
                DocDate = DateTime.Now,
                CompanyCode = _companyCode,
                CustomerUsageType =
                    order.Order.Customer.IdObjectType == (int) CustomerType.Wholesale &&
                    order.Order.Customer.Data.TaxExempt
                        ? "G"
                        : null,
                DocCode =
                    "TAX" +
                    (taxGetType.HasFlag(TaxGetType.PerishableOnly) ? $"{order.Order.Id}-P" : $"{order.Order.Id}-NP"),
                DetailLevel = DetailLevel.Tax,
                Commit = taxGetType.HasFlag(TaxGetType.Commit),
                DocType =
                    taxGetType.HasFlag(TaxGetType.SavePermanent) ? DocType.SalesInvoice : DocType.SalesOrder,
                PurchaseOrderNo =
                    (taxGetType.HasFlag(TaxGetType.PerishableOnly) ? $"{order.Order.Id}-P" : $"{order.Order.Id}-NP"),
                CurrencyCode = "USD",
                Discount = order.DiscountTotal,
                Addresses = new[] {originAddress, destinationAddress}
            };
            return getTaxRequest;
        }

        private void FillAddresses(OrderContext orderContext, out Address originAddress,
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
            destinationAddress = _mapper.ToModel<Address>(orderContext.Order.ShippingAddress);
            destinationAddress.AddressCode = "02";
            destinationAddress.Country = orderContext.GetCountryCode(orderContext.Order.ShippingAddress);
            destinationAddress.Region = orderContext.GetRegionOrStateCode(orderContext.Order.ShippingAddress);
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

        private static IEnumerable<Line> UnionTaxShipping(IEnumerable<Line> lines, OrderContext order)
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
                        order.Order.Customer.Data.TaxExempt
                            ? "G"
                            : null
                }
            });
        }

        private static IEnumerable<Line> ToTaxLines(OrderContext order, TaxGetType taxGetType, int startNumber)
        {
            IEnumerable<SkuOrdered> items;
            if (taxGetType.HasFlag(TaxGetType.PerishableOnly))
            {
                items =
                    order.SkuOrdereds.Union(order.PromoSkus)
                        .Where(s => s.ProductWithoutSkus.IdObjectType != (int) ProductType.NonPerishable);
            }
            else if (taxGetType.HasFlag(TaxGetType.NonPerishableOnly))
            {
                items =
                    order.SkuOrdereds.Union(order.PromoSkus)
                        .Where(s => s.ProductWithoutSkus.IdObjectType == (int) ProductType.NonPerishable);
            }
            else
            {
                items = order.SkuOrdereds.Union(order.PromoSkus);
            }
            return items.Select(
                p => new Line
                {
                    Amount = p.Amount*p.Quantity,
                    Description = p.ProductWithoutSkus.Name,
                    DestinationCode = "02",
                    OriginCode = "01",
                    Discounted = order.DiscountTotal > 0,
                    TaxCode = p.ProductWithoutSkus.Data.TaxCode,
                    Qty = p.Quantity,
                    ItemCode = p.Sku.Code,
                    LineNo = (startNumber++).ToString(CultureInfo.InvariantCulture),
                    Ref1 = p.Sku.Id.ToString(CultureInfo.InvariantCulture),
                    CustomerUsageType =
                        order.Order.Customer.IdObjectType == (int) CustomerType.Wholesale &&
                        order.Order.Customer.Data.TaxExempt
                            ? "G"
                            : null
                });
        }
    }
}