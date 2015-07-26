'use strict';

angular.module('app.core.utils.confirmation.confirmController', [])
.controller('confirmController', ['$scope', '$modalInstance', 'data', function ($scope, $modalInstance, data) {
	function initialize() {
		var confirmText = "Are you sure?";
		if (data.confirmText != null) {
			confirmText = data.confirmText;
		}
		$scope.confirmText = confirmText;
		$scope.okHandler = data.okHandler;
		$scope.cancelHandler = data.cancelHandler;
	};

	$scope.continue = function () {
		$scope.okHandler();
		$scope.close();
	};

	$scope.close = function () {
		if ($scope.cancelHandler) {
			$scope.cancelHandler();
		}
		$modalInstance.close(false);
	};

	initialize();
}]);