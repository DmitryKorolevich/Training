'use strict';

angular.module('app.modules.setting.controllers.logDetailsController', [])
.controller('logDetailsController', ['$scope', '$modalInstance', 'data', 'toaster', function ($scope, $modalInstance, data, toaster) {

	function initialize() {
		$scope.item = data.item;

		$scope.cancel = function () {
			$modalInstance.close();
		};
	}

	initialize();
}]);