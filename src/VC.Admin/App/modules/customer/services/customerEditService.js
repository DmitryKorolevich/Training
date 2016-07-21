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
            if (uiScope.currentCustomer.CreditCards.length > 0)
            {
                uiScope.paymentInfoTab.CreditCardIndex = (uiScope.currentCustomer.CreditCards.length - 1).toString();
            }
            else
            {
                uiScope.paymentInfoTab.CreditCardIndex = undefined;
            }
            $.each(uiScope.forms.card, function (index, element)
            {
                if (element && element.$name == index)
                {
                    element.$setValidity("server", true);
                }
            });
        };

        uiScope.deleteAllCreditCards = function ()
        {
            uiScope.currentCustomer.CreditCards = [];
            uiScope.paymentInfoTab.CreditCardIndex = undefined;
        };

        uiScope.makeBillingAsShippingAddress = function ()
        {
            var address;
            switch (String(uiScope.paymentInfoTab.PaymentMethodType))
            {
                case "1":
                    if (uiScope.paymentInfoTab.CreditCardIndex)
                    {
                        address = uiScope.currentCustomer.CreditCards[uiScope.paymentInfoTab.CreditCardIndex].Address;
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
                case "4":
                    if (uiScope.currentCustomer.NC)
                    {
                        address = uiScope.currentCustomer.NC.Address;
                    }
                    break;
                case "6":
                    if (uiScope.currentCustomer.WireTransfer)
                    {
                        address = uiScope.currentCustomer.WireTransfer.Address;
                    }
                    break;
                case "7":
                    if (uiScope.currentCustomer.Marketing)
                    {
                        address = uiScope.currentCustomer.Marketing.Address;
                    }
                    break;
                case "8":
                    if (uiScope.currentCustomer.VCWellness)
                    {
                        address = uiScope.currentCustomer.VCWellness.Address;
                    }
                    break;
            }
            if (address)
            {
                var activeShipping = uiScope.buildShippingAddressForPartial(uiScope.currentCustomer.Shipping, uiScope.shippingAddressTab.AddressIndex,
                    uiScope.order ? uiScope.order.OnHold && !uiScope.order.UpdateShippingAddressForCustomer : false, uiScope.order && true);
                if (activeShipping)
                {
                    for (var key in activeShipping.Address)
                    {
                        address[key] = activeShipping.Address[key];
                    }
                    address.AddressType = 2;
                    address.Id = 0;
                }
            }
        };
    };

    var initCustomerEdit = function (uiScope)
    {
        uiScope.cardChanged = function ()
        {
            $.each(uiScope.forms.card, function (index, element)
            {
                if (element && element.$name == index)
                {
                    element.$setValidity("server", true);
                }
            });
        }

        uiScope.buildShippingAddressForPartial = function (collection, index, disableValidation, forOrder)
        {
            if (collection === undefined || collection[index] === undefined || uiScope.shippingAddressTab.ShippingEditModels === undefined)
                return undefined;
            if (uiScope.shippingAddressTab.ShippingEditModels[index] === undefined ||
                uiScope.shippingAddressTab.ShippingEditModels[index].Address != collection[index])
            {
                uiScope.shippingAddressTab.ShippingEditModels[index] = { Address: collection[index], formName: 'shipping', index: index, collectionName: 'Shipping', recalculate: forOrder };
            }
            uiScope.shippingAddressTab.ShippingEditModels[index].disableValidation = disableValidation;
            return uiScope.shippingAddressTab.ShippingEditModels[index];
        }

        uiScope.buildCreditCardAddressForPartial = function (collection, index, disableValidation)
        {
            if (collection === undefined || collection[index] === undefined || collection[index].Address === undefined || uiScope.paymentInfoTab.AddressEditModels === undefined)
                return undefined;
            if (uiScope.paymentInfoTab.AddressEditModels[index] === undefined ||
                uiScope.paymentInfoTab.AddressEditModels[index].Address != collection[index].Address)
            {
                uiScope.paymentInfoTab.AddressEditModels[index] = { Address: collection[index].Address, formName: 'card', index: index, collectionName: 'CreditCards' };
            }
            uiScope.paymentInfoTab.AddressEditModels[index].disableValidation = disableValidation;
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

        uiScope.toggleSuspended = function ()
        {
            uiScope.currentCustomer.StatusCode = uiScope.currentCustomer.StatusCode == 4 ? (uiScope.currentCustomer.IsConfirmed ? 2 : 1) : 4;
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
            uiScope.currentCustomer.Shipping[uiScope.shippingAddressTab.AddressIndex].Company = uiScope.currentCustomer.ProfileAddress.Company;
            uiScope.currentCustomer.Shipping[uiScope.shippingAddressTab.AddressIndex].FirstName = uiScope.currentCustomer.ProfileAddress.FirstName;
            uiScope.currentCustomer.Shipping[uiScope.shippingAddressTab.AddressIndex].LastName = uiScope.currentCustomer.ProfileAddress.LastName;
            uiScope.currentCustomer.Shipping[uiScope.shippingAddressTab.AddressIndex].Phone = uiScope.currentCustomer.ProfileAddress.Phone;
            uiScope.currentCustomer.Shipping[uiScope.shippingAddressTab.AddressIndex].Fax = uiScope.currentCustomer.ProfileAddress.Fax;
            if (uiScope.currentCustomer.newEmail)
            {
                uiScope.currentCustomer.Shipping[uiScope.shippingAddressTab.AddressIndex].Email = uiScope.currentCustomer.newEmail;
            } else
            {
                uiScope.currentCustomer.Shipping[uiScope.shippingAddressTab.AddressIndex].Email = uiScope.currentCustomer.Email;
            }
            uiScope.currentCustomer.Shipping[uiScope.shippingAddressTab.AddressIndex].AddressType = 3;
            uiScope.currentCustomer.Shipping[uiScope.shippingAddressTab.AddressIndex].Id = 0;
        };

        uiScope.makeShippingAsBillingAddress = function ()
        {
            var address;
            switch (String(uiScope.paymentInfoTab.PaymentMethodType))
            {
                case "1":
                    if (uiScope.paymentInfoTab.CreditCardIndex)
                    {
                        address = uiScope.currentCustomer.CreditCards[uiScope.paymentInfoTab.CreditCardIndex].Address;
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
                case "4":
                    if (uiScope.currentCustomer.NC)
                    {
                        address = uiScope.currentCustomer.NC.Address;
                    }
                    break;
                case "6":
                    if (uiScope.currentCustomer.WireTransfer)
                    {
                        address = uiScope.currentCustomer.WireTransfer.Address;
                    }
                    break;
                case "7":
                    if (uiScope.currentCustomer.Marketing)
                    {
                        address = uiScope.currentCustomer.Marketing.Address;
                    }
                    break;
                case "8":
                    if (uiScope.currentCustomer.VCWellness)
                    {
                        address = uiScope.currentCustomer.VCWellness.Address;
                    }
                    break;
            }
            if (address)
            {
                var activeShipping = uiScope.buildShippingAddressForPartial(uiScope.currentCustomer.Shipping, uiScope.shippingAddressTab.AddressIndex,
                    uiScope.order ? uiScope.order.OnHold && !uiScope.order.UpdateShippingAddressForCustomer : false, uiScope.order && true);
                if (activeShipping)
                {
                    var defaultField = activeShipping.Address.Default;
                    var shippingAddressType = activeShipping.Address.ShippingAddressType;
                    var preferredShipMethod = activeShipping.Address.PreferredShipMethod;
                    var deliveryInstructions = activeShipping.Address.DeliveryInstructions;
                    for (var key in address)
                    {
                        activeShipping.Address[key] = address[key];
                    }
                    activeShipping.Address.AddressType = 3;
                    activeShipping.Address.Id = 0;
                    activeShipping.Address.Default = defaultField;
                    activeShipping.Address.ShippingAddressType = shippingAddressType;
                    activeShipping.Address.PreferredShipMethod = preferredShipMethod;
                    activeShipping.Address.DeliveryInstructions = deliveryInstructions;
                }
            }
        };

        uiScope.deleteShippingAddress = function ()
        {
            uiScope.currentCustomer.Shipping.splice(uiScope.shippingAddressTab.AddressIndex, 1);
            if (uiScope.currentCustomer.Shipping.length > 0)
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
            if (uiScope.forms.shipping.$valid)
            {
                customerService.createAddressPrototype(uiScope.addEditTracker)
                    .success(function (result)
                    {
                        if (result.Success)
                        {
                            uiScope.currentCustomer.sameShipping = false;
                            uiScope.shippingAddressTab.NewAddress = true;
                            syncCountry(uiScope, result.Data);
                            uiScope.currentCustomer.Shipping.push(result.Data);
                            uiScope.shippingAddressTab.AddressIndex = (parseInt(uiScope.currentCustomer.Shipping.length - 1)).toString();
                        } else
                        {
                            errorHandler(result);
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
            }
            else
            {
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

        uiScope.setDefaultCreditCard = function ()
        {
            angular.forEach(uiScope.currentCustomer.CreditCards, function (creditCard, index)
            {
                if (index != uiScope.paymentInfoTab.CreditCardIndex && creditCard.Default)
                {
                    creditCard.Default = false;
                }
                else if (index == uiScope.paymentInfoTab.CreditCardIndex)
                {
                    creditCard.Default = true;
                }
            });
        };

        uiScope.setNewCreditCard = function (callback)
        {
            if (uiScope.forms.card.$valid)
            {
                customerService.createCreditCardPrototype(uiScope.addEditTracker)
                    .success(function (result)
                    {
                        if (result.Success)
                        {
                            syncCountry(uiScope, result.Data.Address);
                            if (uiScope.currentCustomer.CreditCards.length == 0)
                            {
                                result.Data.Default = true;
                            }
                            uiScope.currentCustomer.CreditCards.push(result.Data);
                            if (uiScope.paymentInfoTab.CreditCardIndex === undefined)
                            {
                                uiScope.paymentInfoTab.CreditCardIndex = "0";
                            }
                            else
                            {
                                uiScope.paymentInfoTab.CreditCardIndex = (uiScope.currentCustomer.CreditCards.length - 1).toString();
                            }
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
                        uiScope.forms.submitted['card'] = false;
                    });
            }
            else
            {
                uiScope.forms.submitted['card'] = true;
            }
            return false;
        };

        uiScope.setNewCheck = function (callback)
        {
            if (uiScope.forms.check.$valid)
            {
                customerService.createCheckPrototype(uiScope.addEditTracker)
                    .success(function (result)
                    {
                        if (result.Success)
                        {
                            syncCountry(uiScope, result.Data.Address);
                            uiScope.currentCustomer.Check = result.Data;
                            uiScope.currentCustomer.Check.formName = 'check';
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
                        uiScope.forms.submitted['check'] = false;
                    });
            }
            else
            {
                uiScope.forms.submitted['check'] = true;
            }
            return false;
        };

        uiScope.setNewOac = function (callback)
        {
            if (uiScope.forms.oac.$valid)
            {
                customerService.createOacPrototype(uiScope.addEditTracker)
                    .success(function (result)
                    {
                        if (result.Success)
                        {
                            syncCountry(uiScope, result.Data.Address);
                            uiScope.currentCustomer.Oac = result.Data;
                            uiScope.currentCustomer.Oac.formName = 'oac';
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
                        uiScope.forms.submitted['oac'] = false;
                    });
            }
            else
            {
                uiScope.forms.submitted['oac'] = true;
            }
            return false;
        };

        uiScope.setNewWireTransfer = function (callback)
        {
            if (uiScope.forms.wiretransfer.$valid)
            {
                customerService.createWireTransferPrototype(uiScope.addEditTracker)
                    .success(function (result)
                    {
                        if (result.Success)
                        {
                            syncCountry(uiScope, result.Data.Address);
                            uiScope.currentCustomer.WireTransfer = result.Data;
                            uiScope.currentCustomer.WireTransfer.formName = 'wiretransfer';
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
                        uiScope.forms.submitted['wiretransfer'] = false;
                    });
            }
            else
            {
                uiScope.forms.submitted['wiretransfer'] = true;
            }
            return false;
        };


        uiScope.setNewMarketing = function (callback)
        {
            if (uiScope.forms.marketing.$valid)
            {
                customerService.createMarketingPrototype(uiScope.addEditTracker)
                    .success(function (result)
                    {
                        if (result.Success)
                        {
                            syncCountry(uiScope, result.Data.Address);
                            uiScope.currentCustomer.Marketing = result.Data;
                            uiScope.currentCustomer.Marketing.formName = 'marketing';
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
                        uiScope.forms.submitted['marketing'] = false;
                    });
            }
            else
            {
                uiScope.forms.submitted['marketing'] = true;
            }
            return false;
        };

        uiScope.setNewVCWellness = function (callback)
        {
            if (uiScope.forms.vcwellness.$valid)
            {
                customerService.createVCWellnessPrototype(uiScope.addEditTracker)
                    .success(function (result)
                    {
                        if (result.Success)
                        {
                            syncCountry(uiScope, result.Data.Address);
                            uiScope.currentCustomer.VCWellness = result.Data;
                            uiScope.currentCustomer.VCWellness.formName = 'vcwellness';
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
                        uiScope.forms.submitted['nc'] = false;
                    });
            }
            else
            {
                uiScope.forms.submitted['vcwellness'] = true;
            }
            return false;
        };

        uiScope.setNewNC = function (callback)
        {
            if (uiScope.forms.nc.$valid)
            {
                customerService.createNCPrototype(uiScope.addEditTracker)
                    .success(function (result)
                    {
                        if (result.Success)
                        {
                            syncCountry(uiScope, result.Data.Address);
                            uiScope.currentCustomer.NC = result.Data;
                            uiScope.currentCustomer.NC.formName = 'nc';
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
                        uiScope.forms.submitted['nc'] = false;
                    });
            }
            else
            {
                uiScope.forms.submitted['vcwellness'] = true;
            }
            return false;
        };
    };

    var initOrderEditCustomerParts = function (uiScope)
    {
        uiScope.setNewCreditCard = function (callback)
        {
            if (uiScope.forms.card.$valid)
            {
                customerService.createCreditCardPrototype(uiScope.addEditTracker)
                    .success(function (result)
                    {
                        if (result.Success)
                        {
                            uiScope.paymentInfoTab.CreditCard = result.Data;
                            syncCountry(uiScope, result.Data.Address);
                            uiScope.paymentInfoTab.CreditCard.formName = "card";
                            uiScope.currentCustomer.CreditCards.push(uiScope.paymentInfoTab.CreditCard);
                            if (uiScope.paymentInfoTab.CreditCardIndex === undefined)
                            {
                                uiScope.paymentInfoTab.CreditCardIndex = "0";
                            }
                            else
                            {
                                uiScope.paymentInfoTab.CreditCardIndex = (uiScope.currentCustomer.CreditCards.length - 1).toString();
                            }
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
                        uiScope.forms.submitted['card'] = false;
                    });
            }
            else
            {
                uiScope.forms.submitted['card'] = true;
            }
            return false;
        };

        uiScope.setNewCheck = function (callback)
        {
            if (uiScope.forms.check.$valid)
            {
                customerService.createCheckPrototype(uiScope.addEditTracker)
                    .success(function (result)
                    {
                        if (result.Success)
                        {
                            uiScope.order.Check = result.Data;
                            syncCountry(uiScope, result.Data.Address);
                            uiScope.order.Check.formName = 'check';
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
                        uiScope.forms.submitted['check'] = false;
                    });
            }
            else
            {
                uiScope.forms.submitted['check'] = true;
            }
            return false;
        };

        uiScope.setNewOac = function (callback)
        {
            if (uiScope.forms.oac.$valid)
            {
                customerService.createOacPrototype(uiScope.addEditTracker)
                    .success(function (result)
                    {
                        if (result.Success)
                        {
                            uiScope.order.Oac = result.Data;
                            syncCountry(uiScope, result.Data.Address);
                            uiScope.order.Oac.formName = 'oac';
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
                        uiScope.forms.submitted['oac'] = false;
                    });
            }
            else
            {
                uiScope.forms.submitted['oac'] = true;
            }
            return false;
        };

        uiScope.setNewWireTransfer = function (callback)
        {
            if (uiScope.forms.wiretransfer.$valid)
            {
                customerService.createWireTransferPrototype(uiScope.addEditTracker)
                    .success(function (result)
                    {
                        if (result.Success)
                        {
                            uiScope.order.WireTransfer = result.Data;
                            syncCountry(uiScope, result.Data.Address);
                            uiScope.order.WireTransfer.formName = 'wiretransfer';
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
                        uiScope.forms.submitted['wiretransfer'] = false;
                    });
            }
            else
            {
                uiScope.forms.submitted['wiretransfer'] = true;
            }
            return false;
        };

        uiScope.setNewMarketing = function (callback)
        {
            if (uiScope.forms.marketing.$valid)
            {
                customerService.createMarketingPrototype(uiScope.addEditTracker)
                    .success(function (result)
                    {
                        if (result.Success)
                        {
                            uiScope.order.Marketing = result.Data;
                            syncCountry(uiScope, result.Data.Address);
                            uiScope.order.Marketing.formName = 'marketing';
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
                        uiScope.forms.submitted['marketing'] = false;
                    });
            }
            else
            {
                uiScope.forms.submitted['marketing'] = true;
            }
            return false;
        };

        uiScope.setNewVCWellness = function (callback)
        {
            if (uiScope.forms.vcwellness.$valid)
            {
                customerService.createVCWellnessPrototype(uiScope.addEditTracker)
                    .success(function (result)
                    {
                        if (result.Success)
                        {
                            uiScope.order.VCWellness = result.Data;
                            syncCountry(uiScope, result.Data.Address);
                            uiScope.order.VCWellness.formName = 'vcwellness';
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
                        uiScope.forms.submitted['vcwellness'] = false;
                    });
            }
            else
            {
                uiScope.forms.submitted['vcwellness'] = true;
            }
            return false;
        };

        uiScope.setNewNC = function (callback)
        {
            if (uiScope.forms.nc.$valid)
            {
                customerService.createNCPrototype(uiScope.addEditTracker)
                    .success(function (result)
                    {
                        if (result.Success)
                        {
                            uiScope.order.NC = result.Data;
                            syncCountry(uiScope, result.Data.Address);
                            uiScope.order.NC.formName = 'nc';
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
                        uiScope.forms.submitted['nc'] = false;
                    });
            }
            else
            {
                uiScope.forms.submitted['nc'] = true;
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

        uiScope.selectedPaymentMethods = [];

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
                highPriNotes.push('<p>Date: ' + $filter('date')(noteItem.DateEdited, 'short') + '</p>' + '<p>Agent: ' + noteItem.EditedBy + '</p>' + '<p>Notes: <p>' + noteItem.Text + '</p></p>');
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