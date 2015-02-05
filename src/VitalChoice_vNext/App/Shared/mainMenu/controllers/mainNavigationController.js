'use strict';

angular.module('app.shared.mainMenu.controllers.mainNavigationController', [
    'app.shared.mainMenu.services.mainNavigationService'
])
.controller('mainNavigationController', ['$scope', 'mainNavigationFactory', function ($scope, mainNavigationFactory) {
	$scope.mainNavigationService = mainNavigationFactory;
	/*$scope.navigation = {
		collection: primaryNavigation.getList('order'),
		current: null
	};

	$scope.$on('$routeChangeSuccess', function (scope, current, previous) {
		if (!current || !current.$$route || !current.$$route.name) {
			$scope.navigation.current = 'dashboard';
			return;
		}
		$scope.navigation.current = current.$$route.name;
	});*/

}]);