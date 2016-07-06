'use strict';

angular.module('app.modules.setting.controllers.settingsController', [])
.controller('settingsController', ['$scope', '$rootScope', '$state', '$stateParams', 'settingService', 'toaster', 'confirmUtil', 'promiseTracker',
    function ($scope, $rootScope, $state, $stateParams, settingService, toaster, confirmUtil, promiseTracker) {
        $scope.refreshTracker = promiseTracker("get");
        $scope.editTracker = promiseTracker("edit");

        function successSaveHandler(result) {
            if (result.Success) {
                toaster.pop('success', "Success!", "Successfully saved.");
                if (result.Data)
                {
                    refreshItems();
                }
                else
                {
                    toaster.pop('error', "Error!", "Server error occured");
                }
            } else {
                var messages = "";
                if (result.Messages) {
                    $scope.forms.submitted = true;
                    $scope.serverMessages = new ServerMessages(result.Messages);
                    $.each(result.Messages, function (index, value) {
                        if (value.Field) {
                            $scope.forms.form[value.Field].$setValidity("server", false);
                        }
                        messages += value.Message + "<br />";
                    });
                }
                toaster.pop('error', "Error!", messages, null, 'trustedHtml');
            }
        };

        function errorHandler(result) {
            toaster.pop('error', "Error!", "Server error occured");
        };

        function initialize() {
            $scope.forms = {};

            $scope.settings={};

            refreshItems();
        }

        function refreshItems()
        {
            settingService.getGlobalSettings($scope.refreshTracker)
                    .success(function (result)
                    {
                        if (result.Success)
                        {
                            $scope.settings = result.Data;
                        } else
                        {
                            errorHandler(result);
                        }
                    }).
                    error(function (result)
                    {
                        errorHandler(result);
                    });
        };

        $scope.save = function () {
            $.each($scope.forms.form, function (index, element) {
            	if (element && element.$name == index) {
                    element.$setValidity("server", true);
                }
            });

            if ($scope.forms.form.$valid) {
                settingService.updateGlobalSettings($scope.settings, $scope.editTracker).success(function (result) {
                    successSaveHandler(result);
                }).
                    error(function (result) {
                        errorHandler(result);
                    });
            } else {
                $scope.forms.form.submitted = true;
            }
        };

        initialize();
    }]);