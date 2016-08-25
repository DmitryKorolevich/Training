angular.module('app.modules.product.controllers.manageProductsOrderController', [])
.controller('manageProductsOrderController', ['$scope', '$rootScope', '$state', '$uibModalInstance', 'productService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil', 'data',
    function ($scope, $rootScope, $state, $uibModalInstance, productService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil, data)
    {
        $scope.refreshTracker = promiseTracker("refresh");

        function errorHandler(result)
        {
            var messages = "";
            if (result.Messages)
            {
                $.each(result.Messages, function (index, value)
                {
                    messages += value.Message + "<br />";
                });
            }
            toaster.pop('error', "Error!", messages, null, 'trustedHtml');
        };

        function refreshItems()
        {
            productService.getProductsOnCategoryOrder($scope.idCategory, $scope.refreshTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        $scope.items = result.Data;
                        addProductsListWatchers();
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

        function initialize()
        {

            $scope.forms = {};

            $scope.items = [];
            $scope.idCategory = data.idCategory;

            refreshItems();
        }

        $scope.save = function ()
        {
            productService.updateProductsOnCategoryOrder($scope.idCategory, $scope.items, $scope.refreshTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        data.thenCallback();
                        $uibModalInstance.close();
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

        $scope.sortableOptions = {
            handle: ' .sortable-move',
            items: ' .variant-panel',
            axis: 'y',
            start: function (e, ui)
            {
                $scope.dragging = true;
            },
            stop: function (e, ui)
            {
                $scope.dragging = false;
            }
        };

        $scope.deleteProduct = function (index)
        {
            $scope.items.splice(index, 1);
        };

        function notifyAboutAddBlockIds(name)
        {
            var blockIds = [];
            $.each($scope.items, function (index, item)
            {
                blockIds.push(item.ProductId);
            });
            var data = {};
            data.name = name;
            data.blockIds = blockIds;
            $scope.$broadcast('productsSearch#in#setBlockIds', data);
        };

        $scope.$on('productsSearch#out#addItems', function (event, args)
        {
            if (args.items)
            {
                $.each(args.items, function (index, item)
                {
                    var add = true;
                    $.each($scope.items, function (index, selectedProduct)
                    {
                        if (item.ProductId == selectedProduct.ProductId)
                        {
                            add = false;
                            return;
                        }
                    });
                    if (add)
                    {
                        var newSelectedProduct = angular.copy(item);
                        $scope.items.push(newSelectedProduct);
                    }
                });
            }
        });

        function addProductsListWatchers()
        {
            $scope.$watchCollection('items', function ()
            {
                notifyAboutAddBlockIds('Products');
            });
            notifyAboutAddBlockIds('Products');
        };

        $scope.cancel = function ()
        {
            $uibModalInstance.close();
        };

        initialize();
    }]);