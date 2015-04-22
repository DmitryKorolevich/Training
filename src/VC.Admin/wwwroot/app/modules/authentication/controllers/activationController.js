'use strict';

angular.module('app.modules.authentication.controllers.activationController', [])
.controller('activationController', ['$scope', '$state', '$rootScope','$stateParams', 'authenticationService', 'promiseTracker', 'toaster', function ($scope, $state, $rootScope, $stateParams, authenticationService, promiseTracker, toaster) {
		var token = $stateParams.token;
		$scope.activationTracker = promiseTracker("activation");
		$scope.readTracker = promiseTracker("read");

		function errorHandler() {
			toaster.pop('error', "Error!", "Server error occured");
		};

		function initialize() {
			authenticationService.getUser(token, $scope.readTracker).success(function(res) {
				if (res.Success) {
					$scope.user = res.Data;
				} else {
					toaster.pop('error', 'Error!', "Can't get user by token");
				}
			}).error(function(res) {
				errorHandler();
			});
		};

		$scope.activate = function(userForm) {
				if (userForm.$valid) {
					authenticationService.activate($scope.user, $scope.activationTracker).success(function(res) {
						if (res.Success) {
							$rootScope.authenticated = true;
							$rootScope.currentUser = res.Data;
							toaster.pop('success', "Success!", "Successfully activated");

							$state.go("index.oneCol.dashboard");
						} else {
							toaster.pop('error', 'Error!', "Can't activate user");
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