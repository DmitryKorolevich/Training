'use strict';

var app = angular
	.module('mainApp', [
		'ui.router',
		'app.core',
		'app.modules',
		'app.shared'
	])
	.config([
		'$stateProvider', '$urlRouterProvider',
		function($stateProvider, $urlRouterProvider) {

			$urlRouterProvider.otherwise('app/shared/partials/404.html');

			$stateProvider
				.state('index', {
					url: '',
					views: {
						'': {
							templateUrl: 'app/shared/layout/partials/index.html',
							controller: 'indexController'
						},
						'topMenu@index': {
							templateUrl: 'app/shared/menu/partials/mainMenu.html',
							controller: 'mainNavigationController'
						},
						'sidebar@index': {
							templateUrl: 'app/shared/menu/partials/sidebar.html',
							controller: 'sidebarController'
						},
						'workingPanel@index': {
							templateUrl: 'app/shared/area/partials/workingPanel.html',
							controller: 'workingPanelController'
						}
					}
				});
		}
	])
//.controller('IndexCtrl', ['$scope', function ($scope) {
//	$scope.Model = {
//		categories: ["Music", "Video"],
//	}
//}])
//.controller('ListCtrl', ['$scope', '$stateParams', function ($scope, $stateParams) {
//	// add items to the model
//	$scope.Model.items = $stateParams.category === "Music"
//	  ? ["Depeche Mode", "Nick Cave"]
//	  : ["Star Wars", "Psycho"]
//	$scope.$on("$destroy", function () { delete $scope.Model.items; })
//}])
//.controller('DetailCtrl', ['$scope', '$stateParams', function ($scope, $stateParams) {
//	// add item to model
//	$scope.Model = {
//		item: $scope.Model.items[$stateParams.id],
//	}
//	$scope.$on("$destroy", function () { delete $scope.Model.item; })
//}])
.run([
		'$rootScope', '$state', '$stateParams',
		function($rootScope, $state, $stateParams) {
			$rootScope.$state = $state;
			$rootScope.$stateParams = $stateParams;
		}
	]);