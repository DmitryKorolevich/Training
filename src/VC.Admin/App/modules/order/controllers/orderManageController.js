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
            if (!$scope.id)
            {
                $state.go('index.oneCol.orderDetail', { id: result.Data.Id });
            }
        }
        else
        {
            var messages = "";
            if (result.Messages)
            {
                $scope.forms.mainForm.submitted = true;
                $scope.forms.mainForm2.submitted = true;
                $scope.forms.GCs.skussubmitted = true;
                $scope.forms.submitted['profile'] = true;
                $scope.forms.submitted['shipping'] = true;
                $scope.forms.submitted['card'] = true;
                $scope.forms.submitted['oac'] = true;
                $scope.forms.submitted['check'] = true;
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
                                return false;
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
                                        return false;
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

        $scope.forms = { submitted: [] };

        $scope.autoShipOrderFrequencies = [
            { Key: 1, Text: '1 Month' },
            { Key: 2, Text: '2 Months' },
            { Key: 3, Text: '3 Months' },
            { Key: 6, Text: '6 Months' }
        ];

        $scope.minimumPerishableThreshold = $rootScope.ReferenceData.AppSettings.GlobalPerishableThreshold;
        $scope.ignoneMinimumPerishableThreshold = $scope.id ? true: false;//only for a new order
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
            ShippingEditModels: {}
        };
        $scope.paymentInfoTab = {
            active: false,
            formNames: ['card', 'oac', 'check'],
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
            customerGetCall: customerService.getExistingCustomer($scope.idCustomer, $scope.addEditTracker)
        }).then(function (result)
        {
            if (result.countriesCall.data.Success && result.customerGetCall.data.Success)
            {
                $scope.countries = result.countriesCall.data.Data;

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

                angular.forEach($scope.currentCustomer.CreditCards, function (creditCard)
                {
                    creditCard.formName = "card";
                    customerEditService.syncCountry($scope, creditCard.Address);
                });
                if ($scope.currentCustomer.CreditCards && $scope.currentCustomer.CreditCards[0])
                    $scope.paymentInfoTab.CreditCardIndex = "0";

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

    $scope.buildOrderShippingAddressForPartial = function (disableValidation)
    {
        if ($scope.order === undefined || $scope.order.Shipping === undefined)
            return undefined;
        if ($scope.shippingAddressTab.OrderShippingEditModel === undefined)
        {
            $scope.shippingAddressTab.OrderShippingEditModel = { Address: $scope.order.Shipping, formName: 'shipping' };
        }
        $scope.shippingAddressTab.OrderShippingEditModel.disableValidation = disableValidation;
        return $scope.shippingAddressTab.OrderShippingEditModel;
    }

    $scope.buildOrderPaymentPartial = function (model, disableValidation)
    {
        model.disableValidation = disableValidation;
        return model;
    }

    var initOrder = function ()
    {
        $scope.oldOrderStatus = $scope.order.OrderStatus;
        $scope.order.OnHold = $scope.order.OrderStatus == 7;//on hold status
        $scope.order.AutoShip = $scope.order.AutoShipFrequency ? true : false;
        $scope.$watch('order.OnHold', function (newValue, oldValue)
        {
            if (newValue)
            {
                $scope.order.OrderStatus = 7;
            }
            else
            {
                $scope.order.OrderStatus = $scope.oldOrderStatus;
            }
        });
        
        $scope.legend.CustomerName = $scope.currentCustomer.ProfileAddress.FirstName + " " + $scope.currentCustomer.ProfileAddress.LastName;
        if ($scope.id)
        {
            $scope.legend.OrderId = $scope.order.Id;
            $scope.legend.OrderDate = $scope.order.DateCreated;
            $scope.legend.OrderStatus = $rootScope.getReferenceItem($rootScope.ReferenceData.OrderStatuses, $scope.order.OrderStatus).Text;
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
        $scope.$broadcast('customerNotess#in#init', data);
    };

    var oldOrderForCalculating = null;

    $scope.requestRecalculate = function ()
    {
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
            $scope.forms.GCs.skussubmitted = true;
            $scope.calculateErrors = result.Messages;
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
            var productErrorMessages = '';
            if ($scope.order.SkuOrdereds.length == 1 && !$scope.order.SkuOrdereds[0].Code)
            {
                productErrorMessages += "You must add at least 1 product. ";
            }
            var productErrorsExist=false;
            if ($scope.calculateErrors != null && $scope.calculateErrors.length != 0)
            {
                productErrorsExist = true;
            }
            if($scope.productsPerishableThresholdIssue && !$scope.ignoneMinimumPerishableThreshold)
            {
                productErrorsExist = true;
            }
            angular.forEach($scope.order.SkuOrdereds, function (skuOrdered, index)
            {
                if (skuOrdered.Messages && skuOrdered.Messages.length != 0)
                {
                    productErrorsExist = true;
                    return;
                }
            });
            if(productErrorsExist)
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
                        if ($scope.currentCustomer.CreditCard == null)
                        {
                            billingErrorMessages += "Check is required. ";
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
                        if ($scope.order.CreditCard == null)
                        {
                            billingErrorMessages += "Check is required. ";
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
            var order = angular.copy($scope.order);
            order.Customer = angular.copy($scope.currentCustomer);

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
            $scope.forms.mainForm.submitted = true;
            $scope.forms.mainForm2.submitted = true;
            $scope.forms.submitted['profile'] = true;
            $scope.forms.submitted['shipping'] = true;
            $scope.forms.submitted['card'] = true;
            $scope.forms.submitted['oac'] = true;
            $scope.forms.submitted['check'] = true;
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
            if (product && product.Code && ($scope.skuFilter.ExactCode != product.Code || $scope.skuFilter.Index!= index || $scope.skuFilter.ExactDescriptionName != ''))
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

    initialize();

}]);