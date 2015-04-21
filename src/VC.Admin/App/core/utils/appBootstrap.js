'use strict';

angular.module('app.core.utils.appBootstrap', [])
	.service('appBootstrap', ['infrastructureService', '$rootScope', 'toaster', function (infrastructureService, $rootScope, toaster) {
			
			function initialize() {
				$rootScope.appStarted = false;

				infrastructureService.getReferenceData().success(function(res) {
					if (res.Success) {
						$rootScope.ReferenceData = res.Data;

						$rootScope.appStarted = true;
					} else {
						toaster.pop('error', 'Error!', "Unable to initialize app properly");
					}
				}).error(function(res) {
					toaster.pop('error', "Error!", "Server error occured");
				});

				$rootScope.authenticated = true; //temp solution
			}

			return {
				initialize : initialize
			}
		}
	]);