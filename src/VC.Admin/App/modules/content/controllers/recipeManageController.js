'use strict';

angular.module('app.modules.content.controllers.recipeManageController', [])
.controller('recipeManageController', ['$scope', '$rootScope', '$state', '$stateParams', 'contentService', 'toaster', 'confirmUtil', 'promiseTracker',
    function ($scope, $rootScope, $state, $stateParams, contentService, toaster, confirmUtil, promiseTracker) {
        $scope.refreshTracker = promiseTracker("get");
        $scope.editTracker = promiseTracker("edit");

        function successSaveHandler(result) {
            if (result.Success) {
                toaster.pop('success', "Success!", "Successfully saved.");
                $scope.id = result.Data.Id;
                $scope.recipe.Id = result.Data.Id;
                $scope.recipe.MasterContentItemId = result.Data.MasterContentItemId;
                $scope.previewUrl = $scope.baseUrl.format($scope.recipe.Url);
            } else {
                var messages = "";
                if (result.Messages) {
                    $scope.forms.recipeForm.submitted = true;
                    $scope.detailsTab.active = true;
                    $scope.serverMessages = new ServerMessages(result.Messages);
                    $.each(result.Messages, function (index, value) {
                        if (value.Field) {
                            $scope.forms.recipeForm[value.Field].$setValidity("server", false);
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

            $scope.baseUrl = $rootScope.ReferenceData.PublicHost + 'recipe/{0}?preview=true';
            $scope.previewUrl = null;

            $scope.detailsTab = {
                active: true
            };
            $scope.forms = {};

            $scope.save = function () {
                $.each($scope.forms.recipeForm, function (index, element) {
                    if (element && element.$name == index) {
                        element.$setValidity("server", true);
                    }
                });

                if ($scope.forms.recipeForm.$valid) {
                    var categoryIds = [];
                    getSelected($scope.rootCategory, categoryIds);
                    $scope.recipe.CategoryIds = categoryIds;

                    contentService.updateRecipe($scope.recipe, $scope.editTracker).success(function (result) {
                        successSaveHandler(result);
                    }).
                        error(function (result) {
                            errorHandler(result);
                        });
                } else {
                    $scope.forms.recipeForm.submitted = true;
                    $scope.detailsTab.active = true;
                }
            };

            contentService.getCategoriesTree({ Type: 1 }, $scope.refreshTracker)//recipe categories
                .success(function (result) {
                    if (result.Success) {
                        $scope.rootCategory = result.Data;
                        contentService.getRecipe($scope.id, $scope.refreshTracker)
			                .success(function (result) {
			                    if (result.Success) {
			                        $scope.recipe = result.Data;
			                        if ($scope.recipe.Url) {
			                            $scope.previewUrl = $scope.baseUrl.format($scope.recipe.Url);
			                        };
			                        setSelected($scope.rootCategory, $scope.recipe.CategoryIds);
			                        addProductsListWatchers();
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

        function setSelected(category, ids) {
            category.IsSelected = false;
            $.each(ids, function (index, id) {
                if (category.Id == id) {
                    category.IsSelected = true;
                }
            });
            $.each(category.SubItems, function (index, value) {
                setSelected(value, ids)
            });
        }

        function getSelected(category, ids) {
            if (category.IsSelected) {
                ids.push(category.Id);
            }
            $.each(category.SubItems, function (index, value) {
                getSelected(value, ids)
            });
        }

        $scope.goToMaster = function (id) {
            $state.go('index.oneCol.masterDetail', { id: id });
        };

        function notifyAboutAddBlockIds(name) {
            var blockIds = [];
            var list = $scope.recipe[name];
            $.each(list, function (index, item) {
                blockIds.push(item.IdProduct);
            });
            var data = {};
            data.name = name;
            data.blockIds = blockIds;
            $scope.$broadcast('productsSearch#in#setBlockIds', data);
        };

        $scope.$on('productsSearch#out#addItems', function (event, args) {
            var list = $scope.recipe[args.name];
            if (list) {
                if (args.items) {
                    var newSelectedProducts = [];
                    $.each(args.items, function (index, item) {
                        var add = true;
                        $.each(list, function (index, selectedProduct) {
                            if (item.Id == selectedProduct.IdProduct) {
                                add = false;
                                return;
                            }
                        });
                        if (add) {
                            var newSelectedProduct = {};
                            newSelectedProduct.IdProduct = item.ProductId;
                            newSelectedProduct.ShortProductInfo = {};
                            newSelectedProduct.ShortProductInfo.ProductName = item.Name;
                            newSelectedProducts.push(newSelectedProduct);
                        }
                    });
                    $.each(newSelectedProducts, function (index, newSelectedProduct) {
                        list.push(newSelectedProduct);
                    });
                }
            }
        });

        function addProductsListWatchers() {
            $scope.$watchCollection('recipe.RecipesToProducts', function () {
                notifyAboutAddBlockIds('RecipesToProducts');
            });
            notifyAboutAddBlockIds('RecipesToProducts');
        };

        $scope.deleteRecipesToProducts = function (index) {
            $scope.recipe.RecipesToProducts.splice(index, 1);
        };

        initialize();
    }]);