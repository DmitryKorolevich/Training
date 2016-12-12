'use strict';

angular.module('app.modules.content.controllers.faqCategoryManageController', [])
.controller('faqCategoryManageController', ['$scope', '$rootScope', '$state', '$stateParams', 'contentService', 'settingService', 'toaster', 'confirmUtil', 'promiseTracker',
function ($scope, $rootScope, $state, $stateParams, contentService, settingService, toaster, confirmUtil, promiseTracker) {
    $scope.refreshTracker = promiseTracker("get");
    $scope.editTracker = promiseTracker("edit");

    function refreshHistory()
    {
        if ($scope.faqCategory && $scope.faqCategory.Id)
        {
            var data = {};
            data.service = settingService;
            data.tracker = $scope.refreshTracker;
            data.idObject = $scope.faqCategory.Id;
            data.idObjectType = 12//content category
            $scope.$broadcast('objectHistorySection#in#refresh', data);
        }
    }

    function successSaveHandler(result) {
        if (result.Success) {
            toaster.pop('success', "Success!", "Successfully saved.");
            $scope.id = result.Data.Id;
            $scope.faqCategory.Id = result.Data.Id;
            $scope.faqCategory.MasterContentItemId = result.Data.MasterContentItemId;
            $scope.previewUrl = $scope.baseUrl.format($scope.faqCategory.Url);
            refreshHistory();
        } else
        {
            $rootScope.fireServerValidation(result, $scope);
        }
    };

    function errorHandler(result) {
        toaster.pop('error', "Error!", "Server error occured");
    };

    function initialize()
    {
        $scope.id = $stateParams.id ? $stateParams.id : 0;

        $scope.baseUrl = 'http://' + $rootScope.ReferenceData.PublicHost + '/faqs/{0}?preview=true';
        $scope.previewUrl = null;

        $scope.detailsTab = {
            active: true
        };

        $scope.loaded = false;
        $scope.forms = {};

        refreshMasters();
    };

    function refreshFAQCategory()
    {
        contentService.getCategory($scope.id, $scope.refreshTracker)
            .success(function (result)
            {
                if (result.Success)
                {
                    $scope.faqCategory = result.Data;
                    $scope.faqCategory.Type = 5;//faq category
                    if ($scope.faqCategory.Url)
                    {
                        $scope.previewUrl = $scope.baseUrl.format($scope.faqCategory.Url);
                    }
                    if (!$scope.faqCategory.MasterContentItemId)
                    {
                        $scope.faqCategory.MasterContentItemId = $scope.MasterContentItemId;
                    };
                    if ($stateParams.categoryid)
                    {
                        $scope.faqCategory.ParentId = $stateParams.categoryid;
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
        contentService.getMasterContentItems({ Type: 5 })//faq category
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
                    refreshFAQCategory();
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
            contentService.updateCategory($scope.faqCategory, $scope.editTracker).success(function (result) {
                successSaveHandler(result);
            }).
                error(function (result) {
                    errorHandler(result);
                });
        } else {
            $scope.forms.submitted = true;
            $scope.detailsTab.active = true;
        }
    };

    $scope.goToMaster = function (id) {
        $state.go('index.oneCol.masterDetail', { id: id });
    };

    initialize();
}]);