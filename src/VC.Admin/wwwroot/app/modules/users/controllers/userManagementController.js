'use strict';

angular.module('app.modules.users.controllers.userManagementController', [])
.controller('userManagementController', ['$scope', 'userService', 'toaster', 'modalUtil', 'confirmUtil', function ($scope, userService, toaster, modalUtil, confirmUtil) {
	function refreshUsers() {
		userService.getUsers($scope.filter)
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
		$scope.filter = { Keyword: "" };

		refreshUsers();
	}

	$scope.filterUsers = function() {
		refreshUsers();
	};

	$scope.open = function (editMode, publicId) {
		var user = {};
		if (editMode) {
			$scope.editing = true;
			userService.getUser(publicId)
				.success(function (result) {
					if (result.Success) {
						openModal(result.Data, editMode);
					} else {
						toaster.pop('error', 'Error!', "Can't get user");
					}
					$scope.editing = false;
				}).
				error(function(result) {
					toaster.pop('error', "Error!", "Server error ocurred");
					$scope.editing = false;
				});
		} else {
			$scope.adding = true;
			userService.createUserPrototype()
				.success(function (result) {
					if (result.Success) {
						user = result.Data;
						openModal(result.Data, editMode);
					} else {
						toaster.pop('error', 'Error!', "Can't create user");
					}
					$scope.adding = false;
				}).
				error(function (result) {
					toaster.pop('error', "Error!", "Server error ocurred");
					$scope.adding = false;
				});
		}
	};

	$scope.delete = function(firstName, lastName, publicId) {
		confirmUtil.confirm(function() {
			$scope.removing = true;
			userService.deleteUser(publicId)
				.success(function(result) {
					if (result.Success) {
						toaster.pop('success', "Success!", "Successfully deleted");
					} else {
						toaster.pop('error', 'Error!', "Can't delete the user");
					}
					refreshUsers();
					$scope.removing = false;
				})
				.error(function(result) {
					toaster.pop('error', "Error!", "Server error ocurred");
					refreshUsers();
					$scope.removing = false;
				});
		}, 'Are you sure you want to delete ' + firstName + ' ' + lastName + ' user?');
	};

	initialize();
}]);