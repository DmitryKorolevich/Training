angular.module('app.modules.product.controllers.productCategoriesController', [])
.controller('productCategoriesController', ['$scope', '$state', '$stateParams', 'productService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker',
    function ($scope, $state, $stateParams, productService, toaster, modalUtil, confirmUtil, promiseTracker) {
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
        productService.getCategoriesTree({ }, $scope.refreshTracker)
            .success(function (result) {
                if (result.Success) {
                    $scope.rootCategory = result.Data;
                    $scope.loaded = true;
                    $scope.categoriesExpanded = true;
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
        $state.go('index.oneCol.productCategoryDetail', { id: id });
    };

    $scope.add = function (id)
    {
        $state.go('index.oneCol.addNewProductCategory', { categoryid: id });
    };

    $scope.productsOrder = function (id)
    {
        modalUtil.open('app/modules/product/partials/manageProductsOrderPopup.html', 'manageProductsOrderController', {
            idCategory: id, thenCallback: function (data)
            {
                
            }
        }, { size: 'lg' });
    };

    $scope.delete = function (id) {
        confirmUtil.confirm(function () {
            productService.deleteCategory(id, $scope.deleteTracker)
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
        productService.updateCategoriesTree($scope.rootCategory, $scope.editTracker)
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

    initialize();
}]);