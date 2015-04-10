'use strict';

angular.module('app.modules.users.controllers.addEditUserController', [])
.controller('addEditUserController', ['$scope', '$modalInstance', 'data', 'userService', 'toaster', function ($scope, $modalInstance, data, userService, toaster) {

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
		$scope.rolesLookup = ["Content User", "Product User", "Order User", "Admin User", "Super Admin User"]; //hardcoded for now
		$scope.roleNames = [];

		$scope.user = data.user;
		$scope.editMode = data.editMode;

		$scope.save = function () {
			if ($scope.userForm.$valid) {
				if ($scope.user.RoleNames.length === 0) {
					toaster.pop('error', 'Error!', 'At least one role should be selected');
					return;
				}

				if ($scope.editMode) {
					userService.updateUser($scope.user).success(function(result) {
							successHandler(result);
						}).
						error(function(result) {
							errorHandler(result);
						});
				} else {
					userService.createUser($scope.user).success(function(result) {
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

	//will be refactored
	$scope.toggleRoleSelection = function (roleName) {
		if (!$scope.user.RoleNames) {
			$scope.user.RoleNames = [];
		}

		var idx = $scope.user.RoleNames.indexOf(roleName);

		if (idx > -1) {
			$scope.user.RoleNames.splice(idx, 1);
		}
		else {
			$scope.user.RoleNames.push(roleName);
		}
	};

	initialize();
}]);