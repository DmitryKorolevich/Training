'use strict';

angular
	.module('app.core.utils.httpInterceptor', [])
	.factory('httpInterceptor', [
				'$q', 'toaster', '$rootScope', 'dataStateRemediator', function ($q, toaster, $rootScope, dataStateRemediator) {
					return {
						'request': function (config) {
							config.headers['Build-Number'] = $rootScope.buildNumber;
							return config;
						},
						'response': function (response) {
							switch (response.status) {
								case 204:
									{
										dataStateRemediator.runSaveDataScenario(response);
										return $q.defer().promise;
									}
									break;
								case 200:
									{
										dataStateRemediator.runRestoreDataScenario(response);
									}
									break;
							}
							return response;
						},
						'responseError': function (response) {
							switch (response.status) {
								case 401:
									{
										$rootScope.$state.go('index.oneCol.login');

										toaster.pop('warning', "Caution!", "You must login before accessing this area.");
										return $q.defer().promise;

									}
									break;
								case 403:
									{
										$rootScope.$state.go(
											$rootScope.$state.previous != null
											&& $rootScope.$state.previous.name !== ''
											&& !$rootScope.unauthorizedArea($rootScope.$state.href($rootScope.$state.previous))
											? $rootScope.$state.previous.name : "index.oneCol.dashboard");

										toaster.pop('warning', "Caution!", "Sorry, but you do not have the appropriate permissions to access this area.");
										return $q.defer().promise;

									}
									break;
							}
							return $q.reject(response);
						}
					};
				}
	]);