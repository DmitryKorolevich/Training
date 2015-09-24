'use strict';

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

                var data = {};
                angular.copy($scope.article, data);
                if (data.PublishedDate)
                {
                    data.PublishedDate = data.PublishedDate.toServerDateTime();
                }

                contentService.updateArticle(data, $scope.editTracker).success(function (result)
                {
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

        contentService.getCategoriesTree({ Type: 3 }, $scope.refreshTracker)//article categories
			.success(function (result) {
			    if (result.Success) {
			        $scope.rootCategory = result.Data;
			        refreshMasters();
			    } else {
			        errorHandler(result);
			    }
			}).
			error(function (result) {
			    errorHandler(result);
			});
    };

    function refreshArticle()
    {
        contentService.getArticle($scope.id, $scope.refreshTracker)
            .success(function (result)
            {
                if (result.Success)
                {
                    $scope.article = result.Data;
                    if ($scope.article.PublishedDate)
                    {
                        $scope.article.PublishedDate = Date.parseDateTime($scope.article.PublishedDate);
                    }
                    if ($scope.article.Url)
                    {
                        $scope.previewUrl = $scope.baseUrl.format($scope.article.Url);
                    }
                    if (!$scope.article.MasterContentItemId)
                    {
                        $scope.article.MasterContentItemId = $scope.MasterContentItemId;
                    };
                    setSelected($scope.rootCategory, $scope.article.CategoryIds);
                    //addProductsListWatchers();
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
        contentService.getMasterContentItems({ Type: 4 })//article
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
                    refreshArticle();
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

    //function notifyAboutAddBlockIds(name) {
    //    var blockIds = [];
    //    var list = $scope.article[name];
    //    $.each(list, function (index, item) {
    //        blockIds.push(item.IdProduct);
    //    });
    //    var data = {};
    //    data.name = name;
    //    data.blockIds = blockIds;
    //    $scope.$broadcast('productsSearch#in#setBlockIds', data);
    //};

    //$scope.$on('productsSearch#out#addItems', function (event, args) {
    //    var list = $scope.article[args.name];
    //    if (list) {
    //        if (args.items) {
    //            var newSelectedProducts = [];
    //            $.each(args.items, function (index, item) {
    //                var add = true;
    //                $.each(list, function (index, selectedProduct) {
    //                    if (item.Id == selectedProduct.IdProduct) {
    //                        add = false;
    //                        return;
    //                    }
    //                });
    //                if (add) {
    //                    var newSelectedProduct = {};
    //                    newSelectedProduct.IdProduct = item.ProductId;
    //                    newSelectedProduct.ShortProductInfo = {};
    //                    newSelectedProduct.ShortProductInfo.ProductName = item.Name;
    //                    newSelectedProducts.push(newSelectedProduct);
    //                }
    //            });
    //            $.each(newSelectedProducts, function (index, newSelectedProduct) {
    //                list.push(newSelectedProduct);
    //            });
    //        }
    //    }
    //});

    //function addProductsListWatchers() {
    //    $scope.$watchCollection('article.ArticlesToProducts', function () {
    //        notifyAboutAddBlockIds('ArticlesToProducts');
    //    });
    //    notifyAboutAddBlockIds('ArticlesToProducts');
    //};

    //$scope.deleteArticlesToProducts = function (index) {
    //    $scope.article.ArticlesToProducts.splice(index, 1);
    //};

    initialize();
}]);