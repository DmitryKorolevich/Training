﻿'use strict';

angular.module('app.modules.content.controllers.articleManageController', [])
.controller('articleManageController', ['$scope', '$rootScope', '$state', '$stateParams', 'appBootstrap', 'modalUtil', 'contentService', 'toaster', 'confirmUtil', 'promiseTracker',
function ($scope, $rootScope, $state, $stateParams, appBootstrap, modalUtil, contentService, toaster, confirmUtil, promiseTracker) {
    $scope.refreshTracker = promiseTracker("get");
    $scope.editTracker = promiseTracker("edit");

    function successSaveHandler(result) {
        if (result.Success) {
            toaster.pop('success', "Success!", "Successfully saved.");
            $scope.id = result.Data.Id;
            $scope.article.Id = result.Data.Id;
            $scope.article.MasterContentItemId = result.Data.MasterContentItemId;
            $scope.previewUrl = $scope.baseUrl.format($scope.article.Url);
        } else {
            var messages = "";
            if (result.Messages) {
                $scope.forms.articleForm.submitted = true;
                $scope.detailsTab.active = true;
                $scope.serverMessages = new ServerMessages(result.Messages);
                $.each(result.Messages, function (index, value) {
                    if (value.Field) {
                        $scope.forms.articleForm[value.Field].$setValidity("server", false);
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
        $scope.descriptionExpanded = false;

        $scope.toogleEditorState = function (property) {
            $scope[property] = !$scope[property];
        };

        $scope.baseUrl = $rootScope.ReferenceData.PublicHost + 'article/{0}?preview=true';
        $scope.previewUrl = null;

        $scope.detailsTab = {
            active: true
        };
        $scope.loaded = false;
        $scope.forms = {};

        $scope.save = function () {
            $.each($scope.forms.articleForm, function (index, element) {
                if (element && element.$name == index) {
                    element.$setValidity("server", true);
                }
            });

            if ($scope.forms.articleForm.$valid) {
                var categoryIds = [];
                getSelected($scope.rootCategory, categoryIds);
                $scope.article.CategoryIds = categoryIds;
                $scope.article.PublishedDate = null;
                if ($scope.article.PublishedDateObject.Date)
                    $scope.article.PublishedDate = $scope.article.PublishedDateObject.Date.toServerDateTime();

                contentService.updateArticle($scope.article, $scope.editTracker).success(function (result) {
                    successSaveHandler(result);
                }).
                    error(function (result) {
                        errorHandler(result);
                    });
            } else {
                $scope.forms.articleForm.submitted = true;
                $scope.detailsTab.active = true;
            }
        };

        contentService.getCategoriesTree({ Type: 3 }, $scope.refreshTracker)//article categories
			.success(function (result) {
			    if (result.Success) {
			        $scope.rootCategory = result.Data;
			        contentService.getArticle($scope.id, $scope.refreshTracker)
                        .success(function (result) {
                            if (result.Success) {
                                $scope.article = result.Data;
                                $scope.article.PublishedDateObject = new DateObject(Date.parseDateTime($scope.article.PublishedDate));
                                if ($scope.article.Url) {
                                    $scope.previewUrl = $scope.baseUrl.format($scope.article.Url);
                                }
                                setSelected($scope.rootCategory, $scope.article.CategoryIds);
                                $scope.loaded = true;
                            } else {
                                errorHandler(result);
                            }
                        }).
                        error(function (result) {
                            errorHandler(result);
                        });
			    } else {
			        errorHandler(result);
			    }
			}).
			error(function (result) {
			    errorHandler(result);
			});
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
    };

    function getSelected(category, ids) {
        if (category.IsSelected) {
            ids.push(category.Id);
        }
        $.each(category.SubItems, function (index, value) {
            getSelected(value, ids);
        });
    };

    $scope.goToMaster = function (id) {
        $state.go('index.oneCol.masterDetail', { id: id });
    };

    initialize();
}]);