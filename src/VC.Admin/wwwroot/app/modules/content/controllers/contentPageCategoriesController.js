﻿angular.module('app.modules.content.controllers.contentPageCategoriesController', [])
.controller('contentPageCategoriesController', ['$scope', '$state', '$stateParams', 'contentService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker',
    function ($scope, $state, $stateParams, contentService, toaster, modalUtil, confirmUtil,promiseTracker) {
    $scope.refreshTracker = promiseTracker("refresh");
    $scope.editTracker = promiseTracker("edit");
    $scope.deleteTracker = promiseTracker("delete");

    function errorHandler(result) {
        var messages = "";
        if (result.Messages) {
            $.each(result.Messages, function (index, value) {
                messages += value.Message + "<br />";
            });
        }
        toaster.pop('error', "Error!", messages, null, 'trustedHtml');
    };

    function loadCategories() {
        contentService.getCategoriesTree({ Type: 7 },$scope.refreshTracker)//contentPage categories
            .success(function (result) {
                if (result.Success) {
                    $scope.rootCategory = result.Data;
                    $scope.loaded = true;
                } else {
                    errorHandler(result);
                }
            })
			.error(function (result) {
			    errorHandler(result);
			});
    };

    function initialize() {
        $scope.mode = $stateParams.mode;
        $scope.id = $stateParams.id;

        $scope.loaded = false;

        loadCategories();
    }

    function removeCategoryFromTree(id) {
        removeCategory($scope.rootCategory, id);
    };

    function removeCategory(category, id) {
        var indexForRemove = null;
        $.each(category.SubItems, function (index, value) {
            if (value.Id == id)
            {
                indexForRemove = index;
                return false;
            }
        });

        if (indexForRemove!=null) {
            category.SubItems.splice(indexForRemove, 1);
        }
        else {
            $.each(category.SubItems, function (index, value) {
                removeCategory(value, id);
            });
        }
    }

    $scope.open = function (id) {
        $state.go('index.oneCol.contentPageCategoryDetail', { id: id });
    };

    $scope.add = function(id)
    {
        $state.go('index.oneCol.addNewContentPageCategory', { categoryid: id });
    }

    $scope.delete = function (id) {
        confirmUtil.confirm(function () {
            contentService.deleteCategory(id,$scope.deleteTracker)
			    .success(function (result) {
			        if (result.Success) {
			            toaster.pop('success', "Success!", "Successfully deleted.");
			            removeCategoryFromTree(id);
			        } else {
			            errorHandler(result);
			        }
			    })
			    .error(function (result) {
			        errorHandler(result);
			    });
        }, 'Are you sure you want to delete this category?');
    };

    $scope.save = function () {
        contentService.updateCategoriesTree($scope.rootCategory,$scope.editTracker)
            .success(function (result) {
                if (result.Success) {
                    toaster.pop('success', "Success!", "Successfully saved.");
                } else {
                    errorHandler(result);
                }
            })
            .error(function (result) {
                errorHandler(result);
            });
    };

    $scope.cancel = function () {
        if ($scope.mode == 'list') {
            $state.go('index.oneCol.manageContentPages');
        }
        if ($scope.mode == 'edit') {
            if ($scope.id) {
                $state.go('index.oneCol.contentPageDetail', { id: $scope.id });
            }
            else {
                $state.go('index.oneCol.addNewContentPage');
            }
        }
    };

    initialize();
}]);