﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using VC.Public.Helpers;
using VC.Public.Models.Cart;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Core.Services;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Checkout;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Content.ContentCrossSells;
using VitalChoice.Infrastructure.Domain.Entities.Roles;
using VitalChoice.Infrastructure.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Transfer.Cart;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Infrastructure.Domain.Transfer.Shipping;
using VitalChoice.Interfaces.Services.Content;
using VitalChoice.Validation.Models;

namespace VC.Public.Controllers
{
    [CustomerStatusCheck]
    public class CartController : CheckoutControllerBase
    {
	    private readonly IProductService _productService;
        private readonly ICheckoutService _checkoutService;
        private readonly IDiscountService _discountService;
        private readonly IGcService _gcService;
	    private readonly IContentCrossSellService _contentCrossSellService;


	    public CartController(IHttpContextAccessor contextAccessor,
            ICustomerService customerService,
            IOrderService orderService, IProductService productService, ICheckoutService checkoutService,
            IAuthorizationService authorizationService, IAppInfrastructureService appInfrastructureService,
            IDynamicMapper<SkuDynamic, Sku> skuMapper, IDynamicMapper<ProductDynamic, Product> productMapper,
            IDiscountService discountService, IGcService gcService, IContentCrossSellService contentCrossSellService, IPageResultService pageResultService)
            : base(contextAccessor, customerService, appInfrastructureService, authorizationService, checkoutService, orderService, skuMapper,productMapper, pageResultService)
        {
	        _productService = productService;
            _checkoutService = checkoutService;
            _discountService = discountService;
            _gcService = gcService;
		    _contentCrossSellService = contentCrossSellService;
        }

	    private async Task<Tuple<OrderDataContext, CustomerCartOrder>> AddToCartInternal(string skuCode, int? autoshipFrequency = null)
	    {
			var existingUid = Request.GetCartUid();
			var sku = await _productService.GetSkuOrderedAsync(skuCode);
			if (sku == null)
				throw new ApiException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantAddProductToCart]);
			CustomerCartOrder cart;
			if (await CustomerLoggedIn())
			{
				var id = GetInternalCustomerId();
				cart = await _checkoutService.GetOrCreateCart(existingUid, id);
			}
			else
			{
				cart = await _checkoutService.GetOrCreateCart(existingUid);
			}

			if (sku.Sku.Data.AutoShipProduct)
			{
				cart.Order.PromoSkus.Clear();
				cart.Order.Skus.Clear();
				cart.Order.Discount = null;
				cart.Order.GeneratedGcs.Clear();
				cart.Order.GiftCertificates.Clear();

				cart.Order.IdObjectType = (int)OrderType.AutoShip;
				cart.Order.Data.AutoShipFrequency = autoshipFrequency;
			}
			else if(cart.Order.Skus.Any() && cart.Order.IdObjectType == (int)OrderType.AutoShip)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CartContainsAutoShip]);
			}
			else
			{
				cart.Order.IdObjectType = (int)OrderType.Normal;
			}

			cart.Order.Skus.AddUpdateKeyed(Enumerable.Repeat(sku, 1).ToArray(),
				ordered => ordered.Sku.Code, skuModel => skuModel.Sku.Code, skuModel =>
				{
					var skuOrdered = _productService.GetSkuOrderedAsync(skuModel.Sku.Code).Result;
					skuOrdered.Quantity = 1;
					skuOrdered.Amount = HasRole(RoleType.Wholesale) ? skuModel.Sku.WholesalePrice : skuModel.Sku.Price;
					return skuOrdered;
				},
				(ordered, skuModel) => ordered.Quantity += 1);
			SetCartUid(cart.CartUid);
			if (!await _checkoutService.UpdateCart(cart))
				throw new ApiException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantAddProductToCart]);

			var context = await OrderService.CalculateOrder(cart.Order, OrderStatus.Incomplete);

		    return new Tuple<OrderDataContext, CustomerCartOrder>(context, cart);
	    }

	    private async Task<IList<CartCrossSellModel>> PopulateCartCrossSells(ContentCrossSellType type)
		{
			var crossSellModels = new List<CartCrossSellModel>();

			var crossSells = await _contentCrossSellService.GetContentCrossSellsAsync(type);
			if (crossSells.Any())
			{
				var skus = await _productService.GetSkusAsync(new VProductSkuFilter() { Ids = crossSells.Select(x => x.IdSku).ToList(), ActiveOnly = true, NotHiddenOnly = true});

				if (skus.Any())
				{
					crossSells = crossSells.Where(x => skus.Select(y => y.SkuId).Contains(x.IdSku)).ToList();

					var wholesale = await CustomerLoggedIn() && HasRole(RoleType.Wholesale);

					crossSellModels.AddRange(from crossSell in crossSells
											 let targetSku = skus.Single(x => x.SkuId == crossSell.IdSku)
											 select new CartCrossSellModel()
											 {
												 Title = crossSell.Title,
												 ImageUrl = crossSell.ImageUrl,
												 Price = wholesale ? targetSku.WholesalePrice ?? 0 : targetSku.Price ?? 0,
												 SkuCode = targetSku.Code
											 });
				}
			}

			return crossSellModels;
		}

		[HttpGet]
        public async Task<Result<ViewCartModel>> InitCartModel()
        {
			var cartModel = new ViewCartModel();

			return await InitCartModelInternal(cartModel);
        }

        public async Task<IActionResult> ViewCart()
        {
	        if (await IsCartEmpty())
	        {
		        return View("EmptyCart");
	        }

			var cartModel = new ViewCartModel();

			await InitCartModelInternal(cartModel);

	        cartModel.CrossSells = await PopulateCartCrossSells(ContentCrossSellType.ViewCart);

			return View(cartModel);
        }

        [HttpPost]
        public Task<Result<string>> ViewCart([FromBody] ViewCartModel model)
        {
            return Task.FromResult<Result<string>>(Url.Action("Welcome", "Checkout"));
        }

	    [HttpPost]
        public async Task<IActionResult> AddToCartView(string skuCode)
	    {
		    Result<ViewCartModel> cartRes = new Result<ViewCartModel>(true, new ViewCartModel());
			try
			{
				cartRes = await AddToCart(skuCode);
				if (!cartRes.Success || cartRes.Data == null)
				{
					throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.SkuNotFound]);
				}
			}
			catch (AppValidationException ex)
			{
				return PartialView("_CartLiteError", ex.Messages.First().Message);
			}

		    cartRes.Data.CrossSells = await PopulateCartCrossSells(ContentCrossSellType.AddToCart);

		    return PartialView("_CartLite", cartRes.Data);
        }

        [HttpPost]
        public async Task<Result<ViewCartModel>> AddToCart(string skuCode)
        {
			var result = new ViewCartModel();

	        var res = await AddToCartInternal(skuCode);

			FillModel(result, res.Item2.Order, res.Item1);
            SetCartUid(res.Item2.CartUid);
            ContextAccessor.HttpContext.Session.Remove(CheckoutConstants.ReceiptSessionOrderId);
            return result;
        }

        [HttpPost]
        public async Task<Result<ViewCartModel>> UpdateCart([FromBody] ViewCartModel model)
        {
            if (model.ShipAsap && model.ShippingDate.HasValue)
            {
                model.ShippingDate = null;
                ModelState.Clear();
            }

            model.ShippingDateError = !ModelState.IsValid
                ? ModelState["ShippingDate"].Errors.Select(x => x.ErrorMessage).FirstOrDefault()
                : string.Empty;

            var existingUid = Request.GetCartUid();
            CustomerCartOrder cart;
            if (await CustomerLoggedIn())
            {
                var id = GetInternalCustomerId();
                cart = await _checkoutService.GetOrCreateCart(existingUid, id);
            }
            else
            {
                cart = await _checkoutService.GetOrCreateCart(existingUid);
            }
            cart.Order.Skus?.MergeKeyed(model.Skus.Where(s => s.Quantity > 0).ToArray(), ordered => ordered.Sku.Code,
                skuModel => skuModel.Code, skuModel =>
                {
                    var result = _productService.GetSkuOrderedAsync(skuModel.Code).Result;
                    result.Quantity = skuModel.Quantity;
                    return result;
                }, (ordered, skuModel) => ordered.Quantity = skuModel.Quantity);
            cart.Order.Discount = await _discountService.GetByCode(model.PromoCode);
            var gcCodes = model.GiftCertificateCodes.Select(x => x.Value).ToList();
            cart.Order.GiftCertificates?.MergeKeyed(
                gcCodes.Select(code => _gcService.GetGiftCertificateAsync(code).Result).Where(g => g != null).ToArray(),
                gc => gc.GiftCertificate?.Code, code => code.Code,
                code => new GiftCertificateInOrder
                {
                    GiftCertificate = code
                });
            if (!model.ShipAsap)
            {
                cart.Order.Data.ShipDelayType = ShipDelayType.EntireOrder;
                if (model.ShippingDate == null)
                {
                    model.ShippingDateError = string.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.FieldIsRequired],
                        "Shipping Date");
                    return model;
                }
                cart.Order.Data.ShipDelayDate = model.ShippingDate;
            }
            else
            {
                cart.Order.Data.ShipDelayType = ShipDelayType.None;
                cart.Order.Data.ShipDelayDate = null;
            }
            cart.Order.Data.ShippingUpgradeP = model.ShippingUpgradeP;
            cart.Order.Data.ShippingUpgradeNP = model.ShippingUpgradeNP;
            if (ModelState.IsValid)
            {
                if (!await _checkoutService.UpdateCart(cart))
                {
                    return new Result<ViewCartModel>(false, model);
                }
            }
            var context = await OrderService.CalculateOrder(cart.Order, OrderStatus.Incomplete);
            FillModel(model, cart.Order, context);
            SetCartUid(cart.CartUid);
            return model;
        }

		[HttpGet]
		[CustomerAuthorize]
		public async Task<IActionResult> AutoShip(string id)
		{
			var sku = await _productService.GetSkuOrderedAsync(id);

			var options = AppInfrastructure.AutoShipOptions;

			var lookup = new List<SelectListItem>();
			if (sku.Sku.Data.AutoShipFrequency1)
			{
				var option = options.Single(x => x.Key == 1);
				lookup.Add(new SelectListItem() { Value = option.Key.ToString(), Text = option.Text });
			}
			if (sku.Sku.Data.AutoShipFrequency2)
			{
				var option = options.Single(x => x.Key == 2);
				lookup.Add(new SelectListItem() { Value = option.Key.ToString(), Text = option.Text });
			}
			if (sku.Sku.Data.AutoShipFrequency3)
			{
				var option = options.Single(x => x.Key == 3);
				lookup.Add(new SelectListItem() { Value = option.Key.ToString(), Text = option.Text });
			}
			if (sku.Sku.Data.AutoShipFrequency6)
			{
				var option = options.Single(x => x.Key == 6);
				lookup.Add(new SelectListItem() { Value = option.Key.ToString(), Text = option.Text });
			}

			ViewBag.AutoShipOptions = lookup;

			return View("AutoShip", new AutoShipModel()
			{
				SkuCode = sku.Sku.Code,
				DisplayName = $"{sku.Sku.Product.Name} {sku.Sku.Product.Data.SubTitle} ({sku.Sku.Data.QTY})",
				IdSchedule = lookup.FirstOrDefault() != null ? Convert.ToInt32(lookup.First().Value):0,
				ProductUrl = sku.Sku.Product.Url
			});
		}

		[HttpPost]
		[CustomerAuthorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AutoShip(AutoShipModel model)
		{
			var options = AppInfrastructure.AutoShipOptions;

			if (options.All(x => x.Key != model.IdSchedule))
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.AnyAutoShipOption]);
			}

			await AddToCartInternal(model.SkuCode, model.IdSchedule);

			return RedirectToAction("AddUpdateBillingAddress", "Checkout");
		}
	}
}