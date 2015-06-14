angular.module('app.modules.setting.controllers.paymentMethodsController', [])
.controller('paymentMethodsController', ['$scope', 'promiseTracker', 'paymentMethodService', 'toaster', '$rootScope', function ($scope, promiseTracker, paymentMethodService, toaster, $rootScope) {
		$scope.refreshTracker = promiseTracker("refresh");

		function initialize() {
			refreshPaymentMethods();
		};

		function refreshPaymentMethods() {
			paymentMethodService.getPaymentMethods($scope.refreshTracker)
				.success(function (result) {
					if (result.Success) {
						$scope.paymentMethods = result.Data;

						applyMatrixState();

					} else {
						toaster.pop('error', 'Error!', "Can't get access to the approved payment methods");
					}
				})
				.error(function (result) {
					toaster.pop('error', "Error!", "Server error ocurred");
				});
		};

		function isApplicableMethod(paymentMethod, customerType) {
			return paymentMethod.CustomerTypes && paymentMethod.CustomerTypes.indexOf(customerType) > -1;
		};

		function updateAvailability(paymentMethod, propertyName, customerType) {
			if (!paymentMethod[propertyName]) {
				if (isApplicableMethod(paymentMethod, customerType)) {
					paymentMethod.CustomerTypes.splice(paymentMethod.CustomerTypes.indexOf(customerType), 1);
				}
			} else {
				if (!isApplicableMethod(paymentMethod, customerType)) {
					paymentMethod.CustomerTypes.push(customerType);
				}
			}
		};

		function applyMatrixState() {
			angular.forEach($scope.paymentMethods, function (paymentMethod) {
				if (isApplicableMethod(paymentMethod, 1)) {
					paymentMethod.RetailAvailable = true;
				}
				if (isApplicableMethod(paymentMethod, 2)) {
					paymentMethod.WholesaleAvailable = true;
				}
			});
		};

		function syncMatrixState() {
			angular.forEach($scope.paymentMethods, function(paymentMethod) {
				updateAvailability(paymentMethod, 'RetailAvailable', 1);
				updateAvailability(paymentMethod, 'WholesaleAvailable', 2);
			});
		};

		$scope.setState = function () {
			syncMatrixState();

			paymentMethodService.setState($scope.paymentMethods, $scope.refreshTracker)
				.success(function(result) {
					if (result.Success) {
						toaster.pop('success', "Success!", "Successfully saved");
					} else {
						var messages = "";
						if (result.Messages) {
							$.each(result.Messages, function (index, value) {
								messages += value.Message + "<br />";
							});
						}
						toaster.pop('error', "Error!", messages, null, 'trustedHtml');
					}

					refreshPaymentMethods();
				}).error(function() {
					toaster.pop('error', "Error!", "Server error occured");

					refreshPaymentMethods();
				})
		};

		$scope.retailApplicable = function(item) {
			return isApplicableMethod(item, 1);
		};

		$scope.wholesaleApplicable = function (item) {
			return isApplicableMethod(item, 2);
		};

		initialize();
}]);