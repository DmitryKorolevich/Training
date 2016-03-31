'use strict';

angular.module('app.modules.order.services.orderEditService', [])
.factory('orderEditService', ['$q', '$filter', '$injector', '$state', '$rootScope', '$timeout', 'customerEditService', 'orderService', 'productService', 'gcService', 'discountService', 'toaster', 'modalUtil',
    function ($q, $filter, $injector, $state, $rootScope, $timeout, customerEditService, orderService, productService, gcService, discountService, toaster, modalUtil)
{
    var initBase = function (uiScope)
    {
        uiScope.activateTab = function (formName)
        {
            if (formName.indexOf('GCs') == 0)
            {
                formName = 'GCs';
            }
            $.each(uiScope.tabs, function (index, item)
            {
                var itemForActive = null;
                if (item.formName == formName)
                {
                    itemForActive = item;
                }
                if (item.formNames)
                {
                    $.each(item.formNames, function (index, form)
                    {
                        if (form == formName)
                        {
                            itemForActive = item;
                            return false;
                        }
                    });
                }
                if (itemForActive)
                {
                    itemForActive.active = true;
                    return false;
                }
            });
        };

        uiScope.fireServerValidation = function (result)
        {
            var messages = "";
            if (result.Messages)
            {
                if (uiScope.forms.topForm != null)
                {
                    uiScope.forms.topForm.submitted = true;
                }
                uiScope.forms.mainForm.submitted = true;
                if (uiScope.forms.mainForm2 != null)
                {
                    uiScope.forms.mainForm2.submitted = true;
                }
                if (uiScope.forms.GCs)
                {
                    uiScope.forms.GCs.skussubmitted = true;
                }
                uiScope.forms.submitted['profile'] = true;
                uiScope.forms.submitted['shipping'] = true;
                uiScope.forms.submitted['card'] = true;
                uiScope.forms.submitted['oac'] = true;
                uiScope.forms.submitted['check'] = true;
                uiScope.forms.submitted['wiretransfer'] = true;
                uiScope.forms.submitted['marketing'] = true;
                uiScope.forms.submitted['vcwellness'] = true;
                uiScope.serverMessages = new ServerMessages(result.Messages);
                uiScope.calculateServerMessages = new ServerMessages(result.Messages);
                var formForShowing = null;
                var form;
                $.each(result.Messages, function (index, value)
                {
                    if (value.Field)
                    {
                        if (value.Field.indexOf("::") >= 0)
                        {
                            var arr = value.Field.split("::");
                            var formName = arr[0];
                            var fieldName = arr[1];
                            if (fieldName.indexOf(".") >= 0)
                            {
                                arr = fieldName.split('.');
                                var collectionName = arr[0];
                                var indexWithName = arr[1];
                                switch (collectionName)
                                {
                                    case 'Shipping':
                                        var collectionIndex = indexWithName.split('i')[1];
                                        uiScope.shippingAddressTab.AddressIndex = collectionIndex;
                                        form = uiScope.forms[formName];
                                        fieldName = arr[2];
                                        if (form[fieldName] != undefined)
                                        {
                                            form[fieldName].$setValidity("server", false);
                                            if (formForShowing == null)
                                            {
                                                formForShowing = formName;
                                            }
                                        }
                                        break;
                                    case 'CreditCards':
                                        var collectionIndex = indexWithName.split('i')[1];
                                        uiScope.paymentInfoTab.CreditCardIndex = collectionIndex;
                                        form = uiScope.forms[formName];
                                        fieldName = arr[2];
                                        if (form[fieldName] != undefined)
                                        {
                                            form[fieldName].$setValidity("server", false);
                                            if (formForShowing == null)
                                            {
                                                formForShowing = formName;
                                            }
                                        }
                                        break;
                                }
                            }
                            form = uiScope.forms[formName];
                            if (form[fieldName] != undefined)
                            {
                                form[fieldName].$setValidity("server", false);
                                if (formForShowing == null)
                                {
                                    formForShowing = formName;
                                }
                            }
                        } else
                        {
                            $.each(uiScope.forms, function (index, form)
                            {
                                if (form && index !== "submitted")
                                {
                                    if (form[value.Field] != undefined)
                                    {
                                        form[value.Field].$setValidity("server", false);
                                        if (formForShowing == null)
                                        {
                                            formForShowing = index;
                                        }
                                    }
                                }
                            });
                        }
                    }
                    messages += value.Message + "<br />";
                });

                if (formForShowing)
                {
                    uiScope.activateTab(formForShowing);
                }
            }
            toaster.pop('error', "Error!", messages, null, 'trustedHtml');
        };
        
        uiScope.buildOrderShippingAddressForPartial = function (disableValidation)
        {
            if (uiScope.order === undefined || uiScope.order.Shipping === undefined)
                return undefined;
            if (uiScope.shippingAddressTab.OrderShippingEditModel === undefined ||
                uiScope.shippingAddressTab.OrderShippingEditModel.Address != uiScope.order.Shipping)
            {
                uiScope.shippingAddressTab.OrderShippingEditModel = { Address: uiScope.order.Shipping, formName: 'shipping', recalculate: true };
            }
            uiScope.shippingAddressTab.OrderShippingEditModel.disableValidation = disableValidation;
            return uiScope.shippingAddressTab.OrderShippingEditModel;
        }

        uiScope.buildOrderPaymentPartial = function (model, disableValidation)
        {
            model.disableValidation = disableValidation;
            return model;
        }
        
        uiScope.oldOrderForCalculating = null;

        uiScope.skusClientValid = function ()
        {
            var isValid = true;
            $.each(uiScope.order.SkuOrdereds, function (index, uiSku)
            {
                uiSku.ClientMessages = [];
            });
            for (var i = 0; i < uiScope.order.SkuOrdereds.length; i++)
            {
                var current = uiScope.order.SkuOrdereds[i];
                if (current.Code)
                {
                    for (var j = i + 1; j < uiScope.order.SkuOrdereds.length; j++)
                    {
                        if (current.Code == uiScope.order.SkuOrdereds[j].Code)
                        {
                            uiScope.order.SkuOrdereds[j].ClientMessages.push("Duplicate SKU");
                            isValid = false;
                        }
                    }
                }
            }
            return isValid;
        };        

        uiScope.clearServerValidation = function ()
        {
            $.each(uiScope.forms, function (index, form)
            {
                if (form)
                {
                    if (index == "GCs")
                    {
                        $.each(form, function (index, subForm)
                        {
                            if (index.indexOf('i') == 0)
                            {
                                $.each(subForm, function (index, element)
                                {
                                    if (element && element.$name == index)
                                    {
                                        element.$setValidity("server", true);
                                    }
                                });
                            }
                        });
                    }
                    else
                    {
                        $.each(form, function (index, element)
                        {
                            if (element && element.$name == index)
                            {
                                element.$setValidity("server", true);
                            }
                        });
                    }
                }
            });
        };        

        uiScope.gcLostFocus = function (index, code)
        {
            if (index != 0 && !code)
            {
                uiScope.order.GCs.splice(index, 1);
            }
            uiScope.requestRecalculate();
        };

        uiScope.getGCs = function (val)
        {
            uiScope.gcsFilter.Code = val;
            return gcService.getGiftCertificates(uiScope.gcsFilter)
                .then(function (result)
                {
                    return result.data.Data.Items.map(function (item)
                    {
                        return item.Code;
                    });
                });
        };

        uiScope.getDiscounts = function (val)
        {
            uiScope.discountsFilter.Code = val;
            return discountService.getDiscounts(uiScope.discountsFilter)
                .then(function (result)
                {
                    return result.data.Data.Items.map(function (item)
                    {
                        return item.Code;
                    });
                });
        };

        uiScope.productAdd = function ()
        {
            if (uiScope.order.SkuOrdereds.length > 0 && !uiScope.order.SkuOrdereds[uiScope.order.SkuOrdereds.length - 1].Code)
            {
                return;
            }
            var product = { Code: '', Id: null, QTY: null, ProductName: '', Price: null, Amount: null, IdProductType: null, Messages: [] };
            uiScope.order.SkuOrdereds.push(product);
        };

        uiScope.productDelete = function (index)
        {
            if (uiScope.order.SkuOrdereds.length == 1)
            {
                uiScope.order.SkuOrdereds.splice(index, 1);
                uiScope.productAdd();
            }
            else
            {
                uiScope.order.SkuOrdereds.splice(index, 1);
            }
            uiScope.requestRecalculate();
        };

        uiScope.topPurchasedProducts = function ()
        {
            modalUtil.open('app/modules/product/partials/topPurchasedProductsPopup.html', 'topPurchasedProductsController', {
                products: uiScope.order.SkuOrdereds, idCustomer: uiScope.currentCustomer.Id, thenCallback: function (data)
                {
                    var newProducts = data;
                    $.each(newProducts, function (index, newProduct)
                    {
                        var add = true;
                        $.each(uiScope.order.SkuOrdereds, function (index, product)
                        {
                            if (newProduct.Code == product.Code)
                            {
                                add = false;
                                return false;
                            }
                        });

                        if (add)
                        {
                            if (uiScope.order.SkuOrdereds.length > 0 && !uiScope.order.SkuOrdereds[uiScope.order.SkuOrdereds.length - 1].Code)
                            {
                                uiScope.order.SkuOrdereds.splice(uiScope.order.SkuOrdereds.length - 1, 1);
                            }

                            var product = {};
                            product.QTY = 1;
                            product.Code = newProduct.Code;
                            product.IdProductType = newProduct.ProductType;
                            product.Id = newProduct.Id;
                            product.ProductName = newProduct.DescriptionName;
                            if (uiScope.currentCustomer.CustomerType == 1)
                            {
                                product.Price = newProduct.Price;
                            }
                            else if (uiScope.currentCustomer.CustomerType == 2)
                            {
                                product.Price = newProduct.WholesalePrice;
                            }
                            product.Amount = product.Price;

                            uiScope.order.SkuOrdereds.push(product);
                        }
                    });

                    uiScope.requestRecalculate();
                }
            });
        };

        uiScope.gcAdd = function ()
        {
            uiScope.order.GCs.push({ Code: '' });
        };

        uiScope.gcDelete = function (index)
        {
            uiScope.order.GCs.splice(index, 1);
        };

        uiScope.getSKUsBySKU = function (val)
        {
            if (val)
            {
                uiScope.skusFilter.Code = val;
                uiScope.skusFilter.DescriptionName = '';
                return productService.getSkus(uiScope.skusFilter)
                    .then(function (result)
                    {
                        return result.data.Data.map(function (item)
                        {
                            return item;
                        });
                    });
            }
        };

        uiScope.skuChangedRequest = null;

        uiScope.skuChanged = function (index)
        {
            //resolving issue with additional load after lost focus from the input in time of selecting a new element
            if (uiScope.skuChangedRequest)
            {
                $timeout.cancel(uiScope.skuChangedRequest);
            }
            uiScope.skuChangedRequest = $timeout(function ()
            {
                var product = uiScope.order.SkuOrdereds[index];
                if (product == null || product.RequestedCode == product.Code)
                {
                    return;
                }
                if (product.Code)
                {
                    uiScope.skuFilter.ExactCode = product.Code;
                    uiScope.skuFilter.Index = index;
                    uiScope.skuFilter.ExactDescriptionName = '';
                    productService.getSku(uiScope.skuFilter)
                        .success(function (result)
                        {
                            if (result.Success)
                            {
                                if (result.Data)
                                {
                                    product.RequestedCode = uiScope.skuFilter.ExactCode;
                                    product.QTY = 1;
                                    product.IdProductType = result.Data.ProductType;
                                    product.ProductName = result.Data.DescriptionName;
                                    product.Id = result.Data.Id;
                                    product.AutoShipProduct = result.Data.AutoShipProduct;
                                    product.AutoShipFrequency1 = result.Data.AutoShipFrequency1;
                                    product.AutoShipFrequency2 = result.Data.AutoShipFrequency2;
                                    product.AutoShipFrequency3 = result.Data.AutoShipFrequency3;
                                    product.AutoShipFrequency6 = result.Data.AutoShipFrequency6;
                                    if (uiScope.currentCustomer.CustomerType == 1)
                                    {
                                        product.Price = result.Data.Price;
                                    }
                                    else if (uiScope.currentCustomer.CustomerType == 2)
                                    {
                                        product.Price = result.Data.WholesalePrice;
                                    }
                                    product.Amount = product.Price;

                                    if (uiScope.order.SkuOrdereds.length == index + 1)
                                    {
                                        uiScope.productAdd();
                                    }

                                    uiScope.requestRecalculate();
                                }
                            } else
                            {
                                errorHandler(result);
                            }
                        })
                        .error(function (result)
                        {
                            errorHandler(result);
                        });
                    uiScope.skuChangedRequest = null;
                }
                else
                {
                    if(product.RequestedCode)
                    {
                        product.RequestedCode='';
                        uiScope.requestRecalculate();
                    }
                }
            }, 20);
        };

        uiScope.getSKUsByProductName = function (val)
        {
            if (val)
            {
                uiScope.skusFilter.Code = '';
                uiScope.skusFilter.DescriptionName = val;
                return productService.getSkus(uiScope.skusFilter)
                    .then(function (result)
                    {
                        return result.data.Data.map(function (item)
                        {
                            return item;
                        });
                    });
            }
        };

        uiScope.productNameChanged = function (index)
        {
            var product = uiScope.order.SkuOrdereds[index];
            if (product)
            {
                uiScope.skuFilter.ExactCode = '';
                uiScope.skuFilter.ExactDescriptionName = product.ProductName;
                productService.getSku(uiScope.skuFilter)
                    .success(function (result)
                    {
                        if (result.Success)
                        {
                            if (result.Data)
                            {
                                product.QTY = 1;
                                product.IdProductType = result.Data.ProductType;
                                product.Code = result.Data.Code;
                                product.RequestedCode = product.Code;
                                product.Id = result.Data.Id;
                                product.AutoShipProduct = result.Data.AutoShipProduct;
                                product.AutoShipFrequency1 = result.Data.AutoShipFrequency1;
                                product.AutoShipFrequency2 = result.Data.AutoShipFrequency2;
                                product.AutoShipFrequency3 = result.Data.AutoShipFrequency3;
                                product.AutoShipFrequency6 = result.Data.AutoShipFrequency6;
                                if (uiScope.currentCustomer.CustomerType == 1)
                                {
                                    product.Price = result.Data.Price;
                                }
                                else if (uiScope.currentCustomer.CustomerType == 2)
                                {
                                    product.Price = result.Data.WholesalePrice;
                                }
                                product.Amount = product.Price;

                                if (uiScope.order.SkuOrdereds.length == index + 1)
                                {
                                    uiScope.productAdd();
                                }

                                uiScope.requestRecalculate();
                            }
                        } else
                        {
                            errorHandler(result);
                        }
                    })
                    .error(function (result)
                    {
                        errorHandler(result);
                    });
            }
        };

        uiScope.refreshOrderHistory = function ()
        {
            if (uiScope.id)
            {
                var data = {};
                data.service = orderService;
                data.tracker = uiScope.addEditTracker;
                data.idObject = uiScope.id;
                data.idObjectType = 2//order
                uiScope.$broadcast('objectHistorySection#in#refresh', data);
            }
        };

        uiScope.sendOrderConfirmationEmail = function ()
        {
            modalUtil.open('app/modules/order/partials/sendOrderConfirmationPopup.html', 'sendOrderConfirmationController', {
                Email: uiScope.currentCustomer.Email,
                Id: uiScope.order.Id,
                thenCallback: function (data)
                {

                }
            }, { size: 'xs' });
        };

        uiScope.sendOrderShippingConfirmationEmail = function ()
        {
            modalUtil.open('app/modules/order/partials/sendOrderShippingConfirmationPopup.html', 'sendOrderShippingConfirmationController', {
                Email: uiScope.currentCustomer.Email,
                OrderStatus: uiScope.order.OrderStatus,
                POrderStatus: uiScope.order.POrderStatus,
                NPOrderStatus: uiScope.order.NPOrderStatus,
                Id: uiScope.order.Id,
                thenCallback: function (data)
                {

                }
            }, { size: 'xs' });
        };

        uiScope.goToCustomer = function ()
        {
            $state.go('index.oneCol.customerDetail', { id: uiScope.idCustomer });
        };
    };

    var initAutoShipLogic = function (uiScope)
    {
        uiScope.initAutoShipOptions = function ()
        {
            //show/hide autoship option
            uiScope.autoShipOrderOptionShow = (uiScope.order.SkuOrdereds.length == 1 || (uiScope.order.SkuOrdereds.length == 2 && !uiScope.order.SkuOrdereds[1].Code))
                && uiScope.order.PromoSkus.length == 0 && uiScope.order.SkuOrdereds[0].AutoShipProduct;
            if (uiScope.autoShipOrderOptionShow)
            {
                var items = [];
                if (uiScope.order.SkuOrdereds[0].AutoShipFrequency1)
                {
                    items.push({ Key: 1, Text: '1 Month' });
                }
                if (uiScope.order.SkuOrdereds[0].AutoShipFrequency2)
                {
                    items.push({ Key: 2, Text: '2 Months' });
                }
                if (uiScope.order.SkuOrdereds[0].AutoShipFrequency3)
                {
                    items.push({ Key: 3, Text: '3 Months' });
                }
                if (uiScope.order.SkuOrdereds[0].AutoShipFrequency6)
                {
                    items.push({ Key: 6, Text: '6 Months' });
                }
                uiScope.autoShipOrderFrequencies = items;
            } else if (uiScope.order.IdObjectType == 2)
            {
                uiScope.order.IdObjectType = 1;
            }
        };
    };    

    var initRecalculate = function (uiScope)
    {
        uiScope.requestRecalculate = function ()
        {
            //additional client validation
            if (!uiScope.skusClientValid())
            {
                return;
            }

            angular.forEach(uiScope.currentCustomer.Shipping, function (shippingItem, index)
            {
                shippingItem.IsSelected = index.toString() == uiScope.shippingAddressTab.AddressIndex;
            });
            var orderForCalculating = angular.copy(uiScope.order);
            orderForCalculating.Customer = angular.copy(uiScope.currentCustomer);
            if (angular.equals(uiScope.oldOrderForCalculating, orderForCalculating))
            {
                return;
            }
            uiScope.oldOrderForCalculating = orderForCalculating;
            if (uiScope.currectCalculateCanceller)
            {
                uiScope.currectCalculateCanceller.resolve("canceled");
            }
            uiScope.currectCalculateCanceller = $q.defer();
            orderService.calculateOrder(orderForCalculating, uiScope.currectCalculateCanceller)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        successCalculateHandler(result.Data);
                    } else
                    {
                        errorHandler(result);
                    }
                    if (uiScope.currectCalculateCanceller)
                    {
                        uiScope.currectCalculateCanceller.reject();
                        uiScope.currectCalculateCanceller = null;
                    }
                })
                .error(function (result)
                {
                    if (result == "canceled")
                    {
                        errorHandler(result);
                        if (uiScope.currectCalculateCanceller)
                        {
                            uiScope.currectCalculateCanceller.reject();
                            uiScope.currectCalculateCanceller = null;
                        }
                    }
                });
        };

        function successCalculateHandler(data)
        {
            uiScope.order.AlaskaHawaiiSurcharge = data.AlaskaHawaiiSurcharge;
            uiScope.order.CanadaSurcharge = data.CanadaSurcharge;
            uiScope.order.StandardShippingCharges = data.StandardShippingCharges;
            uiScope.order.ShippingTotal = data.ShippingTotal;
            uiScope.order.TotalShipping = data.TotalShipping;
            uiScope.order.ProductsSubtotal = data.ProductsSubtotal;
            uiScope.order.DiscountTotal = data.DiscountTotal;
            uiScope.order.DiscountedSubtotal = data.DiscountedSubtotal;
            uiScope.order.DiscountMessage = data.DiscountMessage;
            uiScope.order.TaxTotal = data.TaxTotal;
            uiScope.order.GiftCertificatesSubtotal = data.GiftCertificatesSubtotal;
            uiScope.order.Total = data.Total;

            uiScope.order.ShouldSplit = data.ShouldSplit;

            uiScope.shippingUpgradePOptions = data.ShippingUpgradePOptions;
            uiScope.shippingUpgradeNPOptions = data.ShippingUpgradeNPOptions;

            uiScope.order.ShippingOverride = data.ShippingOverride;
            uiScope.order.SurchargeOverride = data.SurchargeOverride;

            uiScope.productsPerishableThresholdIssue = data.ProductsPerishableThresholdIssue;

            var toDeleteIdxs = [];
            $.each(uiScope.order.SkuOrdereds, function (index, uiSku)
            {
                var found = false;
                $.each(data.SkuOrdereds, function (index, sku)
                {
                    if (uiSku.Code == sku.Code)
                    {
                        uiSku.Price = sku.Price;
                        uiSku.Amount = sku.Amount;
                        uiSku.Quantity = sku.Quantity;
                        uiSku.Messages = sku.Messages;
                        uiSku.GCCodes = sku.GCCodes;
                        found = true;
                        return false;
                    }
                });
                if (!found && uiSku.Id != null)
                {
                    toDeleteIdxs.push(index);
                }
            });

            $.each(toDeleteIdxs, function (index, item)
            {
                uiScope.order.SkuOrdereds.splice(item, 1);
            });

            uiScope.order.PromoSkus = data.PromoSkus;

            //clear the main tab left part validation
            $.each(uiScope.forms, function (index, form)
            {
                if (form)
                {
                    if (index == "GCs")
                    {
                        $.each(form, function (index, subForm)
                        {
                            if (index.indexOf('i') == 0)
                            {
                                $.each(subForm, function (index, element)
                                {
                                    if (element && element.$name == index)
                                    {
                                        element.$setValidity("server", true);
                                    }
                                });
                            }
                        });
                    }
                    else if (index == "mainForm2")
                    {
                        $.each(form, function (index, element)
                        {
                            if (element && element.$name == index)
                            {
                                element.$setValidity("server", true);
                            }
                        });
                    }
                }
            });

            //set server validation for the main tab left part 
            if (data.Messages)
            {
                if (uiScope.forms.mainForm2 != null)
                {
                    uiScope.forms.mainForm2.submitted = true;
                }
                if (uiScope.forms.GCs)
                {
                    uiScope.forms.GCs.skussubmitted = true;
                }
                uiScope.calculateErrors = data.Messages;
                uiScope.calculateServerMessages = new ServerMessages(data.Messages);
                var formForShowing = null;
                $.each(data.Messages, function (index, value)
                {
                    if (value.Field)
                    {
                        if (value.Field.indexOf('.') > -1)
                        {
                            var items = value.Field.split(".");
                            uiScope.forms[items[0]][items[1]][items[2]].$setValidity("server", false);
                            formForShowing = items[0];
                            openSKUs();
                        }
                        else
                        {
                            $.each(uiScope.forms, function (index, form)
                            {
                                if (form)
                                {
                                    if (form[value.Field] != undefined)
                                    {
                                        form[value.Field].$setValidity("server", false);
                                        return false;
                                    }
                                }
                            });
                        }
                    }
                });
            }

            if (uiScope.initAutoShipOptions)
            {
                uiScope.initAutoShipOptions();
            }
        }
    };

    var baseProcessLoadingOrder = function(uiScope)
    {
        if (uiScope.order.ShipDelayDate)
        {
            uiScope.order.ShipDelayDate = Date.parseDateTime(uiScope.order.ShipDelayDate);
        }
        if (uiScope.order.ShipDelayDateP)
        {
            uiScope.order.ShipDelayDateP = Date.parseDateTime(uiScope.order.ShipDelayDateP);
        }
        if (uiScope.order.ShipDelayDateNP)
        {
            uiScope.order.ShipDelayDateNP = Date.parseDateTime(uiScope.order.ShipDelayDateNP);
        }
        if (uiScope.order.IgnoneMinimumPerishableThreshold)
        {
            uiScope.options.ignoneMinimumPerishableThreshold = uiScope.order.IgnoneMinimumPerishableThreshold;
        }
    };

    var initOrderOptions = function (uiScope)
    {
        uiScope.options.oldOrderStatus = uiScope.order.CombinedEditOrderStatus;
        uiScope.order.OnHold = uiScope.order.CombinedEditOrderStatus == 7;//on hold status
        if (uiScope.order.IdObjectType == 1 || uiScope.order.IdObjectType == 2)
        {
            if (uiScope.order.AutoShipFrequency)
            {
                uiScope.order.IdObjectType = 2;
            } else
            {
                uiScope.order.IdObjectType = 1;
            }
        }
        if (uiScope.onHoldWatch)
            uiScope.onHoldWatch();
        uiScope.onHoldWatch = uiScope.$watch('order.OnHold', function (newValue, oldValue)
        {
            if (uiScope.order.CombinedEditOrderStatus != 3 && uiScope.order.CombinedEditOrderStatus != 4 &&
                uiScope.order.CombinedEditOrderStatus != 5)
            {
                if (newValue !== undefined && newValue !== null)
                {
                    if (newValue)
                    {
                        uiScope.order.CombinedEditOrderStatus = 7;
                    }
                    else
                    {
                        if (uiScope.order.CombinedEditOrderStatus != uiScope.options.oldOrderStatus)
                        {
                            uiScope.order.CombinedEditOrderStatus = uiScope.options.oldOrderStatus;
                        }
                        else
                        {

                            uiScope.order.CombinedEditOrderStatus = 2; //processed
                        }
                    }
                }
            }
        });
        uiScope.orderEditDisabled = uiScope.order.CombinedEditOrderStatus == 3 || uiScope.order.CombinedEditOrderStatus == 4
            || uiScope.order.CombinedEditOrderStatus == 5;

        uiScope.legend.CustomerName = uiScope.currentCustomer.ProfileAddress.FirstName + " " + uiScope.currentCustomer.ProfileAddress.LastName;
        if (uiScope.id)
        {
            uiScope.legend.OrderId = uiScope.order.Id;
            uiScope.legend.OrderDate = uiScope.order.DateCreated;
            if (uiScope.order.OrderStatus)
            {
                uiScope.legend.OrderStatus = $rootScope.getReferenceItem($rootScope.ReferenceData.OrderStatuses, uiScope.order.OrderStatus).Text;
            }
            else
            {
                var legendOrderStatus = '';
                if (uiScope.order.POrderStatus)
                {
                    legendOrderStatus += 'P - {0}'.format($rootScope.getReferenceItem($rootScope.ReferenceData.OrderStatuses, uiScope.order.POrderStatus).Text);
                }
                if (uiScope.order.NPOrderStatus)
                {
                    if (legendOrderStatus.length > 0)
                    {
                        legendOrderStatus += ', ';
                    }
                    legendOrderStatus += 'NP - {0}'.format($rootScope.getReferenceItem($rootScope.ReferenceData.OrderStatuses, uiScope.order.NPOrderStatus).Text);
                }
                uiScope.legend.OrderStatus = legendOrderStatus;
            }
        }
        else
        {
            uiScope.legend.CustomerId = uiScope.idCustomer;
        }
    };

    var baseReferencedDataInit = function(uiScope){
        if (uiScope.currentCustomer.SourceDetails)
        {
            uiScope.currentCustomer.SourceValue = uiScope.currentCustomer.SourceDetails;
        } else if (uiScope.currentCustomer.Source)
        {
            var sourceName = uiScope.currentCustomer.Source;
            if (uiScope.orderSources)
            {
                $.each(uiScope.orderSources, function (index, orderSource)
                {
                    if (orderSource.Key == uiScope.currentCustomer.Source)
                    {
                        sourceName = orderSource.Text;
                        return false;
                    }
                });
            }
            uiScope.currentCustomer.SourceValue = sourceName;
        }

        angular.forEach(uiScope.currentCustomer.CreditCards, function (creditCard, index)
        {
            creditCard.formName = "card";
            customerEditService.syncCountry(uiScope, creditCard.Address);
            uiScope.paymentInfoTab.CreditCardIndex = "0";
            if (creditCard.Default)
            {
                uiScope.paymentInfoTab.CreditCardIndex = index.toString();
            }
        });

        if (uiScope.currentCustomer.Oac)
        {
            uiScope.currentCustomer.Oac.formName = "oac";
            customerEditService.syncCountry(uiScope, uiScope.currentCustomer.Oac.Address);
        }
        if (uiScope.currentCustomer.Check)
        {
            uiScope.currentCustomer.Check.formName = "check";
            customerEditService.syncCountry(uiScope, uiScope.currentCustomer.Check.Address);
        }
        if (uiScope.currentCustomer.WireTransfer)
        {
            uiScope.currentCustomer.WireTransfer.formName = "wiretransfer";
            customerEditService.syncCountry(uiScope, uiScope.currentCustomer.WireTransfer.Address);
        }
        if (uiScope.currentCustomer.Marketing)
        {
            uiScope.currentCustomer.Marketing.formName = "marketing";
            customerEditService.syncCountry(uiScope, uiScope.currentCustomer.Marketing.Address);
        }
        if (uiScope.currentCustomer.VCWellness)
        {
            uiScope.currentCustomer.VCWellness.formName = "vcwellness";
            customerEditService.syncCountry(uiScope, uiScope.currentCustomer.VCWellness.Address);
        }

        if (uiScope.order.CreditCard)
        {
            uiScope.order.CreditCard.formName = "card";
            customerEditService.syncCountry(uiScope, uiScope.order.CreditCard.Address);
        }
        if (uiScope.order.Oac)
        {
            uiScope.order.Oac.formName = "oac";
            customerEditService.syncCountry(uiScope, uiScope.order.Oac.Address);
        }
        if (uiScope.order.Check)
        {
            uiScope.order.Check.formName = "check";
            customerEditService.syncCountry(uiScope, uiScope.order.Check.Address);
        }
        if (uiScope.order.WireTransfer)
        {
            uiScope.order.WireTransfer.formName = "wiretransfer";
            customerEditService.syncCountry(uiScope, uiScope.order.WireTransfer.Address);
        }
        if (uiScope.order.Marketing)
        {
            uiScope.order.Marketing.formName = "marketing";
            customerEditService.syncCountry(uiScope, uiScope.order.Marketing.Address);
        }
        if (uiScope.order.VCWellness)
        {
            uiScope.order.VCWellness.formName = "vcwellness";
            customerEditService.syncCountry(uiScope, uiScope.order.VCWellness.Address);
        }
    };

    var baseReferencedDataInitExistOrder = function (uiScope)
    {
        if (uiScope.order.SkuOrdereds)
        {
            $.each(uiScope.order.SkuOrdereds, function (index, item)
            {
                item.RequestedCode = item.Code;
            });
        }

        uiScope.paymentInfoTab.PaymentMethodType = uiScope.order.IdPaymentMethodType;

        uiScope.$watch('paymentInfoTab.CreditCardIndex', function (newValue, oldValue)
        {
            if (newValue && oldValue != newValue)
            {
                //uiScope.order.CreditCard = angular.copy(uiScope.currentCustomer.CreditCards[parseInt(newValue)]);
                uiScope.order.CreditCard = uiScope.currentCustomer.CreditCards[parseInt(newValue)];
            }
        });

        if (!uiScope.order.CreditCard)
        {
            if (uiScope.currentCustomer.CreditCards && uiScope.currentCustomer.CreditCards[0])
            {
                uiScope.order.CreditCard = uiScope.currentCustomer.CreditCards[0];
            }
        }
        if (!uiScope.order.Oac)
        {
            uiScope.order.Oac = uiScope.currentCustomer.Oac;
        }
        if (!uiScope.order.Check)
        {
            uiScope.order.Check = uiScope.currentCustomer.Check;
        }
        if (!uiScope.order.WireTransfer)
        {
            uiScope.order.WireTransfer = uiScope.currentCustomer.WireTransfer;
        }
        if (!uiScope.order.Marketing)
        {
            uiScope.order.Marketing = uiScope.currentCustomer.Marketing;
        }
        if (!uiScope.order.VCWellness)
        {
            uiScope.order.VCWellness = uiScope.currentCustomer.VCWellness;
        }

        if (uiScope.paymentInfoTab.PaymentMethodType == 1)
        {
            uiScope.paymentInfoTab.CreditCard = uiScope.order.CreditCard;
        }
    };

    var orderDataProcessingBeforeSave = function(uiScope){
        if (uiScope.currentCustomer.newEmail || uiScope.currentCustomer.emailConfirm)
        {
            uiScope.currentCustomer.Email = uiScope.currentCustomer.newEmail;
            uiScope.currentCustomer.EmailConfirm = uiScope.currentCustomer.emailConfirm;
        } else
        {
            uiScope.currentCustomer.EmailConfirm = uiScope.currentCustomer.Email;
        }

        if (uiScope.order.ShipDelayType == 1)
        {
            uiScope.order.ShipDelayDateP = null;
            uiScope.order.ShipDelayDateNP = null;
        }
        if (uiScope.order.ShipDelayType == 2)
        {
            uiScope.order.ShipDelayDate = null;
        }

        var order = angular.copy(uiScope.order);
        order.Customer = angular.copy(uiScope.currentCustomer);

        if (order.Customer.InceptionDate)
        {
            order.Customer.InceptionDate = order.Customer.InceptionDate.toServerDateTime();
        }

        if (order.IdObjectType != 2)
        {
            order.AutoShipFrequency = null;
        }

        if (order.ShipDelayDate)
        {
            order.ShipDelayDate = order.ShipDelayDate.toServerDateTime();
        }
        if (order.ShipDelayDateP)
        {
            order.ShipDelayDateP = order.ShipDelayDateP.toServerDateTime();
        }
        if (order.ShipDelayDateNP)
        {
            order.ShipDelayDateNP = order.ShipDelayDateNP.toServerDateTime();
        }

        if (uiScope.options.OverrideEmail)
        {
            order.Customer.Email = null;
            order.Customer.EmailConfirm = null;
        }

        order.SignUpNewsletter = uiScope.options.BrontoSubscribedStatus;

        return order;
    };
        
    var isProductsValid = function(uiScope){
        var productErrorMessages = '';
        if (uiScope.order.SkuOrdereds.length == 1 && !uiScope.order.SkuOrdereds[0].Code)
        {
            productErrorMessages += "You must add at least 1 product. ";
        }
        var productErrorsExist = false;
        if (uiScope.calculateErrors != null && uiScope.calculateErrors.length != 0)
        {
            productErrorsExist = true;
        }
        if (uiScope.productsPerishableThresholdIssue && !uiScope.options.ignoneMinimumPerishableThreshold)
        {
            productErrorsExist = true;
        }
        if (uiScope.productsPerishableThresholdIssue)
        {
            uiScope.order.IgnoneMinimumPerishableThreshold = uiScope.productsPerishableThresholdIssue && uiScope.options.ignoneMinimumPerishableThreshold;
        }
        angular.forEach(uiScope.order.SkuOrdereds, function (skuOrdered, index)
        {
            if (skuOrdered.Messages && skuOrdered.Messages.length != 0)
            {
                productErrorsExist = true;
                return;
            }
        });
        if (productErrorsExist)
        {
            productErrorMessages += "There are some errors in the order. ";
        }

        if (productErrorMessages)
        {
            uiScope.mainTab.active = true;
            toaster.pop('error', 'Error!', productErrorMessages, null, 'trustedHtml');
            return false;
        }

        return true;
    };

    return {
        initBase: initBase,
        initAutoShipLogic : initAutoShipLogic,
        initRecalculate: initRecalculate,
        baseProcessLoadingOrder: baseProcessLoadingOrder,
        initOrderOptions: initOrderOptions,
        baseReferencedDataInit: baseReferencedDataInit,
        baseReferencedDataInitExistOrder: baseReferencedDataInitExistOrder,
        orderDataProcessingBeforeSave: orderDataProcessingBeforeSave,
        isProductsValid: isProductsValid,
    };

}]);