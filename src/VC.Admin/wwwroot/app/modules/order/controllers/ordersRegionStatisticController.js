angular.module('app.modules.order.controllers.ordersRegionStatisticController', [])
.controller('ordersRegionStatisticController', ['$scope', '$rootScope', '$state', 'orderService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
    function ($scope, $rootScope, $state, orderService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil)
    {
        $scope.refreshTracker = promiseTracker("refresh");

        function errorHandler(result)
        {
            toaster.pop('error', "Error!", "Server error occured");
        };

        function refreshItems()
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
                orderService.getOrdersRegionStatistic(data, $scope.refreshTracker)
                    .success(function (result)
                    {
                        if (result.Success)
                        {
                            $scope.items = result.Data;
                            $scope.options.RegionType = $scope.filter.RegionType;
                        } else
                        {
                            errorHandler(result);
                        }
                    })
                    .error(function (result)
                    {
                        errorHandler(result);
                    });
            }
            else
            {
                orderService.getOrdersZipStatistic(data, $scope.refreshTracker)
                    .success(function (result)
                    {
                        if (result.Success)
                        {
                            $scope.items = result.Data;
                            $scope.options.RegionType = $scope.filter.RegionType;
                        } else
                        {
                            errorHandler(result);
                        }
                    })
                    .error(function (result)
                    {
                        errorHandler(result);
                    });
            }
        };

        function initialize()
        {
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
            $scope.filter = {
                To: currentDate.shiftDate('+1d'),
                From: currentDate.shiftDate('-1m'),
                IdCustomerType: null,
                IdOrderType: null,
                RegionType: 1,
            };

            refreshItems();
            $scope.filterChanged();
        }

        $scope.filterItems = function ()
        {
            if ($scope.forms.form.$valid)
            {
                refreshItems();
            }
            else
            {
                $scope.forms.submitted = true;
            }
        };

        $scope.filterChanged = function ()
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
                $scope.options.exportUrl = orderService.getOrdersRegionStatisticReportFile(data, $rootScope.buildNumber);
            }
            else
            {
                $scope.options.exportUrl = orderService.getOrdersZipStatisticReportFile(data, $rootScope.buildNumber);
            }
        };

        $scope.openByRegion = function (region)
        {
            if (!$scope.forms.form.$valid)
            {
                $scope.forms.submitted = true;
                return;
            }
            var data = prepareStateParams();
            data.region = region;
            $state.go('index.oneCol.ordersRegionStatisticDetail', data);
        };

        $scope.openByZip = function (zip)
        {
            if (!$scope.forms.form.$valid)
            {
                $scope.forms.submitted = true;
                return;
            }
            var data = prepareStateParams();
            data.zip = zip;
            $state.go('index.oneCol.ordersRegionStatisticDetail', data);
        };

        function prepareStateParams()
        {
            var data = {
                from: $scope.filter.From,
                to: $scope.filter.To,
            };
            if (data.from)
            {
                data.from = data.from.toQueryParamDateTime();
            }
            if (data.to)
            {
                data.to = data.to.toQueryParamDateTime();
            }
            if ($scope.filter.IdCustomerType)
            {
                data.idordertype = $scope.filter.IdCustomerType;
            }
            if ($scope.filter.IdOrderType)
            {
                data.idordertype = $scope.filter.IdOrderType;
            }
            return data;
        };

        initialize();
    }]);