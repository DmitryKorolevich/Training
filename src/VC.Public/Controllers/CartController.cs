using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using VC.Public.Models.Cart;
using VC.Public.Models.Checkout;
using VitalChoice.Core.GlobalFilters;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Interfaces.Services.Checkout;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Content.ContentCrossSells;
using VitalChoice.Infrastructure.Domain.Entities.Roles;
using VitalChoice.Infrastructure.Domain.Entities.Settings;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Cart;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Infrastructure.Identity.UserManagers;
using VitalChoice.Interfaces.Services.Content;
using VitalChoice.Interfaces.Services.Settings;
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

        public CartController(
            ICustomerService customerService,
            IOrderService orderService, IProductService productService, ICheckoutService checkoutService,
            IAuthorizationService authorizationService, ReferenceData appReferenceData,
            IDynamicMapper<SkuDynamic, Sku> skuMapper, IDynamicMapper<ProductDynamic, Product> productMapper,
            IDiscountService discountService, IGcService gcService, IContentCrossSellService contentCrossSellService, ISettingService settingService, ExtendedUserManager userManager, AppSettings appSettings)
            : base(
                customerService, appReferenceData, authorizationService, checkoutService, orderService, skuMapper, productMapper,
                settingService, userManager, appSettings)
        {
            _productService = productService;
            _checkoutService = checkoutService;
            _discountService = discountService;
            _gcService = gcService;
            _contentCrossSellService = contentCrossSellService;
        }

        [HttpGet]
        public IActionResult EmptyCart()
        {
            return View("EmptyCart");
        }

        [HttpGet]
        public Task<IActionResult> GetCartLiteComponent()
        {
            return Task.FromResult<IActionResult>(ViewComponent("CartLite"));
        }

        [HttpGet]
        public async Task<Result<ViewCartModel>> InitCartModel()
        {
            var cartModel = new ViewCartModel();

            var result = await InitCartModelInternal(cartModel);
            if (!ModelState.IsValid)
            {
                return ResultHelper.CreateErrorResult(ModelState, result);
            }
            return result;
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

        [HttpGet]
        public Task<Result<bool>> CheckCustomerStatus()
        {
            return Task.FromResult<Result<bool>>(true);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCartView([FromBody] ICollection<CartSkuModel> skus)
        {
            try
            {
                var result = new ViewCartModel();
                var addResult = await AddToCartInternal(skus.ToDictionary(s => s.Code));
                await FillModel(result, addResult.Item2.Order, addResult.Item1);
                HttpContext.SetCartUid(addResult.Item2.CartUid);
                HttpContext.Session.Remove(CheckoutConstants.ReceiptSessionOrderIds);
                result.CrossSells = await PopulateCartCrossSells(ContentCrossSellType.AddToCart);
                return PartialView("_CartLite", result);
            }
            catch (AppValidationException ex)
            {
                return PartialView("_CartLiteError", ex.Messages.First().Message);
            }
        }

        [HttpPost]
        public async Task<Result<ViewCartModel>> AddToCart(string skuCode)
        {
            var result = new ViewCartModel();

            var res = await AddToCartInternal(
                new CartSkuModel
                {
                    Code = skuCode,
                    Quantity = 1
                });

            await FillModel(result, res.Item2.Order, res.Item1);
            HttpContext.SetCartUid(res.Item2.CartUid);
            HttpContext.Session.Remove(CheckoutConstants.ReceiptSessionOrderIds);
            return result;
        }

        [HttpPost]
        public async Task<Result<ViewCartModel>> UpdateCart([FromBody] ViewCartModel model)
        {
            bool canUpdate = true;
            if (model == null)
            {
                canUpdate = false;
                model = new ViewCartModel();
                await InitCartModelInternal(model);
            }
            if (model.ShipAsap && model.ShippingDate.HasValue)
            {
                model.ShippingDate = null;
            }

            var cart = await GetCurrentCart();
            using (await CartLocks.GetLockAsync(cart.CartUid))
            {
                if (cart.Order.Skus != null)
                {
                    await
                        cart.Order.Skus.UpdateRemoveKeyedAsync(model.Skus.Where(s => s.Quantity > 0).ToArray(), ordered => ordered.Sku.Code,
                            skuModel => skuModel.Code, (ordered, skuModel) =>
                            {
                                ordered.Quantity = skuModel.Quantity;
                                return TaskCache.CompletedTask;
                            });
                }
                cart.Order.Discount = await _discountService.GetByCode(model.DiscountCode);
                var gcCodes =
                    model.GiftCertificateCodes.Select(x => x.Value?.Trim().ToUpper())
                        .Where(v => !string.IsNullOrWhiteSpace(v))
                        .Distinct()
                        .ToList();
                cart.Order.GiftCertificates?.MergeKeyed(await _gcService.TryGetGiftCertificatesAsync(gcCodes),
                    gc => gc.GiftCertificate?.Code?.Trim().ToUpper(), code => code.Code?.Trim().ToUpper(),
                    code => new GiftCertificateInOrder
                    {
                        GiftCertificate = code
                    });
                if (!model.ShipAsap)
                {
                    cart.Order.Data.ShipDelayType = ShipDelayType.EntireOrder;
                    if (model.ShippingDate == null)
                    {
                        ModelState.AddModelError("ShippingDate",
                            string.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.FieldIsRequired], "Shipping Date"));
                        return ResultHelper.CreateErrorResult(ModelState, model);
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
                var context = await OrderService.CalculateStorefrontOrder(cart.Order, OrderStatus.Incomplete);
                await FillModel(model, cart.Order, context);

                bool updateResult = true;
                if (canUpdate)
                {
                    //TODO: should be sent withMultipleShipmentsService=true for 4 step
                    updateResult = await _checkoutService.UpdateCart(cart);
                }
                if (!ModelState.IsValid)
                    return ResultHelper.CreateErrorResult(ModelState, model);
                if (!updateResult)
                    return new Result<ViewCartModel>(false, model);
                return model;
            }
        }

        [HttpGet]
        [CustomerAuthorize]
        public async Task<IActionResult> AutoShip(string id)
        {
            var sku = await _productService.GetSkuOrderedAsync(id);

            var options = ReferenceData.AutoShipOptions;

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
                DisplayName = $"{sku.Sku.Product.Name} {sku.Sku.Product.SafeData.SubTitle} ({sku.Sku.SafeData.QTY})",
                IdSchedule = lookup.FirstOrDefault() != null ? Convert.ToInt32(lookup.First().Value) : 0,
                ProductUrl = sku.Sku.Product.Url
            });
        }

        [HttpPost]
        [CustomerAuthorize]
        [CustomValidateAntiForgeryToken]
        public async Task<IActionResult> AutoShip(AutoShipModel model)
        {
            var options = ReferenceData.AutoShipOptions;

            if (options.All(x => x.Key != model.IdSchedule))
            {
                throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.AnyAutoShipOption]);
            }

            await AddToCartInternal(new CartSkuModel
            {
                Code = model.SkuCode,
                Quantity = 1
            }, model.IdSchedule);

            return RedirectToAction("AddUpdateShippingMethod", "Checkout");
        }

        private async Task<Tuple<OrderDataContext, CustomerCartOrder>> AddToCartInternal(CartSkuModel skuModelToAdd,
            int? autoshipFrequency = null)
        {
            var existingUid = HttpContext.GetCartUid();
            // ReSharper disable once PossibleInvalidOperationException
            using (await CartLocks.LockWhenAsync(() => existingUid.HasValue, () => existingUid.Value))
            {
                var sku = await _productService.GetSkuOrderedAsync(skuModelToAdd.Code);
                if (sku == null)
                    throw new ApiException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantAddProductToCart]);
                CustomerCartOrder cart;
                var loggedIn = await CustomerLoggedIn();
                var id = GetInternalCustomerId();
                if (loggedIn && id.HasValue)
                {
                    cart = await _checkoutService.GetOrCreateCart(existingUid, id.Value);
                }
                else
                {
                    cart = await _checkoutService.GetOrCreateCart(existingUid, false);
                }
                HttpContext.SetCartUid(cart.CartUid);
                using (
                    await CartLocks.LockWhenAsync(() => existingUid.HasValue && cart.CartUid != existingUid.Value, () => cart.CartUid))
                {
                    if (sku.Sku.SafeData.AutoShipProduct == true && autoshipFrequency.HasValue)
                    {
                        cart.Order.PromoSkus.Clear();
                        cart.Order.Skus.Clear();
                        cart.Order.Discount = null;
                        cart.Order.GiftCertificates.Clear();

                        cart.Order.IdObjectType = (int) OrderType.AutoShip;
                        cart.Order.Data.AutoShipFrequency = autoshipFrequency;
                    }
                    else if (cart.Order.Skus.Count > 0 && cart.Order.IdObjectType == (int) OrderType.AutoShip)
                    {
                        throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CartContainsAutoShip]);
                    }
                    else
                    {
                        cart.Order.IdObjectType = (int) OrderType.Normal;
                    }

                    cart.Order.Skus.AddUpdateKeyed(new[] {sku},
                        ordered => ordered.Sku.Code, skuModel => skuModel.Sku.Code, skuModel =>
                        {
                            var targetQuantity = 1;
                            if (skuModel.Sku.Code == skuModelToAdd.Code)
                            {
                                if (skuModelToAdd.Quantity > 0)
                                {
                                    targetQuantity = skuModelToAdd.Quantity;
                                }
                            }
                            skuModel.Quantity = targetQuantity;
                            skuModel.Amount = HasRole(RoleType.Wholesale) ? skuModel.Sku.WholesalePrice : skuModel.Sku.Price;
                            return skuModel;
                        },
                        (ordered, skuModel) =>
                        {
                            var targetQuantity = 1;
                            if (skuModel.Sku.Code == skuModelToAdd.Code)
                            {
                                if (skuModelToAdd.Quantity > 0)
                                {
                                    targetQuantity = skuModelToAdd.Quantity;
                                }
                            }
                            ordered.Quantity += targetQuantity;
                        });

                    var context = await OrderService.CalculateStorefrontOrder(cart.Order, OrderStatus.Incomplete);
                    if (!await _checkoutService.UpdateCart(cart))
                        throw new ApiException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantAddProductToCart]);
                    return new Tuple<OrderDataContext, CustomerCartOrder>(context, cart);
                }
            }
        }

        private async Task<Tuple<OrderDataContext, CustomerCartOrder>> AddToCartInternal(Dictionary<string, CartSkuModel> skuModels,
            int? autoshipFrequency = null)
        {
            var existingUid = HttpContext.GetCartUid();

            // ReSharper disable once PossibleInvalidOperationException
            using (await CartLocks.LockWhenAsync(() => existingUid.HasValue, () => existingUid.Value))
            {
                var skus = await _productService.GetSkusOrderedAsync(skuModels.Values.Select(p => p.Code).ToList());
                if (skus.Count == 0)
                    throw new ApiException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantAddProductToCart]);
                CustomerCartOrder cart;
                var loggedIn = await CustomerLoggedIn();
                var id = GetInternalCustomerId();
                if (loggedIn && id.HasValue)
                {
                    cart = await _checkoutService.GetOrCreateCart(existingUid, id.Value);
                }
                else
                {
                    cart = await _checkoutService.GetOrCreateCart(existingUid, false);
                }
                HttpContext.SetCartUid(cart.CartUid);
                using (
                    await CartLocks.LockWhenAsync(() => existingUid.HasValue && cart.CartUid != existingUid.Value, () => cart.CartUid))
                {
                    if (skus.Count == 1 && skus.First().Sku.SafeData.AutoShipProduct == true && autoshipFrequency.HasValue)
                    {
                        cart.Order.PromoSkus.Clear();
                        cart.Order.Skus.Clear();
                        cart.Order.Discount = null;
                        cart.Order.GiftCertificates.Clear();

                        cart.Order.IdObjectType = (int) OrderType.AutoShip;
                        cart.Order.Data.AutoShipFrequency = autoshipFrequency;
                    }
                    else if (cart.Order.Skus.Count > 0 && cart.Order.IdObjectType == (int) OrderType.AutoShip)
                    {
                        throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CartContainsAutoShip]);
                    }
                    else
                    {
                        cart.Order.IdObjectType = (int) OrderType.Normal;
                    }

                    cart.Order.Skus.AddUpdateKeyed(skus,
                        ordered => ordered.Sku.Code, skuModel => skuModel.Sku.Code, skuModel =>
                        {
                            CartSkuModel cartSkuModel;
                            var targetQuantity = 1;
                            if (skuModels.TryGetValue(skuModel.Sku.Code, out cartSkuModel))
                            {
                                if (cartSkuModel.Quantity > 0)
                                {
                                    targetQuantity = cartSkuModel.Quantity;
                                }
                            }
                            skuModel.Quantity = targetQuantity;
                            skuModel.Amount = HasRole(RoleType.Wholesale) ? skuModel.Sku.WholesalePrice : skuModel.Sku.Price;
                            return skuModel;
                        },
                        (ordered, skuModel) =>
                        {
                            CartSkuModel cartSkuModel;
                            var targetQuantity = 1;
                            if (skuModels.TryGetValue(skuModel.Sku.Code, out cartSkuModel))
                            {
                                if (cartSkuModel.Quantity > 0)
                                {
                                    targetQuantity = cartSkuModel.Quantity;
                                }
                            }
                            ordered.Quantity += targetQuantity;
                        });

                    var context = await OrderService.CalculateStorefrontOrder(cart.Order, OrderStatus.Incomplete);
                    if (!await _checkoutService.UpdateCart(cart))
                        throw new ApiException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantAddProductToCart]);
                    return new Tuple<OrderDataContext, CustomerCartOrder>(context, cart);
                }
            }
        }

        private async Task<IList<CartCrossSellModel>> PopulateCartCrossSells(ContentCrossSellType type)
        {
            var crossSellModels = new List<CartCrossSellModel>();

            var crossSells = await _contentCrossSellService.GetContentCrossSellsAsync(type);
            if (crossSells.Count > 0)
            {
                var skus =
                    await
                        _productService.GetSkusAsync(new VProductSkuFilter()
                        {
                            Ids = crossSells.Select(x => x.IdSku).ToList(),
                            ActiveOnly = true,
                            NotHiddenOnly = true
                        });

                if (skus.Count > 0)
                {
                    var skuIds = new HashSet<int>(skus.Select(y => y.Id));

                    crossSells = crossSells.Where(x => skuIds.Contains(x.IdSku)).ToList();

                    var wholesale = await CustomerLoggedIn() && HasRole(RoleType.Wholesale);

                    crossSellModels.AddRange(from crossSell in crossSells
                                             let targetSku = skus.Single(x => x.Id == crossSell.IdSku)
                                             select new CartCrossSellModel()
                                             {
                                                 Title = crossSell.Title,
                                                 ImageUrl = crossSell.ImageUrl,
                                                 Price = wholesale ? targetSku.WholesalePrice : targetSku.Price,
                                                 SkuCode = targetSku.Code
                                             });
                }
            }

            return crossSellModels;
        }
    }
}