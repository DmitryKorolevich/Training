'use strict';

angular.module('app.modules.users.controllers.userManagementController', [])
.controller('userManagementController', ['$scope', 'userService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', function ($scope, userService, toaster, modalUtil, confirmUtil, promiseTracker) {
	$scope.refreshTracker = promiseTracker("refresh");
	$scope.deleteTracker = promiseTracker("delete");
	$scope.editTracker = promiseTracker("edit");
	$scope.addTracker = promiseTracker("add");

	function refreshUsers() {
		userService.getUsers($scope.filter, $scope.refreshTracker)
			.success(function (result) {
				if (result.Success) {
					$scope.users = result.Data.Items;
				} else {
					toaster.pop('error', 'Error!', "Can't get access to the users");
				}
			})
			.error(function (result) {
				toaster.pop('error', "Error!", "Server error ocurred");
			});
	};

	function openModal(user, editMode) {
		modalUtil.open('app/modules/users/partials/addEditUser.html', 'addEditUserController', { user: user, editMode: editMode, thenCallback: function() {
			refreshUsers();
		} });
	}

	function initialize() {
		$scope.filter = { SearchText: "" };

		refreshUsers();
	}

	$scope.filterUsers = function() {
		refreshUsers();
	};

	$scope.open = function (editMode, publicId) {
		var user = {};
		if (editMode) {
			userService.getUser(publicId, $scope.editTracker)
				.success(function (result) {
					if (result.Success) {
						openModal(result.Data, editMode);
					} else {
						toaster.pop('error', 'Error!', "Can't get user");
					}
				}).
				error(function(result) {
					toaster.pop('error', "Error!", "Server error ocurred");
				});
		} else {
			userService.createUserPrototype($scope.addTracker)
				.success(function (result) {
					if (result.Success) {
						user = result.Data;
						openModal(result.Data, editMode);
					} else {
						toaster.pop('error', 'Error!', "Can't create user");
					}
				}).
				error(function (result) {
					toaster.pop('error', "Error!", "Server error ocurred");
				});
		}
	};

	$scope.delete = function(fullName, publicId) {
		confirmUtil.confirm(function() {
			userService.deleteUser(publicId, $scope.deleteTracker)
				.success(function(result) {
					if (result.Success) {
						toaster.pop('success', "Success!", "Successfully deleted");
					} else {
						toaster.pop('error', 'Error!', "Can't delete the user");
					}
					refreshUsers();
				})
				.error(function(result) {
					toaster.pop('error', "Error!", "Server error ocurred");
					refreshUsers();
				});
		}, 'Are you sure you want to delete ' + fullName + ' user?');
	};

	initialize();
}]);