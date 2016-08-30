angular.module('app.modules.order.controllers.wholesaleDropShipReportController', [])
.controller('wholesaleDropShipReportController', ['$scope', '$rootScope', '$state', '$q', 'orderService', 'userService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
    function ($scope, $rootScope, $state, $q, orderService, userService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil)
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

            return data;
        };

        function refreshStatistic()
        {
            orderService.getWholesaleDropShipReport(getFilterData(), $scope.refreshTracker)
                .success(function (result) {
                    if (result.Success) {
                        $scope.report = result.Data;
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
            orderService.getOrdersForWholesaleDropShipReport(getFilterData(), $scope.refreshTracker)
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

            $scope.tradeClasses = angular.copy($rootScope.ReferenceData.TradeClasses);
            $scope.tradeClasses.splice(0, 0, { Key: null, Text: 'All Trade Classes' });

            $scope.options = {};

            $scope.forms = {};

            var currentDate = new Date();
            currentDate.setHours(0, 0, 0, 0);
            $scope.filter = {
                To: currentDate.shiftDate('+1d'),
                From: currentDate.shiftDate('-1m'),
                IdCustomerType: 2,//wholesale
                IdTradeClass: 6,//Retail - Drop Ship
                CustomerCompany: null,
                CustomerFirstName: null,
                CustomerLastName: null,
                ShipFirstName: null,
                ShipLastName: null,
                ShippingIdConfirmation: null,
                IdOrder: null,
                PoNumber: null,
                Paging: { PageIndex: 1, PageItemCount: 100 },
            };

            $scope.options.exportUrl = orderService.getOrdersForWholesaleDropShipReportFile(getFilterData(), $rootScope.buildNumber);
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
                $scope.options.exportUrl = orderService.getOrdersForWholesaleDropShipReportFile(getFilterData(), $rootScope.buildNumber);
            }
            else
            {
                $scope.forms.form.submitted = true;
            }
        };

        initialize();
    }]);