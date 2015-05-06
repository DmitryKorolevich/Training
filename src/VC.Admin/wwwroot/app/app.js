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

		    $httpProvider.interceptors.push('$q', '$location', 'toaster', '$rootScope', function ($q, $location, toaster, $rootScope) {
		        return {
		            'responseError': function (response) {
		                switch (response.status) {
		                    case 401: {
		                        $rootScope.$state.go('index.oneCol.login');

		                        toaster.pop('warning', "Caution!", "You must login before accessing this area.");
		                        return $q.defer().promise;

		                    } break;
		                    case 403: {
		                        $rootScope.$state.go(
                                    $rootScope.$state.previous != null
                                    && $rootScope.$state.previous.name !== ''
                                    && !$rootScope.unauthorizedArea($rootScope.$state.href($rootScope.$state.previous).slice(1))
                                    ? $rootScope.$state.previous.name : "index.oneCol.dashboard");

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