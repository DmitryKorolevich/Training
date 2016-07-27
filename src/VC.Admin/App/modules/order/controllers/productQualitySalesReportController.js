angular.module('app.modules.order.controllers.productQualitySalesReportController', [])
.controller('productQualitySalesReportController', ['$scope', '$rootScope', '$state', '$q', 'orderService', 'settingService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
    function ($scope, $rootScope, $state, $q, orderService, settingService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil)
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
            orderService.getProductQualitySalesReportItems(getFilterData(), $scope.refreshTracker)
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
                From: currentDate.shiftDate('-3m'),
                Sorting: gridSorterUtil.resolve(refreshItems, "SalesPerIssue", "Asc"),
            };

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

        $scope.openDetails = function (skucode)
        {
            var data = {
                from: $scope.filter.From,
                to: $scope.filter.To,
                skucode: skucode
            };
            if (data.from)
            {
                data.from = data.from.toQueryParamDateTime();
            }
            if (data.to)
            {
                data.to = data.to.toQueryParamDateTime();
            }

            $state.go('index.oneCol.productQualitySkusReport', data);
        };

        initialize();
    }]);