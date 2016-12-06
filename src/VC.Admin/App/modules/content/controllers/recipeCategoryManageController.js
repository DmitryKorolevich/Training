'use strict';

angular.module('app.modules.content.controllers.recipeCategoryManageController', [])
.controller('recipeCategoryManageController', ['$scope', '$rootScope', '$state', '$stateParams', 'contentService', 'settingService', 'toaster', 'confirmUtil', 'promiseTracker',
    function ($scope, $rootScope, $state, $stateParams, contentService, settingService, toaster, confirmUtil, promiseTracker)
    {
        $scope.refreshTracker = promiseTracker("get");
        $scope.editTracker = promiseTracker("edit");

        function refreshHistory()
        {
            if ($scope.recipeCategory && $scope.recipeCategory.Id)
            {
                var data = {};
                data.service = settingService;
                data.tracker = $scope.refreshTracker;
                data.idObject = $scope.recipeCategory.Id;
                data.idObjectType = 12//content category
                $scope.$broadcast('objectHistorySection#in#refresh', data);
            }
        }

        function successSaveHandler(result) {
            if (result.Success) {
                toaster.pop('success', "Success!", "Successfully saved.");
                $scope.id = result.Data.Id;
                $scope.recipeCategory.Id = result.Data.Id;
                $scope.recipeCategory.MasterContentItemId = result.Data.MasterContentItemId;
                $scope.previewUrl = $scope.baseUrl.format($scope.recipeCategory.Url);
                refreshHistory();
            } else
            {
                $rootScope.fireServerValidation(result, $scope);
            }
        };

        function errorHandler(result) {
            toaster.pop('error', "Error!", "Server error occured");
        };

        function initialize() {
            $scope.id = $stateParams.id ? $stateParams.id : 0;

            $scope.baseUrl = 'http://' + $rootScope.ReferenceData.PublicHost + '/recipes/{0}?preview=true';
            $scope.previewUrl = null;

            $scope.detailsTab = {
                active: true
            };

            $scope.loaded = false;
            $scope.forms = {};

            refreshMasters();
        }

        function refreshRecipeCategory()
        {
            contentService.getCategory($scope.id, $scope.refreshTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        $scope.recipeCategory = result.Data;
                        $scope.recipeCategory.Type = 1;//recipe category
                        if ($scope.recipeCategory.Url)
                        {
                            $scope.previewUrl = $scope.baseUrl.format($scope.recipeCategory.Url);
                        }
                        if (!$scope.recipeCategory.MasterContentItemId)
                        {
                            $scope.recipeCategory.MasterContentItemId = $scope.MasterContentItemId;
                        };
                        if ($stateParams.categoryid)
                        {
                            $scope.recipeCategory.ParentId = $stateParams.categoryid;
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
            contentService.getMasterContentItems({ Type: 1 })//recipe category
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
                        refreshRecipeCategory();
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
                contentService.updateCategory($scope.recipeCategory, $scope.editTracker).success(function (result) {
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