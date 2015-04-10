'use strict';

var app = angular
	.module('mainApp', [
		'ui.router',
		'ui.bootstrap',
		'app.core',
		'app.modules',
		'app.shared'
	])
	.config([
		'$stateProvider', '$urlRouterProvider',
		function($stateProvider, $urlRouterProvider) {
			$urlRouterProvider.otherwise('app/shared/partials/404.html');
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
		'$rootScope', '$state', '$stateParams', 'ngProgress',
		function($rootScope, $state, $stateParams, ngProgress) {
			$rootScope.$state = $state;
			$rootScope.$stateParams = $stateParams;

			$rootScope.authenticated = true; //temp solution

			$rootScope.$on('$stateChangeStart', function() {
				ngProgress.start();
			});
			$rootScope.$on('$stateChangeSuccess', function() {
				ngProgress.complete();
			});
			$rootScope.$on('$stateChangeError', function() {
				ngProgress.complete();
			});
			$rootScope.$on('$stateNotFound', function() {
				ngProgress.complete();
			});
		}
	]);