angular.module('app.modules.healthwise.controllers.healthwisePeriodsController', [])
.controller('healthwisePeriodsController', ['$scope', '$rootScope', '$state', 'healthwiseService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
    function ($scope, $rootScope, $state, healthwiseService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil)
    {
        $scope.refreshTracker = promiseTracker("refresh");

        function errorHandler(result)
        {
            toaster.pop('error', "Error!", "Server error occured");
        };

        function refreshItems()
        {
            var data = angular.copy($scope.filter, data);
            if (data.From)
            {
                data.From = data.From.toServerDateTime();
            }
            if (data.To)
            {
                data.To = data.To.toServerDateTime();
            }
            healthwiseService.getHealthwiseCustomersWithPeriods(data, $scope.refreshTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        $scope.dbItems = result.Data;
                        $scope.totalItems = $scope.dbItems.length;
                        clientItemsFilter();
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

            var currentDate = new Date();
            currentDate.setHours(0, 0, 0, 0);
            $scope.filter = {
                To: currentDate.shiftDate('+1y'),
                From: currentDate.shiftDate('-1y'),
                NotBilledOnly: false,
                Paging: { PageIndex: 1, PageItemCount: 100 },
            };

            refreshItems();
        }

        $scope.filterItems = function ()
        {
            $scope.filter.PageIndex = 1;
            refreshItems();
        };

        $scope.pageChanged = function ()
        {
            clientItemsFilter();
        };

        var clientItemsFilter = function ()
        {
            var items = [];
            var from = ($scope.filter.Paging.PageIndex - 1) * $scope.filter.Paging.PageItemCount;
            var max = from + $scope.filter.Paging.PageItemCount > $scope.dbItems.length ? $scope.dbItems.length
                : from + $scope.filter.Paging.PageItemCount;
            for (var i = from; i < max; i++)
            {
                items.push($scope.dbItems[i]);
            }
            $scope.items = items;
        };
        
        $scope.flagOrder = function ()
        {
            modalUtil.open('app/modules/healthwise/partials/markOrderPopup.html', 'markOrderController', {
                thenCallback: function (data)
                {
                    $scope.filterItems();
                }
            });
        };

        $scope.flagCustomer = function ()
        {
            modalUtil.open('app/modules/healthwise/partials/markCustomerOrdersPopup.html', 'markCustomerOrdersController', {
                thenCallback: function (data)
                {
                    $scope.filterItems();
                }
            });
        };

        initialize();
    }]);