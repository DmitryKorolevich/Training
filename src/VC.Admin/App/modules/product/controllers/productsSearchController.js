angular.module('app.modules.product.controllers.productsSearchController', [])
.controller('productsSearchController', ['$scope', '$rootScope', '$state', 'productService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
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
                        $scope.products = result.Data.Items;
                        refreshItems();
                    } else {
                        errorHandler(result);
                    }
                })
                .error(function (result) {
                    errorHandler(result);
                });
        };

        function refreshItems()
        {
            var items = [];
            $.each($scope.products, function (index, product)
            {
                var add=true;
                $.each($scope.blockIds, function (index, blockId) 
                {
                    if (product.ProductId == blockId)
                    {
                        add = false;
                        return;
                    }
                });
                if (add) {
                    items.push(product);
                };
            });
            $scope.items = items;
        };

        function initialize() {
            $scope.blockIds = [];
            $scope.items = [];
            $scope.products = [];

            $scope.forms = {};

            $scope.filter = {
                SearchText: '',
            };
        }

        $scope.$on('productsSearch#in#setBlockIds', function (event, args) {
            if (args.name == $scope.name) {
                var blockIds = args.blockIds;
                if (blockIds) {
                    $scope.blockIds = blockIds;
                    refreshItems();
                }
            };
        });

        $scope.filterSkus = function () {
            if ($scope.forms.filterForm.$valid) {
                refreshProducts();
            }
            else
            {
                $scope.submitted = true;
            }
        };

        $scope.add = function (item) {
            notifyAboutAddItems([item]);
        };

        function notifyAboutAddItems(items)
        {
            var data = {};
            data.name = $scope.name;
            data.items = items;
            $scope.$emit('productsSearch#out#addItems', data);
        };

        initialize();
    }]);