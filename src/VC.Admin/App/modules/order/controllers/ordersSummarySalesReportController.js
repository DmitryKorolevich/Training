angular.module('app.modules.order.controllers.ordersSummarySalesReportController', [])
.controller('ordersSummarySalesReportController', ['$scope', '$rootScope', '$state', '$q', 'orderService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
    function ($scope, $rootScope, $state, $q, orderService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil)
    {
        $scope.refreshTracker = promiseTracker("refresh");

        function errorHandler(result) {
            var messages = "";
            if (result.Messages) {
                $.each(result.Messages, function (index, value) {
                    messages += value.Message + "<br />";
                });
            }
            toaster.pop('error', "Error!", messages, null, 'trustedHtml');
        };

        function getFilterData(){
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
            if (data.ShipFrom)
            {
                data.ShipFrom = data.ShipFrom.toServerDateTime();
            }
            if (data.ShipTo)
            {
                data.ShipTo = data.ShipTo.toServerDateTime();
            }
            if (data.FirstOrderFrom)
            {
                data.FirstOrderFrom = data.FirstOrderFrom.toServerDateTime();
            }
            if (data.FirstOrderTo)
            {
                data.FirstOrderTo = data.ShipTo.FirstOrderTo();
            }
            if (data.CustomerSourceDetails = 'All')
            {
                data.CustomerSourceDetails = null;
            }

            return data;
        };

        function refreshStatistic()
        {
            orderService.getOrdersSummarySalesOrderTypeStatisticItems(getFilterData(), $scope.refreshTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        $scope.statisticItems = result.Data;
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

        function refreshItems()
        {
            orderService.getOrdersSummarySalesOrderItems(getFilterData(), $scope.refreshTracker)
                .success(function (result) {
                    if (result.Success) {
                        $scope.items = result.Data.Items;
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

            $scope.orderSources = angular.copy($rootScope.ReferenceData.OrderSources);
            $scope.orderSources.splice(0, 0, { Key: null, Text: 'All Order Sources' });

            $scope.orderSourcesCelebrityHealthAdvocate = angular.copy($rootScope.ReferenceData.OrderSourcesCelebrityHealthAdvocate);
            $scope.orderSourcesCelebrityHealthAdvocate.splice(0, 0, { Key: null, Text: 'All' });

            $scope.options = {};

            $scope.forms = {};

            var currentDate = new Date();
            currentDate.setHours(0, 0, 0, 0);
            $scope.filter = {
                To: currentDate.shiftDate('+1d'),
                From: currentDate.shiftDate('-1m'),
                IdCustomerType: 1,//retail
                IdCustomerSource: null,
                CustomerSourceDetails: null,
                Paging: { PageIndex: 1, PageItemCount: 100 },
            };

            $scope.options.exportUrl = orderService.getOrdersSummarySalesOrderItemsReportFile(getFilterData(), $rootScope.buildNumber);
            refreshStatistic();
            refreshItems();
        }

        $scope.filterItems = function ()
        {
            if ($scope.forms.form.$valid)
            {
                refreshStatistic();
                $scope.filter.Paging.PageIndex = 1;
                refreshItems();
            }
            else
            {
                $scope.forms.form.submitted = true;
            }
        };

        $scope.pageChanged = function ()
        {
            refreshItems();
        };

        $scope.filterChanged = function ()
        {
            if ($scope.forms.form.$valid)
            {
                $scope.options.exportUrl = orderService.getOrdersSummarySalesOrderItemsReportFile(getFilterData(), $rootScope.buildNumber);
            }
            else
            {
                $scope.forms.form.submitted = true;
            }
        };

        $scope.idCustomerSourceChanged = function ()
        {
            if ($scope.filter.IdCustomerSource != 4)
            {
                $scope.filter.CustomerSourceDetails = 'All';
            }
            $scope.filterChanged();
        };

        initialize();
    }]);