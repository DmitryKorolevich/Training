'use strict';

angular.module('app.modules.content.controllers.emailTemplateManageController', [])
.controller('emailTemplateManageController', ['$scope', '$rootScope', '$state', '$stateParams', 'contentService', 'settingService', 'toaster', 'confirmUtil', 'promiseTracker',
    function ($scope, $rootScope, $state, $stateParams, contentService, settingService, toaster, confirmUtil, promiseTracker)
    {
        $scope.refreshTracker = promiseTracker("get");

        function refreshHistory()
        {
            if ($scope.emailTemplate && $scope.emailTemplate.Id)
            {
                var data = {};
                data.service = settingService;
                data.tracker = $scope.refreshTracker;
                data.idObject = $scope.emailTemplate.Id;
                data.idObjectType = 18//email template
                $scope.$broadcast('objectHistorySection#in#refresh', data);
            }
        }

        function successSaveHandler(result)
        {
            if (result.Success)
            {
                toaster.pop('success', "Success!", "Successfully saved.");
                $scope.id = result.Data.Id;
                $scope.emailTemplate.Id = result.Data.Id;
                $scope.emailTemplate.MasterContentItemId = result.Data.MasterContentItemId;
                refreshHistory();
            } else
            {
                var messages = "";
                if (result.Messages)
                {
                    $scope.forms.form.submitted = true;
                    $scope.detailsTab.active = true;
                    $scope.serverMessages = new ServerMessages(result.Messages);
                    $.each(result.Messages, function (index, value)
                    {
                        if (value.Field)
                        {
                            $scope.forms.form[value.Field].$setValidity("server", false);
                        }
                        messages += value.Message + "<br />";
                    });
                }
                toaster.pop('error', "Error!", messages, null, 'trustedHtml');
            }
        };

        function errorHandler(result)
        {
            toaster.pop('error', "Error!", "Server error occured");
        };

        function initialize()
        {
            $scope.id = $stateParams.id ? $stateParams.id : 0;
            
            $scope.detailsTab = {
                active: true
            };
            $scope.forms = {};

            refreshMasters();
        };

        $scope.save = function ()
        {
            $.each($scope.forms.form, function (index, element)
            {
                if (element && element.$name == index)
                {
                    element.$setValidity("server", true);
                }
            });

            if ($scope.forms.form.$valid)
            {
                contentService.updateEmailTemplate($scope.emailTemplate, $scope.refreshTracker).success(function (result)
                {
                    successSaveHandler(result);
                }).
                error(function (result)
                {
                    errorHandler(result);
                });
            } else
            {
                $scope.forms.form.submitted = true;
                $scope.detailsTab.active = true;
            }
        };

        function refresh()
        {
            contentService.getEmailTemplate($scope.id, $scope.refreshTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        $scope.emailTemplate = result.Data;
                        if (!$scope.emailTemplate.MasterContentItemId)
                        {
                            $scope.emailTemplate.MasterContentItemId = $scope.MasterContentItemId;
                        };
                        refreshHistory();
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

        function refreshMasters()
        {
            contentService.getMasterContentItems({ Type: 11 })//email templates
                .success(function (result)
                {
                    if (result.Success)
                    {
                        $scope.masters = result.Data;
                        var hasDefailt = false;
                        $.each($scope.masters, function (index, master) {
                            if (master.IsDefault || $scope.masters.length == 1) {
                                hasDefailt = true;
                                $scope.MasterContentItemId = master.Id;
                            };
                        });
                        if (!hasDefailt) {
                            $scope.MasterContentItemId = $scope.masters[0].Id;
                        }
                        refresh();
                    } else
                    {
                        errorHandler(result);
                    }
                })
                .error(function (result)
                {
                    errorHandler(result);
                });
        };

        $scope.goToMaster = function (id)
        {
            $state.go('index.oneCol.masterDetail', { id: id });
        };

        initialize();
    }]);