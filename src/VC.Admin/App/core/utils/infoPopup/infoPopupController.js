'use strict';

angular.module('app.core.utils.infoPopup.infoPopupController', [])
.controller('infoPopupController', ['$scope', '$modalInstance', 'data', function ($scope, $modalInstance, data) {
	function initialize() {
		$scope.text = data.text;
		$scope.title = data.title;
	};

	$scope.ok = function () {
		if (data.okHandler) {
			data.okHandler();
		}
		$modalInstance.close(false);
	};

	initialize();
}]);