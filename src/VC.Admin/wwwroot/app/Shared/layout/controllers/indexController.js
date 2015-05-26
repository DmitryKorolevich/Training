'use strict';

angular.module('app.shared.layout.controllers.indexController', [])
.controller('indexController', ['$scope', 'navigationFactory', function ($scope, navigationFactory) {
	function init() {
		$scope.navigation = navigationFactory;
	}

	init();
}]);