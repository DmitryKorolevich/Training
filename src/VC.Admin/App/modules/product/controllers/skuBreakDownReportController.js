angular.module('app.modules.product.controllers.skuBreakDownReportController', [])
.controller('skuBreakDownReportController', ['$scope', '$rootScope', '$state', '$q', 'productService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
    function ($scope, $rootScope, $state, $q, productService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil)
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

            return data;
        };

        function refreshItems()
        {
            productService.getSkuBreakDownReportItems(getFilterData(), $scope.refreshTracker)
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
                To: currentDate.shiftDate('+1d'),
                From: currentDate.shiftDate('-3d'),
            };

            $scope.options.exportUrl = productService.getSkuBreakDownReportItemsReportFile(getFilterData(), $rootScope.buildNumber);
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
                $scope.options.exportUrl = productService.getSkuBreakDownReportItemsReportFile(getFilterData(), $rootScope.buildNumber);
            }
            else
            {
                $scope.forms.form.submitted = true;
            }
        };

        $scope.openOrders = function (idsku)
        {
            var data = {};
            data.idsku = idsku;
            if ($scope.filter.From)
            {
                data.from = $scope.filter.From.toQueryParamDateTime();
            }
            if ($scope.filter.To)
            {
                data.to = $scope.filter.To.toQueryParamDateTime();
            }
            $state.go('index.oneCol.manageOrders', data);
        };

        initialize();
    }]);