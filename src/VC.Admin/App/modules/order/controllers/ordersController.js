angular.module('app.modules.order.controllers.ordersController',[])
.controller('ordersController', ['$scope', '$rootScope', '$state', 'orderService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
    function ($scope, $rootScope, $state, orderService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil)
    {
        $scope.refreshTracker = promiseTracker("refresh");
        $scope.deleteTracker = promiseTracker("delete");

        function errorHandler(result) {
            toaster.pop('error', "Error!", "Server error occured");
        };

        function refreshOrders() {
            orderService.getOrders($scope.filter, $scope.refreshTracker)
                .success(function (result) {
                    if (result.Success) {
                        $scope.items = result.Data.Items;
                        $.each($scope.items, function(index, item)
                        {
                            item.AllowExport = item.OrderStatus == 1 || item.OrderStatus == 2 || item.OrderStatus == 6 || item.OrderStatus == 7;
                            item.IsSelected = item.OrderStatus == 3;//Shipped
                        });
                        $scope.totalItems = result.Data.Count;
                    } else {
                        errorHandler(result);
                    }
                })
                .error(function (result) {
                    errorHandler(result);
                });
        };

        function initialize()
        {
            $scope.customerTypes = angular.copy($rootScope.ReferenceData.CustomerTypes);
            $scope.customerTypes.splice(0, 0, { Key: null, Text: 'All Customer Types' });

            $scope.orderStatuses = angular.copy($rootScope.ReferenceData.OrderStatuses);
            $scope.orderStatuses.splice(0, 0, { Key: null, Text: 'All Order Statuses' });

            $scope.orderTypes = angular.copy($rootScope.ReferenceData.OrderTypes);
            $scope.orderTypes.splice(0, 0, { Key: null, Text: 'All Order Types' });

            $scope.pOrderTypes = angular.copy($rootScope.ReferenceData.POrderTypes);
            $scope.pOrderTypes.splice(0, 0, { Key: null, Text: 'All  P/NP Types' });

            $scope.shippingMethods = [ 
                { Key: null, Text: 'All Shipping Methods' },
                {Key: 1, Text: 'Upgraded'},
                {Key: 2, Text: 'None'}
            ];
            $scope.settings = {};
            $scope.settings.allExport = false;

            $scope.forms = {};

            var currentDate = new Date();
            currentDate.setHours(0, 0, 0, 0);
            $scope.filter = {
                To: currentDate.shiftDate('+1d'),
                From: currentDate.shiftDate('-1m'),
                ShipDate: false,
                OrderStatus: null,
                IdOrderSource: null,
                POrderType: null,
                IdCustomerType: null,
                IdShippingMethod: null,
                Paging: { PageIndex: 1, PageItemCount: 100 },
                Sorting: gridSorterUtil.resolve(refreshOrders,"DateCreated","Desc")
            };

            $scope.$watch("filter.ShipDate", function (newValue, oldValue)
            {
                if (newValue)
                {
                    $scope.filter.OrderStatus = 3;//Shipped
                }
                else
                {
                    $scope.filter.OrderStatus = null;
                }
            });

            refreshOrders();
        }

        $scope.filterOrders = function ()
        {
            if ($scope.forms.form.$valid)
            {
                $scope.filter.Paging.PageIndex = 1;
                refreshOrders();
            }
            else
            {
                $scope.forms.form.submitted = true;
            }
        };

        $scope.pageChanged = function () {
            refreshOrders();
        };

        $scope.delete = function () {
        };

        $scope.allExportCall = function()
        {
            $.each($scope.items, function(index, item)
            {
                if (item.AllowExport)
                {
                    item.IsSelected = $scope.settings.allExport;
                }
            });
        };

        $scope.itemExportChanged = function (item)
        {
            if (!item.IsSelected && $scope.settings.allExport)
            {
                $scope.settings.allExport = false;
            }
        };

        $scope.exportOrders = function()
        {

        };

        initialize();
    }]);