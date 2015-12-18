﻿'use strict';

angular.module('app.modules.content.controllers.recipeCategoryManageController', [])
.controller('recipeCategoryManageController', ['$scope', '$rootScope', '$state', '$stateParams', 'contentService', 'toaster', 'confirmUtil', 'promiseTracker',
    function ($scope, $rootScope, $state, $stateParams, contentService, toaster, confirmUtil, promiseTracker) {
        $scope.refreshTracker = promiseTracker("get");
        $scope.editTracker = promiseTracker("edit");

        function successSaveHandler(result) {
            if (result.Success) {
                toaster.pop('success', "Success!", "Successfully saved.");
                $scope.id = result.Data.Id;
                $scope.recipeCategory.Id = result.Data.Id;
                $scope.recipeCategory.MasterContentItemId = result.Data.MasterContentItemId;
                $scope.previewUrl = $scope.baseUrl.format($scope.recipeCategory.Url);
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
                        $.each($scope.masters, function (index, master)
                        {
                            if (master.IsDefault)
                            {
                                $scope.MasterContentItemId = master.Id;
                            };
                        });
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
                $scope.forms.form.submitted = true;
                $scope.detailsTab.active = true;
            }
        };

        $scope.goToMaster = function (id) {
            $state.go('index.oneCol.masterDetail', { id: id });
        };

        initialize();
    }]);