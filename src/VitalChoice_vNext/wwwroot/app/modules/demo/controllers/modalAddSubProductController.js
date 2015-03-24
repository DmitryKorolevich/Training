'use strict';

angular.module('app.modules.demo.controllers.modalAddSubProductController', [])
.controller('modalAddSubProductController', ['$scope', '$modalInstance', 'items', function ($scope, $modalInstance, items) {
	$scope.items = items;

	$scope.ok = function () {
		$modalInstance.dismiss('cancel');
	};

	$scope.cancel = function () {
		$modalInstance.dismiss('cancel');
	};
}]);