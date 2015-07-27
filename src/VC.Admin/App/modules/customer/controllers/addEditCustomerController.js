'use strict';

angular.module('app.modules.customer.controllers.addEditCustomerController', [])
.controller('addEditCustomerController', ['$scope', 'customerService', 'toaster', 'promiseTracker', '$rootScope', '$q', '$state', '$stateParams', function ($scope, customerService, toaster, promiseTracker, $rootScope, $q, $state, $stateParams) {
	$scope.addEditTracker = promiseTracker("addEdit");

	function initialize() {
		$scope.editMode = $stateParams.id != null;

		$scope.inceptionDateOpened = false;

		$scope.currentCustomer = { CustomerType: 1};

		$scope.accountProfileTab = {
			active: true,
			formName: 'profile',
		};
		$scope.shippingAddressTab = {
			active: false,
			formName: 'shipping'
		};
		$scope.customerNotesTab = {
			active: false,
			formName: 'customerNote'
		};
		$scope.paymentInfoTab = {
		    active: false,
            formName: 'billing'
		};
		$scope.creditCardTypes = $rootScope.ReferenceData.CreditCardTypes;
		var tabs = [];
		tabs.push($scope.accountProfileTab);
		tabs.push($scope.shippingAddressTab);
		tabs.push($scope.paymentInfoTab);
		tabs.push($scope.customerNotesTab);
		$scope.tabs = tabs;

		$scope.forms = {};

		$scope.forms.submitted = [];

		$q.all({ countriesCall: customerService.getCountries($scope.addEditTracker), paymentMethodsCall: customerService.getPaymentMethods($scope.currentCustomer.CustomerType, $scope.addEditTracker), orderNotesCall: customerService.getOrderNotes($scope.currentCustomer.CustomerType, $scope.addEditTracker) }).then(function (result) {
			if (result.countriesCall.data.Success && result.paymentMethodsCall.data.Success && result.orderNotesCall.data.Success) {
				$scope.countries = result.countriesCall.data.Data;
				$scope.paymentMethods = result.paymentMethodsCall.data.Data;
				$scope.orderNotes = result.orderNotesCall.data.Data;
				if (!$scope.editMode) {
					customerService.createCustomerPrototype($scope.addEditTracker)
						.success(function(result) {
							if (result.Success) {
							    $scope.currentCustomer = result.Data;
								$scope.accountProfileTab.Address = $scope.currentCustomer.ProfileAddress;
								$scope.shippingAddressTab.Address = $scope.currentCustomer.Shipping[0];
								$scope.customerNotesTab.CustomerNote = $scope.currentCustomer.CustomerNotes[0];
								$scope.paymentInfoTab.Address = {};
							} else {
								toaster.pop('error', 'Error!', "Can't create customer");
							}
						}).
						error(function(result) {
							toaster.pop('error', "Error!", "Server error ocurred");
						});
				} else {
					customerService.getExistingCustomer($stateParams.id, $scope.addEditTracker)
						.success(function (result) {
							if (result.Success) {
								$scope.currentCustomer = result.Data;
								$scope.accountProfileTab.Address = $scope.currentCustomer.ProfileAddress;
								$scope.paymentInfoTab.PaymentMethodType = $scope.currentCustomer.DefaultPaymentMethod;
								$scope.paymentInfoTab.Address = {};
								$scope.switchPaymentMethodType();
								angular.forEach($scope.currentCustomer.Shipping, function(shippingItem) {
								    syncCountry(shippingItem);
								    if (shippingItem.Default)
								    {
								        $scope.shippingAddressTab.Address = shippingItem;
								    }
								});

								syncCountry($scope.currentCustomer.ProfileAddress);

								syncDefaultPaymentMethod();
								createCustomerNoteProto();

								updateCustomerNoteUIId();
							} else {
								toaster.pop('error', 'Error!', "Can't create customer");
							}
						}).
						error(function (result) {
							toaster.pop('error', "Error!", "Server error ocurred");
						});
				}
			} else {
				toaster.pop('error', 'Error!', "Can't get reference data");
			}
		}, function(result) {
			toaster.pop('error', "Error!", "Server error ocurred");
		});
	};

	function createCustomerNoteProto() {
	    customerService.createCustomerNotePrototype($scope.addEditTracker)
									.success(function (result) {
									    if (result.Success) {
									        $scope.customerNotesTab.CustomerNote = result.Data;
									    } else {
									        toaster.pop('error', 'Error!', "Can't process customer notes");
									    }
									}).
									error(function (result) {
									    toaster.pop('error', "Error!", "Server error ocurred");
									})
									.then(function () {
									    $scope.forms.submitted['customerNote'] = false;
									});
	};

	$scope.getLast4 = function (str) {
	    if (str == null)
	        return undefined;
	    var start = str.length - 4;
	    if (start < 0)
	        start = 0;
	    return str.slice(start, str.length);
	};

	$scope.getCreditCardTypeName = function(idType) {
	    for (var idx = 0; idx < $scope.creditCardTypes.length;idx++)
	    {
	        if ($scope.creditCardTypes[idx].Key == idType)
	            return $scope.creditCardTypes[idx].Text;
	    }
	}

	function syncCountry(addressItem) {
		var selectedCountry = $.grep($scope.countries, function (country) {
			return country.Id == addressItem.Country.Id;
		})[0];

		addressItem.Country = selectedCountry;
	};

	function clearServerValidation() {
		$.each($scope.forms, function (index, form) {
			if (form && !(typeof form === 'boolean')) {
				$.each(form, function (index, element) {
					if (element && element.$name == index) {
						element.$setValidity("server", true);
					}
				});
			}
		});
	};

	function activateTab(formName) {
		$.each($scope.tabs, function (index, item) {
			if (item.formName == formName) {
				item.active = true;
				return false;
			}
		});
	};

	function successHandler(result) {
		if (result.Success) {
			toaster.pop('success', "Success!", "Successfully saved");
			$state.go("index.oneCol.manageCustomers");
		} else {
			var messages = "";
			if (result.Messages) {
			    $scope.forms.submitted['profile'] = true;
			    $scope.forms.submitted['shipping'] = true;
			    $scope.forms.submitted['customerNote'] = true;
			    $scope.forms.submitted['billing'] = true;
				$scope.serverMessages = new ServerMessages(result.Messages);
				var formForShowing = null;
				$.each(result.Messages, function (index, value) {
					if (value.Field) {
						$.each($scope.forms, function(index, form) {
							if (form && !(typeof form === 'boolean')) {
								if (form[value.Field] != undefined) {
									form[value.Field].$setValidity("server", false);
									if (formForShowing == null) {
										formForShowing = index;
									}
									return false;
								}
							}
						});
					}
					messages += value.Message + "<br />";
				});

				if (formForShowing) {
					activateTab(formForShowing);
				}
			}
			toaster.pop('error', "Error!", messages, null, 'trustedHtml');
		}
	};

	function errorHandler(result) {
		toaster.pop('error', "Error!", "Server error occured");
	};

	function updateCustomerNoteUIId() {
		angular.forEach($scope.currentCustomer.CustomerNotes, function (customerNote, index) {
			customerNote.UIId = index;
		});
	};

	function setCountryValidity() {
		$.each($scope.forms, function (index, form) {
			if (form && !(typeof form === 'boolean')) {
				if (form.Country && form.Country.$viewValue && form.Country.$viewValue.Id == 0) {
					form.Country.$setValidity("required", false);
				}
				if (form.State && form.State.$viewValue == 0) {
					form.State.$setValidity("required", false);
				}
			}
		});
	};

	function syncDefaultPaymentMethod() {
		if (!$scope.currentCustomer.ApprovedPaymentMethods || $scope.currentCustomer.ApprovedPaymentMethods.length == 0) {
			return;
		}

		if (!$scope.selectedPaymentMethods) {
			$scope.selectedPaymentMethods = [];
		}

		angular.forEach($scope.currentCustomer.ApprovedPaymentMethods, function(approvedPM) {
			$scope.selectedPaymentMethods.push({ Id: approvedPM, Name: $.grep($scope.paymentMethods, function(pm) {
				return pm.Id == approvedPM;
			})[0].Name });
		});
	};

	$scope.setNewBillingAddress = function () {
	    customerService.createAddressPrototype($scope.addEditTracker)
            .success(function (result) {
                if (result.Success) {
                    $scope.paymentInfoTab.sameBilling = false;
                    result.Data.AddressType = 2;
                    $scope.paymentInfoTab.Address = result.Data;
                } else {
                    successHandler(result);
                }
            }).
            error(function (result) {
                toaster.pop('error', "Error!", "Server error ocurred");
            })
            .then(function () {
                $scope.forms.submitted['billing'] = false;
            });
	    return false;
	};

	$scope.setNewCreditCard = function (callback) {
	    customerService.createCreditCardPrototype($scope.addEditTracker)
            .success(function (result) {
                if (result.Success) {
                    $scope.currentCustomer.CreditCards.push(result.Data);
                    $scope.paymentInfoTab.CreditCard = result.Data;
                    if (callback)
                        callback(result.Data);
                } else {
                    successHandler(result);
                }
            }).
            error(function (result) {
                toaster.pop('error', "Error!", "Server error ocurred");
            })
            .then(function () {
                $scope.forms.submitted['billing'] = false;
            });
	    return false;
	};

	$scope.setNewCheck = function (callback) {
	    customerService.createCheckPrototype($scope.addEditTracker)
            .success(function (result) {
                if (result.Success) {
                    $scope.currentCustomer.Check = result.Data;
                    if (callback)
                        callback(result.Data);
                } else {
                    successHandler(result);
                }
            }).
            error(function (result) {
                toaster.pop('error', "Error!", "Server error ocurred");
            })
            .then(function () {
                $scope.forms.submitted['billing'] = false;
            });
	    return false;
	};

	$scope.setNewOac = function (callback) {
	    customerService.createOacPrototype($scope.addEditTracker)
            .success(function (result) {
                if (result.Success) {
                    $scope.currentCustomer.Oac = result.Data;
                    if (callback)
                        callback(result.Data);
                } else {
                    successHandler(result);
                }
            }).
            error(function (result) {
                toaster.pop('error', "Error!", "Server error ocurred");
            })
            .then(function () {
                $scope.forms.submitted['billing'] = false;
            });
	    return false;
	};

	$scope.switchPaymentMethodType = function () {
	    switch ($scope.paymentInfoTab.PaymentMethodType) {
	        //CC
	        case "1":
	            if ($scope.currentCustomer.CreditCards && $scope.currentCustomer.CreditCards[0] && $scope.currentCustomer.CreditCards[0].Address) {
	                $scope.paymentInfoTab.sameBilling = false;
	                $scope.paymentInfoTab.Address = $scope.currentCustomer.CreditCards[0].Address;
	            }
	            else {
	                $scope.setNewCreditCard(function (result) {
	                    $scope.paymentInfoTab.sameBilling = false;
	                    $scope.paymentInfoTab.Address = result.Address;
	                });
	            }
	            break;

	            //OAC
	        case "2":
	            if ($scope.currentCustomer.Oac && $scope.currentCustomer.Oac.Address) {
	                $scope.paymentInfoTab.sameBilling = false;
	                $scope.paymentInfoTab.Address = $scope.currentCustomer.Oac.Address;
	            }
	            else {
	                $scope.setNewOac(function (result) {
	                    $scope.paymentInfoTab.sameBilling = false;
	                    $scope.paymentInfoTab.Address = result.Address;
	                });
	            }
	            break;

	            //Check
	        case "3":
	            if ($scope.currentCustomer.Check && $scope.currentCustomer.Check.Address)
	            {
	                $scope.paymentInfoTab.sameBilling = false;
	                $scope.paymentInfoTab.Address = $scope.currentCustomer.Check.Address;
	            }
	            else {
	                $scope.setNewCheck(function (result) {
	                    $scope.paymentInfoTab.sameBilling = false;
	                    $scope.paymentInfoTab.Address = result.Address;
	                });
	            }
	            break;
	    }
	};

	$scope.save = function () {
		clearServerValidation();

		var valid = true;

		setCountryValidity();

		$.each($scope.forms, function (index, form) {
			if (form && !(typeof form === 'boolean')) {
				if (!form.$valid && index != 'submitted') {
					valid = false;
					activateTab(index);
					return false;
				}
			}
		});

		if (valid) {
			if ($scope.currentCustomer.newEmail || $scope.currentCustomer.emailConfirm) {
				$scope.currentCustomer.Email = $scope.currentCustomer.newEmail;
				//$scope.currentCustomer.ProfileAddress.Email = $scope.newEmail;
				$scope.currentCustomer.EmailConfirm = $scope.currentCustomer.emailConfirm;
			} else {
				$scope.currentCustomer.EmailConfirm = $scope.currentCustomer.Email;
			}

			if (!$scope.editMode) {
				$scope.currentCustomer.Shipping[0].Default = true;
			}

			$scope.saving = true;
			customerService.createUpdateCustomer($scope.currentCustomer, $scope.addEditTracker).success(function (result) {
					successHandler(result);
				}).
				error(function(result) {
					errorHandler(result);
				});
		} else {
		    $scope.forms.submitted['profile'] = true;
		    $scope.forms.submitted['shipping'] = true;
		    $scope.forms.submitted['customerNote'] = true;
		    $scope.forms.submitted['billing'] = true;
		}
	};

	$scope.togglePaymentMethodSelection = function (paymentMethod) {
		if (!$scope.currentCustomer.ApprovedPaymentMethods || $scope.currentCustomer.ApprovedPaymentMethods.length == 0) {
			$scope.currentCustomer.ApprovedPaymentMethods = [];
		}

		if (!$scope.selectedPaymentMethods) {
			$scope.selectedPaymentMethods = [];
		}

		var idx = -1;
		$.grep($scope.selectedPaymentMethods, function (elem, index) {
			if (elem.Id == paymentMethod.Id) {
				idx = index;
				return;
			}
		});

		if (idx > -1) {
			$scope.currentCustomer.ApprovedPaymentMethods.splice(idx, 1);
			$scope.selectedPaymentMethods.splice(idx, 1);
		}
		else {
			$scope.selectedPaymentMethods.push({ Id: paymentMethod.Id, Name: paymentMethod.Name });
			$scope.currentCustomer.ApprovedPaymentMethods.push(paymentMethod.Id);
		}
	};

	$scope.toggleOrderNoteSelection = function (orderNoteId) {
		if (!$scope.currentCustomer.OrderNotes) {
			$scope.currentCustomer.OrderNotes = [];
		}

		var idx = $scope.currentCustomer.OrderNotes.indexOf(orderNoteId);

		if (idx > -1) {
			$scope.currentCustomer.OrderNotes.splice(idx, 1);
		}
		else {
			$scope.currentCustomer.OrderNotes.push(orderNoteId);
		}
	};

	$scope.makeAsProfileAddress = function() {
	    if ($scope.currentCustomer.sameShipping) {
	        var defaultValue = $scope.shippingAddressTab.Address.Default;
		    for (var key in $scope.currentCustomer.ProfileAddress) {
				$scope.shippingAddressTab.Address[key] = $scope.currentCustomer.ProfileAddress[key];
			}
			if ($scope.currentCustomer.newEmail) {
				$scope.shippingAddressTab.Address.Email = $scope.currentCustomer.newEmail;
			} else {
				$scope.shippingAddressTab.Address.Email = $scope.currentCustomer.Email;
			}
			$scope.shippingAddressTab.Address.Default = defaultValue;
			$scope.shippingAddressTab.Address.AddressType = 3;
			$scope.shippingAddressTab.Address.Id = 0;
		}
	};

	$scope.makeBillingAsProfileAddress = function () {
	    if ($scope.paymentInfoTab.sameBilling) {
	        for (var key in $scope.currentCustomer.ProfileAddress) {
	            $scope.paymentInfoTab.Address[key] = $scope.currentCustomer.ProfileAddress[key];
	        }
	        if ($scope.currentCustomer.newEmail) {
	            $scope.paymentInfoTab.Address.Email = $scope.currentCustomer.newEmail;
	        } else {
	            $scope.paymentInfoTab.Address.Email = $scope.currentCustomer.Email;
	        }
	        $scope.paymentInfoTab.Address.AddressType = 2;
	        $scope.paymentInfoTab.Address.Id = 0;
	    }
	};

	function deleteShippingAddressLocal(id) {
	    var idx = -1;

	    angular.forEach($scope.currentCustomer.Shipping, function (item, index) {
	        if (item.Id == id) {
	            idx = index;
	            return;
	        }
	    });

	    $scope.currentCustomer.Shipping.splice(idx, 1);
	    if (idx < $scope.currentCustomer.Shipping.length) {
	        $scope.shippingAddressTab.Address = $scope.currentCustomer.Shipping[idx];
	    }
	    else if ($scope.currentCustomer.Shipping.length > 0) {
	        $scope.shippingAddressTab.Address = $scope.currentCustomer.Shipping[0];
	    }
	    else {
	        $scope.setNewAddress();
	    }
	}

	$scope.deleteSelectedCreditCard = function (id) {
	    if ($scope.editMode) {
	        customerService.deleteCreditCard(id, $scope.addEditTracker)
                .success(function (result) {
                    if (result.Success) {
                        deleteShippingAddressLocal(id);
                        toaster.pop('success', "Success!", "Shipping Address was succesfully deleted");
                    }
                    else {
                        successHandler(result);
                        //toaster.pop('error', 'Error!', "Can't delete shipping address");
                    }
                })
                .error(function (result) {
                    toaster.pop('error', "Error!", "Server error ocurred");
                });
	    }
	    else {
	        deleteShippingAddressLocal(id);
	        toaster.pop('success', "Success!", "Shipping Address was succesfully deleted");
	    }
	}

	$scope.deleteSelectedShipping = function (id) {
	    if ($scope.editMode) {
	        customerService.deleteAddress(id, $scope.addEditTracker)
                .success(function (result) {
                    if (result.Success) {
                        deleteShippingAddressLocal(id);
                        toaster.pop('success', "Success!", "Shipping Address was succesfully deleted");
                    }
                    else {
                        successHandler(result);
                        //toaster.pop('error', 'Error!', "Can't delete shipping address");
                    }
                })
                .error(function (result) {
                    toaster.pop('error', "Error!", "Server error ocurred");
                });
	    }
	    else {
	        deleteShippingAddressLocal(id);
	        toaster.pop('success', "Success!", "Shipping Address was succesfully deleted");
	    }
	};

	function deleteCustomerNoteLocal(id) {
	    var idx = -1;

	    angular.forEach($scope.currentCustomer.CustomerNotes, function (item, index) {
	        if (item.Id == id) {
	            idx = index;
	            return;
	        }
	    });

	    $scope.currentCustomer.CustomerNotes.splice(idx, 1);
	}

	$scope.deleteCustomerNote = function(id) {
	    if ($scope.editMode) {
	        customerService.deleteNote(id, $scope.addEditTracker)
                .success(function (result) {
                    if (result.Success) {
                        deleteCustomerNoteLocal(id);
                        toaster.pop('success', "Success!", "Customer Note was succesfully deleted");
                    }
                    else {
                        successHandler(result);
                        //toaster.pop('error', 'Error!', "Can't delete customer note");
                    }
                })
                .error(function (result) {
                    toaster.pop('error', "Error!", "Server error ocurred");
                });
	    }
	    else {
	        deleteCustomerNoteLocal(id);
	        toaster.pop('success', "Success!", "Customer Note was succesfully deleted");
	    }
	};

	$scope.addNewCustomerNote = function () {
	    clearServerValidation();

	    if ($scope.forms.customerNote.$valid) {
	        if ($scope.editMode) {
	            customerService.addNote($scope.customerNotesTab.CustomerNote, $scope.currentCustomer.Id, $scope.addEditTracker)
                    .success(function (result) {
                        if (result.Success) {
                            $scope.currentCustomer.CustomerNotes.push(result.Data);
                            updateCustomerNoteUIId();
                            createCustomerNoteProto();
                            toaster.pop('success', "Success!", "Customer Note was succesfully added");
                        }
                        else {
                            successHandler(result);
                            //toaster.pop('error', 'Error!', "Can't add Customer Note");
                        }
                    })
                    .error(function (result) {
                        toaster.pop('error', "Error!", "Server error ocurred");
                    });
	        }
	        else {
	            $scope.currentCustomer.CustomerNotes.push(angular.copy($scope.customerNotesTab.CustomerNote));
	            updateCustomerNoteUIId();
	            createCustomerNoteProto();
	        }
	    } else {
	        $scope.forms.submitted['customerNote'] = true;
	    }
	};

	$scope.cancelAddNewShipping = function () {
	    $scope.shippingAddressTab.Address = $scope.currentCustomer.Shipping[0];
	    $scope.shippingAddressTab.NewAddress = false;
	};

	$scope.addNewShipping = function () {
	    clearServerValidation();

	    setCountryValidity();

	    if ($scope.forms.shipping.$valid) {
	        if ($scope.editMode) {
	            customerService.addAddress($scope.shippingAddressTab.Address, $scope.currentCustomer.Id, $scope.addEditTracker).success(function (result) {
	                if (result.Success) {
	                    syncCountry(result.Data);
	                    $scope.currentCustomer.Shipping.push(result.Data);
	                    $scope.shippingAddressTab.Address = result.Data;
	                    $scope.shippingAddressTab.NewAddress = false;
	                    toaster.pop('success', "Success!", "Customer address was succesfully added");
	                }
	                else {
	                    successHandler(result);
	                    //toaster.pop('error', 'Error!', "Can't add shipping address");
	                }
	            }).
                error(function (result) {
                    toaster.pop('error', "Error!", "Server error ocurred");
                });
	        }
	        else {
	            var newAddress = angular.copy($scope.shippingAddressTab.Address);
	            syncCountry(newAddress);
	            $scope.currentCustomer.Shipping.push(newAddress);
	            $scope.shippingAddressTab.NewAddress = false;
	        }
	    } else {
	        $scope.forms.submitted['shipping'] = true;
	    }
	};

	$scope.setNewAddress = function () {
	    customerService.createAddressPrototype($scope.addEditTracker)
            .success(function (result) {
                if (result.Success) {
                    $scope.currentCustomer.sameShipping = false;
                    $scope.shippingAddressTab.NewAddress = true;
                    $scope.shippingAddressTab.Address = result.Data;
                } else {
                    successHandler(result);
                    //toaster.pop('error', 'Error!', "Can't add shipping address");
                }
            }).
            error(function (result) {
                toaster.pop('error', "Error!", "Server error ocurred");
            })
            .then(function () {
                $scope.forms.submitted['shipping'] = false;
            });
	    return false;
	};

	$scope.setDefaultAddress = function(id) {
		angular.forEach($scope.currentCustomer.Shipping, function(shippingItem) {
		    if (shippingItem.Id != id && shippingItem.Default) {
				shippingItem.Default = false;
			}
		    else if (shippingItem.Id == id)
			{
			    shippingItem.Default = true;
			}
		});
	};


	initialize();
}]);