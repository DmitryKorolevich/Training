'use strict';

angular.module('app.modules.users.controllers.addEditUserController', [])
.controller('addEditUserController', ['$scope', '$modalInstance', 'data', 'userService', 'toaster', 'promiseTracker', '$rootScope', function ($scope, $modalInstance, data, userService, toaster, promiseTracker, $rootScope) {
	$scope.saveTracker = promiseTracker("save");
	$scope.resendTracker = promiseTracker("resend");

	function successHandler(result) {
		if (result.Success) {
			toaster.pop('success', "Success!", "Successfully saved");
			$modalInstance.close();
		} else {
			var messages = "";
			if (result.Messages) {
				$scope.userForm.submitted = true;
				$scope.serverMessages = new ServerMessages(result.Messages);
				$.each(result.Messages, function(index, value) {
					if (value.Field && $scope.userForm[value.Field.toLowerCase()]) {
						$scope.userForm[value.Field.toLowerCase()].$setValidity("server", false);
					}
					messages += value.Message + "<br />";
				});
			}
			toaster.pop('error', "Error!", messages, null, 'trustedHtml');
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
			$.each($scope.userForm, function(index, element) {
				if (element.$name == index) {
					element.$setValidity("server", true);
				}
			});

			if ($scope.userForm.$valid) {
				if (!$scope.user.RoleIds || $scope.user.RoleIds.length === 0) {
					toaster.pop('error', 'Error!', $rootScope.getValidationMessage("ValidationMessages.AtLeastOneRole"));
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

			} else {
				$scope.userForm.submitted = true;
			}
		};

		$scope.resend = function() {
			userService.resendActivation($scope.user.PublicId, $scope.resendTracker)
				.success(function(result) {
					if (result.Success) {
						toaster.pop('success', "Success!", "Successfully sent");
						$modalInstance.close();
					} else {
						var messages = "";
						if (result.Messages) {
							$.each(result.Messages, function(index, value) {
								messages += value.Message + "<br />";
							});
						}
						toaster.pop('error', "Error!", messages, null, 'trustedHtml');
					}
					data.thenCallback();
				}).error(function() {
					toaster.pop('error', "Error!", "Server error occured");
					data.thenCallback();
				});
		};

		$scope.cancel = function () {
			$modalInstance.close();
		};
	}

	$scope.toggleRoleSelection = function (roleId) {
		if (!$scope.user.RoleIds) {
			$scope.user.RoleIds = [];
		}

		var idx = $scope.user.RoleIds.indexOf(roleId);

		if (idx > -1) {
			$scope.user.RoleIds.splice(idx, 1);
		}
		else {
			$scope.user.RoleIds.push(roleId);
		}
	};

	initialize();
}]);