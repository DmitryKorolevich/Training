using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VitalChoice.Validation.Models;
using System;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using System.Security.Claims;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Settings;
using Newtonsoft.Json;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Entities.Permissions;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Settings;
using System.Linq;
using VC.Admin.Models.Orders;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Business.CsvExportMaps.Orders;
using VitalChoice.Infrastructure.Domain.Constants;
using Microsoft.Net.Http.Headers;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Infrastructure.Domain.Entities.Tokens;
using Microsoft.Extensions.Options;
using VitalChoice.Infrastructure.Domain.Options;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using VitalChoice.Interfaces.Services.Users;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VC.Admin.Models.Customer;
using VitalChoice.Business.Helpers;

namespace VC.Admin.Controllers
{
    public class OrderInvoiceController : BaseController
    {
        private readonly IOrderService _orderService;
        private readonly IDynamicMapper<OrderDynamic, Order> _mapper;
        //private readonly IDynamicMapper<AddressDynamic, OrderAddress> _addressMapper;
        //private readonly ICustomerService _customerService;
        private readonly ITokenService _tokenService;
        private readonly IOptions<AppOptions> _options;
        //private readonly IAdminUserService _adminUserService;
        //private readonly ICountryService _countryService;
        //private readonly IAppInfrastructureService _appInfrastructureService;
        //private readonly ITrackingService _trackingService;
        //private readonly ILogger logger;

        public OrderInvoiceController(
            IOrderService orderService,
            IDynamicMapper<OrderDynamic, Order> mapper,
            ITokenService tokenService,
            //ILoggerProviderExtended loggerProvider,
            //ICustomerService customerService,
            //IDynamicMapper<AddressDynamic, OrderAddress> addressMapper,
            IOptions<AppOptions> options
            //IAdminUserService adminUserService,
            //ICountryService countryService,
            //IAppInfrastructureService appInfrastructureService,
            //ITrackingService trackingService*/
            )
        {
            _orderService = orderService;
            _mapper = mapper;
            //_customerService = customerService;
            //_addressMapper = addressMapper;
            _tokenService = tokenService;
            _options = options;
            //_adminUserService = adminUserService;
            //_countryService = countryService;
            //_appInfrastructureService = appInfrastructureService;
            //_trackingService = trackingService;
            //this.logger = loggerProvider.CreateLogger<OrderInvoiceController>();
        }

        [HttpGet]
        [AdminAuthorize(PermissionType.Orders)]
        public async Task<IActionResult> Base(int id)
        {
            var order = await _orderService.SelectAsync(id,true);
            if(order==null)
            {
                return View(null);
            }
            
            Token token = new Token();
            token.DateCreated = DateTime.Now;
            token.DateExpired = token.DateCreated.AddMinutes(BaseAppConstants.DEFAULT_TOKEN_EXPIRED_MINUTES);
            token.IdTokenType = TokenType.OrderInvoicePDFAdminGenerateRequest;
            token.Data = order.Id.ToString();
            token = await _tokenService.InsertTokenAsync(token);
            if(token==null)
            {
                return View(null);
            }

            OrderInvoiceModel model = await _mapper.ToModelAsync<OrderInvoiceModel>(order);
            string invoicePageUrl = String.Format(BaseAppConstants.ORDER_INVOICE_PAGE_URL_TEMPLATE, _options.Value.AdminHost, token.IdToken.ToString().ToLower());
            model.PDFUrl = String.Format(BaseAppConstants.PDF_URL_GENERATE_ORDER_INVOICE_TEMPLATE,_options.Value.PDFMyUrl.ServiceUrl,
                _options.Value.PDFMyUrl.LicenseKey, WebUtility.UrlEncode(invoicePageUrl), order.Id);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Pdf(string id)
        {
            Guid idToken;
            Guid.TryParse(id, out idToken);
            Token token = null;
            int idOrder=0;
            if (idToken != Guid.Empty)
            {
                token = await _tokenService.GetTokenAsync(idToken, TokenType.OrderInvoicePDFAdminGenerateRequest);
                if(token!=null && token.DateExpired<DateTime.Now)
                {
                    token = null;
                }
                if(token!=null) 
                {
                    Int32.TryParse(token.Data, out idOrder);
                }
            }
            if(idOrder == 0)
            {
                return View(null);
            }

            var order = await _orderService.SelectAsync(idOrder, true);
            if (order == null)
            {
                return View(null);
            }

            OrderInvoiceModel model = await _mapper.ToModelAsync<OrderInvoiceModel>(order);
            return View(model);
        } 
    }
}