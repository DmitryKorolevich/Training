angular.module('app.modules.product.controllers.skusSearchController', [])
.controller('skusSearchController', ['$scope', '$rootScope', '$state', 'productService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
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

        function refreshSkus() {
            productService.getSkus($scope.filter, $scope.refreshTracker)
                .success(function (result) {
                    if (result.Success) {
                        $scope.skus = result.Data;
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
            $.each($scope.skus, function (index, sku)
            {
                var add=true;
                $.each($scope.blockIds, function (index, blockId) 
                {
                    if (sku.Id == blockId)
                    {
                        add = false;
                        return;
                    }
                });
                if (add) {
                    items.push(sku);
                };
            });
            $scope.items = items;
        };

        function initialize() {
            $scope.blockIds = [];
            $scope.items = [];
            $scope.skus = [];

            $scope.forms = {};

            $scope.filter = {
                SearchText: '',
            };
        }

        $scope.$on('skusSearch#in#init', function (event, args)
        {
            if (args.name == $scope.name)
            {
                $scope.filter.IdProductTypes = args.IdProductTypes;
            };
        });

        $scope.$on('skusSearch#in#setBlockIds', function (event, args) {
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
                refreshSkus();
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
            $scope.$emit('skusSearch#out#addItems', data);
        };

        initialize();
    }]);