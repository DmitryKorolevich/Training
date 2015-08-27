'use strict';

angular.module('app.modules.customer.services.customerEditService', [])
.factory('customerEditService', ['$q', '$filter', '$injector', 'customerService', 'toaster', function ($q, $filter, $injector, customerService, toaster)
{
    function errorHandler(result)
    {
        toaster.pop('error', "Error!", "Server error occured");
    };

    function clearServerValidation(uiScope)
    {
        $.each(uiScope.forms, function (index, form)
        {
            if (form && !(typeof form === 'boolean'))
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

    function setCountryValidity(uiScope)
    {
        $.each(uiScope.forms, function (index, form)
        {
            if (form && !(typeof form === 'boolean'))
            {
                if (form.Country && form.Country.$viewValue && form.Country.$viewValue.Id == 0)
                {
                    form.Country.$setValidity("required", false);
                }
                if (form.State && form.State.$viewValue == 0)
                {
                    form.State.$setValidity("required", false);
                }
            }
        });
    };

    var initBase = function (uiScope)
    {
        uiScope.getLast4 = function (str)
        {
            if (str == null)
                return undefined;
            var start = str.length - 4;
            if (start < 0)
                start = 0;
            return str.slice(start, str.length);
        };

        uiScope.getCreditCardTypeName = function (idType)
        {
            for (var idx = 0; idx < uiScope.creditCardTypes.length; idx++)
            {
                if (uiScope.creditCardTypes[idx].Key == idType)
                    return uiScope.creditCardTypes[idx].Text;
            }
        };        

        uiScope.deleteSelectedCreditCard = function ()
        {
            uiScope.currentCustomer.CreditCards.splice(uiScope.paymentInfoTab.CreditCardIndex, 1);
            if (uiScope.currentCustomer.CreditCards.length > 0 && uiScope.paymentInfoTab.CreditCardIndex >= uiScope.currentCustomer.CreditCards.length)
            {
                uiScope.paymentInfoTab.CreditCardIndex = (uiScope.currentCustomer.CreditCards.length - 1).toString();
            }
            else
            {
                uiScope.paymentInfoTab.CreditCardIndex = undefined;
            }
        };
    };

    var initCustomerEdit = function (uiScope)
    {
        uiScope.buildShippingAddressForPartial = function (collection, index) {
            if (collection === undefined || collection[index] === undefined || uiScope.shippingAddressTab.ShippingEditModels === undefined)
                return undefined;
            if (uiScope.shippingAddressTab.ShippingEditModels[index] === undefined)
            {
                uiScope.shippingAddressTab.ShippingEditModels[index] = { Address: collection[index], formName: 'shipping', index: index, collectionName: 'Shipping' };
            }
            return uiScope.shippingAddressTab.ShippingEditModels[index];
        }

        uiScope.buildCreditCardAddressForPartial = function (collection, index) {
            if (collection === undefined || collection[index] === undefined || collection[index].Address === undefined || uiScope.paymentInfoTab.AddressEditModels === undefined)
                return undefined;
            if (uiScope.paymentInfoTab.AddressEditModels[index] === undefined) {
                uiScope.paymentInfoTab.AddressEditModels[index] = { Address: collection[index].Address, formName: 'billing', index: index, collectionName: 'CreditCards' };
            }
            return uiScope.paymentInfoTab.AddressEditModels[index];
        }

        uiScope.togglePaymentMethodSelection = function (paymentMethod)
        {
            if (!uiScope.currentCustomer.ApprovedPaymentMethods || uiScope.currentCustomer.ApprovedPaymentMethods.length == 0)
            {
                uiScope.currentCustomer.ApprovedPaymentMethods = [];
            }

            if (!uiScope.selectedPaymentMethods)
            {
                uiScope.selectedPaymentMethods = [];
            }

            var idx = -1;
            $.grep(uiScope.selectedPaymentMethods, function (elem, index)
            {
                if (elem.Id == paymentMethod.Id)
                {
                    idx = index;
                    return;
                }
            });

            if (idx > -1)
            {
                uiScope.currentCustomer.ApprovedPaymentMethods.splice(idx, 1);
                uiScope.selectedPaymentMethods.splice(idx, 1);
            }
            else
            {
                uiScope.selectedPaymentMethods.push({ Id: paymentMethod.Id, Name: paymentMethod.Name });
                uiScope.currentCustomer.ApprovedPaymentMethods.push(paymentMethod.Id);
            }
        };

        uiScope.toggleOrderNoteSelection = function (orderNoteId)
        {
            if (!uiScope.currentCustomer.OrderNotes)
            {
                uiScope.currentCustomer.OrderNotes = [];
            }

            var idx = uiScope.currentCustomer.OrderNotes.indexOf(orderNoteId);

            if (idx > -1)
            {
                uiScope.currentCustomer.OrderNotes.splice(idx, 1);
            }
            else
            {
                uiScope.currentCustomer.OrderNotes.push(orderNoteId);
            }
        };

        uiScope.$watch("currentCustomer.CustomerType", function (newValue, oldValue)
        {
            if (newValue)
            {
                $q.all(
                {
                    paymentMethodsCall: customerService.getPaymentMethods(uiScope.currentCustomer.CustomerType, uiScope.addEditTracker),
                    orderNotesCall: customerService.getOrderNotes(uiScope.currentCustomer.CustomerType, uiScope.addEditTracker)
                }).then(function (result)
                {
                    if (result.paymentMethodsCall.data.Success && result.orderNotesCall.data.Success)
                    {
                        uiScope.paymentMethods = result.paymentMethodsCall.data.Data;
                        uiScope.orderNotes = result.orderNotesCall.data.Data;

                        var orderNotes = [];
                        $.each(uiScope.currentCustomer.OrderNotes, function (index, id)
                        {
                            var add = false;
                            $.each(uiScope.orderNotes, function (index, orderNote)
                            {
                                if (id == orderNote.Id)
                                {
                                    add = true;
                                    return;
                                }
                            });
                            if (add)
                            {
                                orderNotes.push(id);
                            }
                        });
                        uiScope.currentCustomer.OrderNotes = orderNotes;

                        var aprovedPaymentMethods = [];
                        $.each(uiScope.currentCustomer.ApprovedPaymentMethods, function (index, id)
                        {
                            var add = false;
                            $.each(uiScope.paymentMethods, function (index, paymentMethod)
                            {
                                if (id == paymentMethod.Id)
                                {
                                    add = true;
                                    return;
                                }
                            });
                            if (add)
                            {
                                aprovedPaymentMethods.push(id);
                            }
                        });
                        uiScope.currentCustomer.ApprovedPaymentMethods = aprovedPaymentMethods;
                        syncDefaultPaymentMethod(uiScope);
                    } else
                    {
                        errorHandler(result);
                    }
                });
            }
        }, function (result)
        {
            errorHandler(result);
        });

        uiScope.makeAsProfileAddress = function ()
        {
            var defaultValue = uiScope.currentCustomer.Shipping[uiScope.shippingAddressTab.AddressIndex].Default;
            for (var key in uiScope.currentCustomer.ProfileAddress)
            {
                uiScope.currentCustomer.Shipping[uiScope.shippingAddressTab.AddressIndex][key] = uiScope.currentCustomer.ProfileAddress[key];
            }
            if (uiScope.currentCustomer.newEmail)
            {
                uiScope.currentCustomer.Shipping[uiScope.shippingAddressTab.AddressIndex].Email = uiScope.currentCustomer.newEmail;
            } else
            {
                uiScope.currentCustomer.Shipping[uiScope.shippingAddressTab.AddressIndex].Email = uiScope.currentCustomer.Email;
            }
            uiScope.currentCustomer.Shipping[uiScope.shippingAddressTab.AddressIndex].Default = defaultValue;
            uiScope.currentCustomer.Shipping[uiScope.shippingAddressTab.AddressIndex].AddressType = 3;
            uiScope.currentCustomer.Shipping[uiScope.shippingAddressTab.AddressIndex].Id = 0;
        };

        uiScope.deleteShippingAddress = function ()
        {
            uiScope.currentCustomer.Shipping.splice(uiScope.shippingAddressTab.AddressIndex, 1);
            if (uiScope.currentCustomer.Shipping.length > 0 && uiScope.shippingAddressTab.AddressIndex >= uiScope.currentCustomer.Shipping.length)
            {
                uiScope.shippingAddressTab.AddressIndex = (uiScope.currentCustomer.Shipping.length - 1).toString();
            }
            else
            {
                uiScope.setNewAddress();
            }
        };

        uiScope.setNewAddress = function ()
        {
            if (uiScope.forms.shipping.$valid) {
                customerService.createAddressPrototype(uiScope.addEditTracker)
                    .success(function (result) {
                        if (result.Success) {
                            uiScope.currentCustomer.sameShipping = false;
                            uiScope.shippingAddressTab.NewAddress = true;
                            syncCountry(uiScope, result.Data);
                            uiScope.currentCustomer.Shipping.push(result.Data);
                            uiScope.shippingAddressTab.AddressIndex = (parseInt(uiScope.shippingAddressTab.AddressIndex) + 1).toString();
                        } else {
                            errorHandler(result);
                        }
                    }).
                    error(function (result) {
                        errorHandler(result);
                    })
                    .then(function () {
                        uiScope.forms.submitted['shipping'] = false;
                    });
            }
            else {
                uiScope.forms.submitted['shipping'] = true;
            }
            return false;
        };

        uiScope.setDefaultAddress = function ()
        {
            angular.forEach(uiScope.currentCustomer.Shipping, function (shippingItem, index)
            {
                if (index != uiScope.shippingAddressTab.AddressIndex && shippingItem.Default)
                {
                    shippingItem.Default = false;
                }
                else if (index == uiScope.shippingAddressTab.AddressIndex)
                {
                    shippingItem.Default = true;
                }
            });
        };

        uiScope.makeBillingAsProfileAddress = function () {
            var address;
            switch (String(uiScope.paymentInfoTab.PaymentMethodType)) {
                case "1":
                    if (uiScope.paymentInfoTab.CreditCardIndex) {
                        address = uiScope.currentCustomer.CreditCards[uiScope.paymentInfoTab.CreditCardIndex].Address;
                    }
                    break;
                case "2":
                    if (uiScope.currentCustomer.Oac) {
                        address = uiScope.currentCustomer.Oac.Address;
                    }
                    break;
                case "3":
                    if (uiScope.currentCustomer.Check) {
                        address = uiScope.currentCustomer.Check.Address;
                    }
                    break;
            }
            if (address) {
                for (var key in uiScope.currentCustomer.ProfileAddress) {
                    address[key] = uiScope.currentCustomer.ProfileAddress[key];
                }
                if (uiScope.currentCustomer.newEmail) {
                    address.Email = uiScope.currentCustomer.newEmail;
                } else {
                    address.Email = uiScope.currentCustomer.Email;
                }
                address.AddressType = 2;
                address.Id = 0;
            }
        };

        uiScope.setNewCreditCard = function (callback)
        {
            if (uiScope.forms.billing.$valid) {
                customerService.createCreditCardPrototype(uiScope.addEditTracker)
                    .success(function (result) {
                        if (result.Success) {
                            uiScope.currentCustomer.CreditCards.push(result.Data);
                            if (uiScope.paymentInfoTab.CreditCardIndex === undefined) {
                                uiScope.paymentInfoTab.CreditCardIndex = "0";
                            }
                            else {
                                uiScope.paymentInfoTab.CreditCardIndex = (parseInt(uiScope.paymentInfoTab.CreditCardIndex) + 1).toString();
                            }
                            if (callback)
                                callback(result.Data);
                        } else {
                            successHandler(result);
                        }
                    }).
                    error(function (result) {
                        errorHandler(result);
                    })
                    .then(function () {
                        uiScope.forms.submitted['billing'] = false;
                    });
            }
            else
            {
                uiScope.forms.submitted['billing'] = true;
            }
            return false;
        };

        uiScope.setNewCheck = function (callback)
        {
            if (uiScope.forms.billing.$valid) {
                customerService.createCheckPrototype(uiScope.addEditTracker)
                    .success(function (result) {
                        if (result.Success) {
                            uiScope.currentCustomer.Check = result.Data;
                            uiScope.currentCustomer.Check.formName = uiScope.paymentInfoTab.formName;
                            if (callback)
                                callback(result.Data);
                        } else {
                            successHandler(result);
                        }
                    }).
                    error(function (result) {
                        errorHandler(result);
                    })
                    .then(function () {
                        uiScope.forms.submitted['billing'] = false;
                    });
            }
            else {
                uiScope.forms.submitted['billing'] = true;
            }
            return false;
        };

        uiScope.setNewOac = function (callback)
        {
            if (uiScope.forms.billing.$valid) {
                customerService.createOacPrototype(uiScope.addEditTracker)
                    .success(function (result) {
                        if (result.Success) {
                            uiScope.currentCustomer.Oac = result.Data;
                            uiScope.currentCustomer.Oac.formName = uiScope.paymentInfoTab.formName;
                            if (callback)
                                callback(result.Data);
                        } else {
                            successHandler(result);
                        }
                    }).
                    error(function (result) {
                        errorHandler(result);
                    })
                    .then(function () {
                        uiScope.forms.submitted['billing'] = false;
                    });
            }
            else {
                uiScope.forms.submitted['billing'] = true;
            }
            return false;
        };
    };

    var initOrderEditCustomerParts = function (uiScope)
    {
        uiScope.setNewCreditCard = function (callback)
        {
            if (uiScope.forms.billing.$valid) {
                customerService.createCreditCardPrototype(uiScope.addEditTracker)
                    .success(function (result) {
                        if (result.Success) {
                            uiScope.paymentInfoTab.CreditCard = result.Data;
                            uiScope.paymentInfoTab.CreditCard.formName = uiScope.paymentInfoTab.formName;
                            if (callback)
                                callback(result.Data);
                        } else {
                            successHandler(result);
                        }
                    }).
                    error(function (result) {
                        errorHandler(result);
                    })
                    .then(function () {
                        uiScope.forms.submitted['billing'] = false;
                    });
            }
            else {
                uiScope.forms.submitted['billing'] = true;
            }
            return false;
        };

        uiScope.setNewCheck = function (callback)
        {
            if (uiScope.forms.billing.$valid) {
                customerService.createCheckPrototype(uiScope.addEditTracker)
                    .success(function (result) {
                        if (result.Success) {
                            uiScope.order.Check = result.Data;
                            uiScope.order.Check.formName = uiScope.paymentInfoTab.formName;
                            if (callback)
                                callback(result.Data);
                        } else {
                            successHandler(result);
                        }
                    }).
                    error(function (result) {
                        errorHandler(result);
                    })
                    .then(function () {
                        uiScope.forms.submitted['billing'] = false;
                    });
            }
            else {
                uiScope.forms.submitted['billing'] = true;
            }
            return false;
        };

        uiScope.setNewOac = function (callback)
        {
            if (uiScope.forms.billing.$valid) {
                customerService.createOacPrototype(uiScope.addEditTracker)
                    .success(function (result) {
                        if (result.Success) {
                            uiScope.order.Oac = result.Data;
                            uiScope.order.Oac.formName = uiScope.paymentInfoTab.formName;
                            if (callback)
                                callback(result.Data);
                        } else {
                            successHandler(result);
                        }
                    }).
                    error(function (result) {
                        errorHandler(result);
                    })
                    .then(function () {
                        uiScope.forms.submitted['billing'] = false;
                    });
            }
            else {
                uiScope.forms.submitted['billing'] = true;
            }
            return false;
        };
    };

    var syncCountry = function (uiScope, addressItem)
    {
        if (addressItem && addressItem.Country)
        {
            var selectedCountry = $.grep(uiScope.countries, function (country)
            {
                return country.Id == addressItem.Country.Id;
            })[0];

            addressItem.Country = selectedCountry;
        }
    };

    var syncDefaultPaymentMethod = function (uiScope)
    {
        if (!uiScope.currentCustomer || !uiScope.currentCustomer.ApprovedPaymentMethods || uiScope.currentCustomer.ApprovedPaymentMethods.length == 0 || !uiScope.paymentMethods)
        {
            return;
        }

        if (!uiScope.selectedPaymentMethods)
        {
            uiScope.selectedPaymentMethods = [];
        }

        angular.forEach(uiScope.currentCustomer.ApprovedPaymentMethods, function (approvedPM)
        {
            uiScope.selectedPaymentMethods.push({
                Id: approvedPM, Name: $.grep(uiScope.paymentMethods, function (pm)
                {
                    return pm.Id == approvedPM;
                })[0].Name
            });
        });
    };

    var showHighPriNotes = function (uiScope)
    {
        var highPriNotes = [];
        angular.forEach(uiScope.currentCustomer.CustomerNotes, function (noteItem)
        {
            if (noteItem.Priority == 1)
            {
                highPriNotes.push('<p>Date: ' + $filter('date')(noteItem.DateEdited, 'short') + '</p>' + '<p>Agent: ' + noteItem.EditedBy + '</p>' + '<p>Notes: <p class="container">' + noteItem.Text + '</p></p>');
            }
        });
        if (highPriNotes.length > 0)
        {
            var infoPopupUtil = $injector.get('infoPopupUtil');
            infoPopupUtil.info('High Priority Notes', highPriNotes.join('<hr />'), undefined, true);
        }
    };

    return {
        initBase: initBase,
        initCustomerEdit: initCustomerEdit,
        initOrderEditCustomerParts: initOrderEditCustomerParts,
        syncDefaultPaymentMethod: syncDefaultPaymentMethod,
        syncCountry: syncCountry,
        showHighPriNotes: showHighPriNotes,
    };

}]);