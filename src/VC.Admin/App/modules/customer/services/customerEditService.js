'use strict';

angular.module('app.modules.customer.services.customerEditService', [])
.factory('customerEditService', ['$q', 'customerService', 'toaster', function ($q, customerService, toaster)
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

        uiScope.deleteSelectedCreditCard = function (id)
        {
            var idx = -1;

            angular.forEach(uiScope.currentCustomer.CreditCards, function (item, index)
            {
                if (item.Id == id)
                {
                    idx = index;
                    return;
                }
            });

            uiScope.currentCustomer.CreditCards.splice(idx, 1);
            if (idx < uiScope.currentCustomer.CreditCards.length)
            {
                uiScope.paymentInfoTab.CreditCard = uiScope.currentCustomer.CreditCards[idx];
            }
            else if (uiScope.currentCustomer.CreditCards.length > 0)
            {
                uiScope.paymentInfoTab.CreditCard = uiScope.currentCustomer.CreditCards[0];
            }
            else
            {
                uiScope.paymentInfoTab.CreditCard = undefined;
            }
        };
    };

    var initCustomerEdit = function (uiScope)
    {
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
            if (uiScope.currentCustomer.sameShipping)
            {
                var defaultValue = uiScope.shippingAddressTab.Address.Default;
                for (var key in uiScope.currentCustomer.ProfileAddress)
                {
                    uiScope.shippingAddressTab.Address[key] = uiScope.currentCustomer.ProfileAddress[key];
                }
                if (uiScope.currentCustomer.newEmail)
                {
                    uiScope.shippingAddressTab.Address.Email = uiScope.currentCustomer.newEmail;
                } else
                {
                    uiScope.shippingAddressTab.Address.Email = uiScope.currentCustomer.Email;
                }
                uiScope.shippingAddressTab.Address.Default = defaultValue;
                uiScope.shippingAddressTab.Address.AddressType = 3;
                uiScope.shippingAddressTab.Address.Id = 0;
            }
        };

        function deleteShippingAddressLocal(id)
        {
            var idx = -1;

            angular.forEach(uiScope.currentCustomer.Shipping, function (item, index)
            {
                if (item.Id == id)
                {
                    idx = index;
                    return;
                }
            });

            uiScope.currentCustomer.Shipping.splice(idx, 1);
            if (idx < uiScope.currentCustomer.Shipping.length)
            {
                uiScope.shippingAddressTab.Address = uiScope.currentCustomer.Shipping[idx];
            }
            else if (uiScope.currentCustomer.Shipping.length > 0)
            {
                uiScope.shippingAddressTab.Address = uiScope.currentCustomer.Shipping[0];
            }
            else
            {
                uiScope.setNewAddress();
            }
        };

        uiScope.deleteSelectedShipping = function (id)
        {
            customerService.deleteAddress(id, uiScope.addEditTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        deleteShippingAddressLocal(id);
                        toaster.pop('success', "Success!", "Shipping Address was succesfully deleted");
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

        uiScope.cancelAddNewShipping = function ()
        {
            uiScope.shippingAddressTab.Address = uiScope.currentCustomer.Shipping[0];
            uiScope.shippingAddressTab.NewAddress = false;
        };

        uiScope.addNewShipping = function ()
        {
            clearServerValidation(uiScope);

            setCountryValidity(uiScope);

            if (uiScope.forms.shipping.$valid)
            {
                if (uiScope.editMode)
                {
                    customerService.addAddress(uiScope.shippingAddressTab.Address, uiScope.currentCustomer.Id, uiScope.addEditTracker).success(function (result)
                    {
                        if (result.Success)
                        {
                            syncCountry(uiScope, result.Data);
                            uiScope.currentCustomer.Shipping.push(result.Data);
                            uiScope.shippingAddressTab.Address = result.Data;
                            uiScope.shippingAddressTab.NewAddress = false;
                            toaster.pop('success', "Success!", "Customer address was succesfully added");
                        }
                        else
                        {
                            errorHandler(result);
                            //toaster.pop('error', 'Error!', "Can't add shipping address");
                        }
                    }).
                    error(function (result)
                    {
                        errorHandler(result);
                    });
                }
                else
                {
                    var newAddress = angular.copy(uiScope.shippingAddressTab.Address);
                    syncCountry(uiScope, newAddress);
                    uiScope.currentCustomer.Shipping.push(newAddress);
                    uiScope.shippingAddressTab.NewAddress = false;
                }
            } else
            {
                uiScope.forms.submitted['shipping'] = true;
            }
        };

        uiScope.setNewAddress = function ()
        {
            customerService.createAddressPrototype(uiScope.addEditTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        uiScope.currentCustomer.sameShipping = false;
                        uiScope.shippingAddressTab.NewAddress = true;
                        uiScope.shippingAddressTab.Address = result.Data;
                    } else
                    {
                        errorHandler(result);
                        //toaster.pop('error', 'Error!', "Can't add shipping address");
                    }
                }).
                error(function (result)
                {
                    errorHandler(result);
                })
                .then(function ()
                {
                    uiScope.forms.submitted['shipping'] = false;
                });
            return false;
        };

        uiScope.setDefaultAddress = function (id)
        {
            angular.forEach(uiScope.currentCustomer.Shipping, function (shippingItem)
            {
                if (shippingItem.Id != id && shippingItem.Default)
                {
                    shippingItem.Default = false;
                }
                else if (shippingItem.Id == id)
                {
                    shippingItem.Default = true;
                }
            });
        };

        uiScope.makeBillingAsProfileAddress = function ()
        {
            if (uiScope.paymentInfoTab.sameBilling)
            {
                var address;
                switch (String(uiScope.paymentInfoTab.PaymentMethodType))
                {
                    case "1":
                        if (uiScope.paymentInfoTab.CreditCard)
                        {
                            address = uiScope.paymentInfoTab.CreditCard.Address;
                        }
                        break;
                    case "2":
                        if (uiScope.currentCustomer.Oac)
                        {
                            address = uiScope.currentCustomer.Oac.Address;
                        }
                        break;
                    case "3":
                        if (uiScope.currentCustomer.Check)
                        {
                            address = uiScope.currentCustomer.Check.Address;
                        }
                        break;
                }
                if (address)
                {
                    for (var key in uiScope.currentCustomer.ProfileAddress)
                    {
                        address[key] = uiScope.currentCustomer.ProfileAddress[key];
                    }
                    if (uiScope.currentCustomer.newEmail)
                    {
                        address.Email = uiScope.currentCustomer.newEmail;
                    } else
                    {
                        address.Email = uiScope.currentCustomer.Email;
                    }
                    address.AddressType = 2;
                    address.Id = 0;
                }
            }
        };

        uiScope.setNewCreditCard = function (callback)
        {
            customerService.createCreditCardPrototype(uiScope.addEditTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        uiScope.currentCustomer.CreditCards.push(result.Data);
                        uiScope.paymentInfoTab.CreditCard = result.Data;
                        uiScope.paymentInfoTab.CreditCard.formName = uiScope.paymentInfoTab.formName;
                        uiScope.paymentInfoTab.sameBilling = false;
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
                    uiScope.forms.submitted['billing'] = false;
                });
            return false;
        };

        uiScope.setNewCheck = function (callback)
        {
            customerService.createCheckPrototype(uiScope.addEditTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        uiScope.currentCustomer.Check = result.Data;
                        uiScope.currentCustomer.Check.formName = uiScope.paymentInfoTab.formName;
                        uiScope.paymentInfoTab.sameBilling = false;
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
                    uiScope.forms.submitted['billing'] = false;
                });
            return false;
        };

        uiScope.setNewOac = function (callback)
        {
            customerService.createOacPrototype(uiScope.addEditTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        uiScope.currentCustomer.Oac = result.Data;
                        uiScope.currentCustomer.Oac.formName = uiScope.paymentInfoTab.formName;
                        uiScope.paymentInfoTab.sameBilling = false;
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
                    uiScope.forms.submitted['billing'] = false;
                });
            return false;
        };
    };

    var initOrderEditCustomerParts = function (uiScope)
    {
        uiScope.setNewCreditCard = function (callback)
        {
            customerService.createCreditCardPrototype(uiScope.addEditTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        uiScope.paymentInfoTab.CreditCard = result.Data;
                        uiScope.paymentInfoTab.CreditCard.formName = uiScope.paymentInfoTab.formName;
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
                    uiScope.forms.submitted['billing'] = false;
                });
            return false;
        };

        uiScope.setNewCheck = function (callback)
        {
            customerService.createCheckPrototype(uiScope.addEditTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        uiScope.order.Check = result.Data;
                        uiScope.order.Check.formName = uiScope.paymentInfoTab.formName;
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
                    uiScope.forms.submitted['billing'] = false;
                });
            return false;
        };

        uiScope.setNewOac = function (callback)
        {
            customerService.createOacPrototype(uiScope.addEditTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        uiScope.order.Oac = result.Data;
                        uiScope.order.Oac.formName = uiScope.paymentInfoTab.formName;
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
                    uiScope.forms.submitted['billing'] = false;
                });
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