angular.module('app.modules.customer.controllers.wholesaleSummaryReportController', [])
.controller('wholesaleSummaryReportController', ['$scope', '$rootScope', '$state', 'customerService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
    function ($scope, $rootScope, $state, customerService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil)
    {
        $scope.refreshTracker = promiseTracker("refresh");
        $scope.refreshItemsTracker = promiseTracker("refreshItems");        

        function errorHandler(result) {
            var messages = "";
            if (result.Messages) {
                $.each(result.Messages, function (index, value) {
                    messages += value.Message + "<br />";
                });
            }
            toaster.pop('error', "Error!", messages, null, 'trustedHtml');
        };

        function refreshSummary()
        {
            customerService.getWholesaleSummaryReport($scope.refreshTracker)
                .success(function (result) {
                    if (result.Success) {
                        $scope.summary = result.Data;
                    } else {
                        errorHandler(result);
                    }
                })
                .error(function (result) {
                    errorHandler(result);
                });
        };

        function refreshItemsForMonths()
        {
            customerService.getWholesaleSummaryReportMonthStatistic($scope.options.count, $scope.options.include, $scope.refreshItemsTracker)
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
            $scope.months = [3, 6, 9, 12];
            $scope.options = {};
            $scope.options.include = true;
            $scope.options.count=3;

            refreshSummary();
            refreshItemsForMonths();
        }

        $scope.refresh = function ()
        {
            refreshItemsForMonths();
        };

        initialize();
    }]);