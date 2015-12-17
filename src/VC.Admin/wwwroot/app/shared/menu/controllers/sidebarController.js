'use strict';

angular.module('app.shared.menu.controllers.sidebarController', [])
.controller('sidebarController', ['$scope', '$stateParams', 'sidebarFactory', function ($scope, $stateParams, sidebarFactory) {
	function init() {
		$scope.moduleName = $stateParams.name;
		$scope.sidebar = {};

		var sidebar = $.grep(sidebarFactory.sidebars, function(element) {
			return element.moduleName === $scope.moduleName;
		});

		if (sidebar.length > 0) {
			$scope.sidebar = sidebar[0];
		}
	};

	init();
}]);