angular.module('app.modules.order.controllers.customerOrdersController', [])
.controller('customerOrdersController', ['$scope', '$rootScope', '$state', 'orderService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
    function ($scope, $rootScope, $state, orderService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil)
    {
        $scope.$on('customerOrders#in#init', function (event, args)
        {
            $scope.idCustomer = args.idCustomer;
            $scope.ordersFilter = {
                idCustomer: $scope.idCustomer,
                Paging: { PageIndex: 1, PageItemCount: 20 },
                Sorting: gridSorterUtil.resolve(refreshOrdersHistory, "DateCreated", "Desc")
            };
            if ($scope.idCustomer)
            {
                refreshOrdersHistory();
            }
        });

        $scope.$on('customerOrders#in#refresh', function (event, args)
        {
            $scope.ordersFilter.Paging.PageIndex = 1;
            refreshOrdersHistory();
        });

        function initialize()
        {

        };

        $scope.ordersPageChanged = function ()
        {
            refreshOrdersHistory();
        };

        function refreshOrdersHistory()
        {
            orderService.getOrders($scope.ordersFilter, $scope.addEditTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        $scope.ordersHistory = result.Data.Items;
                        $scope.ordersTotalItems = result.Data.Count;
                    } else
                    {
                        toaster.pop('error', 'Error!', "Can't process order history");
                    }
                })
                .error(function (result)
                {
                    toaster.pop('error', 'Error!', "Can't process order history");
                });
        };

        initialize();
    }]);