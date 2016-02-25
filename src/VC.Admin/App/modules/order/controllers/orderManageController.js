'use strict';

angular.module('app.modules.order.controllers.orderManageController', [])
.controller('orderManageController', ['$q', '$scope', '$rootScope', '$filter', '$injector', '$state', '$stateParams', '$timeout', 'modalUtil', 'orderService', 'customerService',
    'productService', 'gcService', 'discountService', 'settingService', 'toaster', 'confirmUtil', 'promiseTracker', 'customerEditService', 'gridSorterUtil',
function ($q, $scope, $rootScope, $filter, $injector, $state, $stateParams, $timeout, modalUtil, orderService, customerService, productService, gcService, discountService,
    settingService, toaster, confirmUtil, promiseTracker, customerEditService, gridSorterUtil)
{
	$scope.addEditTracker = promiseTracker("addEdit");
	$scope.resetTracker = promiseTracker("reset");
	$scope.resendTracker = promiseTracker("resend");

    function successSaveHandler(result)
	{
        processLoadingOrder(result);
        if (result.Success)
        {
            toaster.pop('success', "Success!", "Successfully saved.");
            if (!$scope.id)
            {
                $state.go('index.oneCol.orderDetail', { id: result.Data.Id });
            }
            else
            {
                refreshOrderHistory();
            }
        }
        else
        {
            var messages = "";
            if (result.Messages)
            {
                if ($scope.forms.topForm != null)
                {
                    $scope.forms.topForm.submitted = true;
                }
                $scope.forms.mainForm.submitted = true;
                $scope.forms.mainForm2.submitted = true;
                $scope.forms.GCs.skussubmitted = true;
                $scope.forms.submitted['profile'] = true;
                $scope.forms.submitted['shipping'] = true;
                $scope.forms.submitted['card'] = true;
                $scope.forms.submitted['oac'] = true;
                $scope.forms.submitted['check'] = true;
                $scope.forms.submitted['wiretransfer'] = true;
                $scope.forms.submitted['marketing'] = true;
                $scope.forms.submitted['vcwellness'] = true;
                $scope.serverMessages = new ServerMessages(result.Messages);
                $scope.calculateServerMessages = new ServerMessages(result.Messages);
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
                                        $scope.shippingAddressTab.AddressIndex = collectionIndex;
                                        form = $scope.forms[formName];
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
                                        $scope.paymentInfoTab.CreditCardIndex = collectionIndex;
                                        form = $scope.forms[formName];
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
                            form = $scope.forms[formName];
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
                            $.each($scope.forms, function (index, form)
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
            formNames: ['topForm', 'mainForm', 'mainForm2', 'GCs'],
            name: $scope.id ? 'Edit Order' : 'New Order',
        };
        $scope.accountProfileTab = {
            active: false,
            formName: 'profile',
        };
        $scope.shippingAddressTab = {
            active: false,
            formName: 'shipping',
            ShippingEditModels: {}
        };
        $scope.paymentInfoTab = {
            active: false,
            formNames: ['card', 'oac', 'check', 'wiretransfer', 'marketing', 'vcwellness'],
            AddressEditModels: {}
        };
        $scope.customerNotesTab = {
            active: false,
            formName: 'customerNote',
        };
        $scope.customerFilesTab = {
            active: false,
            formName: 'customerFile'
        };
        var tabs = [];
        tabs.push($scope.mainTab);
        tabs.push($scope.accountProfileTab);
        tabs.push($scope.shippingAddressTab);
        tabs.push($scope.paymentInfoTab);
        tabs.push($scope.customerNotesTab);
        tabs.push($scope.customerFilesTab);
        $scope.tabs = tabs;

        loadOrder();
    };

    var processLoadingOrder = function (result) {
        if (result.Success) {
            $scope.order = result.Data;

            if ($scope.order.ShipDelayDate) {
                $scope.order.ShipDelayDate = Date.parseDateTime($scope.order.ShipDelayDate);
            }
            if ($scope.order.ShipDelayDateP) {
                $scope.order.ShipDelayDateP = Date.parseDateTime($scope.order.ShipDelayDateP);
            }
            if ($scope.order.ShipDelayDateNP) {
                $scope.order.ShipDelayDateNP = Date.parseDateTime($scope.order.ShipDelayDateNP);
            }
            if ($scope.order.IgnoneMinimumPerishableThreshold) {
                $scope.options.ignoneMinimumPerishableThreshold = $scope.order.IgnoneMinimumPerishableThreshold;
            }

            customerEditService.initBase($scope);
            if ($scope.id) {
                $scope.idCustomer = $scope.order.IdCustomer;
                customerEditService.initOrderEditCustomerParts($scope);
            }
            else {
                if ($scope.idCustomer == 0) {
                    $state.go('index.oneCol.manageCustomers', {});
                    return;
                }
                $scope.order.UpdateShippingAddressForCustomer = true;
                customerEditService.initCustomerEdit($scope);
            }

            if ($scope.idOrderSource) {
                loadOrderSource();
            }
            else {
                loadReferencedData();
            }
        } else {
            errorHandler(result);
        }
    };

    var loadOrder = function ()
    {
        orderService.getOrder($scope.id, $scope.idCustomer!=0 ? $scope.idCustomer : null, false, $scope.addEditTracker)
            .success(function (result)
            {
                processLoadingOrder(result);
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

    var refreshOrderHistory = function ()
    {
        if ($scope.id)
        {
            var data = {};
            data.service = orderService;
            data.tracker = $scope.addEditTracker;
            data.idObject = $scope.id;
            data.idObjectType = 2//order
            $scope.$broadcast('objectHistorySection#in#refresh', data);
        }
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
                refreshOrderHistory();
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
                if ($scope.currentCustomer.SourceDetails)
                {
                    $scope.currentCustomer.SourceValue = $scope.currentCustomer.SourceDetails;
                } else if ($scope.currentCustomer.Source)
                {
                    var sourceName = $scope.currentCustomer.Source;
                    $.each($scope.orderSources, function (index, orderSource)
                    {
                        if (orderSource.Key == $scope.currentCustomer.Source)
                        {
                            sourceName = orderSource.Text;
                            return false;
                        }
                    });
                    $scope.currentCustomer.SourceValue = sourceName;
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
                    $scope.$watch('shippingAddressTab.AddressIndex', function (newValue, oldValue)
                    {
                        if (newValue && oldValue != newValue)
                        {
                            $scope.shippingAddressTab.OrderShippingEditModel = undefined;
                            $scope.order.Shipping = angular.copy($scope.currentCustomer.Shipping[parseInt(newValue)]);
                        }
                    });
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

                angular.forEach($scope.currentCustomer.CreditCards, function (creditCard, index)
                {
                    creditCard.formName = "card";
                    customerEditService.syncCountry($scope, creditCard.Address);
                    $scope.paymentInfoTab.CreditCardIndex = "0";
                    if (creditCard.Default)
                    {
                        $scope.paymentInfoTab.CreditCardIndex = index.toString();
                    }
                });

                if ($scope.currentCustomer.Oac)
                {
                    $scope.currentCustomer.Oac.formName = "oac";
                    customerEditService.syncCountry($scope, $scope.currentCustomer.Oac.Address);
                }
                if ($scope.currentCustomer.Check)
                {
                    $scope.currentCustomer.Check.formName = "check";
                    customerEditService.syncCountry($scope, $scope.currentCustomer.Check.Address);
                }
                if ($scope.currentCustomer.WireTransfer)
                {
                    $scope.currentCustomer.WireTransfer.formName = "wiretransfer";
                    customerEditService.syncCountry($scope, $scope.currentCustomer.WireTransfer.Address);
                }
                if ($scope.currentCustomer.Marketing)
                {
                    $scope.currentCustomer.Marketing.formName = "marketing";
                    customerEditService.syncCountry($scope, $scope.currentCustomer.Marketing.Address);
                }
                if ($scope.currentCustomer.VCWellness)
                {
                    $scope.currentCustomer.VCWellness.formName = "vcwellness";
                    customerEditService.syncCountry($scope, $scope.currentCustomer.VCWellness.Address);
                }

                if ($scope.order.CreditCard)
                {
                    $scope.order.CreditCard.formName = "card";
                    customerEditService.syncCountry($scope, $scope.order.CreditCard.Address);
                }
                if ($scope.order.Oac)
                {
                    $scope.order.Oac.formName = "oac";
                    customerEditService.syncCountry($scope, $scope.order.Oac.Address);
                }
                if ($scope.order.Check)
                {
                    $scope.order.Check.formName = "check";
                    customerEditService.syncCountry($scope, $scope.order.Check.Address);
                }
                if ($scope.order.WireTransfer)
                {
                    $scope.order.WireTransfer.formName = "wiretransfer";
                    customerEditService.syncCountry($scope, $scope.order.WireTransfer.Address);
                }
                if ($scope.order.Marketing)
                {
                    $scope.order.Marketing.formName = "marketing";
                    customerEditService.syncCountry($scope, $scope.order.Marketing.Address);
                }
                if ($scope.order.VCWellness)
                {
                    $scope.order.VCWellness.formName = "vcwellness";
                    customerEditService.syncCountry($scope, $scope.order.VCWellness.Address);
                }

                if ($scope.id)
                {
                    $scope.paymentInfoTab.PaymentMethodType = $scope.order.IdPaymentMethodType;

                    $scope.$watch('paymentInfoTab.CreditCardIndex', function (newValue, oldValue)
                    {
                        if (newValue && oldValue != newValue)
                        {
                            //$scope.order.CreditCard = angular.copy($scope.currentCustomer.CreditCards[parseInt(newValue)]);
                            $scope.order.CreditCard = $scope.currentCustomer.CreditCards[parseInt(newValue)];
                        }
                    });

                    if (!$scope.order.CreditCard)
                    {
                        if ($scope.currentCustomer.CreditCards && $scope.currentCustomer.CreditCards[0])
                        {
                            $scope.order.CreditCard = $scope.currentCustomer.CreditCards[0];
                        }
                    }
                    if (!$scope.order.Oac)
                    {
                        $scope.order.Oac = $scope.currentCustomer.Oac;
                    }
                    if (!$scope.order.Check)
                    {
                        $scope.order.Check = $scope.currentCustomer.Check;
                    }
                    if (!$scope.order.WireTransfer)
                    {
                        $scope.order.WireTransfer = $scope.currentCustomer.WireTransfer;
                    }
                    if (!$scope.order.Marketing)
                    {
                        $scope.order.Marketing = $scope.currentCustomer.Marketing;
                    }
                    if (!$scope.order.VCWellness)
                    {
                        $scope.order.VCWellness = $scope.currentCustomer.VCWellness;
                    }

                    if ($scope.paymentInfoTab.PaymentMethodType == 1)
                    {
                        $scope.paymentInfoTab.CreditCard = $scope.order.CreditCard;
                    }
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

    $scope.goToCustomer = function ()
    {
        $state.go('index.oneCol.customerDetail', { id: $scope.idCustomer });
    };

    $scope.buildOrderShippingAddressForPartial = function (disableValidation)
    {
        if ($scope.order === undefined || $scope.order.Shipping === undefined)
            return undefined;
        if ($scope.shippingAddressTab.OrderShippingEditModel === undefined ||
            $scope.shippingAddressTab.OrderShippingEditModel.Address != $scope.order.Shipping)
        {
            $scope.shippingAddressTab.OrderShippingEditModel = { Address: $scope.order.Shipping, formName: 'shipping', recalculate: true };
        }
        $scope.shippingAddressTab.OrderShippingEditModel.disableValidation = disableValidation;
        return $scope.shippingAddressTab.OrderShippingEditModel;
    }

    $scope.buildOrderPaymentPartial = function (model, disableValidation)
    {
        model.disableValidation = disableValidation;
        return model;
    }

    var onHoldWatch = null;

    var initOrder = function ()
    {
        $scope.oldOrderStatus = $scope.order.CombinedEditOrderStatus;
        $scope.order.OnHold = $scope.order.CombinedEditOrderStatus == 7;//on hold status
        $scope.order.AutoShip = $scope.order.AutoShipFrequency ? true : false;
        if (onHoldWatch)
            onHoldWatch();
        onHoldWatch = $scope.$watch('order.OnHold', function (newValue, oldValue)
        {
            if ($scope.order.CombinedEditOrderStatus != 3 && $scope.order.CombinedEditOrderStatus != 4 &&
                $scope.order.CombinedEditOrderStatus != 5)
            {
                if (newValue !== undefined && newValue !== null)
                {
                    if (newValue)
                    {
                        $scope.order.CombinedEditOrderStatus = 7;
                    }
                    else
                    {
                        if ($scope.order.CombinedEditOrderStatus != $scope.oldOrderStatus)
                        {
                            $scope.order.CombinedEditOrderStatus = $scope.oldOrderStatus;
                        }
                        else
                        {

                            $scope.order.CombinedEditOrderStatus = 2; //processed
                        }
                    }
                }
            }
        });
        $scope.orderEditDisabled = $scope.order.CombinedEditOrderStatus == 3 || $scope.order.CombinedEditOrderStatus == 4
            || $scope.order.CombinedEditOrderStatus == 5;

        $scope.legend.CustomerName = $scope.currentCustomer.ProfileAddress.FirstName + " " + $scope.currentCustomer.ProfileAddress.LastName;
        if ($scope.id)
        {
            $scope.legend.OrderId = $scope.order.Id;
            $scope.legend.OrderDate = $scope.order.DateCreated;
            if ($scope.order.OrderStatus)
            {
                $scope.legend.OrderStatus = $rootScope.getReferenceItem($rootScope.ReferenceData.OrderStatuses, $scope.order.OrderStatus).Text;
            }
            else
            {
                var legendOrderStatus = '';
                if($scope.order.POrderStatus)
                {
                    legendOrderStatus += 'P - {0}'.format($rootScope.getReferenceItem($rootScope.ReferenceData.OrderStatuses, $scope.order.POrderStatus).Text);
                }
                if ($scope.order.NPOrderStatus)
                {
                    if (legendOrderStatus.length > 0)
                    {
                        legendOrderStatus += ', ';
                    }
                    legendOrderStatus += 'NP - {0}'.format($rootScope.getReferenceItem($rootScope.ReferenceData.OrderStatuses, $scope.order.NPOrderStatus).Text);
                }
                $scope.legend.OrderStatus = legendOrderStatus;
            }
        }
        else
        {
            $scope.legend.CustomerId = $scope.idCustomer;
        }
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

    var oldOrderForCalculating = null;

    var skusClientValid = function ()
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
                    if (current.Code == $scope.order.SkuOrdereds[j].Code)
                    {
                        $scope.order.SkuOrdereds[j].ClientMessages.push("Duplicate SKU");
                        isValid = false;
                    }
                }
            }
        }
        return isValid;
    };

    $scope.requestRecalculate = function ()
    {
        //additional client validation
        if (!skusClientValid())
        {
            return;
        }

        angular.forEach($scope.currentCustomer.Shipping, function (shippingItem, index)
        {
            shippingItem.IsSelected = index.toString() == $scope.shippingAddressTab.AddressIndex;
        });
        var orderForCalculating = angular.copy($scope.order);
        orderForCalculating.Customer = angular.copy($scope.currentCustomer);
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
        $scope.order.TotalShipping = data.TotalShipping;
        $scope.order.ProductsSubtotal = data.ProductsSubtotal;
        $scope.order.DiscountTotal = data.DiscountTotal;
        $scope.order.DiscountedSubtotal = data.DiscountedSubtotal;
        $scope.order.DiscountMessage = data.DiscountMessage;
        $scope.order.TaxTotal = data.TaxTotal;
        $scope.order.GiftCertificatesSubtotal = data.GiftCertificatesSubtotal;
        $scope.order.Total = data.Total;

        $scope.order.ShouldSplit = data.ShouldSplit;

        $scope.shippingUpgradePOptions = data.ShippingUpgradePOptions;
        $scope.shippingUpgradeNPOptions = data.ShippingUpgradeNPOptions;

        $scope.order.ShippingOverride = data.ShippingOverride;
        $scope.order.SurchargeOverride = data.SurchargeOverride;

        $scope.productsPerishableThresholdIssue = data.ProductsPerishableThresholdIssue;

        var toDeleteIdxs = [];
        $.each($scope.order.SkuOrdereds, function (index, uiSku)
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
            $scope.order.SkuOrdereds.splice(item, 1);
        });

        $scope.order.PromoSkus = data.PromoSkus;

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
            $scope.forms.mainForm2.submitted = true;
            $scope.forms.GCs.skussubmitted = true;
            $scope.calculateErrors = data.Messages;
            $scope.calculateServerMessages = new ServerMessages(data.Messages);
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

        //show/hide autoship option
        $scope.autoShipOrderOptionShow = ($scope.order.SkuOrdereds.length == 1 || ($scope.order.SkuOrdereds.length == 2 && !$scope.order.SkuOrdereds[1].Code))
            && $scope.order.PromoSkus.length == 0 && $scope.order.SkuOrdereds[0].AutoShipProduct;
        if ($scope.autoShipOrderOptionShow)
        {
            var items = [];
            if($scope.order.SkuOrdereds[0].AutoShipFrequency1)
            {
                items.push({ Key: 1, Text: '1 Month' });
            }
            if($scope.order.SkuOrdereds[0].AutoShipFrequency2)
            {
                items.push({ Key: 2, Text: '2 Months' });
            }
            if($scope.order.SkuOrdereds[0].AutoShipFrequency3)
            {
                items.push({ Key: 3, Text: '3 Months' });
            }
            if($scope.order.SkuOrdereds[0].AutoShipFrequency6)
            {
                items.push({ Key: 6, Text: '6 Months' });
            }
            $scope.autoShipOrderFrequencies = items;
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

        if (valid && skusClientValid())
        {
            var productErrorMessages = '';
            if ($scope.order.SkuOrdereds.length == 1 && !$scope.order.SkuOrdereds[0].Code)
            {
                productErrorMessages += "You must add at least 1 product. ";
            }
            var productErrorsExist = false;
            if ($scope.calculateErrors != null && $scope.calculateErrors.length != 0)
            {
                productErrorsExist = true;
            }
            if ($scope.productsPerishableThresholdIssue && !$scope.options.ignoneMinimumPerishableThreshold)
            {
                productErrorsExist = true;
            }
            if ($scope.productsPerishableThresholdIssue)
            {
                $scope.order.IgnoneMinimumPerishableThreshold = $scope.productsPerishableThresholdIssue && $scope.options.ignoneMinimumPerishableThreshold;
            }
            angular.forEach($scope.order.SkuOrdereds, function (skuOrdered, index)
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
                $scope.mainTab.active = true;
                toaster.pop('error', 'Error!', productErrorMessages, null, 'trustedHtml');
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
                if (!$scope.id)
                {
                    if ($scope.order.IdPaymentMethodType == 1)//card
                    {
                        if ($scope.currentCustomer.CreditCards.length == 0)
                        {
                            billingErrorMessages += "Credit Card is required. ";
                        }
                        else
                        {
                            angular.forEach($scope.currentCustomer.CreditCards, function (cardItem, index)
                            {
                                cardItem.IsSelected = index.toString() == $scope.paymentInfoTab.CreditCardIndex;
                            });
                        }
                    }
                    if ($scope.order.IdPaymentMethodType == 2)//oac
                    {
                        if ($scope.currentCustomer.Oac == null)
                        {
                            billingErrorMessages += "On Approved Credit is required. ";
                        }
                    }
                    if ($scope.order.IdPaymentMethodType == 3)//check
                    {
                        if ($scope.currentCustomer.Check == null)
                        {
                            billingErrorMessages += "Check is required. ";
                        }
                    }
                    if ($scope.order.IdPaymentMethodType == 6)
                    {
                        if ($scope.currentCustomer.WireTransfer == null)
                        {
                            billingErrorMessages += "Wire Transfer is required. ";
                        }
                    }
                    if ($scope.order.IdPaymentMethodType == 7)
                    {
                        if ($scope.currentCustomer.Marketing == null)
                        {
                            billingErrorMessages += "Marketing payment info is required. ";
                        }
                    }
                    if ($scope.order.IdPaymentMethodType == 8)
                    {
                        if ($scope.currentCustomer.VCWellness == null)
                        {
                            billingErrorMessages += "VC Wellness Employee Program is required. ";
                        }
                    }
                }
                else
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
                }
            }

            if (billingErrorMessages)
            {
                $scope.paymentInfoTab.active = true;
                toaster.pop('error', 'Error!', billingErrorMessages, null, 'trustedHtml');
                return;
            }

            if ($scope.currentCustomer.newEmail || $scope.currentCustomer.emailConfirm)
            {
                $scope.currentCustomer.Email = $scope.currentCustomer.newEmail;
                $scope.currentCustomer.EmailConfirm = $scope.currentCustomer.emailConfirm;
            } else
            {
                $scope.currentCustomer.EmailConfirm = $scope.currentCustomer.Email;
            }

            if ($scope.order.ShipDelayType == 1)
            {
                $scope.order.ShipDelayDateP = null;
                $scope.order.ShipDelayDateNP = null;
            }
            if ($scope.order.ShipDelayType == 2)
            {
                $scope.order.ShipDelayDate = null;
            }

            var order = angular.copy($scope.order);
            order.Customer = angular.copy($scope.currentCustomer);

            if (order.Customer.InceptionDate)
            {
                order.Customer.InceptionDate = order.Customer.InceptionDate.toServerDateTime();
            }

            if (!order.AutoShip)
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

            if ($scope.options.OverrideEmail)
            {
                order.Customer.Email = null;
                order.Customer.EmailConfirm = null;
            }

            orderService.updateOrder(order, $scope.addEditTracker).success(function (result)
            {
                successSaveHandler(result);
            }).
            error(function (result)
            {
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
            $scope.forms.submitted['profile'] = true;
            $scope.forms.submitted['shipping'] = true;
            $scope.forms.submitted['card'] = true;
            $scope.forms.submitted['oac'] = true;
            $scope.forms.submitted['check'] = true;
            $scope.forms.submitted['wiretransfer'] = true;
            $scope.forms.submitted['marketing'] = true;
            $scope.forms.submitted['vcwellness'] = true;
            toaster.pop('error', "Error!", "Validation errors, please correct field values.", null, 'trustedHtml');
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
                        product.Id = newProduct.Id;
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
        //resolving issue with additional load after lost focus from the input in time of selecting a new element
        if (skuChangedRequest)
        {
            $timeout.cancel(skuChangedRequest);
        }
        skuChangedRequest = $timeout(function ()
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
                                if ($scope.currentCustomer.CustomerType == 1)
                                {
                                    product.Price = result.Data.Price;
                                }
                                else if ($scope.currentCustomer.CustomerType == 2)
                                {
                                    product.Price = result.Data.WholesalePrice;
                                }
                                product.Amount = product.Price;

                                if ($scope.order.SkuOrdereds.length == index + 1)
                                {
                                    $scope.productAdd();
                                }

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
            else
            {
                if(product.RequestedCode)
                {
                    product.RequestedCode='';
                    $scope.requestRecalculate();
                }
            }
        }, 20);
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
                            if ($scope.currentCustomer.CustomerType == 1)
                            {
                                product.Price = result.Data.Price;
                            }
                            else if ($scope.currentCustomer.CustomerType == 2)
                            {
                                product.Price = result.Data.WholesalePrice;
                            }
                            product.Amount = product.Price;

                            if ($scope.order.SkuOrdereds.length == index + 1)
                            {
                                $scope.productAdd();
                            }

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