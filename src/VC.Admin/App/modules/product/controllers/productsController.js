﻿angular.module('app.modules.product.controllers.productsController', [])
.controller('productsController', ['$scope', '$rootScope', '$state', 'productService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
    function ($scope, $rootScope, $state, productService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil) {
        $scope.refreshTracker = promiseTracker("refresh");
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

        function refreshProducts() {
            productService.getProducts($scope.filter, $scope.refreshTracker)
                .success(function (result) {
                    if (result.Success) {
                        $.each(result.Data.Items, function (index, item) {
                            if (item.Thumbnail) {
                                item.ThumbnailUrl = self.baseUrl.format(item.Thumbnail);
                            }
                        });
                        $scope.products = result.Data.Items;
                        $scope.totalItems = result.Data.Count;
                    } else {
                        errorHandler(result);
                    }
                })
                .error(function (result) {
                    errorHandler(result);
                });
        };

        function initialize() {
            self.baseUrl = 'https://' + $rootScope.ReferenceData.PublicHost + '{0}';

            $scope.filter = {
                SearchText: '',
                CategoryId: null,
                Paging: { PageIndex: 1, PageItemCount: 100 },
                Sorting: gridSorterUtil.resolve(refreshProducts, "Name", "Asc")
            };
        }

        $scope.filterProducts = function () {
            $scope.filter.Paging.PageIndex = 1;
            refreshProducts();
        };

        $scope.pageChanged = function () {
            refreshProducts();
        };

        $scope.open = function (id) {
            if (id) {
                $state.go('index.oneCol.productDetail', { id: id });
            }
            else {
                modalUtil.open('app/modules/product/partials/addProductPopup.html', 'addProductPopupController', {
                    thenCallback: function (data) {
                        $state.go('index.oneCol.addNewProduct', { type: data });
                    }
                });
            }
        };

        $scope.delete = function (id) {
            confirmUtil.confirm(function () {
                productService.deleteProduct(id, $scope.deleteTracker)
                    .success(function (result) {
                        if (result.Success) {
                            toaster.pop('success', "Success!", "Successfully deleted.");
                            refreshProducts();
                        } else {
                            errorHandler(result);
                        }
                    })
                    .error(function (result) {
                        errorHandler(result);
                    });
            }, 'Are you sure you want to delete this product?');
        };

        initialize();
    }]);