'use strict';

angular.module('app.modules.order.controllers.orderManageController', [])
.controller('orderManageController', ['$q', '$scope', '$rootScope', '$filter', '$injector', '$state', '$stateParams', '$timeout', 'modalUtil', 'orderService', 'customerService',
    'productService', 'gcService', 'discountService', 'settingService', 'toaster', 'confirmUtil', 'promiseTracker', 'customerEditService', 'orderEditService', 'gridSorterUtil',
function ($q, $scope, $rootScope, $filter, $injector, $state, $stateParams, $timeout, modalUtil, orderService, customerService, productService, gcService, discountService,
    settingService, toaster, confirmUtil, promiseTracker, customerEditService, orderEditService, gridSorterUtil)
{
	$scope.addEditTracker = promiseTracker("addEdit");
	$scope.resetTracker = promiseTracker("reset");
	$scope.resendTracker = promiseTracker("resend");

    function successSaveHandler(result)
	{
        if (result.Success)
        {
            toaster.pop('success', "Success!", "Successfully saved.");
            if (!$scope.id)
            {
                $rootScope.BrontoSubscribedStatus = {
                    Id: result.Data.Id,
                    Value: $scope.options.BrontoSubscribedStatus,
                };
                $state.go('index.oneCol.orderDetail', { id: result.Data.Id });
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
        $scope.idCustomer = $stateParams.idcustomer ? $stateParams.idcustomer : 0;
        $scope.idOrderSource = $stateParams.idsource ? $stateParams.idsource : 0;

        $scope.forms = { submitted: [] };

        $scope.autoShipOrderFrequencies = [];

        $scope.minimumPerishableThreshold = $rootScope.ReferenceData.AppSettings.GlobalPerishableThreshold;
        $scope.options = {};
        $scope.options.ignoneMinimumPerishableThreshold = false;
        $scope.orderSources = $rootScope.ReferenceData.OrderSources;
        $scope.orderSourcesCelebrityHealthAdvocate = $rootScope.ReferenceData.OrderSourcesCelebrityHealthAdvocate;
        $scope.orderPreferredShipMethod = $rootScope.ReferenceData.OrderPreferredShipMethod;
        $scope.creditCardTypes = $rootScope.ReferenceData.CreditCardTypes;

        $scope.discountsFilter = {
            Code: '',
            StatusCode: 2,
            Paging: { PageIndex: 1, PageItemCount: 20 },
        };

        $scope.gcsFilter = {
            Code: '',
            StatusCode: 2,
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
            index: 1,
            formNames: ['topForm', 'mainForm', 'mainForm2', 'GCs'],
            name: $scope.id ? 'Edit Order' : 'New Order',
        };
        $scope.accountProfileTab = {
            index: 2,
            formName: 'profile',
        };
        $scope.shippingAddressTab = {
            index: 3,
            formName: 'shipping',
            ShippingEditModels: {}
        };
        $scope.paymentInfoTab = {
            index: 4,
            formNames: ['card', 'oac', 'check', 'wiretransfer', 'marketing', 'vcwellness', 'nc'],
            AddressEditModels: {}
        };
        $scope.customerNotesTab = {
            index: 5,
            formName: 'customerNote',
        };
        $scope.customerFilesTab = {
            index: 6,
            formName: 'customerFile'
        };
        $scope.options.activeTabIndex = $scope.mainTab.index;
        var tabs = [];
        tabs.push($scope.mainTab);
        tabs.push($scope.accountProfileTab);
        tabs.push($scope.shippingAddressTab);
        tabs.push($scope.paymentInfoTab);
        tabs.push($scope.customerNotesTab);
        tabs.push($scope.customerFilesTab);
        $scope.tabs = tabs;

        $scope.giftOrderChanged = function() {
            if (!$scope.order.GiftOrder) {
                $scope.order.GiftMessage = "";
            }
        }

        loadOrder();
    };

    var processLoadingOrder = function (result) {
        if (result.Success) {
            $scope.order = result.Data;
            orderEditService.baseProcessLoadingOrder($scope);

            if(!$scope.options.inited)
            {
                customerEditService.initBase($scope);
                orderEditService.initBase($scope);
                orderEditService.initAutoShipLogic($scope);
                orderEditService.initRecalculate($scope);
                if ($scope.id) {
                    $scope.idCustomer = $scope.order.IdCustomer;
                    customerEditService.initOrderEditCustomerParts($scope);
                    $scope.initAutoShipOptions();
                }
                else {
                    if ($scope.idCustomer == 0) {
                        $state.go('index.oneCol.manageCustomers', {});
                        return;
                    }
                    $scope.order.UpdateShippingAddressForCustomer = true;
                    customerEditService.initCustomerEdit($scope);
                }
            }

            if ($scope.idOrderSource) {
                loadOrderSource();
            }
            else {
                loadReferencedData();
            }
        }
    };

    var loadOrder = function ()
    {
        orderService.getOrder($scope.id, $scope.idCustomer!=0 ? $scope.idCustomer : null, false, $scope.addEditTracker)
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

    var loadOrderSource = function ()
    {
        orderService.getOrder($scope.idOrderSource, null, true, $scope.addEditTracker)
            .success(function (result)
            {
                if (result.Success)
                {
                    if (result.Data.SkuOrdereds && result.Data.SkuOrdereds.length > 0)
                    {
                        $scope.order.SkuOrdereds = result.Data.SkuOrdereds;
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
            customerGetCall: customerService.getExistingCustomer($scope.idCustomer, $scope.addEditTracker)
        }).then(function (result)
        {
            if (result.countriesCall.data.Success && result.customerGetCall.data.Success)
            {
                $scope.refreshOrderHistory();
                $scope.countries = result.countriesCall.data.Data;

                $scope.currentCustomer = result.customerGetCall.data.Data;
                if ($scope.currentCustomer.InceptionDate)
                {
                    $scope.currentCustomer.InceptionDate = Date.parseDateTime($scope.currentCustomer.InceptionDate);
                }
                $scope.options.DBStatusCode = $scope.currentCustomer.StatusCode;
                if (!$scope.currentCustomer.Email)
                {
                    $scope.options.OverrideEmail = true;
                }
                if ($scope.id==0 && $scope.currentCustomer && $scope.currentCustomer.StatusCode == 4)
                {
                    $state.go('index.oneCol.customerDetail', { id: $scope.currentCustomer.Id });
                    return;
                }
                if (!$scope.options.inited)
                {
                    $scope.options.inited = true;
                    loadBrontoSubscribedStatus();
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
                    //$scope.shippingAddressTab.AddressIndex = "0";
                    //$scope.$watch('shippingAddressTab.AddressIndex', function (newValue, oldValue)
                    //{
                    //    if ($scope.options.EnableShippingAddressIndexUpdate && newValue && oldValue != newValue)
                    //    {
                    //        $scope.shippingAddressTab.OrderShippingEditModel = undefined;
                    //        $scope.order.Shipping = angular.copy($scope.currentCustomer.Shipping[parseInt(newValue)]);
                    //    }
                    //});
                }
                else
                {
                    angular.forEach($scope.currentCustomer.Shipping, function (shippingItem, index)
                    {
                        customerEditService.syncCountry($scope, shippingItem);
                        if (shippingItem.Default)
                        {
                            $scope.shippingAddressTab.AddressIndex = index.toString();
                        }
                    });
                }

                orderEditService.baseReferencedDataInit($scope);

                if ($scope.id)
                {
                    orderEditService.baseReferencedDataInitExistOrder($scope);
                }
                else
                {
                    $scope.paymentInfoTab.PaymentMethodType = $scope.currentCustomer.DefaultPaymentMethod;
                }

                customerEditService.syncDefaultPaymentMethod($scope);
                customerEditService.showHighPriNotes($scope);

                initOrder();
                initCustomerFiles();
                initCustomerNotes();

                initOrdersList();
                initAutoShipsList();

                if (($scope.id || $scope.idOrderSource) && !$scope.orderEditDisabled)
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

    var loadBrontoSubscribedStatus = function ()
    {
        if ($rootScope.BrontoSubscribedStatus && $rootScope.BrontoSubscribedStatus.Id == $scope.order.Id)
        {
            $scope.options.BrontoSubscribedStatus = $rootScope.BrontoSubscribedStatus.Value;
            $scope.options.BrontoSubscribedStatusLoaded = true;
        }
        else
        {
            if ($scope.currentCustomer.Email)
            {
                orderService.getIsBrontoSubscribed($scope.currentCustomer.Email)
                    .success(function (result)
                    {
                        if (result.Success)
                        {
                            $scope.options.BrontoSubscribedStatus = result.Data;
                            $scope.options.BrontoSubscribedStatusLoaded = true;
                        }
                    })
                    .error(function (result)
                    {
                    });
            }
        }
        $rootScope.BrontoSubscribedStatus = null;
    };
    
    var initOrder = function ()
    {
        orderEditService.initOrderOptions($scope);
    };

    function initCustomerFiles()
    {
        var data = {};
        data.files = $scope.currentCustomer.Files;
        data.publicId = $scope.currentCustomer.PublicId;
        data.addEditTracker = $scope.addEditTracker;
        $scope.$broadcast('customerFiles#in#init', data);
    };

    function initCustomerNotes()
    {
        var data = {};
        data.customerNotes = $scope.currentCustomer.CustomerNotes;
        data.addEditTracker = $scope.addEditTracker;
        $scope.$broadcast('customerNotes#in#init', data);
    };

    function initOrdersList()
    {
        var data = {};
        data.idCustomer = $scope.idCustomer;
        $scope.$broadcast('customerOrders#in#init', data);
    };

    function initAutoShipsList() {
    	var data = {};
    	data.idCustomer = $scope.idCustomer;
    	$scope.$broadcast('customerAutoShips#in#init', data);
    };

    $scope.save = function () {
        var deferredRecalculate = $q.defer();
        $scope.addEditTracker.addPromise(deferredRecalculate.promise);
        $scope.requestRecalculate(function () {
            $scope.clearServerValidation();
            var valid = true;
            $.each($scope.forms, function (index, form) {
                if (form && index != 'submitted') {
                    if (!form.$valid) {
                        valid = false;
                        $scope.activateTab(index);
                        return false;
                    }
                }
            });

            if (valid && $scope.skusClientValid()) {
                if (!orderEditService.isProductsValid($scope)) {
                    deferredRecalculate.reject();
                    return;
                }

                angular.forEach($scope.currentCustomer.Shipping, function (shippingItem, index) {
                    shippingItem.IsSelected = index.toString() == $scope.shippingAddressTab.AddressIndex;
                });

                $scope.order.IdPaymentMethodType = $scope.paymentInfoTab.PaymentMethodType;
                var billingErrorMessages = '';
                if ($scope.order.IdPaymentMethodType == 1)//card
                {
                    if ($scope.currentCustomer.CreditCards.length != 0) {
                        angular.forEach($scope.currentCustomer.CreditCards, function (cardItem, index) {
                            cardItem.IsSelected = index.toString() == $scope.paymentInfoTab.CreditCardIndex;
                        });
                    }
                }
                if (!$scope.order.OnHold) {
                    if (!$scope.id) {
                        if ($scope.order.IdPaymentMethodType == 1)//card
                        {
                            if ($scope.currentCustomer.CreditCards.length == 0) {
                                billingErrorMessages += "Credit Card is required. ";
                            }
                            else {
                                angular.forEach($scope.currentCustomer.CreditCards, function (cardItem, index) {
                                    cardItem.IsSelected = index.toString() == $scope.paymentInfoTab.CreditCardIndex;
                                });
                            }
                        }
                        if ($scope.order.IdPaymentMethodType == 2)//oac
                        {
                            if ($scope.currentCustomer.Oac == null) {
                                billingErrorMessages += "On Approved Credit is required. ";
                            }
                        }
                        if ($scope.order.IdPaymentMethodType == 3)//check
                        {
                            if ($scope.currentCustomer.Check == null) {
                                billingErrorMessages += "Check is required. ";
                            }
                        }
                        if ($scope.order.IdPaymentMethodType == 6) {
                            if ($scope.currentCustomer.WireTransfer == null) {
                                billingErrorMessages += "Wire Transfer is required. ";
                            }
                        }
                        if ($scope.order.IdPaymentMethodType == 7) {
                            if ($scope.currentCustomer.Marketing == null) {
                                billingErrorMessages += "Marketing payment info is required. ";
                            }
                        }
                        if ($scope.order.IdPaymentMethodType == 8) {
                            if ($scope.currentCustomer.VCWellness == null) {
                                billingErrorMessages += "VC Wellness Employee Program is required. ";
                            }
                        }
                        if ($scope.order.IdPaymentMethodType == 4)
                        {
                            if ($scope.currentCustomer.NC == null) {
                                billingErrorMessages += "No Charge is required. ";
                            }
                        }
                    }
                    else {
                        if ($scope.order.IdPaymentMethodType == 1)//card
                        {
                            if ($scope.order.CreditCard == null) {
                                billingErrorMessages += "Credit Card is required. ";
                            }
                        }
                        if ($scope.order.IdPaymentMethodType == 2)//oac
                        {
                            if ($scope.order.Oac == null) {
                                billingErrorMessages += "On Approved Credit is required. ";
                            }
                        }
                        if ($scope.order.IdPaymentMethodType == 3)//check
                        {
                            if ($scope.order.Check == null) {
                                billingErrorMessages += "Check is required. ";
                            }
                        }
                        if ($scope.order.IdPaymentMethodType == 6) {
                            if ($scope.order.WireTransfer == null) {
                                billingErrorMessages += "Wire Transfer is required. ";
                            }
                        }
                        if ($scope.order.IdPaymentMethodType == 7) {
                            if ($scope.order.Marketing == null) {
                                billingErrorMessages += "Marketing payment info is required. ";
                            }
                        }
                        if ($scope.order.IdPaymentMethodType == 8) {
                            if ($scope.order.VCWellness == null) {
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
                }

                if (billingErrorMessages) {
                    $scope.paymentInfoTab.active = true;
                    toaster.pop('error', 'Error!', billingErrorMessages, null, 'trustedHtml');
                    deferredRecalculate.reject();
                    return;
                }

                var order = orderEditService.orderDataProcessingBeforeSave($scope);

                orderService.updateOrder(order, $scope.addEditTracker).success(function (result) {
                    deferredRecalculate.resolve();
                    successSaveHandler(result);
                }).
                error(function (result) {
                    deferredRecalculate.resolve();
                    errorHandler(result);
                });
                //billing info - for exist order all data should be sent and backend will save only needed one based on IdPaymentMethodType
            } else {
                if ($scope.forms.topForm != null) {
                    $scope.forms.topForm.submitted = true;
                }
                $scope.forms.mainForm.submitted = true;
                $scope.forms.mainForm2.submitted = true;
                $scope.forms.submitted['profile'] = true;
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

    $scope.resend = function () {
    	customerService.resendActivation($scope.currentCustomer.PublicUserId, $scope.resendTracker)
			.success(function (result) {
				if (result.Success) {
					toaster.pop('success', "Success!", "Successfully sent");

				} else {
					var messages = "";
					if (result.Messages) {
						$.each(result.Messages, function (index, value) {
							messages += value.Message + "<br />";
						});
					}
					toaster.pop('error', "Error!", messages, null, 'trustedHtml');
				}
			}).error(function () {
				toaster.pop('error', "Error!", "Server error occured");
			});
    };

    $scope.resetPassword = function () {
    	customerService.resetPassword($scope.currentCustomer.PublicUserId, $scope.resetTracker)
			.success(function (result) {
				if (result.Success) {
					toaster.pop('success', "Success!", "Successfully reset");
				} else {
					var messages = "";
					if (result.Messages) {
						$.each(result.Messages, function (index, value) {
							messages += value.Message + "<br />";
						});
					}
					toaster.pop('error', "Error!", messages, null, 'trustedHtml');
				}
			}).error(function () {
				toaster.pop('error', "Error!", "Server error occured");
			});
    };

    initialize();
}]);