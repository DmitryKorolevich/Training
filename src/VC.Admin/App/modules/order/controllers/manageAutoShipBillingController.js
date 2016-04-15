'use strict';

angular.module('app.modules.order.controllers.manageAutoShipBillingController', [])
.controller('manageAutoShipBillingController', ['$scope', '$uibModalInstance', 'data', 'orderService', 'toaster', 'promiseTracker', '$rootScope','customerEditService', function ($scope, $uibModalInstance, data, orderService, toaster, promiseTracker, $rootScope, customerEditService) {
	$scope.saveTracker = promiseTracker("save");
	$scope.resendTracker = promiseTracker("resend");
	$scope.resetTracker = promiseTracker("reset");

	function successHandler(result) {
		if (result.Success) {
			toaster.pop('success', "Success!", "Successfully saved");
			$uibModalInstance.close();

		} else {
			var messages = "";
			if (result.Messages) {
				$scope.forms.submitted["creditCard"] = true;
				$scope.serverMessages = new ServerMessages(result.Messages);
				$.each(result.Messages, function(index, value) {
					if (value.Field && $scope.forms.creditCard[value.Field]) {
						$scope.forms.creditCard[value.Field].$setValidity("server", false);
					}
					messages += value.Message + "<br />";
				});
			}
			toaster.pop('error', "Error!", messages, null, 'trustedHtml');
		}
		data.thenCallback();
	};

	function errorHandler(result) {
		toaster.pop('error', "Error!", "Server error occured");
		data.thenCallback();
	};

	function initialize() {
		$scope.forms = { submitted: [] };

		customerEditService.initBase($scope);

		$scope.countries = data.countries;
		$scope.creditCards = data.creditCards;
		$scope.orderId = data.orderId;
		$scope.creditCard = $.grep($scope.creditCards, function(element) {
			return element.IsSelected;
		})[0];

		$scope.creditCardTypes = $rootScope.ReferenceData.CreditCardTypes;

		angular.forEach($scope.creditCards, function (cc, index) {
			cc.formName = "creditCard";
			customerEditService.syncCountry($scope, cc.Address);
		});
	}

	$scope.save = function() {
		$.each($scope.forms.creditCard, function(index, element) {
			if (element && element.$name == index) {
				element.$setValidity("server", true);
			}
		});

		if ($scope.forms.creditCard.$valid) {
			$scope.saving = true;
			orderService.updateAutoShipBilling($scope.creditCard,$scope.orderId, $scope.saveTracker).success(function(result) {
					successHandler(result);
				}).
				error(function(result) {
					errorHandler(result);
				});
		} else {
			$scope.forms.submitted["creditCard"] = true;
		}
	};


	$scope.cancel = function () {
		$uibModalInstance.close();
	};

	initialize();
}]);