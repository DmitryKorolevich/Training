'use strict';

angular.module('app.modules.content.controllers.contentPageCategoryManageController', [])
.controller('contentPageCategoryManageController', ['$scope', '$rootScope', '$state', '$stateParams', 'contentService', 'settingService', 'toaster', 'confirmUtil', 'promiseTracker',
    function ($scope, $rootScope, $state, $stateParams, contentService, settingService, toaster, confirmUtil, promiseTracker) {
        $scope.refreshTracker = promiseTracker("get");
        $scope.editTracker = promiseTracker("edit");

        function refreshHistory()
        {
            if ($scope.contentPageCategory && $scope.contentPageCategory.Id)
            {
                var data = {};
                data.service = settingService;
                data.tracker = $scope.refreshTracker;
                data.idObject = $scope.contentPageCategory.Id;
                data.idObjectType = 12//content category
                $scope.$broadcast('objectHistorySection#in#refresh', data);
            }
        }

        function successSaveHandler(result) {
            if (result.Success) {
                toaster.pop('success', "Success!", "Successfully saved.");
                $scope.id = result.Data.Id;
                $scope.contentPageCategory.Id = result.Data.Id;
                $scope.contentPageCategory.MasterContentItemId = result.Data.MasterContentItemId;
                $scope.previewUrl = $scope.baseUrl.format($scope.contentPageCategory.Url);
                refreshHistory();
            } else {
                var messages = "";
                if (result.Messages) {
                    $scope.forms.form.submitted = true;
                    $scope.detailsTab.active = true;
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
            $scope.id = $stateParams.id ? $stateParams.id : 0;

            $scope.baseUrl = 'http://' + $rootScope.ReferenceData.PublicHost + '/contents/{0}?preview=true';
            $scope.previewUrl = null;

            $scope.detailsTab = {
                active: true
            };

            $scope.loaded = false;
            $scope.forms = {};

            refreshMasters();
        }

        function refreshCategory()
        {
            contentService.getCategory($scope.id, $scope.refreshTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        $scope.contentPageCategory = result.Data;
                        $scope.contentPageCategory.Type = 7;//contentPage category
                        if ($scope.contentPageCategory.Url)
                        {
                            $scope.previewUrl = $scope.baseUrl.format($scope.contentPageCategory.Url);
                        }
                        if (!$scope.contentPageCategory.MasterContentItemId)
                        {
                            $scope.contentPageCategory.MasterContentItemId = $scope.MasterContentItemId;
                        };
                        if ($stateParams.categoryid)
                        {
                            $scope.contentPageCategory.ParentId = $stateParams.categoryid;
                        }
                        $scope.loaded = true;
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
            contentService.getMasterContentItems({ Type: 7 })//contentCategory
                .success(function (result)
                {
                    if (result.Success)
                    {
                        $scope.masters = result.Data;
                        $.each($scope.masters, function (index, master)
                        {
                            if (master.IsDefault)
                            {
                                $scope.MasterContentItemId = master.Id;
                            };
                        });
                        $scope.MastersLoaded = true;
                        refreshCategory();
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

        $scope.save = function () {
            $.each($scope.forms.form, function (index, element) {
                if (element && element.$name == index) {
                    element.$setValidity("server", true);
                }
            });

            if ($scope.forms.form.$valid) {
                contentService.updateCategory($scope.contentPageCategory, $scope.editTracker).success(function (result) {
                    successSaveHandler(result);
                }).
                    error(function (result) {
                        errorHandler(result);
                    });
            } else {
                $scope.forms.form.submitted = true;
                $scope.detailsTab.active = true;
            }
        };

        $scope.goToMaster = function (id) {
            $state.go('index.oneCol.masterDetail', { id: id });
        };

        initialize();
    }]);