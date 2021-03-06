﻿'use strict';

angular.module('app.modules.content.controllers.contentPageManageController', [])
.controller('contentPageManageController', ['$scope', '$rootScope', '$state', '$stateParams', 'contentService', 'settingService', 'toaster', 'confirmUtil', 'promiseTracker',
    function ($scope, $rootScope, $state, $stateParams, contentService, settingService, toaster, confirmUtil, promiseTracker)
    {
        $scope.refreshTracker = promiseTracker("get");
        $scope.editTracker = promiseTracker("edit");

        function refreshHistory()
        {
            if ($scope.contentPage && $scope.contentPage.Id)
            {
                var data = {};
                data.service = settingService;
                data.tracker = $scope.refreshTracker;
                data.idObject = $scope.contentPage.Id;
                data.idObjectType = 11//content page
                $scope.$broadcast('objectHistorySection#in#refresh', data);
            }
        }

        function successSaveHandler(result) {
            if (result.Success) {
                toaster.pop('success', "Success!", "Successfully saved.");
                $scope.id = result.Data.Id;
                $scope.contentPage.Id = result.Data.Id;
                $scope.contentPage.MasterContentItemId = result.Data.MasterContentItemId;
                $scope.previewUrl = $scope.baseUrl.format($scope.contentPage.Url);
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
            $scope.descriptionExpanded = false;

            $scope.toogleEditorState = function (property) {
                $scope[property] = !$scope[property];
            };

            $scope.statuses = $rootScope.ReferenceData.ContentItemStatusNames;

            $scope.baseUrl = 'http://' + $rootScope.ReferenceData.PublicHost + '/content/{0}?preview=true';
            $scope.previewUrl = null;

            $scope.detailsTab = {
                active: true
            };
            $scope.loaded = false;
            $scope.forms = {};

            $scope.save = function () {
                $.each($scope.forms.contentPageForm, function (index, element) {
                    if (element && element.$name == index) {
                        element.$setValidity("server", true);
                    }
                });

                if ($scope.forms.contentPageForm.$valid) {
                    var categoryIds = [];
                    getSelected($scope.rootCategory, categoryIds);
                    $scope.contentPage.CategoryIds = categoryIds;
                    getUIstatus($scope.contentPage);

                    contentService.updateContentPage($scope.contentPage, $scope.editTracker).success(function (result) {
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

            refreshCategories();
            refreshMasters();
        };

        $scope.updateCategoriesCollapsed = function (expand)
        {
            if (expand)
            {
                $scope.$broadcast('angular-ui-tree:expand-all');
            }
            else
            {
                $scope.$broadcast('angular-ui-tree:collapse-all');
            }
            $scope.categoriesExpanded = expand;
        };

        function refreshContentPage() {
            if ($scope.MastersLoaded && $scope.CategoriesLoaded) {
                contentService.getContentPage($scope.id, $scope.refreshTracker)
                    .success(function (result) {
                        if (result.Success) {
                            $scope.contentPage = result.Data;
                            if ($scope.contentPage.Url) {
                                $scope.previewUrl = $scope.baseUrl.format($scope.contentPage.Url);
                            };
                            if (!$scope.contentPage.MasterContentItemId) {
                                $scope.contentPage.MasterContentItemId = $scope.MasterContentItemId;
                            };
                            setUIstatus($scope.contentPage);
                            setSelected($scope.rootCategory, $scope.contentPage.CategoryIds);
                            $scope.loaded = true;
                            refreshHistory();
                        } else {
                            errorHandler(result);
                        }
                    }).
                    error(function (result) {
                        errorHandler(result);
                    });
            }
        };

        function refreshCategories() {
            contentService.getCategoriesTree({ Type: 7 }, $scope.refreshTracker)//contentPage categories
                .success(function (result) {
                    if (result.Success) {
                        $scope.rootCategory = result.Data;
                        $scope.CategoriesLoaded = true;
                        refreshContentPage();
                    } else {
                        errorHandler(result);
                    }
                }).
                error(function (result) {
                    errorHandler(result);
                });
        };

        function refreshMasters() {
            contentService.getMasterContentItems({ Type: 8 })//contentPage
                .success(function (result) {
                    if (result.Success) {
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
                        refreshContentPage();
                    } else {
                        errorHandler(result);
                    }
                })
                .error(function (result) {
                    errorHandler(result);
                });
        };

        function setUIstatus(contentPage) {
            contentPage.Status = contentPage.StatusCode + ":" + contentPage.Assigned;
        };

        function getUIstatus(contentPage) {
            var items = contentPage.Status.split(':');
            contentPage.StatusCode = parseInt(items[0]);
            contentPage.Assigned = parseInt(items[1]);
        };

        function setSelected(category, ids) {
            category.IsSelected = false;
            $.each(ids, function (index, id) {
                if (category.Id == id) {
                    category.IsSelected = true;
                }
            });
            $.each(category.SubItems, function (index, value) {
                setSelected(value, ids);
            });
        }

        function getSelected(category, ids) {
            if (category.IsSelected) {
                ids.push(category.Id);
            }
            $.each(category.SubItems, function (index, value) {
                getSelected(value, ids);
            });
        }

        $scope.goToMaster = function (id) {
            $state.go('index.oneCol.masterDetail', { id: id });
        };

        initialize();
    }]);