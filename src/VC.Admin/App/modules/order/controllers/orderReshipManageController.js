'use strict';

angular.module('app.modules.order.controllers.orderReshipManageController', [])
.controller('orderReshipManageController', ['$q', '$scope', '$rootScope', '$filter', '$injector', '$state', '$stateParams', '$timeout', 'modalUtil', 'orderService', 'customerService',
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
                if (result.Data.EGiftNewOrderEmail)
                {
                    modalUtil.open('app/modules/gc/partials/sendEmail.html', 'sendEmailController', result.Data.EGiftNewOrderEmail);
                }
                $state.go('index.oneCol.orderReshipDetail', { id: result.Data.Id });
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

        $scope.autoShipOrderFrequencies = [];

        $scope.minimumPerishableThreshold = $rootScope.ReferenceData.AppSettings.GlobalPerishableThreshold;
        $scope.options = {};
        $scope.options.ignoneMinimumPerishableThreshold = false;
        $scope.orderSources = $rootScope.ReferenceData.OrderSources;
        $scope.orderSourcesCelebrityHealthAdvocate = $rootScope.ReferenceData.OrderSourcesCelebrityHealthAdvocate;
        $scope.orderPreferredShipMethod = $rootScope.ReferenceData.OrderPreferredShipMethod;
        $scope.creditCardTypes = $rootScope.ReferenceData.CreditCardTypes;

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
            index: 1,
            formNames: ['topForm', 'mainForm', 'mainForm2', 'GCs'],
            name: $scope.id ? 'Edit Reship Order' : 'New Reship Order',
        };
        $scope.shippingAddressTab = {
            index: 2,
            formName: 'shipping',
            ShippingEditModels: {}
        };
        $scope.paymentInfoTab = {
            index: 3,
            formNames: ['card', 'oac', 'check', 'wiretransfer', 'marketing', 'vcwellness', 'nc'],
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
            if (!$scope.order.Id)
            {
                $scope.options.showCopyButtonsOnOrder = true;
            }
            orderEditService.baseProcessLoadingOrder($scope);

            if (!$scope.options.inited)
            {
                $scope.options.inited = true;
                customerEditService.initBase($scope);
                orderEditService.initBase($scope);
                orderEditService.initRecalculate($scope);
                $scope.idCustomer = $scope.order.IdCustomer;
                customerEditService.initOrderEditCustomerParts($scope);
            }

            loadReferencedData();
        }
    };

    var loadOrder = function ()
    {
        orderService.getReshipOrder($scope.id, $scope.idOrderSource, $scope.idCustomer, $scope.addEditTracker)
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

                customerEditService.syncCountry($scope, $scope.currentCustomer.ProfileAddress);

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
                customerEditService.syncDefaultPaymentMethod($scope);

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

    var initOrder = function ()
    {
        orderEditService.initOrderOptions($scope);
        if ($scope.order.IdOrderSource)
        {
            $scope.legend.IdOrderSource = $scope.order.IdOrderSource;
        }
        if ($scope.order.OrderSourceDateCreated)
        {
            $scope.legend.OrderSourceDateCreated = $scope.order.OrderSourceDateCreated;
        }
        if ($scope.order.OrderSourceTotal)
        {
            $scope.legend.OrderSourceTotal = $scope.order.OrderSourceTotal;
        }
        $scope.options.AllReshipProblemSkus = true;
    };

    $scope.toggleAllReshipProblemSkus = function ()
    {
        $.each($scope.order.ReshipProblemSkus, function (index, item)
        {
            item.Used = $scope.options.AllReshipProblemSkus;
        });
    };

    $scope.toggleReshipProblemSku = function (item)
    {
        var allUsed = true;
        $.each($scope.order.ReshipProblemSkus, function (index, item)
        {
            if (!item.Used)
            {
                allUsed = false;
                return false;
            }
        });
        $scope.options.AllReshipProblemSkus = allUsed;
    };

    $scope.save = function ()
    {
        var deferredRecalculate = $q.defer();
        $scope.addEditTracker.addPromise(deferredRecalculate.promise);
        $scope.requestRecalculate(function ()
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
                if (!orderEditService.isProductsValid($scope))
                {
                    deferredRecalculate.reject();
                    return;
                }

                angular.forEach($scope.currentCustomer.Shipping, function (shippingItem, index)
                {
                    shippingItem.IsSelected = index.toString() == $scope.shippingAddressTab.AddressIndex;
                });

                $scope.order.IdPaymentMethodType = $scope.paymentInfoTab.PaymentMethodType;
                var billingErrorMessages = '';
                if ($scope.order.IdPaymentMethodType == 1)//card
                {
                    if ($scope.currentCustomer.CreditCards.length != 0)
                    {
                        angular.forEach($scope.currentCustomer.CreditCards, function (cardItem, index)
                        {
                            cardItem.IsSelected = index.toString() == $scope.paymentInfoTab.CreditCardIndex;
                        });
                    }
                }
                if (!$scope.order.OnHold)
                {
                    if ($scope.order.IdPaymentMethodType == 1)//card
                    {
                        if ($scope.order.CreditCard == null)
                        {
                            billingErrorMessages += "Credit Card is required. ";
                        }
                    }
                    if ($scope.order.IdPaymentMethodType == 2)//oac
                    {
                        if ($scope.order.Oac == null)
                        {
                            billingErrorMessages += "On Approved Credit is required. ";
                        }
                    }
                    if ($scope.order.IdPaymentMethodType == 3)//check
                    {
                        if ($scope.order.Check == null)
                        {
                            billingErrorMessages += "Check is required. ";
                        }
                    }
                    if ($scope.order.IdPaymentMethodType == 6)
                    {
                        if ($scope.order.WireTransfer == null)
                        {
                            billingErrorMessages += "Wire Transfer is required. ";
                        }
                    }
                    if ($scope.order.IdPaymentMethodType == 7)
                    {
                        if ($scope.order.Marketing == null)
                        {
                            billingErrorMessages += "Marketing payment info is required. ";
                        }
                    }
                    if ($scope.order.IdPaymentMethodType == 8)
                    {
                        if ($scope.order.VCWellness == null)
                        {
                            billingErrorMessages += "VC Wellness Employee Program is required. ";
                        }
                    }
                    if ($scope.order.IdPaymentMethodType == 4)
                    {
                        if ($scope.order.NC == null)
                        {
                            billingErrorMessages += "No Charge is required. ";
                        }
                    }
                }

                if (billingErrorMessages)
                {
                    toaster.pop('error', 'Error!', billingErrorMessages, null, 'trustedHtml');
                    $scope.options.activeTabIndex = $scope.paymentInfoTab.index;
                    deferredRecalculate.reject();
                    return;
                }

                var order = orderEditService.orderDataProcessingBeforeSave($scope);

                orderService.updateReshipOrder(order, $scope.addEditTracker).success(function (result)
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

    initialize();
}]);