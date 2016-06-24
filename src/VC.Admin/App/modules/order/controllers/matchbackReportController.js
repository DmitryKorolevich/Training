angular.module('app.modules.order.controllers.matchbackReportController', [])
.controller('matchbackReportController', ['$scope', '$rootScope', '$state', '$q', 'orderService', 'productService', 'discountService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
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
            orderService.getMatchbackReportItems(getFilterData(), $scope.refreshTracker)
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
            $scope.orderSourceTypes = angular.copy($rootScope.ReferenceData.OrderSourceTypes);
            $scope.orderSourceTypes.splice(0, 0, { Key: null, Text: 'All Order Sources' });

            $scope.options = {};

            $scope.forms = {};

            var currentDate = new Date();
            currentDate.setHours(0, 0, 0, 0);
            $scope.filter = {
                To: currentDate.shiftDate('+1d'),
                From: currentDate.shiftDate('-7d'),
                IdOrderSource: null,
                Paging: { PageIndex: 1, PageItemCount: 100 },
            };

            $scope.options.exportUrl = orderService.getMatchbackItemsReportFile(getFilterData(), $rootScope.buildNumber);
            refreshItems();
        }

        $scope.filterItems = function ()
        {
            if ($scope.forms.form.$valid)
            {
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
                $scope.options.exportUrl = orderService.getMatchbackItemsReportFile(getFilterData(), $rootScope.buildNumber);
            }
            else
            {
                $scope.forms.form.submitted = true;
            }
        };

        initialize();
    }]);