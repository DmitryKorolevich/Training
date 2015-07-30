angular.module('app.modules.product.controllers.inventoryCategoriesController', [])
.controller('inventoryCategoriesController', ['$scope', '$state', '$stateParams', 'productService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker',
    function ($scope, $state, $stateParams, productService, toaster, modalUtil, confirmUtil, promiseTracker) {
    $scope.refreshTracker = promiseTracker("refresh");
    $scope.editTracker = promiseTracker("edit");
    $scope.deleteTracker = promiseTracker("delete");

    var MAX_LEVEL = 5;

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
        productService.getInventoryCategoriesTree({ }, $scope.refreshTracker)
            .success(function (result) {
                if (result.Success) {
                    $scope.rootCategories = result.Data;
                    $.each($scope.rootCategories, function (index, category) {
                        initCategory(category);
                    });
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
        loadCategories();
    }

    var getCategoriesTreeViewScope = function () {
        return angular.element($('.categories .ya-treeview').get(0)).scope();
    };

    $scope.updateCategoriesCollapsed = function (expand) {
        var scope = getCategoriesTreeViewScope();
        if (expand) {
            scope.expandAll();
        }
        else {
            scope.collapseAll();
        }
        $scope.categoriesExpanded = expand;
    };

    function initCategory(category)
    {
        category.AllowDelete = !category.SubItems || category.SubItems.length == 0;
        var level = 1;
        var resCategory = null;
        $.each($scope.rootCategories, function (index, value) {
            var result = getCategoryWithLevel(value, category.Id, level);
            if (result) {
                resCategory = result;
                return false;
            }
        });
        category.Level = resCategory.level;
        category.AllowAdd = category.Level < MAX_LEVEL;
        if (category.SubItems) {
            $.each(category.SubItems, function (index, value) {
                value.ParentId = category.Id;
                initCategory(value);
            });
        }
    };

    function getCategoryWithLevel(category, id, level)
    {
        var categoryForSearch = null;
        if (category.Id == id) {
            categoryForSearch = category;
        };
        if (categoryForSearch != null) {
            return { category: categoryForSearch, level: level };
        }
        else
        {
            level = level + 1;
            $.each(category.SubItems, function (index, value) {
                if (value.Id == id) {
                    categoryForSearch = value;
                    return false;
                }
            });
        }

        if (categoryForSearch != null) {
            return { category: categoryForSearch, level: level};
        }
        else {
            var tempResult = null;
            $.each(category.SubItems, function (index, value) {
                var result = getCategoryWithLevel(value, id, level);
                if (result)
                {
                    tempResult = result;
                    return false;
                }
            });
            if (tempResult) {
                return tempResult;
            };
        }

        return null;
    }
        
    function removeCategoryFromTree(id) {
        var indexForRemove = null;
        $.each($scope.rootCategories, function (index, category) {
            if (category.Id == id) {
                indexForRemove = index;
                return false;
            }
            removeCategory(category, id);
        });

        if (indexForRemove != null) {
            $scope.rootCategories.splice(indexForRemove, 1);
        }

        $.each($scope.rootCategories, function (index, category) {
            initCategory(category);
        });
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

    function updateCategory(category, addInCategoryId,addToRoot) {
        if (addToRoot) {
            category.SubItems = [];
            $scope.rootCategories.push(category);
        }
        else
        {
            var categoryIdForSearch = addInCategoryId;
            if (!addInCategoryId) {
                categoryIdForSearch = category.Id;
            }

            var categoryForSearch = null;
            var level = 1;
            $.each($scope.rootCategories, function (index, value) {
                var result = getCategoryWithLevel(value, categoryIdForSearch, level);
                if (result) {
                    categoryForSearch = result;
                    return false;
                }
            });
            if (categoryForSearch) {
                categoryForSearch = categoryForSearch.category;
            }

            if (addInCategoryId) {
                category.SubItems = [];
                categoryForSearch.SubItems.push(category);
            }
            else {
                categoryForSearch.Name = category.Name;
            }
        }

        //reinit all categories
        $.each($scope.rootCategories, function (index, category) {
            initCategory(category);
        });
    };

    $scope.edit = function (id) {
        modalUtil.open('app/modules/product/partials/inventoryCategoryDetail.html', 'inventoryCategoryManageController', {
            id: id, thenCallback: function (data) {
                updateCategory(data);
            }
        });
    };

    $scope.add = function (categoryid)
    {
        modalUtil.open('app/modules/product/partials/inventoryCategoryDetail.html', 'inventoryCategoryManageController', {
            categoryid: categoryid, thenCallback: function (data) {
                updateCategory(data, categoryid, categoryid == null);
            }
        });
    }

    $scope.delete = function (id) {
        confirmUtil.confirm(function () {
            productService.deleteInventoryCategory(id, $scope.deleteTracker)
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
        productService.updateInventoryCategoriesTree($scope.rootCategories, $scope.editTracker)
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

    $scope.treeOptions = {
        accept: function (sourceNode, destNodes, destIndex) {
            var data = sourceNode.$modelValue;
            var destType = destNodes.$element.attr('data-type');
            return ((destType == 0 && data.ParentId==null) || data.ParentId == destType); // only accept the same type
        }
    };

    initialize();
}]);