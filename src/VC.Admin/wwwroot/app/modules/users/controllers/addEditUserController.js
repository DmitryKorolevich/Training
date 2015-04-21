'use strict';

angular.module('app.modules.users.controllers.addEditUserController', [])
.controller('addEditUserController', ['$scope', '$modalInstance', 'data', 'userService', 'toaster', 'promiseTracker', '$rootScope', function ($scope, $modalInstance, data, userService, toaster, promiseTracker, $rootScope) {
	$scope.saveTracker = promiseTracker("save");

	function successHandler(result) {
		if (result.Success) {
			toaster.pop('success', "Success!", "Successfully saved");
		} else {
			toaster.pop('error', "Error!", "Can't save your changes");
		}
		data.thenCallback();
	};

	function errorHandler(result) {
		toaster.pop('error', "Error!", "Server error occured");
		data.thenCallback();
	};

	function initialize() {
		$scope.user = data.user;
		$scope.editMode = data.editMode;
		$scope.userStatuses = $.grep($rootScope.ReferenceData.UserStatuses, function(elem) {
			return elem.Key !== 0;
		});

		$scope.save = function () {
			if ($scope.userForm.$valid) {
				if (!$scope.user.RoleIds || $scope.user.RoleIds.length === 0) {
					toaster.pop('error', 'Error!', 'At least one role should be selected');
					return;
				}

				$scope.saving = true;
				if ($scope.editMode) {
					userService.updateUser($scope.user,$scope.saveTracker).success(function(result) {
							successHandler(result);
						}).
						error(function(result) {
							errorHandler(result);
						});
				} else {
					userService.createUser($scope.user,$scope.saveTracker).success(function(result) {
							successHandler(result);
						}).
						error(function(result) {
							errorHandler(result);
						});
				}
				$modalInstance.close();
			} else {
				$scope.userForm.submitted = true;
			}
		};

		$scope.cancel = function () {
			$modalInstance.close();
		};
	}

	$scope.toggleRoleSelection = function (roleName) {
		if (!$scope.user.RoleIds) {
			$scope.user.RoleIds = [];
		}

		var idx = $scope.user.RoleIds.indexOf(roleName);

		if (idx > -1) {
			$scope.user.RoleIds.splice(idx, 1);
		}
		else {
			$scope.user.RoleIds.push(roleName);
		}
	};

	initialize();
}]);