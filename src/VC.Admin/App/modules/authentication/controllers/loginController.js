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
				$.each(loginForm, function(index, element) {
					if (element && element.$name == index) {
						element.$setValidity("server", true);
					}
				});

				if (loginForm.$valid) {
					authenticationService.login($scope.login, $scope.loginTracker).success(function(res) {
						if (res.Success) {
							$rootScope.authenticated = true;
							$rootScope.currentUser = res.Data;

							infrastructureService.getReferenceData().success(function(res) {
								if (res.Success) {
								    $rootScope.ReferenceData = res.Data;
								    if ($rootScope.currentUser.IsSuperAdmin || $.inArray(2, $rootScope.currentUser.Permissions)>=0)//orders
								    {
								        $rootScope.$state.go("index.oneCol.manageOrders");
								    }
								} else {
									toaster.pop('error', 'Error!', "Unable to refresh reference data");
								}
							}).error(function(res) {
								toaster.pop('error', "Error!", "Server error occured");
							});

							$state.go($state.previous != null
                                && $state.previous.name !== '' 
                                && !$rootScope.unauthorizedArea($state.href($state.previous))
                                ? $state.previous.name : "index.oneCol.dashboard");
						} else {
							var messages = "";
							if (res.Messages) {
								loginForm.submitted = true;
								$scope.serverMessages = new ServerMessages(res.Messages);
								$.each(res.Messages, function(index, value) {
									if (value.Field && loginForm[value.Field.toLowerCase()]) {
										loginForm[value.Field.toLowerCase()].$setValidity("server", false);
									}
									messages += value.Message + "<br />";
								});
							}
							toaster.pop('error', "Error!", messages, null, 'trustedHtml');
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