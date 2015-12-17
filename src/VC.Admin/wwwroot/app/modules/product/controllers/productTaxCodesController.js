﻿angular.module('app.modules.product.controllers.productTaxCodesController', [])
.controller('productTaxCodesController', ['$scope', '$rootScope', '$state', 'productService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
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
                    if (result.Success)
                    {
                        $.each(result.Data.Items, function (index, item)
                        {
                            item.NewTaxCode = item.TaxCode;
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
            self.baseUrl = $rootScope.ReferenceData.PublicHost.substring(0, $rootScope.ReferenceData.PublicHost.length - 1) + '{0}';

            $scope.filter = {
                SearchText: '',
                CategoryId: null,
                Paging: { PageIndex: 1, PageItemCount: 100 },
                Sorting: gridSorterUtil.resolve(refreshProducts, "Name", "Asc")
            };

            refreshProducts();
        }

        $scope.filterProducts = function () {
            $scope.filter.Paging.PageIndex = 1;
            refreshProducts();
        };

        $scope.pageChanged = function () {
            refreshProducts();
        };

        $scope.update = function ()
        {
            var productsForUpdate = [];
            $.each($scope.products, function (index, item)
            {
                if (item.NewTaxCode != item.TaxCode)
                {
                    var productForUpdate = angular.copy(item);
                    productForUpdate.TaxCode = productForUpdate.NewTaxCode;
                    productsForUpdate.push(productForUpdate);
                }
            });
            if (productsForUpdate.length>0)
            {
                productService.updateProductTaxCodes(productsForUpdate, $scope.refreshTracker).success(function (result)
                {
                    refreshProducts();
                }).error(function (result) {
                    errorHandler(result);
                });
            }
        };

        initialize();
    }]);