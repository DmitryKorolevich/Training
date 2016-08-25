using System.Threading.Tasks;
using System;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Entities.Permissions;
using VC.Admin.Models.Orders;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Tokens;
using Microsoft.Extensions.Options;
using VitalChoice.Infrastructure.Domain.Options;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace VC.Admin.Controllers
{
    public class OrderInvoiceController : BaseController
    {
        private readonly IOrderService _orderService;
        private readonly IDynamicMapper<OrderDynamic, Order> _mapper;
        private readonly ITokenService _tokenService;
        private readonly IOptions<AppOptions> _options;

        public OrderInvoiceController(
            IOrderService orderService,
            IDynamicMapper<OrderDynamic, Order> mapper,
            ITokenService tokenService,
            IOptions<AppOptions> options
        )
        {
            _orderService = orderService;
            _mapper = mapper;
            _tokenService = tokenService;
            _options = options;
        }

        [HttpGet]
        [AdminAuthorize(PermissionType.Orders)]
        public async Task<IActionResult> Base(int id)
        {
            var order = await _orderService.SelectAsync(id, true);
            if (order == null)
            {
                return View(null);
            }

            var token =
                await
                    _tokenService.CreateTokenAsync(order.Id.ToString(), TimeSpan.FromMinutes(BaseAppConstants.DEFAULT_TOKEN_EXPIRED_MINUTES),
                        TokenType.OrderInvoicePdfAdminGenerateRequest);
            if (token == null)
            {
                return View(null);
            }

            OrderInvoiceModel model = await _mapper.ToModelAsync<OrderInvoiceModel>(order);
            string invoicePageUrl = String.Format(BaseAppConstants.ORDER_INVOICE_PAGE_URL_TEMPLATE, _options.Value.AdminHost,
                token.IdToken.ToString().ToLower());
            model.PDFUrl = String.Format(BaseAppConstants.PDF_URL_GENERATE_ORDER_INVOICE_TEMPLATE, _options.Value.PDFMyUrl.ServiceUrl,
                _options.Value.PDFMyUrl.LicenseKey, WebUtility.UrlEncode(invoicePageUrl), order.Id);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Pdf(string id)
        {
            Guid idToken;
            if (Guid.TryParse(id, out idToken) && idToken != Guid.Empty)
            {
                var token = await _tokenService.GetValidToken(idToken, TokenType.OrderInvoicePdfAdminGenerateRequest);
                if (token != null)
                {
                    int idOrder;
                    if (int.TryParse(token.Data, out idOrder))
                    {
                        var order = await _orderService.SelectAsync(idOrder, true);
                        if (order != null)
                        {
                            OrderInvoiceModel model = await _mapper.ToModelAsync<OrderInvoiceModel>(order);
                            return View(model);
                        }
                    }
                }
            }
            return View(null);
        }
    }
}