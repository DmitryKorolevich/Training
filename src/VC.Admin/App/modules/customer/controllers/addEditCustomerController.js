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
								$scope.SelectedShipping = $scope.shippingAddressTab.Address;
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
								$scope.shippingAddressTab.Address = $scope.SelectedShipping = $scope.currentCustomer.Shipping[0];
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

	$scope.save = function () {
		clearServerValidation();

		var valid = true;
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
				$scope.SelectedShipping[key] = $scope.currentCustomer.ProfileAddress[key];
			}
			$scope.SelectedShipping.Email = $scope.currentCustomer.Email;
			$scope.SelectedShipping.AddressType = 3;
		}
	};

	$scope.deleteSelectedShipping = function (id) {
		var idx = -1;

		angular.forEach($scope.currentCustomer.Shipping, function (item, index) {
			if (item.Id == id) {
				idx = index;
				return;
			}
		});

		$scope.currentCustomer.Shipping.splice(idx, 1);
	};

	$scope.addNewShipping = function () {
		customerService.createAddressPrototype($scope.addEditTracker)
						.success(function (result) {
							if (result.Success) {
								$scope.currentCustomer.Shipping.push(result.Data);
							} else {
								toaster.pop('error', 'Error!', "Can't add shipping address");
							}
						}).
						error(function (result) {
							toaster.pop('error', "Error!", "Server error ocurred");
						});
	};


	initialize();
}]);