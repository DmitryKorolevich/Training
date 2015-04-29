'use strict';

angular.module('app.modules.authentication.controllers.loginController', [])
	.controller('loginController', [
		'$scope', 'promiseTracker', 'toaster', 'authenticationService', 'infrastructureService', '$rootScope', '$state',
		function($scope, promiseTracker, toaster, authenticationService, infrastructureService, $rootScope, $state) {
			$scope.loginTracker = promiseTracker("login");

			function errorHandler() {
				toaster.pop('error', "Error!", "Server error occured");
			};

			function initialize() {
				$scope.login = {};
			};

			$scope.signIn = function(loginForm) {
				if (loginForm.$valid) {
					authenticationService.login($scope.login, $scope.loginTracker).success(function(res) {
						if (res.Success) {
							$rootScope.authenticated = true;
							$rootScope.currentUser = res.Data;

							infrastructureService.getReferenceData().success(function(res) {
								if (res.Success) {
									$rootScope.ReferenceData = res.Data;
								} else {
									toaster.pop('error', 'Error!', "Unable to refresh reference data");
								}
							}).error(function(res) {
								toaster.pop('error', "Error!", "Server error occured");
							});

							$state.go("index.oneCol.dashboard");
						} else {
							var messages = "";
							if (res.Messages) {
								$.each(res.Messages, function(index, value) {
									messages += value.Message + "<br />";
								});
							} else {
								messages = "Can't login";
							}

							toaster.pop('error', 'Error!', messages, null, 'trustedHtml');
						}
					}).error(function(res) {
						errorHandler();
					});
				} else {
					loginForm.submitted = true;
				}
			};

			initialize();
		}
	]);