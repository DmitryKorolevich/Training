'use strict';

angular.module('app.modules.profile.controllers.profileController', [])
.controller('profileController', ['$scope', 'profileService', '$rootScope', 'promiseTracker', 'toaster', 'commonActionsUtil', function ($scope, profileService, $rootScope, promiseTracker, toaster, commonActionsUtil) {
        $scope.profileTracker = promiseTracker('profile');
    
        function initialize() {
            $scope.form = {};

            refreshProfile();
        };

        function refreshProfile() {
            profileService.getProfile($scope.profileTracker)
            .success(function (result) {
                if (result.Success) {
                    $scope.profile = result.Data;
                } else {
                    var messages = "";
                    if (result.Messages) {
                        $.each(result.Messages, function (index, value) {
                            messages += value.Message + "<br />";
                        });
                    } else {
                        messages = "Can't get user";
                    }

                    toaster.pop('error', 'Error!', messages, null, 'trustedHtml');
                }
            }).error(function(res) {
                toaster.pop('error', "Error!", "Server error ocurred");
            });
        }

        $scope.save = function() {
            $.each($scope.form.profileForm, function (index, element) {
            	if (element && element.$name == index) {
                    element.$setValidity("server", true);
                }
            });

            if ($scope.form.profileForm.$valid) {
                profileService.updateProfile($scope.profile, $scope.profileTracker)
                    .success(function(result) {
                        if (result.Success) {
                            toaster.pop('success', "Success!", "Successfully saved");

                            $scope.profile = result.Data;

                            $rootScope.currentUser.FirstName = $scope.profile.FirstName;
                            $rootScope.currentUser.LastName = $scope.profile.LastName;
                            $rootScope.currentUser.Email = $scope.profile.Email;
                        } else {
                            var messages = "";
                            if (result.Messages) {
                                $scope.form.profileForm.submitted = true;
                                $scope.serverMessages = new ServerMessages(result.Messages);
                                $.each(result.Messages, function (index, value) {
                                    if (value.Field && $scope.form.profileForm[value.Field]) {
                                        $scope.form.profileForm[value.Field].$setValidity("server", false);
                                    }
                                    messages += value.Message + "<br />";
                                });
                            }
                            toaster.pop('error', "Error!", messages, null, 'trustedHtml');
                        }

                        refreshProfile();
                    }).error(function(result) {
                        toaster.pop('error', "Error!", "Server error occured");

                        refreshProfile();
                    });
            } else {
                $scope.form.profileForm.submitted = true;
            }
        };

        $scope.cancel = function() {
            commonActionsUtil.cancel();
        };

        initialize();
}]);