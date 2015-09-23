angular.module('app.modules.product.controllers.outOfStocksController', [])
.controller('outOfStocksController', ['$scope', '$rootScope', '$state', 'productService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
    function ($scope, $rootScope, $state, productService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil)
    {
        $scope.refreshTracker = promiseTracker("refresh");
        $scope.deleteTracker = promiseTracker("delete");

        function errorHandler(result)
        {
            toaster.pop('error', "Error!", "Server error occured");
        };

        function refreshItems()
        {
            productService.getProductOutOfStockContainers($scope.refreshTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        $scope.items = result.Data;
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

            $scope.filter = {};

            refreshItems();
        }

        $scope.filterItems = function ()
        {
            if ($scope.forms.form.$valid)
            {
                refreshItems();
            }
            else
            {
                $scope.forms.form.submitted = true;
            }
        };

        $scope.send = function ()
        {
            confirmUtil.confirm(function ()
            {
                var ids = [];
                $.each($scope.items, function (index, container)
                {
                    $.each(container.Requests, function (index, item)
                    {
                        if (item.IsSelected)
                        {
                            ids.push(item.Id);
                        }
                    });
                });
                productService.sendProductOutOfStockRequests(ids, $scope.deleteTracker)
                    .success(function (result)
                    {
                        if (result.Success)
                        {
                            toaster.pop('success', "Success!", "Successfully sent.");
                            refreshItems();
                        } else
                        {
                            errorHandler(result);
                        }
                    })
                    .error(function (result)
                    {
                        errorHandler(result);
                    });
            }, 'Are you sure you want to send these emails?');
        };

        $scope.containerIsSelectedChanged = function (container)
        {
            $.each(container.Requests, function (index, item)
            {
                item.IsSelected = container.IsSelected;
            });
        };

        initialize();
    }]);