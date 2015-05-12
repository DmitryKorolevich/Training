'use strict';

angular.module('app.modules.authentication.controllers.resetPasswordController', [])
	.controller('resetPasswordController', [
		'$scope', '$state', '$rootScope', '$stateParams', 'authenticationService', 'promiseTracker', 'toaster', 'infrastructureService', function($scope, $state, $rootScope, $stateParams, authenticationService, promiseTracker, toaster, infrastructureService) {
			var token = $stateParams.token;
			$scope.resetTracker = promiseTracker("reset");

			function errorHandler() {
				toaster.pop('error', "Error!", "Server error occured");
			};

			function initialize() {
				authenticationService.logout().success(function() {
					$rootScope.authenticated = false;
					$rootScope.currentUser = {};

				}).error(function() {
					toaster.pop('error', 'Error!', "Can't perform log out");
				});

				$scope.reset = { Token: token };
			};

			$scope.resetPassword = function (resetForm) {
				$.each(resetForm, function(index, element) {
					if (element.$name == index) {
						element.$setValidity("server", true);
					}
				});

				if (resetForm.$valid) {
					authenticationService.resetPassword($scope.reset, $scope.resetTracker).success(function(res) {
						if (res.Success) {
							$rootScope.authenticated = true;
							$rootScope.currentUser = res.Data;
							toaster.pop('success', "Success!", "Successfully reset");

							infrastructureService.getReferenceData().success(function (res) {
								if (res.Success) {
									$rootScope.ReferenceData = res.Data;
								} else {
									toaster.pop('error', 'Error!', "Unable to refresh reference data");
								}
							}).error(function (res) {
								toaster.pop('error', "Error!", "Server error occured");
							});

							$state.go("index.oneCol.dashboard");
						} else {
							var messages = "";
							if (res.Messages) {
								resetForm.submitted = true;
								$scope.serverMessages = new ServerMessages(res.Messages);
								$.each(res.Messages, function(index, value) {
									if (value.Field && resetForm[value.Field]) {
										resetForm[value.Field].$setValidity("server", false);
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
					resetForm.submitted = true;
				}
			};

			initialize();

		}
	]);