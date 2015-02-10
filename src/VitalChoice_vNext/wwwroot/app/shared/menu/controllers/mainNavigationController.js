'use strict';

angular.module('app.shared.menu.controllers.mainNavigationController', [])
.controller('mainNavigationController', ['$scope', 'navigationFactory', function ($scope, navigationFactory) {
	$scope.mainNavigationService = navigationFactory;
	alert("menu");

}]);