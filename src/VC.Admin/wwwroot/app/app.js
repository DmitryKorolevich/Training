'use strict';

var app = angular
	.module('mainApp', [
		'templates',
		'ui.router',
		'ui.bootstrap',
		'app.core',
		'app.modules',
		'app.shared'
	])
	.config([
		'$stateProvider', '$urlRouterProvider','$httpProvider',
		function ($stateProvider, $urlRouterProvider, $httpProvider) {
			$urlRouterProvider.when('/',[ '$state', function($state) {
				$state.go('index.oneCol.dashboard');
			}]);
			$urlRouterProvider.otherwise('/404');

			$httpProvider.interceptors.push("httpInterceptor");
		}
	])
	.run([
		'$rootScope', '$state', '$stateParams', 'appBootstrap',
		function ($rootScope, $state, $stateParams, appBootstrap) {
		    function initialize() {
				$rootScope.$state = $state;
				$rootScope.$stateParams = $stateParams;

				appBootstrap.initialize();
			};

			initialize();
		}
	]);