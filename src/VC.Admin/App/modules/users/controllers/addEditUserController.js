'use strict';

angular.module('app.modules.users.controllers.addEditUserController', [])
.controller('addEditUserController', ['$scope', '$uibModalInstance', 'data', 'userService', 'toaster', 'promiseTracker', '$rootScope',
    function ($scope, $uibModalInstance, data, userService, toaster, promiseTracker, $rootScope)
{
    $scope.refreshTracker = promiseTracker("refresh");
    $scope.saveTracker = promiseTracker("save");
	$scope.resendTracker = promiseTracker("resend");
	$scope.resetTracker = promiseTracker("reset");

	function successHandler(result) {
		if (result.Success) {
			toaster.pop('success', "Success!", "Successfully saved");
			$uibModalInstance.close();

			if ($scope.editMode && $scope.signedInUser) {
			    $rootScope.currentUser.Email = $scope.user.Email;
			    $rootScope.currentUser.FirstName = $scope.user.FirstName;
			    $rootScope.currentUser.LastName = $scope.user.LastName;
			}
		} else {
		    $rootScope.fireServerValidation(result, $scope);
		}
		data.thenCallback();
	};

	function errorHandler(result) {
		toaster.pop('error', "Error!", "Server error occured");
		data.thenCallback();
	};

	function initialize()
	{
	    $scope.user = data.user;
	    $scope.forms = {};

	    $scope.signedInUser = $scope.user.Email === $rootScope.currentUser.Email;

	    $scope.editMode = data.editMode;
	    $scope.userStatuses = $.grep($rootScope.ReferenceData.UserStatuses, function (elem)
	    {
	        return elem.Key !== 0;
	    });

	    $scope.save = function ()
	    {
	        $.each($scope.forms.userForm, function (index, element)
	        {
	            if (element && element.$name == index)
	            {
	                element.$setValidity("server", true);
	            }
	        });

	        if ($scope.forms.userForm.$valid)
	        {
	            if (!$scope.user.RoleIds || $scope.user.RoleIds.length === 0)
	            {
	                toaster.pop('error', 'Error!', $rootScope.getValidationMessage("ValidationMessages.AtLeastOneRole"));
	                return;
	            }

	            $scope.saving = true;
	            if ($scope.editMode)
	            {
	                userService.updateUser($scope.user, $scope.saveTracker).success(function (result)
	                {
	                    successHandler(result);
	                }).
                        error(function (result)
                        {
                            errorHandler(result);
                        });
	            } else
	            {
	                userService.createUser($scope.user, $scope.saveTracker).success(function (result)
	                {
	                    successHandler(result);
	                }).
                        error(function (result)
                        {
                            errorHandler(result);
                        });
	            }

	        } else
	        {
	            $scope.forms.submitted = true;
	        }
	    };

	    $scope.resend = function ()
	    {
	        userService.resendActivation($scope.user.PublicId, $scope.resendTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        toaster.pop('success', "Success!", "Successfully sent");
                        $uibModalInstance.close();
                    } else
                    {
                        var messages = "";
                        if (result.Messages)
                        {
                            $.each(result.Messages, function (index, value)
                            {
                                messages += value.Message + "<br />";
                            });
                        }
                        toaster.pop('error', "Error!", messages, null, 'trustedHtml');
                    }
                    data.thenCallback();
                }).error(function ()
                {
                    toaster.pop('error', "Error!", "Server error occured");
                    data.thenCallback();
                });
	    };

	    $scope.resetPassword = function ()
	    {
	        userService.resetPassword($scope.user.PublicId, $scope.resetTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        toaster.pop('success', "Success!", "Successfully reset");
                        $uibModalInstance.close();
                    } else
                    {
                        var messages = "";
                        if (result.Messages)
                        {
                            $.each(result.Messages, function (index, value)
                            {
                                messages += value.Message + "<br />";
                            });
                        }
                        toaster.pop('error', "Error!", messages, null, 'trustedHtml');
                    }
                    data.thenCallback();
                }).error(function ()
                {
                    toaster.pop('error', "Error!", "Server error occured");
                    data.thenCallback();
                });
	    };

	    $scope.cancel = function ()
	    {
	        $uibModalInstance.close();
	    };

	    userService.getAdminTeams($scope.refreshTracker).success(function (result)
	    {
	        if (result.Success)
	        {
	            $scope.adminTeams = result.Data;
	            $scope.adminTeams.splice(0, 0, { Id: null, Name: 'Not Specified' });
	        }
	        else
	        {
	            errorHandler(result);
	        }
	    }).
        error(function (result)
        {
            errorHandler(result);
        });
	};


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