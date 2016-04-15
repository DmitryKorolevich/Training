'use strict';

angular.module('app.core.utils.infoPopup.infoPopupController', [])
.controller('infoPopupController', ['$scope', '$uibModalInstance', 'data', function ($scope, $uibModalInstance, data) {
	function initialize() {
		$scope.text = data.text;
		$scope.title = data.title;
	};

	$scope.ok = function () {
		if (data.okHandler) {
			data.okHandler();
		}
		$uibModalInstance.close(false);
	};

	initialize();
}]);