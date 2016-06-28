angular.module('app.modules.product.controllers.skuPricesController', [])
.controller('skuPricesController', ['$scope', '$rootScope', '$state', 'productService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
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

        function refreshItems() {
            productService.getSkusPrices($scope.filter, $scope.refreshTracker)
                .success(function (result) {
                    if (result.Success)
                    {
                        $.each(result.Data.Items, function (index, item)
                        {
                            item.NewPrice = item.Price;
                            item.NewWholesalePrice = item.WholesalePrice;
                        });
                        $scope.items = result.Data.Items;
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
            $scope.forms={};

            $scope.filter = {
                SearchText: null,
                Paging: { PageIndex: 1, PageItemCount: 100 },
            };

            refreshItems();
        }

        $scope.filterItems = function ()
        {
            $scope.filter.Paging.PageIndex = 1;
            refreshItems();
        };

        $scope.pageChanged = function () {
            refreshItems();
        };

        $scope.update = function ()
        {
            if ($scope.forms.form.$valid)
            {
                var skusForUpdate = [];
                $.each($scope.items, function (index, item)
                {
                    if (item.NewPrice != item.Price || item.NewWholesalePrice != item.WholesalePrice)
                    {
                        var skuForUpdate = angular.copy(item);
                        skuForUpdate.Price = skuForUpdate.NewPrice;
                        skuForUpdate.WholesalePrice = skuForUpdate.NewWholesalePrice;
                        skusForUpdate.push(skuForUpdate);
                    }
                });
                if (skusForUpdate.length > 0)
                {
                    productService.updateSkusPrices(skusForUpdate, $scope.refreshTracker).success(function (result)
                    {
                        refreshItems();
                        toaster.pop('success', "Success!", "Successfully saved.");
                    }).error(function (result)
                    {
                        errorHandler(result);
                    });
                }
            }
            else
            {
                $scope.forms.submitted = true;
            }
        };

        initialize();
    }]);