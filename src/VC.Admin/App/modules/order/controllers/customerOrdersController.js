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
                        errorHandler(result);
                    }
                })
                .error(function (result)
                {
                    errorHandler(result);
                });
        };

        initialize();
    }]);