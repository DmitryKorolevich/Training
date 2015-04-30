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
	.run([
		'$rootScope', '$state', '$stateParams', 'ngProgress', '$timeout', 'appBootstrap',
		function($rootScope, $state, $stateParams, ngProgress, $timeout, appBootstrap) {
			$rootScope.$on('$stateChangeStart', function() {
				ngProgress.start();
			});
			$rootScope.$on('$stateChangeSuccess', function(event, toState, toParams, fromState) {
				ngProgress.complete();
				$state.previous = fromState;
			});
			$rootScope.$on('$stateChangeError', function() {
				ngProgress.complete();
			});
			$rootScope.$on('$stateNotFound', function() {
				ngProgress.complete();
			});

			function initialize() {
				$rootScope.$state = $state;
				$rootScope.$stateParams = $stateParams;

				appBootstrap.initialize();
			};

			initialize();
		}
	]);