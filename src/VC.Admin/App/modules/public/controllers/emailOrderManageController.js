'use strict';

angular.module('app.modules.public.controllers.emailOrderManageController', [])
.controller('emailOrderManageController', ['$q', '$scope', '$rootScope', '$filter', '$injector', '$state', '$stateParams', '$timeout', 'modalUtil',
    'publicService', 'productService', 'customerService', 'toaster', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
function ($q, $scope, $rootScope, $filter, $injector, $state, $stateParams, $timeout, modalUtil,
    publicService, productService, customerService, toaster, confirmUtil, promiseTracker, gridSorterUtil)
{
    $scope.addEditTracker = promiseTracker("addEdit");

    function successSaveHandler(result)
    {
        if (result.Success)
        {
            if (result.Data)
            {
                modalUtil.open('app/modules/setting/partials/infoDetailsPopup.html', 'infoDetailsPopupController', {
                    Header: "Order Successfully Emailed!",
                    Messages: [{ Message: "Your order has been successfully emailed to customer service for review and input. Click OK to clear this form and send another order or simply close your browser window now." }],
                    OkButton: {
                        Label: 'Ok',
                        Handler: function ()
                        {
                            $state.reload();
                        }
                    },
                }, { size: 'xs', backdrop: false });
            }
        }
        else
        {
            $rootScope.fireServerValidation(result, $scope);
        }
    };

    function errorHandler(result)
    {
        toaster.pop('error', "Error!", "Server error occured");
    };

    function initialize()
    {
        $scope.forms = { submitted: [] };
        $scope.options = {};

        $scope.skusFilter = {
            Code: '',
            DescriptionName: '',
            ActiveOnly: true,
            Paging: { PageIndex: 1, PageItemCount: 20 },
        };

        $scope.skuFilter = {
            ExactCode: '',
            ExactDescriptionName: '',
            Paging: { PageIndex: 1, PageItemCount: 1 },
        };

        $scope.mainTab = {
            index: 1,
            formNames: ['topForm', 'mainForm'],
            name: 'New Order',
        };
        $scope.shippingAddressTab = {
            index: 3,
            formName: 'shipping',
            ShippingEditModels: {}
        };
        $scope.paymentInfoTab = {
            index: 4,
            formNames: ['marketing', 'nc'],
            AddressEditModels: {}
        };
        $scope.options.activeTabIndex = $scope.mainTab.index;
        var tabs = [];
        tabs.push($scope.mainTab);
        tabs.push($scope.shippingAddressTab);
        tabs.push($scope.paymentInfoTab);
        $scope.tabs = tabs;

        loadSettings();
    };

    var loadSettings = function ()
    {
        $q.all({
            countriesCall: customerService.getCountries($scope.addEditTracker),
            emailOrderSettingsCall: publicService.getEmailOrderSettings($scope.addEditTracker)
        }).then(function (result)
        {
            if (result.countriesCall.data.Success && result.emailOrderSettingsCall.data.Success)
            {
                $scope.allCountries = result.countriesCall.data.Data;
                if ($scope.allCountries)
                {
                    var countries = $.grep($scope.allCountries, function (item, i)
                    {
                        return item.IdVisibility == 1 || item.IdVisibility == 3;
                    });
                    $scope.countries = countries;
                }                

                $.each(result.emailOrderSettingsCall.data.Data, function (index, lookup)
                {
                    if (lookup.Name == 'EmailOrderRequestors')
                    {
                        $scope.options.EmailOrderRequestors = lookup.Items;
                    }
                    if (lookup.Name == 'EmailOrderReasons')
                    {
                        $scope.options.EmailOrderReasons = lookup.Items;
                    }
                });
                loadEmailOrder();
            }
        }, function (result)
        {
            errorHandler(result);
        });
    };

    var loadEmailOrder = function ()
    {
        publicService.getEmailOrder($scope.addEditTracker)
            .success(function (result)
            {
                if (result.Success)
                {
                    $scope.order = result.Data;
                    syncCountry($scope, $scope.order.Shipping);
                    $scope.order.Shipping.State = null;
                } else
                {
                    $scope.fireServerValidation(result);
                }
            }).
            error(function (result)
            {
                errorHandler(result);
            });
    };

    $scope.save = function ()
    {
        $scope.clearServerValidation();
        var valid = true;
        $.each($scope.forms, function (index, form)
        {
            if (form && index != 'submitted')
            {
                if (!form.$valid)
                {
                    valid = false;
                    $scope.activateTab(index);
                    return false;
                }
            }
        });

        if (valid && $scope.skusClientValid())
        {
            if (!isProductsValid($scope))
            {
                return;
            }

            var billingErrorMessages = '';
            if ($scope.order.IdPaymentMethodType == 7)
            {
                if ($scope.order.Marketing == null)
                {
                    billingErrorMessages += "Marketing payment info is required. ";
                }
            }
            if ($scope.order.IdPaymentMethodType == 4)
            {
                if ($scope.order.NC == null)
                {
                    billingErrorMessages += "No Charge is required. ";
                }
            }
            if (billingErrorMessages)
            {
                toaster.pop('error', 'Error!', billingErrorMessages, null, 'trustedHtml');
                $scope.options.activeTabIndex = $scope.paymentInfoTab.index;
                return;
            }

            var data = angular.copy($scope.order);

            publicService.sendEmailOrder(data, $scope.addEditTracker)
                .success(function (result)
                {
                    successSaveHandler(result);
                }).
                error(function (result)
                {
                    errorHandler(result);
                });
        } else
        {
            if ($scope.forms.topForm != null)
            {
                $scope.forms.topForm.submitted = true;
            }
            $scope.forms.mainForm.submitted = true;
            $scope.forms.submitted['shipping'] = true;
            $scope.forms.submitted['marketing'] = true;
            $scope.forms.submitted['nc'] = true;
            toaster.pop('error', "Error!", "Validation errors, please correct field values.", null, 'trustedHtml');
        }
    };

    var isProductsValid = function ($scope)
    {
        var productErrorMessages = '';
        if ($scope.order.SkuOrdereds.length == 1 && !$scope.order.SkuOrdereds[0].Code)
        {
            productErrorMessages += "You must add at least 1 product. ";
        }
        var productErrorsExist = false;
        angular.forEach($scope.order.SkuOrdereds, function (skuOrdered, index)
        {
            if (skuOrdered.ErrorMessages && skuOrdered.ErrorMessages.length != 0)
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
            $scope.activateTab($scope.mainTab.index);
            toaster.pop('error', 'Error!', productErrorMessages, null, 'trustedHtml');
            return false;
        }

        return true;
    };

    $scope.skusClientValid = function ()
    {
        var isValid = true;
        $.each($scope.order.SkuOrdereds, function (index, uiSku)
        {
            uiSku.ClientMessages = [];
        });
        for (var i = 0; i < $scope.order.SkuOrdereds.length; i++)
        {
            var current = $scope.order.SkuOrdereds[i];
            if (current.Code)
            {
                for (var j = i + 1; j < $scope.order.SkuOrdereds.length; j++)
                {
                    if (current.Code == $scope.order.SkuOrdereds[j].Code || (current.Code && $scope.order.SkuOrdereds[j].Code
                        && current.Code.toUpperCase() == $scope.order.SkuOrdereds[j].Code.toUpperCase()))
                    {
                        $scope.order.SkuOrdereds[j].ClientMessages.push("Duplicate SKU");
                        isValid = false;
                    }
                }
            }
        }
        return isValid;
    };

    $scope.clearServerValidation = function ()
    {
        $.each($scope.forms, function (index, form)
        {
            if (form)
            {
                $.each(form, function (index, element)
                {
                    if (element && element.$name == index)
                    {
                        element.$setValidity("server", true);
                    }
                });
            }
        });
    };

    $scope.getSKUsBySKU = function (val)
    {
        if (val)
        {
            $scope.skusFilter.Code = val;
            $scope.skusFilter.DescriptionName = '';
            return productService.getSkus($scope.skusFilter)
                .then(function (result)
                {
                    return result.data.Data.map(function (item)
                    {
                        return item;
                    });
                });
        }
    };

    $scope.skuChangedRequest = null;

    $scope.skuChanged = function (index)
    {
        //resolving issue with additional load after lost focus from the input in time of selecting a new element
        if ($scope.skuChangedRequest)
        {
            $timeout.cancel($scope.skuChangedRequest);
        }
        $scope.skuChangedRequest = $timeout(function ()
        {
            var product = $scope.order.SkuOrdereds[index];
            if (product == null || product.RequestedCode == product.Code)
            {
                return;
            }
            if (product.Code)
            {
                $scope.skuFilter.ExactCode = product.Code;
                $scope.skuFilter.Index = index;
                $scope.skuFilter.ExactDescriptionName = '';
                productService.getSku($scope.skuFilter)
                    .success(function (result)
                    {
                        if (result.Success)
                        {
                            if (result.Data)
                            {
                                product.RequestedCode = $scope.skuFilter.ExactCode;
                                product.QTY = 1;
                                product.IdProductType = result.Data.ProductType;
                                product.ProductName = result.Data.DescriptionName;
                                product.Id = result.Data.Id;
                                product.AutoShipProduct = result.Data.AutoShipProduct;
                                product.AutoShipFrequency1 = result.Data.AutoShipFrequency1;
                                product.AutoShipFrequency2 = result.Data.AutoShipFrequency2;
                                product.AutoShipFrequency3 = result.Data.AutoShipFrequency3;
                                product.AutoShipFrequency6 = result.Data.AutoShipFrequency6;
                                product.Price = result.Data.Price;
                                product.Amount = product.Price;

                                if ($scope.order.SkuOrdereds.length == index + 1)
                                {
                                    $scope.productAdd();
                                }
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
                $scope.skuChangedRequest = null;
            }
            else
            {
                if (product.RequestedCode)
                {
                    product.RequestedCode = '';
                }
            }
        }, 100);
    };

    $scope.getSKUsByProductName = function (val)
    {
        if (val)
        {
            $scope.skusFilter.Code = '';
            $scope.skusFilter.DescriptionName = val;
            return productService.getSkus($scope.skusFilter)
                .then(function (result)
                {
                    return result.data.Data.map(function (item)
                    {
                        return item;
                    });
                });
        }
    };

    $scope.productNameChanged = function (index)
    {
        var product = $scope.order.SkuOrdereds[index];
        if (product)
        {
            $scope.skuFilter.ExactCode = '';
            $scope.skuFilter.ExactDescriptionName = product.ProductName;
            productService.getSku($scope.skuFilter)
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
                            product.Price = result.Data.Price;
                            product.Amount = product.Price;

                            if ($scope.order.SkuOrdereds.length == index + 1)
                            {
                                $scope.productAdd();
                            }
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

    $scope.productAdd = function ()
    {
        if ($scope.order.SkuOrdereds.length > 0 && !$scope.order.SkuOrdereds[$scope.order.SkuOrdereds.length - 1].Code)
        {
            return;
        }
        var product = { Code: '', Id: null, QTY: null, ProductName: '', Price: null, Amount: null, IdProductType: null, Messages: [] };
        $scope.order.SkuOrdereds.push(product);
    };

    $scope.productDelete = function (index)
    {
        if ($scope.order.SkuOrdereds.length == 1)
        {
            $scope.order.SkuOrdereds.splice(index, 1);
            $scope.productAdd();
        }
        else
        {
            $scope.order.SkuOrdereds.splice(index, 1);
        }
    };

    $scope.buildOrderShippingAddressForPartial = function ()
    {
        if ($scope.order === undefined || $scope.order.Shipping === undefined)
            return undefined;
        if ($scope.shippingAddressTab.OrderShippingEditModel === undefined ||
            $scope.shippingAddressTab.OrderShippingEditModel.Address != $scope.order.Shipping)
        {
            $scope.shippingAddressTab.OrderShippingEditModel = { Address: $scope.order.Shipping, formName: 'shipping', recalculate: false };
        }
        $scope.shippingAddressTab.OrderShippingEditModel.disableValidation = false;
        return $scope.shippingAddressTab.OrderShippingEditModel;
    };

    $scope.makeShippingAsBillingAddressOrder = function ()
    {
        var address;
        switch (String($scope.order.IdPaymentMethodType))
        {
            case "4":
                if ($scope.order.NC.Address)
                {
                    address = $scope.order.NC.Address;
                }
                break;
            case "7":
                if ($scope.order.Marketing.Address)
                {
                    address = $scope.order.Marketing.Address;
                }
                break;
        }
        if (address)
        {
            var activeShipping = $scope.buildOrderShippingAddressForPartial();
            if (activeShipping)
            {
                for (var key in address)
                {
                    activeShipping.Address[key] = address[key];
                }
                address.AddressType = 3;
            }
        }
    };

    $scope.makeBillingAsShippingAddressOrder = function ()
    {
        var address;
        switch (String($scope.order.IdPaymentMethodType))
        {
            case "4":
                address = $scope.order.NC.Address;
                break;
            case "7":
                address = $scope.order.Marketing.Address;
                break;
        }
        if (address)
        {
            var activeShipping = $scope.buildOrderShippingAddressForPartial();
            if (activeShipping)
            {
                for (var key in activeShipping.Address)
                {
                    address[key] = activeShipping.Address[key];
                }

                address.AddressType = 2;
            }
        }
    };

    $scope.setNewMarketing = function (callback)
    {
        if ($scope.forms.marketing.$valid)
        {
            customerService.createMarketingPrototype($scope.addEditTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        $scope.order.Marketing = result.Data;
                        syncCountry($scope, result.Data.Address);
                        $scope.order.Marketing.formName = 'marketing';
                        if (callback)
                            callback(result.Data);
                    } else
                    {
                        successHandler(result);
                    }
                }).
                error(function (result)
                {
                    errorHandler(result);
                })
                .then(function ()
                {
                    $scope.forms.submitted['marketing'] = false;
                });
        }
        else
        {
            $scope.forms.submitted['marketing'] = true;
        }
        return false;
    };

    $scope.setNewNC = function (callback)
    {
        if ($scope.forms.nc.$valid)
        {
            customerService.createNCPrototype($scope.addEditTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        $scope.order.NC = result.Data;
                        syncCountry($scope, result.Data.Address);
                        $scope.order.NC.formName = 'nc';
                        if (callback)
                            callback(result.Data);
                    } else
                    {
                        successHandler(result);
                    }
                }).
                error(function (result)
                {
                    errorHandler(result);
                })
                .then(function ()
                {
                    $scope.forms.submitted['nc'] = false;
                });
        }
        else
        {
            $scope.forms.submitted['nc'] = true;
        }
        return false;
    };

    var syncCountry = function (scope, addressItem)
    {
        if (addressItem && addressItem.Country)
        {
            var selectedCountry = $.grep(scope.countries, function (country)
            {
                return country.Id == addressItem.Country.Id;
            })[0];

            addressItem.Country = selectedCountry;
        }
    };

    $scope.activateTab = function (formName)
    {
        $.each($scope.tabs, function (index, item)
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
                $scope.options.activeTabIndex = itemForActive.index;
                return false;
            }
        });
    };

    $scope.recalculate = function (item)
    {
        if (item.QTY && item.Price)
        {
            var qty = parseInt(item.QTY);
            var price = parseFloat(item.Price);
            item.Amount = Math.round(qty * price*100)/100;
        }
        else
        {
            item.Amount = 0;
        }
    };

    initialize();
}]);