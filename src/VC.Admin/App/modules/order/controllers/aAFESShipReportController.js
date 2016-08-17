angular.module('app.modules.order.controllers.aAFESShipReportController', [])
.controller('aAFESShipReportController', ['$scope', '$rootScope', '$state', '$q', 'orderService', 'productService', 'discountService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
    function ($scope, $rootScope, $state, $q, orderService, productService, discountService , toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil)
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

        function refreshItems()
        {
            orderService.getAAFESReportItems(getFilterData(), $scope.refreshTracker)
                .success(function (result) {
                    if (result.Success) {
                        $scope.items = result.Data;
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
            $scope.options = {};

            $scope.forms = {};

            var currentDate = new Date();
            currentDate.setHours(0, 0, 0, 0);
            $scope.filter = {
                ShipFrom: currentDate.shiftDate('-1d'),
                ShipTo: currentDate.shiftDate('+1d'),
            };

            $scope.options.exportUrl = orderService.getAAFESReportItemsReportFile(getFilterData(), $rootScope.buildNumber);
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

        $scope.filterChanged = function ()
        {
            if ($scope.forms.form.$valid)
            {
                $scope.options.exportUrl = orderService.getAAFESReportItemsReportFile(getFilterData(), $rootScope.buildNumber);
            }
            else
            {
                $scope.forms.form.submitted = true;
            }
        };

        initialize();
    }]);