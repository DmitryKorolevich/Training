'use strict';

angular.module('app.modules.demo.controllers.modalAddSubProductController', [])
.controller('modalAddSubProductController', ['$scope', '$uibModalInstance', 'items', function ($scope, $uibModalInstance, items) {
	$scope.items = items;

	$scope.ok = function () {
		$uibModalInstance.close();
	};

	$scope.cancel = function () {
		$uibModalInstance.close();
	};
}]);