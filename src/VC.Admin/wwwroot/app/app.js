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
		'$stateProvider', '$urlRouterProvider','$httpProvider',
		function ($stateProvider, $urlRouterProvider, $httpProvider) {
		    $urlRouterProvider.otherwise('/404');

		    $httpProvider.interceptors.push('$q', '$location', 'toaster', function ($q, $location, toaster) {
		        return {
		            'responseError': function (response) {
		                switch (response.status) {
		                    case 401: {
		                        $location.path("/authentication/login");
		                        return $q.defer().promise;
		                    } break;
		                    case 403: {
		                        //$state.go($state.previous != null && $state.previous.name !== 'index.oneCol.login' && $state.previous.name !== '' ? $state.previous.name : "index.oneCol.dashboard");

		                        toaster.pop('warning', "Caution!", "Sorry, but you do not have the appropriate permissions to access this area.");
		                        return $q.defer().promise;
		                    } break;
		                }
		                return $q.reject(response);
		            }
		        };
		    });
		}
	])
	.run([
		'$rootScope', '$state', '$stateParams', 'ngProgress', '$timeout', 'appBootstrap',
		function ($rootScope, $state, $stateParams, ngProgress, $timeout, appBootstrap) {
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