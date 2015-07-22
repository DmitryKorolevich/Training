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

		var tabs = [];
		tabs.push($scope.accountProfileTab);
		tabs.push($scope.shippingAddressTab);
		tabs.push($scope.customerNotesTab)
		$scope.tabs = tabs;

		$scope.forms = {};

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
								$scope.shippingAddressTab.Address = $scope.currentCustomer.Shipping[0];

								angular.forEach($scope.currentCustomer.Shipping, function(shippingItem) {
									syncCountry(shippingItem);
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
									    $scope.forms.customerNoteSubmitted = false;
									});
	};

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
				$scope.forms.submitted = true;
				$scope.forms.shippingSubmitted = true;
				$scope.forms.customerNoteSubmitted = true;
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

	$scope.save = function () {
		clearServerValidation();

		//if ($scope.shippingAddressTab.NewAddress) {
		//	addNewShipping();
		//}

		var valid = true;

		setCountryValidity();

		$.each($scope.forms, function (index, form) {
			if (form && !(typeof form === 'boolean')) {
				if (!form.$valid && index != 'submitted' && index != 'shippingSubmitted' && index != 'customerNoteSubmitted') {
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
			$scope.forms.submitted = true;
			$scope.forms.shippingSubmitted = true;
			$scope.forms.customerNoteSubmitted = true;
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
			for (var key in $scope.currentCustomer.ProfileAddress) {
				$scope.shippingAddressTab.Address[key] = $scope.currentCustomer.ProfileAddress[key];
			}
			if ($scope.currentCustomer.newEmail) {
				$scope.shippingAddressTab.Address.Email = $scope.currentCustomer.newEmail;
			} else {
				$scope.shippingAddressTab.Address.Email = $scope.currentCustomer.Email;
			}

			$scope.shippingAddressTab.Address.AddressType = 3;
			$scope.shippingAddressTab.Address.Id = 0;
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
                        toaster.pop('error', 'Error!', "Can't delete shipping address");
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
                        toaster.pop('error', 'Error!', "Can't delete customer note");
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
                            toaster.pop('error', 'Error!', "Can't add Customer Note");
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
	        $scope.forms.customerNoteSubmitted = true;
	    }
	};

	$scope.addNewShipping = function () {
	    clearServerValidation();

	    setCountryValidity();

	    if ($scope.forms.shipping.$valid) {
	        if ($scope.editMode) {
	            var newAddress = angular.copy($scope.shippingAddressTab.Address);
	            customerService.addAddress(newAddress, $scope.currentCustomer.Id, $scope.addEditTracker).success(function (result) {
	                if (result.Success) {
	                    syncCountry(result.Data);
	                    $scope.currentCustomer.Shipping.push(result.Data);
	                    toaster.pop('success', "Success!", "Customer address was succesfully added");
	                }
	                else {
	                    toaster.pop('error', 'Error!', "Can't add shipping address");
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
	        }

	        $scope.shippingAddressTab.NewAddress = false;
	    } else {
	        $scope.forms.shippingSubmitted = true;
	    }
	};

	$scope.setNewAddress = function () {
		if ($scope.shippingAddressTab.NewAddress) {
			customerService.createAddressPrototype($scope.addEditTracker)
				.success(function(result) {
				    if (result.Success) {
				        $scope.currentCustomer.sameShipping = false;
						$scope.shippingAddressTab.Address = result.Data;
					} else {
						toaster.pop('error', 'Error!', "Can't add shipping address");
					}
				}).
				error(function(result) {
					toaster.pop('error', "Error!", "Server error ocurred");
				})
				.then(function() {
					$scope.forms.shippingSubmitted = false;
					//$scope.forms.submitted = false;
					//$scope.forms.customerNoteSubmitted = false;
				});
		} else {
			//$scope.shippingAddressTab.Address = $scope.currentCustomer.Shipping[0];
		}

		return false;
	};

	$scope.setDefaultAddress = function() {
		angular.forEach($scope.currentCustomer.Shipping, function(shippingItem) {
			if (shippingItem != $scope.shippingAddressTab.Address) {
				shippingItem.Default = false;
			}
		});
	};


	initialize();
}]);