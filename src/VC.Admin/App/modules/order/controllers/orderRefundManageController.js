'use strict';

angular.module('app.modules.order.controllers.orderRefundManageController', [])
.controller('orderRefundManageController', ['$q', '$scope', '$rootScope', '$filter', '$injector', '$state', '$stateParams', '$timeout', 'modalUtil', 'orderService', 'customerService',
    'productService', 'gcService', 'discountService', 'settingService', 'toaster', 'confirmUtil', 'promiseTracker', 'customerEditService', 'orderEditService', 'gridSorterUtil',
function ($q, $scope, $rootScope, $filter, $injector, $state, $stateParams, $timeout, modalUtil, orderService, customerService, productService, gcService, discountService,
    settingService, toaster, confirmUtil, promiseTracker, customerEditService, orderEditService, gridSorterUtil)
{
    $scope.addEditTracker = promiseTracker("addEdit");

    function successSaveHandler(result)
    {
        if (result.Success)
        {
            toaster.pop('success', "Success!", "Successfully saved.");
            if (!$scope.id)
            {
                $state.go('index.oneCol.orderRefundDetail', { id: result.Data.Id });
            }
            else
            {
                processLoadingOrder(result);
                $scope.refreshOrderHistory();
            }
        }
        else
        {
            $scope.fireServerValidation(result);
        }
    };

    function errorHandler(result)
    {
        toaster.pop('error', "Error!", "Server error occured");
    };

    function initialize()
    {
        $scope.id = $stateParams.id ? $stateParams.id : 0;
        $scope.idCustomer = $stateParams.idcustomer;
        $scope.idOrderSource = $stateParams.idsource;

        $scope.forms = { submitted: [] };

        $scope.options = {};

        $scope.legend = {};

        $scope.mainTab = {
            index: 1,
            formNames: ['topForm', 'mainForm', 'mainForm2', ],
            name: $scope.id ? 'Refund Order' : 'New Refund Order',
        };
        $scope.shippingAddressTab = {
            index: 2,
            formName: 'shipping',
            ShippingEditModels: {}
        };
        $scope.paymentInfoTab = {
            index: 3,
            formNames: ['oac'],
            AddressEditModels: {}
        };
        $scope.options.activeTabIndex = $scope.mainTab.index;
        var tabs = [];
        tabs.push($scope.mainTab);
        tabs.push($scope.shippingAddressTab);
        tabs.push($scope.paymentInfoTab);
        $scope.tabs = tabs;

        loadOrder();
    };

    var processLoadingOrder = function (result)
    {
        if (result.Success)
        {
            $scope.order = result.Data;
            orderEditService.baseProcessLoadingOrder($scope);

            if (!$scope.options.inited)
            {
                $scope.options.inited = true;
                customerEditService.initBase($scope);
                orderEditService.initBase($scope);
                $scope.idCustomer = $scope.order.IdCustomer;
                customerEditService.initOrderEditCustomerParts($scope);
            }

            loadReferencedData();

            $scope.options.RefundGiftCertificatesEnable = $scope.order.RefundGCsUsedOnOrder > 0;
            $scope.options.ManualRefundOverrideEnable = $scope.order.ManualRefundOverride > 0;
        }
    };

    var loadOrder = function ()
    {
        orderService.getRefundOrder($scope.id, $scope.idOrderSource, $scope.idCustomer, $scope.addEditTracker)
            .success(function (result)
            {
                if (result.Success)
                {
                    processLoadingOrder(result);
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
    };

    var loadReferencedData = function ()
    {
        $q.all({
            countriesCall: customerService.getCountries($scope.addEditTracker),
            customerGetCall: customerService.getExistingCustomer($scope.idCustomer, $scope.addEditTracker)
        }).then(function (result)
        {
            if (result.countriesCall.data.Success && result.customerGetCall.data.Success)
            {
                $scope.refreshOrderHistory();
                $scope.allCountries = result.countriesCall.data.Data;
                $scope.countries = result.countriesCall.data.Data;
                $scope.currentCustomer = result.customerGetCall.data.Data;
                $scope.filterCountries();

                if ($scope.currentCustomer.InceptionDate)
                {
                    $scope.currentCustomer.InceptionDate = Date.parseDateTime($scope.currentCustomer.InceptionDate);
                }
                $scope.options.DBStatusCode = $scope.currentCustomer.StatusCode;

                customerEditService.syncCountry($scope, $scope.order.Shipping);
                angular.forEach($scope.currentCustomer.Shipping, function (shippingItem)
                {
                    customerEditService.syncCountry($scope, shippingItem);
                });

                $scope.options.ShippingAddressIndexWatch = $scope.$watch('shippingAddressTab.AddressIndex', function (newValue, oldValue)
                {
                    if (newValue && oldValue != newValue)
                    {
                        $scope.shippingAddressTab.OrderShippingEditModel = undefined;
                        $scope.order.Shipping = angular.copy($scope.currentCustomer.Shipping[parseInt(newValue)]);
                    }
                });

                orderEditService.baseReferencedDataInit($scope);
                orderEditService.baseReferencedDataInitExistOrder($scope);

                initOrder();

                if (!$scope.orderEditDisabled)
                {
                    $scope.requestRecalculate();
                }
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

    $scope.requestRecalculate = function (callback)
    {
        if ($scope.id)
        {
            return;
        }
        if (!$scope.forms.mainForm.$valid || !$scope.forms.mainForm2.$valid)
        {
            $scope.forms.mainForm.submitted = true;
            $scope.forms.mainForm2.submitted = true;
        }
        else
        {
            var orderForCalculating = angular.copy($scope.order);
            orderForCalculating.Customer = angular.copy($scope.currentCustomer);

            $scope.oldOrderForCalculating = orderForCalculating;
            if ($scope.currectCalculateCanceller)
            {
                $scope.currectCalculateCanceller.resolve("canceled");
            }
            $scope.currectCalculateCanceller = $q.defer();
            orderService.calculateRefundOrder(orderForCalculating, $scope.currectCalculateCanceller)
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
                    if (callback) {
                        callback(result);
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
                    if (callback) {
                        callback(result);
                    }
                });
        }
    };

    function successCalculateHandler(data)
    {
        $scope.order.ShippingTotal = data.ShippingTotal;
        $scope.order.ProductsSubtotal = data.ProductsSubtotal;
        $scope.order.DiscountTotal = data.DiscountTotal;
        $scope.order.DiscountedSubtotal = data.DiscountedSubtotal;
        $scope.order.DiscountMessage = data.DiscountMessage;
        $scope.order.TaxTotal = data.TaxTotal;
        $scope.order.AutoTotal = data.AutoTotal;
        $scope.order.Total = data.Total;

        $scope.order.ManualShippingTotal = data.ManualShippingTotal;
        $scope.order.RefundGCsUsedOnOrder = data.RefundGCsUsedOnOrder;
        $scope.order.ManualRefundOverride = data.ManualRefundOverride;

        $.each($scope.order.RefundSkus, function (index, uiSku)
        {
            var found = false;
            $.each(data.RefundSkus, function (index, sku)
            {
                if (uiSku.IdSku == sku.IdSku)
                {
                    uiSku.RefundValue = sku.RefundValue;
                    return false;
                }
            });
        });

        $.each($scope.order.RefundOrderToGiftCertificates, function (index, uiGc)
        {
            var found = false;
            $.each(data.RefundOrderToGiftCertificates, function (index, gc)
            {
                if (uiGc.IdGiftCertificate == gc.IdGiftCertificate)
                {
                    uiGc.Amount = gc.Amount;
                    return false;
                }
            });
        });
    };

    var initOrder = function ()
    {
        orderEditService.initOrderOptions($scope);
        $scope.legend.IdOrderSource = $scope.order.IdOrderSource;
        $scope.legend.OrderSourceRefundIds = $scope.order.OrderSourceRefundIds;
        $scope.legend.OrderSourceDateCreated = $scope.order.OrderSourceDateCreated;
        $scope.legend.OrderSourceTotal = $scope.order.OrderSourceTotal;
        $scope.legend.OrderSourcePaymentMethodType = $scope.order.OrderSourcePaymentMethodType;
    };

    $scope.save = function ()
    {
        var deferredRecalculate = $q.defer();
        $scope.addEditTracker.addPromise(deferredRecalculate.promise);
        $scope.requestRecalculate(function () {
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

            if (valid)
            {
                $scope.order.IdPaymentMethodType = $scope.paymentInfoTab.PaymentMethodType;
                var billingErrorMessages = '';

                if (!$scope.order.OnHold)
                {
                    if ($scope.order.IdPaymentMethodType == 2)//oac
                    {
                        if ($scope.order.Oac == null)
                        {
                            billingErrorMessages += "On Approved Credit is required. ";
                        }
                    }
                }

                if (billingErrorMessages)
                {
                    $scope.paymentInfoTab.active = true;
                    toaster.pop('error', 'Error!', billingErrorMessages, null, 'trustedHtml');
                    deferredRecalculate.reject();
                    return;
                }

                var order = orderEditService.orderDataProcessingBeforeSave($scope);

                orderService.addRefundOrder(order, $scope.addEditTracker).success(function (result)
                {
                    deferredRecalculate.reject();
                    successSaveHandler(result);
                }).
                error(function (result)
                {
                    deferredRecalculate.reject();
                    errorHandler(result);
                });
                //billing info - for exist order all data should be sent and backend will save only needed one based on IdPaymentMethodType
            } else
            {
                if ($scope.forms.topForm != null)
                {
                    $scope.forms.topForm.submitted = true;
                }
                $scope.forms.mainForm.submitted = true;
                $scope.forms.mainForm2.submitted = true;
                $scope.forms.submitted['shipping'] = true;
                $scope.forms.submitted['card'] = true;
                $scope.forms.submitted['oac'] = true;
                $scope.forms.submitted['check'] = true;
                $scope.forms.submitted['wiretransfer'] = true;
                $scope.forms.submitted['marketing'] = true;
                $scope.forms.submitted['vcwellness'] = true;
                $scope.forms.submitted['nc'] = true;
                toaster.pop('error', "Error!", "Validation errors, please correct field values.", null, 'trustedHtml');
                deferredRecalculate.reject();
            }                               
        }, function ()
        {
            deferredRecalculate.reject();
            toaster.pop('error', "Error!", "Validation errors, please correct field values.", null, 'trustedHtml');
        });
    };

    $scope.cancelRefund = function ()
    {
        confirmUtil.confirm(function ()
        {
            orderService.cancelRefundOrder($scope.order.Id, $scope.addEditTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        toaster.pop('success', "Success!", "Successfully canceled.");
                        $state.go('index.oneCol.manageOrders');
                    } else
                    {
                        errorHandler(result);
                    }
                })
                .error(function (result)
                {
                    errorHandler(result);
                });
        }, 'Are you sure you want to cancel this refund?');
    };

    $scope.toggleAllActiveRefundSkus = function ()
    {
        $.each($scope.order.RefundSkus, function (index, item)
        {
            if (!item.Disabled)
            {
                item.Active = $scope.options.allActiveRefundSkus;
            }
        });
        $scope.requestRecalculate();
    };

    $scope.toggleRefundSku = function (item)
    {
        var allActive = true;
        $.each($scope.order.RefundSkus, function (index, item)
        {
            if (!item.Disabled && !item.Active)
            {
                allActive = false;
                return false;
            }
        });
        $scope.options.allActiveRefundSkus = allActive;
        $scope.requestRecalculate();
    };

    $scope.toggleRefundGiftCertificates = function ()
    {
        $scope.order.RefundGCsUsedOnOrder = 0;
        $scope.requestRecalculate();
    };

    $scope.toggleManualRefundOverride = function ()
    {
        $scope.order.ManualRefundOverride = 0;
        $scope.requestRecalculate();
    };

    initialize();
}]);