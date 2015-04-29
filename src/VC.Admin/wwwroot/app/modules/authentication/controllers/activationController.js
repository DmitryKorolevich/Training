'use strict';

angular.module('app.modules.authentication.controllers.activationController', [])
.controller('activationController', ['$scope', '$state', '$rootScope','$stateParams', 'authenticationService', 'promiseTracker', 'toaster', 'infrastructureService', function ($scope, $state, $rootScope, $stateParams, authenticationService, promiseTracker, toaster, infrastructureService) {
		var token = $stateParams.token;
		$scope.activationTracker = promiseTracker("activation");
		$scope.readTracker = promiseTracker("read");

		function errorHandler() {
			toaster.pop('error', "Error!", "Server error occured");
		};

		function initialize() {
			authenticationService.logout().success(function() {
				$rootScope.authenticated = false;
				$rootScope.currentUser = {};

				authenticationService.getUser(token, $scope.readTracker).success(function(res) {
					if (res.Success) {
						$scope.user = res.Data;
					} else {
						var messages = "";
						if (res.Messages) {
							$.each(res.Messages, function(index, value) {
								messages += value.Message + "<br />";
							});
						} else {
							messages = "Can't get user by token";
						}
						
						$state.go("index.oneCol.login");

						toaster.pop('error', 'Error!', messages, null, 'trustedHtml');
					}
				}).error(function(res) {
					$state.go("index.oneCol.login");
					errorHandler();
				});
			}).error(function() {
				toaster.pop('error', 'Error!', "Can't perform log out");
			});
		};

		$scope.activate = function(userForm) {
				if (userForm.$valid) {
					authenticationService.activate($scope.user, $scope.activationTracker).success(function(res) {
						if (res.Success) {
							$rootScope.authenticated = true;
							$rootScope.currentUser = res.Data;
							toaster.pop('success', "Success!", "Successfully activated");

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
								messages = "Can't activate user";
							}

							toaster.pop('error', 'Error!', messages, null, 'trustedHtml');
						}
					}).error(function(res) {
						errorHandler();
					});
				} else {
					userForm.submitted = true;
				}
			};

		initialize();

}]);