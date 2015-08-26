'use strict';

angular.module('app.modules.order.controllers.orderManageController', [])
.controller('orderManageController', ['$q', '$scope', '$rootScope', '$filter', '$injector', '$state', '$stateParams', '$timeout', 'modalUtil', 'orderService', 'customerService',
    'productService', 'gcService', 'discountService', 'toaster', 'confirmUtil', 'promiseTracker', 'customerEditService',
function ($q, $scope, $rootScope, $filter, $injector, $state, $stateParams, $timeout, modalUtil, orderService, customerService, productService, gcService, discountService,
    toaster, confirmUtil, promiseTracker, customerEditService)
{
    $scope.addEditTracker = promiseTracker("addEdit");

    function successSaveHandler(result)
    {
        if (result.Success)
        {
            toaster.pop('success', "Success!", "Successfully saved.");
            $scope.goBack();
        }
        else
        {
            var messages = "";
            if (result.Messages)
            {
                $scope.forms.mainForm.submitted = true;
                $scope.forms.mainForm2.submitted = true;
                forms.GCs.skussubmitted = true;
                $scope.forms.submitted['profile'] = true;
                $scope.forms.submitted['shipping'] = true;
                $scope.forms.submitted['billing'] = true;
                $scope.serverMessages = new ServerMessages(result.Messages);
                $scope.calculateServerMessages = new ServerMessages(result.Messages);
                var formForShowing = null;
                $.each(result.Messages, function (index, value)
                {
                    if (value.Field)
                    {
                        if (value.Field.indexOf('.') > -1)
                        {
                            var items = value.Field.split(".");
                            $scope.forms[items[0]][items[1]][items[2]].$setValidity("server", false);
                            formForShowing = items[0];
                            openSKUs();
                        }
                        else
                        {
                            $.each($scope.forms, function (index, form)
                            {
                                if (form)
                                {
                                    if (form[value.Field] != undefined)
                                    {
                                        form[value.Field].$setValidity("server", false);
                                        if (formForShowing == null)
                                        {
                                            formForShowing = index;
                                        }
                                        return false;
                                    }
                                }
                            });
                        }
                    }
                });

                if (formForShowing)
                {
                    activateTab(formForShowing);
                }
            }
            toaster.pop('error', "Error!", messages, null, 'trustedHtml');
        }
    };

    function activateTab(formName)
    {
        if (formName.indexOf('GCs') == 0)
        {
            formName = 'GCs';
        }
        $.each($scope.tabs, function (index, item)
        {
            if (item.formName == formName)
            {
                item.active = true;
            }
            if (item.formNames)
            {
                $.each(item.formNames, function (index, form)
                {
                    if (form == formName)
                    {
                        item.active = true;
                        return false;
                    }
                });
            }
            if (item.active)
            {
                return false;
            }
        });
    };

    function errorHandler(result)
    {
        toaster.pop('error', "Error!", "Server error occured");
    };

    function initialize()
    {
        $scope.id = $stateParams.id ? $stateParams.id : 0;
        $scope.idCustomer = $stateParams.idcustomer ? $stateParams.idcustomer : 0;

        $scope.forms = { submitted: [] };

        $scope.autoShipOrderFrequencies = [
            { Key: 1, Text: '1 Month' },
            { Key: 2, Text: '2 Months' },
            { Key: 3, Text: '3 Months' },
            { Key: 6, Text: '6 Months' }
        ];

        $scope.minimumPerishableThreshold = 65;//should be loaded on edit open
        $scope.ignoneMinimumPerishableThreshold = false;
        $scope.orderSources = $rootScope.ReferenceData.OrderSources;
        $scope.orderSourcesCelebrityHealthAdvocate = $rootScope.ReferenceData.OrderSourcesCelebrityHealthAdvocate;
        $scope.orderPreferredShipMethod = $rootScope.ReferenceData.OrderPreferredShipMethod;
        $scope.creditCardTypes = $rootScope.ReferenceData.CreditCardTypes;

        $scope.discountsFilter = {
            Code: '',
            Paging: { PageIndex: 1, PageItemCount: 20 },
        };

        $scope.gcsFilter = {
            Code: '',
            Paging: { PageIndex: 1, PageItemCount: 20 },
        };

        $scope.skusFilter = {
            Code: '',
            DescriptionName: '',
            Paging: { PageIndex: 1, PageItemCount: 20 },
        };

        $scope.skuFilter = {
            ExactCode: '',
            ExactDescriptionName: '',
            Paging: { PageIndex: 1, PageItemCount: 1 },
        };

        $scope.legend = {};

        $scope.mainTab = {
            active: true,
            formNames: ['mainForm', 'mainForm2', 'GCs'],
            name: $scope.id ? 'Edit Order' : 'New Order',
        };
        $scope.accountProfileTab = {
            active: false,
            formName: 'profile',
        };
        $scope.shippingAddressTab = {
            active: false,
            formName: 'shipping',
        };
        $scope.paymentInfoTab = {
            active: false,
            formName: 'billing'
        };
        $scope.customerNotesTab = {
            active: false,
            formName: 'customerNote',
        };
        var tabs = [];
        tabs.push($scope.mainTab);
        tabs.push($scope.accountProfileTab);
        tabs.push($scope.shippingAddressTab);
        tabs.push($scope.paymentInfoTab);
        tabs.push($scope.customerNotesTab);
        $scope.tabs = tabs;

        loadOrder();
    };

    var loadOrder = function ()
    {
        orderService.getOrder($scope.id, $scope.addEditTracker)
            .success(function (result)
            {
                if (result.Success)
                {
                    $scope.order=result.Data;

                    customerEditService.initBase($scope);
                    if ($scope.id)
                    {
                        $scope.idCustomer = $scope.order.IdCustomer;
                        customerEditService.initOrderEditCustomerParts($scope);
                    }
                    else
                    {
                        $scope.order.UpdateShippingAddressForCustomer = true;
                        customerEditService.initCustomerEdit($scope);
                    }
                    loadReferencedData();
                } else
                {
                    errorHandler(result);
                }
            })
            .error(function (result)
            {
                errorHandler(result);
            });
    };

    var loadReferencedData = function ()
    {
        $q.all({
            countriesCall: customerService.getCountries($scope.addEditTracker),
            customerNotePrototypeCall: customerService.createCustomerNotePrototype($scope.addEditTracker),
            customerGetCall: customerService.getExistingCustomer($scope.idCustomer, $scope.addEditTracker)
        }).then(function (result)
        {
            if (result.countriesCall.data.Success && result.customerNotePrototypeCall.data.Success && result.customerGetCall.data.Success)
            {
                $scope.countries = result.countriesCall.data.Data;

                $scope.customerNotePrototype = result.customerNotePrototypeCall.data.Data;
                $scope.customerNote = $scope.customerNotePrototype;

                $scope.currentCustomer = result.customerGetCall.data.Data;
                if ($scope.currentCustomer.SourceDetails)
                {
                    $scope.currentCustomer.SourceValue = $scope.currentCustomer.SourceDetails;
                } else if ($scope.currentCustomer.Source)
                {
                    $scope.currentCustomer.SourceValue = $scope.currentCustomer.Source;
                }
                $scope.accountProfileTab.Address = $scope.currentCustomer.ProfileAddress;
                customerEditService.syncCountry($scope, $scope.currentCustomer.ProfileAddress);

                customerEditService.syncCountry($scope, $scope.order.Shipping);
                angular.forEach($scope.currentCustomer.Shipping, function (shippingItem)
                {
                    customerEditService.syncCountry($scope, shippingItem);
                });
                if ($scope.id)
                {
                    $scope.shippingAddressTab.Address = $scope.order.Shipping;
                }
                else
                {
                    angular.forEach($scope.currentCustomer.Shipping, function (shippingItem)
                    {
                        if (shippingItem.Default)
                        {
                            $scope.shippingAddressTab.Address = shippingItem;
                        }
                    });
                }

                $scope.paymentInfoTab.Address = {};
                angular.forEach($scope.currentCustomer.CreditCards, function (creditCard)
                {
                    creditCard.formName = $scope.paymentInfoTab.formName;
                    customerEditService.syncCountry($scope, creditCard.Address);
                });
                if ($scope.currentCustomer.Oac)
                {
                    $scope.currentCustomer.Oac.formName = $scope.paymentInfoTab.formName;
                    customerEditService.syncCountry($scope, $scope.currentCustomer.Oac.Address);
                }
                if ($scope.currentCustomer.Check)
                {
                    $scope.currentCustomer.Check.formName = $scope.paymentInfoTab.formName;
                    customerEditService.syncCountry($scope, $scope.currentCustomer.Check.Address);
                }

                if ($scope.order.CreditCard)
                {
                    $scope.order.CreditCard.formName = $scope.paymentInfoTab.formName;
                    customerEditService.syncCountry($scope, $scope.order.CreditCard.Address);
                }
                if ($scope.order.Oac)
                {
                    $scope.order.Oac.formName = $scope.paymentInfoTab.formName;
                    customerEditService.syncCountry($scope, $scope.order.Oac.Address);
                }
                if ($scope.order.Check)
                {
                    $scope.order.Check.formName = $scope.paymentInfoTab.formName;
                    customerEditService.syncCountry($scope, $scope.order.Check.Address);
                }

                if ($scope.id)
                {
                    $scope.paymentInfoTab.PaymentMethodType = $scope.order.IdPaymentMethodType;

                    if (!$scope.order.CreditCard)
                    {
                        if ($scope.currentCustomer.CreditCards && $scope.currentCustomer.CreditCards[0])
                        {
                            $scope.order.CreditCard = $scope.currentCustomer.CreditCards[0];
                        }
                    }
                    if (!$scope.order.Oac)
                    {
                        $scope.order.CreditCard = $scope.currentCustomer.Oac;
                    }
                    if (!$scope.order.Check)
                    {
                        $scope.order.Check = $scope.currentCustomer.Check;
                    }

                    if ($scope.paymentInfoTab.PaymentMethodType == 1)
                    {
                        $scope.paymentInfoTab.CreditCard = $scope.order.CreditCard;
                    }
                }
                else
                {
                    $scope.paymentInfoTab.PaymentMethodType = $scope.currentCustomer.DefaultPaymentMethod;
                    if ($scope.currentCustomer.CreditCards && $scope.currentCustomer.CreditCards[0])
                    {
                        $scope.paymentInfoTab.CreditCard = $scope.currentCustomer.CreditCards[0];
                    }
                }

                customerEditService.syncDefaultPaymentMethod($scope);
                customerEditService.showHighPriNotes($scope);

                initOrder();
            }
            else
            {
                errorHandler(result);
            }
        }, function (result)
        {
            errorHandler(result);
        });
    };

    var initOrder = function ()
    {
        $scope.order.OnHold = $scope.order.StatusCode = 3;//on hold status
        $scope.order.AutoShip = $scope.order.AutoShipFrequency ? true : false;
        $scope.$watch('order.OnHold', function (newValue, oldValue)
        {
            if (newValue)
            {
                $scope.order.StatusCode = 3;
            }
        });

        //TODO: set needed data to the legend row
        $scope.legend.CustomerName = "Test";
        $scope.legend.CustomerId = $scope.idCustomer;
    };

    var oldOrderForCalculating = null;

    $scope.requestRecalculate = function ()
    {
        var orderForCalculating = angular.copy($scope.order);
        if (angular.equals(oldOrderForCalculating, orderForCalculating))
        {
            return;
        }

        oldOrderForCalculating = orderForCalculating;
        if ($scope.currectCalculateCanceller)
        {
            $scope.currectCalculateCanceller.resolve("canceled");
        }
        $scope.currectCalculateCanceller = $q.defer();
        orderService.calculateOrder(orderForCalculating, $scope.currectCalculateCanceller)
            .success(function (result)
            {
                if (result.Success)
                {
                    successCalculateHandler(result.Data);
                } else
                {
                    errorHandler(result);
                }
                if ($scope.currectCalculateCanceller)
                {
                    $scope.currectCalculateCanceller.reject();
                    $scope.currectCalculateCanceller = null;
                }
            })
            .error(function (result)
            {
                if (result == "canceled")
                {
                    errorHandler(result);
                    if ($scope.currectCalculateCanceller)
                    {
                        $scope.currectCalculateCanceller.reject();
                        $scope.currectCalculateCanceller = null;
                    }
                }
            });
    };

    function successCalculateHandler(data)
    {
        $scope.order.AlaskaHawaiiSurcharge = data.AlaskaHawaiiSurcharge;
        $scope.order.CanadaSurcharge = data.CanadaSurcharge;
        $scope.order.StandardShippingCharges = data.StandardShippingCharges;
        $scope.order.ShippingTotal = data.ShippingTotal;
        $scope.order.ProductsSubtotal = data.ProductsSubtotal;
        $scope.order.DiscountTotal = data.DiscountTotal;
        $scope.order.DiscountedSubtotal = data.DiscountedSubtotal;
        $scope.order.DiscountMessage = data.DiscountMessage;
        $scope.order.TaxTotal = data.TaxTotal;
        $scope.order.Total = data.Total;

        $scope.shippingUpgradePOptions = data.ShippingUpgradePOptions;
        $scope.shippingUpgradeNPOptions = data.ShippingUpgradeNPOptions;

        $scope.productsPerishableThresholdIssue = data.ProductsPerishableThresholdIssue;

        $.each($scope.order.SkuOrdereds, function (index, uiSku)
        {
            $.each(data.SkuOrdereds, function (index, sku)
            {
                if (uiSku.Code == sku.Code)
                {
                    uiSku.Amount = sku.Amount;
                    uiSku.Messages = sku.Messages;
                    return false;
                }
            });
        });

        //clear the main tab left part validation
        $.each($scope.forms, function (index, form)
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
                else if(index=="mainForm2")
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
            $scope.forms.mainForm2.submitted = true;
            forms.GCs.skussubmitted = true;
            $scope.calculateServerMessages = new ServerMessages(result.Messages);
            var formForShowing = null;
            $.each(data.Messages, function (index, value)
            {
                if (value.Field)
                {
                    if (value.Field.indexOf('.') > -1)
                    {
                        var items = value.Field.split(".");
                        $scope.forms[items[0]][items[1]][items[2]].$setValidity("server", false);
                        formForShowing = items[0];
                        openSKUs();
                    }
                    else
                    {
                        $.each($scope.forms, function (index, form)
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
    }

    var clearServerValidation = function ()
    {
        $.each($scope.forms, function (index, form)
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

    $scope.save = function ()
    {
        clearServerValidation();

        var valid = true;
        $.each($scope.forms, function (index, form)
        {
            if (form && index != 'submitted')
            {
                if (!form.$valid)
                {
                    valid = false;
                    activateTab(index);
                    return false;
                }
            }
        });

        if (valid)
        {
            $scope.order.ShippingAddress = $scope.shippingAddressTab.Address;

            $scope.order.IdPaymentMethodType = $scope.paymentInfoTab.PaymentMethodType;
            if (!$scope.id)
            {
                if ($scope.order.IdPaymentMethodType == 1)
                {
                    $scope.order.CreditCard = paymentInfoTab.CreditCard;
                } else if ($scope.order.IdPaymentMethodType == 2)
                {
                    $scope.order.Oac = currentCustomer.Oac;
                } else if ($scope.order.IdPaymentMethodType == 3)
                {
                    $scope.order.Check = currentCustomer.Check;
                }
            }
            else
            {
                if ($scope.order.IdPaymentMethodType == 1)
                {
                    $scope.order.CreditCard = paymentInfoTab.CreditCard;
                }
            }
            //billing info - for exist order all data should be sent and backend will save only needed one based on IdPaymentMethodType
        } else
        {
            $scope.forms.mainForm.submitted = true;
            $scope.forms.mainForm2.submitted = true;
            $scope.forms.submitted['profile'] = true;
            $scope.forms.submitted['shipping'] = true;
            $scope.forms.submitted['billing'] = true;
        }
    };

    function deleteCustomerNoteLocal(id)
    {
        var idx = -1;

        angular.forEach($scope.currentCustomer.CustomerNotes, function (item, index)
        {
            if (item.Id == id)
            {
                idx = index;
                return;
            }
        });

        $scope.currentCustomer.CustomerNotes.splice(idx, 1);
    }

    $scope.deleteCustomerNote = function (id)
    {
        customerService.deleteNote(id, $scope.addEditTracker)
            .success(function (result)
            {
                if (result.Success)
                {
                    deleteCustomerNoteLocal(id);
                    toaster.pop('success', "Success!", "Customer Note was succesfully deleted");
                }
            })
            .error(function (result)
            {
                errorHandler(result);
            });
    };

    $scope.addNewCustomerNote = function ()
    {
        clearServerValidation();
        if ($scope.forms.customerNote.$valid)
        {
            customerService.addNote($scope.customerNote, $scope.currentCustomer.Id, $scope.addEditTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        $scope.currentCustomer.CustomerNotes.push(result.Data);
                        $scope.customerNote = $scope.customerNotePrototype;
                        toaster.pop('success', "Success!", "Customer Note was succesfully added");
                    }
                    else
                    {
                        errorHandler(result);
                    }
                })
                .error(function (result)
                {
                    errorHandler(result);
                });
        } else
        {
            $scope.forms.customerNote.submitted = true;
        }
    };

    $scope.gcLostFocus = function (index, code)
    {
        if (index != 0 && !code)
        {
            $scope.order.GCs.splice(index, 1);
        }
        $scope.requestRecalculate();
    };

    $scope.getGCs = function (val)
    {
        $scope.gcsFilter.Code = val;
        return gcService.getGiftCertificates($scope.gcsFilter)
            .then(function (result)
            {
                return result.data.Data.Items.map(function (item)
                {
                    return item.Code;
                });
            });
    };

    $scope.getDiscounts = function (val)
    {
        $scope.discountsFilter.Code = val;
        return discountService.getDiscounts($scope.discountsFilter)
            .then(function (result)
            {
                return result.data.Data.Items.map(function (item)
                {
                    return item.Code;
                });
            });
    };

    $scope.productAdd = function ()
    {
        if ($scope.order.SkuOrdereds.length > 0 && !$scope.order.SkuOrdereds[$scope.order.SkuOrdereds.length - 1].Code)
        {
            return;
        }
        var product = { Code: '', Id: null, QTY: '', ProductName: '', Price: null, Amount: '', IdProductType: null, Messages: [] };
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
        $scope.requestRecalculate();
    };

    $scope.topPurchasedProducts = function ()
    {
        modalUtil.open('app/modules/product/partials/topPurchasedProductsPopup.html', 'topPurchasedProductsController', {
            products: $scope.order.SkuOrdereds, thenCallback: function (data)
            {
                var newProducts = data;
                $.each(newProducts, function (index, newProduct)
                {
                    var add = true;
                    $.each($scope.order.SkuOrdereds, function (index, product)
                    {
                        if (newProduct.Code == product.Code)
                        {
                            add = false;
                            return false;
                        }
                    });

                    if (add)
                    {
                        if ($scope.order.SkuOrdereds.length > 0 && !$scope.order.SkuOrdereds[$scope.order.SkuOrdereds.length - 1].Code)
                        {
                            $scope.order.SkuOrdereds.splice($scope.order.SkuOrdereds.length - 1, 1);
                        }

                        var product = {};
                        product.QTY = 1;
                        product.Code = newProduct.Code;
                        product.IdProductType = newProduct.ProductType;
                        product.ProductName = newProduct.DescriptionName;
                        if ($scope.currentCustomer.CustomerType == 1)
                        {
                            product.Price = newProduct.Price;
                        }
                        else if ($scope.currentCustomer.CustomerType == 2)
                        {
                            product.Price = newProduct.WholesalePrice;
                        }
                        product.Amount = product.Price;

                        $scope.order.SkuOrdereds.push(product);
                    }
                });

                $scope.requestRecalculate();
            }
        });
    };

    $scope.gcAdd = function ()
    {
        $scope.order.GCs.push({ Code: '' });
    };

    $scope.gcDelete = function (index)
    {
        $scope.order.GCs.splice(index, 1);
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

    var skuChangedRequest = null;

    $scope.skuChanged = function (index)
    {
        //resolving issue with additional load after lost focus from the input if time of selecting a new element
        if (skuChangedRequest)
        {
            $timeout.cancel(skuChangedRequest);
        }
        skuChangedRequest = $timeout(function ()
        {
            var product = $scope.order.SkuOrdereds[index];
            if (product.RequestedCode == product.Code)
            {
                return;
            }
            if (product && product.Code && ($scope.skuFilter.ExactCode != product.Code || $scope.skuFilter.ExactDescriptionName != ''))
            {
                $scope.skuFilter.ExactCode = product.Code;
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
                                if ($scope.currentCustomer.CustomerType == 1)
                                {
                                    product.Price = result.Data.Price;
                                }
                                else if ($scope.currentCustomer.CustomerType == 2)
                                {
                                    product.Price = result.Data.WholesalePrice;
                                }
                                product.Amount = product.Price;

                                $scope.requestRecalculate();
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
                skuChangedRequest = null;
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
                            if ($scope.currentCustomer.CustomerType == 1)
                            {
                                product.Price = result.Data.Price;
                            }
                            else if ($scope.currentCustomer.CustomerType == 2)
                            {
                                product.Price = result.Data.WholesalePrice;
                            }
                            product.Amount = product.Price;

                            $scope.requestRecalculate();
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

    initialize();

}]);