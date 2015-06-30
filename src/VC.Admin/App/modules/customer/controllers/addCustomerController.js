'use strict';

angular.module('app.modules.customer.controllers.addCustomerController', [])
.controller('addCustomerController', ['$scope', 'customerService', 'toaster', 'promiseTracker', '$rootScope', '$q', function ($scope, customerService, toaster, promiseTracker, $rootScope, $q) {
	$scope.addTracker = promiseTracker("add");

	function initialize() {
		$scope.inceptionDateOpened = false;

		$scope.currentCustomer = { CustomerType: 1};

		$scope.accountProfileTab = { active: true };
		$scope.shippingAddressTab = { active: false };
		$scope.customerNotesTab = { active: false };

		$q.all({ countriesCall: customerService.getCountries($scope.addTracker), paymentMethodsCall: customerService.getPaymentMethods($scope.currentCustomer.CustomerType, $scope.addTracker), orderNotesCall: customerService.getOrderNotes($scope.currentCustomer.CustomerType, $scope.addTracker) }).then(function (result) {
			if (result.countriesCall.data.Success && result.paymentMethodsCall.data.Success && result.orderNotesCall.data.Success) {
				$scope.countries = result.countriesCall.data.Data;
				$scope.paymentMethods = result.paymentMethodsCall.data.Data;
				$scope.orderNotes = result.orderNotesCall.data.Data;
				customerService.createCustomerPrototype($scope.addTracker)
					.success(function (result) {
						if (result.Success) {
							$scope.currentCustomer = result.Data;
						} else {
							toaster.pop('error', 'Error!', "Can't create customer");
						}
					}).
					error(function (result) {
						toaster.pop('error', "Error!", "Server error ocurred");
					});
			} else {
				toaster.pop('error', 'Error!', "Can't get reference data");
			}
		}, function(result) {
			toaster.pop('error', "Error!", "Server error ocurred");
		});
	};

	function successHandler(result) {
		if (result.Success) {
			toaster.pop('success', "Success!", "Successfully saved");
		} else {
			var messages = "";
			if (result.Messages) {
				$scope.customerForm.submitted = true;
				$scope.serverMessages = new ServerMessages(result.Messages);
				$.each(result.Messages, function (index, value) {
					if (value.Field && $scope.customerForm[value.Field.toLowerCase()]) {
						$scope.customerForm[value.Field.toLowerCase()].$setValidity("server", false);
					}
					messages += value.Message + "<br />";
				});
			}
			toaster.pop('error', "Error!", messages, null, 'trustedHtml');
		}
	};

	function errorHandler(result) {
		toaster.pop('error', "Error!", "Server error occured");
	};

	$scope.save = function () {
		$.each($scope.customerForm, function (index, element) {
			if (element && element.$name == index) {
				element.$setValidity("server", true);
			}
		});

		if ($scope.customerForm.$valid) {
			$scope.saving = true;

			customerService.createCustomer($scope.currentCustomer, $scope.addTracker).success(function (result) {
					successHandler(result);
				}).
				error(function(result) {
					errorHandler(result);
				});
		} else {
			$scope.customerForm.submitted = true;
		}
	};

	$scope.togglePaymentMethodSelection = function (paymentMethod) {
		if (!$scope.currentCustomer.PaymentMethods) {
			$scope.currentCustomer.PaymentMethods = [];
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
			$scope.currentCustomer.PaymentMethods.splice(idx, 1);
			$scope.selectedPaymentMethods.splice(idx, 1);
		}
		else {
			$scope.selectedPaymentMethods.push({ Id: paymentMethod.Id, Name: paymentMethod.Name });
			$scope.currentCustomer.PaymentMethods.push(paymentMethod.Id);
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

	initialize();
}]);