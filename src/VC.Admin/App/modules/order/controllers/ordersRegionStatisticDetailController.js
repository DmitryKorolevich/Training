angular.module('app.modules.order.controllers.ordersRegionStatisticDetailController', [])
.controller('ordersRegionStatisticDetailController', ['$scope', '$rootScope', '$state', '$stateParams', 'orderService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
    function ($scope, $rootScope, $state, $stateParams, orderService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil)
    {
        $scope.refreshTracker = promiseTracker("refresh");

        function errorHandler(result)
        {
            toaster.pop('error', "Error!", "Server error occured");
        };

        function prepareFilter()
        {
            var data = {};
            angular.copy($scope.filter, data);
            if (data.From)
            {
                data.From = data.From.toServerDateTime();
            }
            if (data.To)
            {
                data.To = data.To.toServerDateTime();
            }
            if (data.RegionType == 1)
            {
                data.Zip = null;
            }
            else
            {
                data.Region = null;
            }
            return data;
        }

        function refreshItems()
        {
            var data = prepareFilter();
            orderService.getOrderWithRegionInfoItems(data, $scope.refreshTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        $scope.items = result.Data.Items
                        $scope.options.TotalItems = result.Data.Count;
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

        function refreshTotal()
        {
            $scope.options.Total = null;
            var data = prepareFilter();
            orderService.getOrderWithRegionInfoAmount(data, $scope.refreshTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        $scope.options.Total = result.Data;
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
            var region = $stateParams.region;
            var zip = $stateParams.zip;
            var from = $stateParams.from;
            var to = $stateParams.to;
            var idcustomertype = $stateParams.idcustomertype ? $stateParams.idcustomertype : null;
            var idordertype = $stateParams.idordertype ? $stateParams.idordertype : null;
            var regionType = 1;
            if (region)
            {
                zip = null;
                regionType = 1;
            }
            if (zip)
            {
                region = null;
                regionType = 2;
            }
            
            $scope.regionTypes = [
                { Key: 1, Text: 'State/Province' },
                { Key: 2, Text: 'Zip' }
            ];

            $scope.customerTypes = angular.copy($rootScope.ReferenceData.CustomerTypes);
            $scope.customerTypes.splice(0, 0, { Key: null, Text: 'All Customer Types' });

            $scope.orderTypes = angular.copy($rootScope.ReferenceData.POrderTypes);
            $scope.orderTypes.splice(0, 0, { Key: null, Text: 'All  P/NP Types' });

            $scope.forms = {};
            $scope.options = {};
            $scope.options.RegionType = 1;

            var currentDate = new Date();
            currentDate.setHours(0, 0, 0, 0);
            if (from)
            {
                from = Date.parseDateTime(from);
            }
            else
            {
                from = currentDate.shiftDate('-1m');
            }
            if (to)
            {
                to = Date.parseDateTime(to);
            }
            else
            {
                to = currentDate.shiftDate('+1d');
            }
            $scope.filter = {
                To: to,
                From: from,
                IdCustomerType: idcustomertype,
                IdOrderType: idordertype,
                RegionType: regionType,
                Region: region,
                Zip: zip,
                Paging: { PageIndex: 1, PageItemCount: 100 },
            };

            refreshItems();
            refreshTotal();
            $scope.filterChanged();
        }

        $scope.filterItems = function ()
        {
            if ($scope.forms.form.$valid)
            {
                $scope.filter.Paging.PageIndex = 1;
                refreshItems();
                refreshTotal();
            }
            else
            {
                $scope.forms.submitted = true;
            }
        };

        $scope.pageChanged = function ()
        {
            refreshItems();
        };

        $scope.filterChanged = function ()
        {
            var data = prepareFilter();
            $scope.options.exportUrl = orderService.getOrderWithRegionInfoItemsReportFile(data, $rootScope.buildNumber);
        };

        initialize();
    }]);