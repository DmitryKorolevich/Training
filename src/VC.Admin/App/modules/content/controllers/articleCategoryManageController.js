'use strict';

angular.module('app.modules.content.controllers.articleCategoryManageController', [])
.controller('articleCategoryManageController', ['$scope', '$rootScope', '$state', '$stateParams', 'contentService', 'settingService', 'toaster', 'confirmUtil', 'promiseTracker',
function ($scope, $rootScope, $state, $stateParams, contentService, settingService, toaster, confirmUtil, promiseTracker) {
    $scope.refreshTracker = promiseTracker("get");
    $scope.editTracker = promiseTracker("edit");

    function refreshHistory()
    {
        if ($scope.articleCategory && $scope.articleCategory.Id)
        {
            var data = {};
            data.service = settingService;
            data.tracker = $scope.refreshTracker;
            data.idObject = $scope.articleCategory.Id;
            data.idObjectType = 12//content category
            $scope.$broadcast('objectHistorySection#in#refresh', data);
        }
    }

    function successSaveHandler(result) {
        if (result.Success) {
            toaster.pop('success', "Success!", "Successfully saved.");
            $scope.id = result.Data.Id;
            $scope.articleCategory.Id = result.Data.Id;
            $scope.articleCategory.MasterContentItemId = result.Data.MasterContentItemId;
            $scope.previewUrl = $scope.baseUrl.format($scope.articleCategory.Url);
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

    function initialize()
    {
        $scope.id = $stateParams.id ? $stateParams.id : 0;

        $scope.baseUrl = 'http://' + $rootScope.ReferenceData.PublicHost + '/articles/{0}?preview=true';
        $scope.previewUrl = null;

        $scope.detailsTab = {
            active: true
        };

        $scope.loaded = false;
        $scope.forms = {};

        refreshMasters();
    };

    function refreshCategory()
    {
        contentService.getCategory($scope.id, $scope.refreshTracker)
            .success(function (result)
            {
                if (result.Success)
                {
                    $scope.articleCategory = result.Data;
                    $scope.articleCategory.Type = 3;//article category
                    if ($scope.articleCategory.Url)
                    {
                        $scope.previewUrl = $scope.baseUrl.format($scope.articleCategory.Url);
                    }
                    if ($stateParams.categoryid)
                    {
                        $scope.articleCategory.ParentId = $stateParams.categoryid;
                    }
                    if (!$scope.articleCategory.MasterContentItemId)
                    {
                        $scope.articleCategory.MasterContentItemId = $scope.MasterContentItemId;
                    };
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
        contentService.getMasterContentItems({ Type: 3 })//article category
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
            contentService.updateCategory($scope.articleCategory, $scope.editTracker).success(function (result) {
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