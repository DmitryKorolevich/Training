angular.module('app.modules.order.controllers.orderSkuCountReportController', [])
.controller('orderSkuCountReportController', ['$scope', '$rootScope', '$state', '$q', 'orderService', 'productService', 'discountService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
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
            orderService.getOrderSkuCountReport(getFilterData(), $scope.refreshTracker)
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

        function initialize()
        {
            $scope.options = {};

            $scope.forms = {};

            $scope.skusFilter = {
                Code: '',
                Paging: { PageIndex: 1, PageItemCount: 20 },
            };

            var currentDate = new Date();
            currentDate.setHours(0, 0, 0, 0);
            $scope.filter = {
                To: currentDate.shiftDate('+1d'),
                From: currentDate.shiftDate('-3m'),
                Unique: false,
                SkuCode: null,
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

        $scope.pageChanged = function ()
        {
            refreshItems();
        };

        $scope.getSKUsBySKU = function (val)
        {
            if (val)
            {
                $scope.skusFilter.Code = val;
                return productService.getSkus($scope.skusFilter)
                    .then(function (result)
                    {
                        return result.data.Data.map(function (item)
                        {
                            return item;
                        });
                    });
            }
        };

        initialize();
    }]);