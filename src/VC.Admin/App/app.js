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
		'$stateProvider', '$urlRouterProvider','$httpProvider', '$locationProvider',
		function ($stateProvider, $urlRouterProvider, $httpProvider, $locationProvider) {
			$urlRouterProvider.when('/',[ '$state', function($state) {
				$state.go('index.oneCol.dashboard');
			}]);
			$urlRouterProvider.otherwise('/404');

			$httpProvider.interceptors.push("httpInterceptor");

			$locationProvider.html5Mode(true);
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