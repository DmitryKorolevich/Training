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
					} else {
						toaster.pop('error', 'Error!', "Can't get access to the approved payment methods");
					}
				})
				.error(function (result) {
					toaster.pop('error', "Error!", "Server error ocurred");
				});
		};

		function isApplicableMethod(item, key) {
			return paymentMethod.CustomerTypes && paymentMethod.CustomerTypes.indexOf(customerType) > -1;
		};

		function updateAvailability(paymentMethod, customerType) {
			var index = isApplicableMethod(customerType);
			if (index > -1) {
				paymentMethod.CustomerTypes.splice(idx, 1);
			} else {
				paymentMethod.CustomerTypes.push(customerType);
			}
		};

		$scope.setState = function() {
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

		$scope.updateRetail = function(paymentMethod) {
			updateAvailability(paymentMethod, 1);
		};

		$scope.updateWholesale = function (paymentMethod) {
			updateAvailability(paymentMethod, 1);
		};

		initialize();
}]);